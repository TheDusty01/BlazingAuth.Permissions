using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazingAuth.Permissions.Client
{
    public class BlazingAuthAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public BlazingAuthAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
        {
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            if (user.Identity?.IsAuthenticated == true)
            {
                var identity = (ClaimsIdentity)user.Identity;

                ClaimArrayToMultipleClaims(account, identity, identity.RoleClaimType);
                ClaimArrayToMultipleClaims(account, identity, BlazingAuthClaims.Permission.Type);
            }

            return user;
        }

        protected static void ClaimArrayToMultipleClaims(RemoteUserAccount account, ClaimsIdentity identity, string claimType)
        {
            var claims = identity.FindAll(claimType).ToArray();
            if (!claims.Any())
                return;

            foreach (var existingClaim in claims)
            {
                identity.RemoveClaim(existingClaim);
            }

            var itemElem = account.AdditionalProperties[claimType];
            if (itemElem is JsonElement items)
            {
                if (items.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        identity.AddClaim(new Claim(claimType, item.GetString()!));
                    }
                }
                else
                {
                    identity.AddClaim(new Claim(claimType, items.GetString()!));
                }
            }

        }
    }
}
