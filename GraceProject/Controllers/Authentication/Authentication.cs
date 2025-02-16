using System.Security.Principal;

namespace GraceProject.Controllers.Authentication
{
    public class Authentication
    {
        private readonly RequestDelegate _next;

        public Authentication(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            // Check if the user is not authenticated and the requested path is not the login or sign-up page
            if (!context.User.Identity.IsAuthenticated && !IsLoginOrSignUpPage(path))
            {
                //context.Response.Redirect("/Identity/Account/Login");
                //return;
            }

            await _next(context);
        }

        private bool IsLoginOrSignUpPage(string path)
        {
            // Adjust these paths to match your actual login and sign-up page routes
            return path.Equals("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase) ||
                   path.Equals("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase) ||
                   path.Equals("/Identity/Account/ForgotPassword", StringComparison.OrdinalIgnoreCase);

            
        }
    }
}
