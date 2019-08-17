# CoreX.Extensions.Logging
A suitable logging for microservices scenarios, where you need to track logs without sacrificing performance or storage. 

## Getting started
In startup.cs:
```
// ConfigureServices
services.AddHttpLog(Configuration);

// Configure, right before UseMVC
app.UseHttpLog();
app.UseMvc();
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