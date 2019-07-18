# KestrelShark

Network tracing for Kestrel.

## Usage

* Install `dotnet-trace`: `dotnet tool install -g dotnet-trace --version 3.0.0-preview7.19365.2`
* Add reference to `KestrelShark`
* Add KestrelShark middleware **AFTER** `UseHttps`:

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.ConfigureEndpointDefaults(listenOptions =>
                {
                    listenOptions.UseHttps();
                    listenOptions.UseConnectionTracing(); // <-- THIS
                });
            });
        });
```

* Launch the app
* Optional: use `dotnet trace list-processes` to find the PID
* Run `dotnet trace collect -p [process ID] --providers "Microsoft-AspNetCore-NetworkTrace"`
* Do some HTTP/2
* Hit ENTER or CTRL-C to stop the trace
* Open in PerfView, use ALT-D to dump raw data for individual events to see the data:

![image](https://user-images.githubusercontent.com/7574/61495993-a66aec80-a96f-11e9-8b86-817831dc230e.png)

![image](https://user-images.githubusercontent.com/7574/61496019-c1d5f780-a96f-11e9-9f6e-87e68afd8eab.png)


## Next Steps

* Integrate in to Kestrel directly (and other servers?)
* Add "viewer" either by converting the file to something like .pcap, or just build our own viewer app.