using BlazingAuth.Permissions.Sample.Server.Data;
using BlazingAuth.Permissions.Sample.Server.Models;
using BlazingAuth.Permissions.Sample.Shared;
using BlazingAuth.Permissions.Sample.Shared.Requests;
using BlazingAuth.Permissions.Sample.Shared.Responses;
using BlazingAuth.Permissions.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazingAuth.Permissions.Sample.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        private ValidationProblemDetails ProblemDetailsFromIdentityResult(IdentityResult result)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(err.Code, err.Description);
            }

            return ProblemDetailsFactory.CreateValidationProblemDetails(HttpContext, ModelState);
        }

        [AuthorizePermission(AppPermissions.ViewAllUsers)]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await dbContext.Users.Select(x => new User(x.Id, x.UserName, x.Email, x.EmailConfirmed))
                .ToArrayAsync();

            return Ok(users);
        }

        [AuthorizePermission(AppPermissions.ViewAllUsers)]
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            return Ok(new User(user.Id, user.UserName, user.Email, user.EmailConfirmed));
        }

        // One of the specified permission is needed
        [AuthorizePermission(AppPermissions.ViewAllPermissions, AppPermissions.ManagePermissions)]
        [HttpGet("users/{userId}/permissions")]
        public async Task<IActionResult> GetPermissions(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();


            var permissions = (await userManager.GetClaimsAsync(user))
                .Where(x => x.Type == BlazingAuthClaims.Permission.Type)
                .Select(x => x.Value)
                .ToList();

            var roles = await dbContext.UserRoles.Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .ToListAsync();

            var userPermissions = await dbContext.RoleClaims.Where(x => roles.Contains(x.RoleId))
                .Where(x => x.ClaimType == BlazingAuthClaims.Permission.Type)
                .Select(x => x.ClaimValue).ToListAsync();

            permissions.AddRange(userPermissions);

            return Ok(permissions);
        }

        // Both permissions are needed
        [AuthorizePermission(AppPermissions.ViewAllPermissions)]
        [AuthorizePermission(AppPermissions.ManagePermissions)]
        [HttpPost("users/{userId}/permissions")]
        public async Task<IActionResult> AddPermission(string userId, PermissionRequest request)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            var result = await userManager.AddClaimAsync(user, new Claim(BlazingAuthClaims.Permission.Type, request.Permission!));

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetPermissions), routeValues: new { userId }, null);
            }
            else
            {
                return BadRequest(ProblemDetailsFromIdentityResult(result));
            }
        }

        // Both permissions are needed
        [AuthorizePermission(AppPermissions.ViewAllPermissions)]
        [AuthorizePermission(AppPermissions.ManagePermissions)]
        [HttpDelete("users/{userId}/permissions/{permission}")]
        public async Task<IActionResult> RemovePermission(string userId, string permission)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            var result = await userManager.RemoveClaimAsync(user, new Claim(BlazingAuthClaims.Permission.Type, permission));

            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(ProblemDetailsFromIdentityResult(result));
            }
        }

    }
}
