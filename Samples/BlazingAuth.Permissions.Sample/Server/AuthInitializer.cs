using BlazingAuth.Permissions.Sample.Server.Data;
using BlazingAuth.Permissions.Sample.Server.Models;
using BlazingAuth.Permissions.Sample.Shared;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlazingAuth.Permissions.Sample.Server
{
    public class AuthInitializer
    {
        public const string SuperAdminUserName = "superadmin@superadmin.com";
        public const string SuperAdminEmail = "superadmin@superadmin.com";
        public const string SuperAdminPassword = "SuperAdmin1@";
        public const string SuperAdminRole = "SuperAdmin";

        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly IUserEmailStore<ApplicationUser> emailStore;

        public AuthInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserStore<ApplicationUser> userStore)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.userStore = userStore;
            emailStore = GetEmailStore();
        }

        #region Helper
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException($"The {nameof(AuthInitializer)} requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)userStore;
        }
        #endregion

        public async Task InitializeAsync()
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            var createSuperAdminRole = !(await roleManager.RoleExistsAsync(SuperAdminRole));
            if (createSuperAdminRole)
            {
                // Create admin role
                var roleResult = await roleManager.CreateAsync(new IdentityRole(SuperAdminRole));
                if (!roleResult.Succeeded)
                {
                    throw new Exception(roleResult.ToString());
                }
            }

            var role = await roleManager.FindByNameAsync(SuperAdminRole);
            if (role is null)
            {
                throw new Exception($"'{SuperAdminRole}' role not found");
            }

            // Add role claims to super admin role
            await SetupAdminRoleClaimsAsync(role);

            // Create super admin user
            var user = await userManager.FindByNameAsync(SuperAdminUserName);
            if (user is null)
            {
                await CreateSuperAdminUser();
            }

            // Save changes
            await transaction.CommitAsync();
        }

        private async Task SetupAdminRoleClaimsAsync(IdentityRole role)
        {
            async Task AddPermission(string permissionValue)
            {
                var result = await roleManager.AddClaimAsync(role, new Claim(BlazingAuthClaims.Permission.Type, permissionValue));
                if (!result.Succeeded)
                {
                    throw new Exception(result.ToString());
                }
            }

            var claims = await roleManager.GetClaimsAsync(role);

            var hasPermissionsClaims = claims.Any(x => x.Type == BlazingAuthClaims.Permission.Type);
            if (!hasPermissionsClaims)
            {
                // Add claims
                await AddPermission(AppPermissions.ViewAllUsers);
                await AddPermission(AppPermissions.ManagePermissions);
                await AddPermission(AppPermissions.ViewAllPermissions);
            }
        }

        private async Task CreateSuperAdminUser()
        {
            var user = new ApplicationUser();
            await userStore.SetUserNameAsync(user, SuperAdminUserName, default);
            await emailStore.SetEmailAsync(user, SuperAdminEmail, default);

            var userResult = await userManager.CreateAsync(user, SuperAdminPassword);
            if (!userResult.Succeeded)
            {
                throw new Exception($"CreateUser: {userResult}");
            }

            await emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);

            var addToRoleResult = await userManager.AddToRoleAsync(user, SuperAdminRole);
            if (!addToRoleResult.Succeeded)
            {
                throw new Exception($"AddToRole: {addToRoleResult}");
            }
        }
    }
}
