# CoreX.Entensions.Http
Common Http extensions for ASP.NET Core that provide features (in my opinion) that should have been default to dotnet.
## Features
### Better logging for BadRequests
```
services.EnableLoggingForBadRequests();
``` 

When a client request your API with a wrong message, the default dotnet behaviour is not to log any warnings or errors and just write a simple output 400 to the log, which gives no hint about the cause of the error nor what the caller sent. This extension fixes this, by logging a warning with the validation errors and the full request to enable better debugging of such issues.

### Automatic logging for HttpClient
```
services.AddHttpClientLogging(Configuration);
```

Auto registers a handler for all HttpClients created using IHttpClientFactory, that automatically logs outgoing requests and their responses.

Possible Configurations in appsettings.json:
```
  "HttpClientLogging": {
    "Enabled": true,
    "Html": true,
    "Headers": true,
    "Body":  false
  },
```

### Headers Propagation
```
// default headers are already added like x-correlation-id
services.AddHeaderPropagation(options => 
{
    options.HeaderNames.Add("X-My-Header");
});
```
Automatically propagate (forward) headers from the current requests into any outgoing requests. Works with all HttpClients created using IHttpClientFactory. [Default propagated headers](https://github.com/Bishoymly/CoreX.Extensions/blob/master/src/CoreX.Extensions.Http/HeaderPropagation/HeaderPropagationOptions.cs#L13)
Useful to propagate correlation Id or authorization token.

Thanks to implementation from [David Fowler](https://gist.github.com/davidfowl/c34633f1ddc519f030a1c0c5abe8e867)

### Extensions to log full requests/response messages
Extension methods that adds **ToStringContent** and **ToHtml** to the following classes:
* HttpRequest
* HttpRequestMessage
* HttpResponseMessage

Which are useful to log full request/reponse contents in logs that appear in console or browser.
