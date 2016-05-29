# Raygun.Owin

[![Build Status](https://ci.appveyor.com/api/projects/status/acwjo8gsxa6u12ur?svg=true)](https://ci.appveyor.com/project/xt0rted/raygun-owin)
[![NuGet Status](http://img.shields.io/nuget/v/Raygun.Owin.svg?style=flat)](https://www.nuget.org/packages/Raygun.Owin/)
[![MyGet Status](https://img.shields.io/myget/13degrees/vpre/Raygun.Owin.svg?style=flat&label=myget)](http://www.myget.org/f/13degrees)

OWIN middleware for [Raygun.io](http://raygun.io/)

Raygun.Owin is based around the official library from [Mindscape](https://github.com/MindscapeHQ/raygun4net) but does not reference `System.Web`. Because of this, and the lack of `HttpContext`, the more generic `IDictionary<string, object>` environment is used making it compatible with any OWIN implementation.


## Install with NuGet

- `Install-Package Raygun.Owin`
- `Install-Package Raygun.Owin.AppBuilder` - IAppBuilder support


## Usage

The primary use of this library is to log unhandled exceptions but it may also be used to log unhandled requests or as a starting point for your own logging framework.


### Setup

The API key and endpoint override can be set in the middleware settings, or they can be set in your config's `appSettings` like so:

```xml
<appSettings>
    <add key="raygun:apiEndpoint" value="https://api.raygun.io/entries" />
    <add key="raygun:apiKey" value="abc123" />
    <add key="raygun:tags" value="tag1, tag2, tag3" /> <!-- supported delimiters: , ; | -->
    <add key="raygun:applicationVersion" value="1.2.3.4-beta2" />
    <add key="raygun:throwOnError" value="false" />
    <add key="raygun:excludeErrorsFromLocal" value="true" />
</appSettings>
```

#### Handling Exceptions

To log unhandled exceptions you will need to register the `RaygunUnhandledExceptionMiddleware` before the middleware you would like to log errors for. Typically this would be before all other middleware is registered.


#### Handling Requests

To log unhandled requests you will need to register the `RaygunUnhandledRequestMiddleware` before the middleware you would like to log unhandled requests for. Since requests will pass through you may also choose to register a catch all error page. You may also exclude this altogether and log the unhandled requests from inside your application or another piece of middleware.


### IAppBuilder Example

*See the [Raygun.Owin.Samples.NancyFX](/samples/Raygun.Owin.Samples.NancyFX) project for a more complete example.*

```csharp
public class Startup
{
    public static void Configuration(IAppBuilder app)
    {
        var settings = new RaygunSettings();

#if DEBUG
        settings.Tags.Add("debug");
#endif

        app.UseRaygun(options => {
            options.LogUnhandledExceptions = true;
            options.LogUnhandledRequests = true;
            options.Settings = settings;
        });

        app.UseCassette();

        app.UseNancy();
        app.UseStageMarker(PipelineStage.MapHandler);
    }
}
```


#### Caveats

Since Raygun.Owin is middleware exceptions thrown during application setup will not be caught. To log these you will need to wrap your setup code in a try/catch block and then log the exceptions manually like so:

```csharp
public class Startup
{
    public static void Configuration(IAppBuilder app)
    {
        try
        {
            var container = LoadIoCContainer();
            RunDbMigrations(container);
            SetupMiddleware(app, container);
        }
        catch (Exception ex)
        {
            new RaygunClient().SendInBackground(null, ex);
            throw;
        }
    }
}
```

*If you update your `Startup` class to inherit from `RaygunStartup` then this will be handled for you.*


## Usage with NancyFX

By default [Nancy](http://nancyfx.org/) is a terminating middleware which means requests will not be passed on if they can't be handled. Because of this unhandled exceptions won't be passed on for the `RaygunUnhandledExceptionMiddleware` to log. One way to get around this is like so:

```csharp
public class Startup
{
    public static void Configuration(IAppBuilder app)
    {
        var settings = new RaygunSettings();

        app.UseRaygun(options => {
            options.LogUnhandledExceptions = true;
            options.LogUnhandledRequests = true;
            options.Settings = settings;
        });

        app.UseNancy(options =>
        {
            options.PerformPassThrough = context =>
            {
                if (context.Response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Exception exception;
                    if (context.TryGetException(out exception))
                    {
                        if (exception is RequestExecutionException)
                        {
                            exception = exception.InnerException;
                        }

                        new RaygunClient(settings).SendInBackground(context.GetOwinEnvironment(), exception);
                    }
                }

                return false;
            };
        });
    }
}
```

## Usage with Web API

Web API swallows exceptions in order to gracefully return their details in the call result. Because of this you will need to add an extra filter to your Web API setup, or individual controllers, which will store the exception in the current OWIN request using the key `webapi.exception`.

The following is an example of this:

```csharp
public class Startup
{
    public static void Configuration(IAppBuilder app)
    {
        var config = new HttpConfiguration();
        config.Filters.Add(new OwinRequestExceptionLoggerAttribute());
        app.UseWebApi(config);
    }
}

public class OwinRequestExceptionLoggerAttribute : ExceptionFilterAttribute
{
    public override void OnException(HttpActionExecutedContext actionExecutedContext)
    {
        var owinContext = actionExecutedContext.ActionContext.Request.GetOwinContext();
        owinContext.Set("webapi.exception", actionExecutedContext.Exception);
    }
}
```


## Copyright and license

Copyright (c) 2016 Brian Surowiec under [the MIT License](LICENSE).
