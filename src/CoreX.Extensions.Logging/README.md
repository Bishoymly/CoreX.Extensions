[![NuGet version (CoreX.Extensions.Logging)](https://img.shields.io/nuget/v/CoreX.Extensions.Logging.svg?style=flat-square)](https://www.nuget.org/packages/CoreX.Extensions.Logging/)

# CoreX.Extensions.Logging
A simple, realtime display of logs for microservices, where you need to track logs without sacrificing performance or storage. No memory/storage/database required. Just open your browser "/log" to display realtime logs.

![image.png](../../images/Log.PNG)

## Getting started

Add the nuget package: [![NuGet version (CoreX.Extensions.Logging)](https://img.shields.io/nuget/v/CoreX.Extensions.Logging.svg?style=flat-square)](https://www.nuget.org/packages/CoreX.Extensions.Logging/)

In startup.cs:
```
public void ConfigureServices(IServiceCollection services)
{
  services.AddHttpLog(Configuration);
}
```
```
public async void Configure(IApplicationBuilder app)
{
  ...
  
  app.UseHttpLog();
  app.UseEndpoints(endpoints =>
  {
      endpoints.MapControllers();
  });
}

```
Then browse your app /log to view a realtime console view of your application!

## Configurations 
In appsettings.json put the following section to be able to configure the middleware:
```
  "HttpLogger": {
    "Enabled": true,
    "TimestampFormat": "hh:mm:ss.fff",
    "AllowForAnonymous": true,
    "AllowForUser": "",
    "AllowForRole": ""
  },
```

## /log query options
Browsing the /log url allows you to display filtered logs by certain parameters. Like the following examples:

Display only warnings
```
/log?level=warning
```

Display only logs for a specific user: uses the username authenticated for the HttpContext
```
/log?user=admin@email.com
```

Display only my logs: uses cookies to display logs coming only from my browser
```
/log?my
```


## Getting logs from multiple sources
Having a web application calling other APIs, sometimes you need to see logs from all sequentially in one location. If you have multiple applications running with enabled /log extension, you can make one of them fetch the logs from the others by adding named remotes in appsettings.

```
  "HttpLogger": {

    "Remotes": [
      {
        "name": "API",
        "url": "https://localhost:44371/log"
      }
    ]
  },
```
