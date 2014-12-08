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
    <add key="raygun:apiKey" value="abc123" />
    <add key="raygun:apiEndpoint" value="https://api.raygun.io/entries" />
</appSettings>
```

#### Handling Exceptions

To log unhandled exceptions you will need to register the `RaygunUnhandledExceptionMiddleware` before the middleware you would like to log errors for. Typically this would be before all other middleware is registered.


#### Handling Requests

To log unhandled requests you will need to register the `RaygunUnhandledRequestMiddleware` before the middleware you would like to log unhandled requests for. Since requests will pass through you may also choose to register a catch all error page. You may also exclude this altogether and log the unhandled requests from inside your application or another piece of middleware.


### Example

```csharp
public class Startup
{
    public static void Configuration(IAppBuilder app)
    {
        app.UseRaygunUnhandledExceptionLogger();
        app.UseRaygunUnhandledRequestLogger();

        app.UseCassette();
        app.UseNancy(options =>
        {
            // NOTE: requires nancy v0.20.0 or newer
            // this will allow 404 errors to pass through to the next piece of middleware
            options.PassThroughWhenStatusCodesAre(HttpStatusCode.NotFound);
        });
    }
}
```


#### Caveats

Exceptions which are thrown during application setup will not be caught. To log these you will need to wrap your setup code in a try/catch block and then log the exceptions manually like so:

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


## Development

This repository contains `.git*` and `.hg*` files. This allows for the use of either Git or [Hg](http://mercurial.selenic.com/) using Fog Creek's [Kiln Harmony](http://www.fogcreek.com/kiln/).

To use Hg you will need to clone or fork this repository and then import it into your instance of Kiln as a Git repository. From there you will be able to work in Hg exclusively aside from when pushing changes up to GitHub. You will also need to enable the [EOL extension](http://mercurial.selenic.com/wiki/EolExtension) for this repository if you do not have it setup globally.


## Copyright and license

Copyright (c) 2014 Brian Surowiec under [the MIT License](LICENSE).