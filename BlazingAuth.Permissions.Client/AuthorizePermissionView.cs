using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazingAuth.Permissions.Client
{
    /// <summary>
    /// Displays differing content depending on the user's authorization status.
    /// </summary>
    public class AuthorizePermissionView : AuthorizeViewCore
    {
        private readonly IAuthorizeData[] selfAsAuthorizeData;

        /// <summary>
        /// Constructs an instance of <see cref="AuthorizeView"/>.
        /// </summary>
        public AuthorizePermissionView()
        {
            selfAsAuthorizeData = new[] { new AuthorizeDataAdapter(this) };
            Resource = this;
        }

        /// <summary>
        /// A comma delimited list of roles that are allowed to display the content.
        /// </summary>
        [Parameter] public string? Roles { get; set; }

        /// <summary>
        /// A comma delimited list of permissions that are needed to display the content.
        /// </summary>
        [Parameter] public string? AndPermissions { get; set; }

        /// <summary>
        /// A list of permissions that only one of is needed to display the content.
        /// Gets overriden by <see cref="AndPermissions"/> if specified.
        /// </summary>
        [Parameter] public string[]? AndPermissionsList { get; set; }

        /// <summary>
        /// A comma delimited list of permissions that only one of is needed to display the content.
        /// </summary>
        [Parameter] public string? OrPermissions { get; set; }

        /// <summary>
        /// A list of permissions that only one of is needed to display the content.
        /// Gets overriden by <see cref="OrPermissions"/> if specified.
        /// </summary>
        [Parameter] public string[]? OrPermissionsList { get; set; }

        /// <summary>
        /// Gets the data used for authorization.
        /// </summary>
        protected override IAuthorizeData[] GetAuthorizeData()
            => selfAsAuthorizeData;

        protected override Task OnParametersSetAsync()
        {
            // Set lists
            AndPermissionsList ??= AndPermissions?.Split(',');
            OrPermissionsList ??= OrPermissions?.Split(',');

            // Proceed with authorization
            return base.OnParametersSetAsync();
        }
    }

    // This is so the AuthorizeView can avoid implementing IAuthorizeData (even privately)
    internal class AuthorizeDataAdapter : IAuthorizeData
    {
        private readonly AuthorizePermissionView component;

        public AuthorizeDataAdapter(AuthorizePermissionView component)
        {
            this.component = component ?? throw new ArgumentNullException(nameof(component));
        }

        public string? Policy
        {
            get => BlazingAuthPolicies.Permission;
            set => throw new NotSupportedException();
        }

        public string? Roles
        {
            get => component.Roles;
            set => throw new NotSupportedException();
        }

        // AuthorizeView doesn't expose any such parameter, as it wouldn't be used anyway,
        // since we already have the ClaimsPrincipal by the time AuthorizeView gets involved.
        public string? AuthenticationSchemes
        {
            get => null;
            set => throw new NotSupportedException();
        }
    }
}
