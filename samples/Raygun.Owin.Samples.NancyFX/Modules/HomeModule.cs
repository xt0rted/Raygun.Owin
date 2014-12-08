namespace Raygun.Owin.Samples.NancyFX.Modules
{
    using System;

    using Nancy;
    using Nancy.Cookies;

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

            Get["/"] = _ => View["Index"];

            Get["/error"] = _ =>
            {
                throw new NotImplementedException("Route: /error");
            };

            Get["/form-error"] = _ => View["FormError"];
            Post["/form-error"] = _ =>
            {
                throw new NotImplementedException("Route: /form-error");
            };
        }
    }
}