# CoreX.Extensions.Logging
A suitable logging for microservices scenarios, where you need to track logs without sacrificing performance or storage. 

# Getting started
In startup.cs:
```
// ConfigureServices
services.AddHttpLog();

// Configure
app.UseHttpLog();
```

Then browse your app /log to view a realtime console view of your application!

## Optionally to edit configurations: 
In startup.cs:
```
// ConfigureServices
services.AddHttpLog(Configuration);
```

Then in appsettings.json:
```
"HttpLogger": {
    "Enabled": true,
    "TimestampFormat": "hh:mm:ss.fff"
  },
```
