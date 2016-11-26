## StastN

[![Build status](https://ci.appveyor.com/api/projects/status/2935gqj2013whaoe/branch/master?svg=true)](https://ci.appveyor.com/project/tparnell8/statsn/branch/master) [![Coverage Status](https://coveralls.io/repos/github/TryStatsN/StatsN/badge.svg?branch=master)](https://coveralls.io/github/TryStatsN/StatsN?branch=master) 


StastN is a modern high performance Stastd client for dotnet core. StatsN supports both TCP and UDP, although UDP is recommended. Largely inspired by the [statsd-csharp-client](https://github.com/Pereingo/statsd-csharp-client) and the [statsd.net client](https://github.com/lukevenediger/statsd-csharp-client). Both projects In my mind are awesome :facepunch:, just not exactly what I was looking for.

I wrote this client, because I found unit testing [statics](https://github.com/Pereingo/statsd-csharp-client) less than fun, and tired of waiting for [features to be published](https://github.com/lukevenediger/statsd-csharp-client/issues/17). Or support for [features](https://github.com/lukevenediger/statsd-csharp-client/blob/250f02caaf5fdbf3e112ac57c86d5a8bdb9618c5/StatsdClient/IStatsd.cs#L54) that statsd [does not actually supprt](https://github.com/etsy/statsd/issues/467).

This client attempts to help testability by using interfaces, observability by allowing you to register functions to listen for exceptions and logging that occurs inside the client, and scalability by really making the code perform well.

## Getting started

In short the api is easy. You can get a new IStatsd with a few different ways, and then you can log metrics with an IStatsd implementation. Here are some examples.


```csharp
IStatsd statsd = Statsd.New<Udp>(a=>a.HostOrIp = "10.22.2.1", Port = 8125);
IStatsd statsd = Statsd.New<Tcp>(a=>a.HostOrIp = "10.22.2.1"); //use tcp
IStatsd statsd = Statsd.New(a=>a.HostOrIp = "10.22.2.1"); //defaults to udp
IStatsd statsd = Statsd.New(new StatsdOptions(){ HostOrIp = "127.0.0.1"}); //defaults to udp
IStatsd statsd = new Stastd(new StatsdOptions(){ HostOrIp = "127.0.0.1"});  //defaults to udp
IStatsd statsd = new Stastd(new StatsdOptions(){ HostOrIp = "127.0.0.1"}, new Tcp()); //pass a new udp client. You could in theory make your own transport if you inherit from BaseCommunicationProvider


statsd.CountAsync("myapp.counterstat"); //default to 1 aka increment
statsd.CountAsync("myapp.counterstat", 6);
statsd.CountAsync("myapp.counterstat", -6);
statsd.TimerAsync("myapp.timeMyFunction", ()=>{
 //code to instrument
});
statsd.TimerAsync("myapp.timeData", 400); //400ms
statsd.GaugeAsync("autotest.gaugeyo", 422);
statsd.GaugeDeltaAsync("autotest.gaugeyo", -10);
statsd.SetAsync("autotest.setyo", 888);

```

## Logging

Like most statsd clients, this client **avoids throwing exceptions at all costs**. Any errors/exceptions created will be logged as a Systems.Diagnostics.Trace messages.

You can pass lambda into the `StatsdOptions` class to be passed exceptions and log messages, instead of getting them through the Trace system.


```csharp

            var opt = new StatsdOptions
            {
                OnExceptionGenerated = (exception) => { /* handle exception */ },
				OnLogEventGenerated = (log) => { /* handle log msg */ }
            };
			var stats = Statsd.New(opt);

```

or

```csharp

var stats = Statsd.New(a=>a.OnExceptionGenerated = (exception) => { /* handle exception */ });
```

## Buffering metrics

By setting the `BufferMetrics` property in the options object to true, the metrics will be buffered thus sending less packets. The Buffer size defaults to 512, which is [documented by statsd](https://github.com/etsy/statsd/blob/master/docs/metric_types.md#multi-metric-packets). You may change its size using the BufferSize property of `StastdOptions`. This uses a Concurrent Queue to Queue up the metrics and a `BackgroundWorker` to peal metrics off the Queue and send them along aggregated.

```csharp

var opt = new StatsdOptions(){

    BufferMetrics = true,
    BufferSize = 512
};

```

## Awaiting metrics

By default the various logging metric functions return Tasks. **You do not need to await on these** If you await on these and you have buffered metrics off , you will return after the bytes have been added to the network stream. If you await, and buffered metrics are on then your await will return when your metric has been added to the Queue of metrics to be sent.

