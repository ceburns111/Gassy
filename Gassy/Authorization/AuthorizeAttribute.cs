using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Gassy.Entities;

namespace Gassy.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<RoleId> _roles; 

        public AuthorizeAttribute(params RoleId[] roles)
        {
            _roles = roles ?? new RoleId[] { };
        }

         public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine("(on) Authorizing...");
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = (User)context.HttpContext.Items["User"]; //user?
            if (user == null || (_roles.Any() && !_roles.Contains(user.RoleId)))
            {
                // not logged in or role not authorized
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}