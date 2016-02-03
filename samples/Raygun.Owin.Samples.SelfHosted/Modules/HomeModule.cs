namespace Raygun.Owin.Samples.SelfHosted.Modules
{
    using System;
    using System.Linq;

    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Cookies;
    using Nancy.ModelBinding;
    using Nancy.Security;

    using Raygun.Owin.Samples.SelfHosted.Models;

    public class HomeModule : NancyModule
    {
        private const string CookieName = "raygun.owin.sample";

        public HomeModule()
        {
            Before.AddItemToStartOfPipeline(ctx =>
            {
                if (!ctx.Request.Cookies.ContainsKey(CookieName))
                {
                    ctx.NegotiationContext.Cookies.Add(new NancyCookie(CookieName, DateTime.UtcNow.ToString("U"), true, false, null));
                }

                return null;
            });

            Get["/"] = _ => View["Index", new
            {
                IsAuthenticated = Context.CurrentUser != null
            }];

            Get["/error"] = _ =>
            {
                throw new NotImplementedException("Route: /error");
            };

            Get["/form-error"] = _ => View["FormError"];
            Post["/form-error"] = _ =>
            {
                throw new NotImplementedException("Route: /form-error");
            };

            Get["/login"] = _ => View["Login"];
            Post["/login"] = _ =>
            {
                var model = this.Bind<LoginModel>();

                var user = UserMapper.Users.FirstOrDefault(u => string.Equals(u.UserName, model.UserName, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    throw new NotImplementedException("User not found.");
                }

                return this.LoginAndRedirect(user.Id);
            };

            Get["/logout"] = _ => this.LogoutAndRedirect("~/");

            Get["/account"] = _ =>
            {
                this.RequiresAuthentication();

                var user = (UserIdentity) Context.CurrentUser;

                return View["Account", new
                {
                    Id = user.UserId,
                    UserName = user.UserName
                }];
            };
        }
    }
}