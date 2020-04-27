using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace JN.Authentication.APITest.Handlers
{
    internal class CustomAuthorizationHandler : AuthorizationHandler<CustomRequirement>
    {
        private const string AdminClaimType = "IsAdmin";

        public CustomAuthorizationHandler(IConfiguration config)
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
        {
            // exit if we don't have the claim
            if (!context.User.HasClaim(c => c.Type == AdminClaimType))
            {
                return Task.CompletedTask;
            }

            // exit if we can't read an boolean from the 'IsAdmin' claim

            if (!bool.TryParse(context.User
                .FindFirst(c => c.Type == AdminClaimType).Value, out var isAdmin))
            {
                return Task.CompletedTask;
            }

            // Validate that the isAdmin is TRUE
            if (isAdmin)
            {
                // Mark the requirement as satisfied
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    // A custom authorization requirement 
    internal class CustomRequirement : IAuthorizationRequirement
    {
        public bool IsAdmin { get; private set; }

        public CustomRequirement(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }

 
    }
}