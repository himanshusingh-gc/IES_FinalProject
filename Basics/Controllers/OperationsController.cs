using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public OperationsController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Open()
        {
            var cookieJar = new CookieJar();  // create new jar
            
            // authorize
            await _authorizationService.AuthorizeAsync(User, cookieJar, CookieJarAuthOperations.Open);
            return View(); 
        }
        // // GET
        // public IActionResult Index()
        // {
        //     return View();
        // }
    }
    
    // cookie jar
    public class CookieJar
    {
        public string Name { get; set; }
    }
    
    // operations on cookie jar
    public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";
    }
    
    public static class CookieJarAuthOperations
    {
        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement
        {
            Name = CookieJarOperations.Open
        };
    }
    
    public class CookieJarAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        // handles operations on cookies
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CookieJar cookieJar)
        {
            // look operation
            if (requirement.Name == CookieJarOperations.Look)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);     // allow only if authenticated
                }
            }
            else if (requirement.Name == CookieJarOperations.ComeNear)
            {
                if (context.User.HasClaim("Friend", "Good"))
                {
                    context.Succeed(requirement);    // allow only to friend
                }
            }

            return Task.CompletedTask;    // complete the operation
        }
    }
}