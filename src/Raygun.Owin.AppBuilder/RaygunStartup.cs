namespace Raygun.Owin
{
    using System;
    using System.Threading.Tasks;

    using global::Owin;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public abstract class RaygunStartup
    {
        protected virtual RaygunSettings SetupSettings()
        {
            return new RaygunSettings();
        }

        public virtual void Configuration(IAppBuilder app)
        {
            var settings = SetupSettings();

            try
            {
                SetupErrorHandling(settings);
                SetupConfiguration(app, settings);
            }
            catch (Exception ex)
            {
                ReportError(settings, ex);

                throw;
            }
        }

        public abstract void SetupConfiguration(IAppBuilder app, RaygunSettings settings);

        public virtual void SetupErrorHandling(RaygunSettings settings)
        {
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                ReportError(settings, args.Exception);
            };
        }

        public void ReportError(RaygunSettings settings, Exception exception)
        {
            ReportError(settings, null, exception);
        }

        public virtual void ReportError(RaygunSettings settings, OwinEnvironment environment, Exception exception)
        {
            new RaygunClient(settings).SendInBackground(environment, exception);
        }
    }
}