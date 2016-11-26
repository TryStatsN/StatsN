## StastN


StastN is a modern high performance Stastd client for dotnet core. StatsN supports both TCP and UDP, although UDP is recommended.


## Getting started

You can get a new IStatsd with a few different ways, and then you can log metrics. Here are some examples.


```csharp
IStatsd statsd = Statsd.New<Udp>(a=>a.hostOrIp = "10.22.2.1", Port = 8125);
IStatsd statsd = Statsd.New<Tcp>(a=>a.hostOrIp = "10.22.2.1");
IStatsd statsd = Statsd.New(a=>a.hostOrIp = "10.22.2.1"); //defaults to udp
IStatsd statsd = Statsd.New(new StatsdOptions(){ hostOrIp = "127.0.0.1"}); //defaults to udp
IStatsd statsd = new Stastd(new StatsdOptions(){ hostOrIp = "127.0.0.1"});  //defaults to udp
IStatsd statsd = new Stastd(new StatsdOptions(){ hostOrIp = "127.0.0.1"}, new Udp());


statsd.CountAsync("myapp.counterstat");
statsd.TimerAsync("myapp.timeMyFunction", ()=>{
 //code to instrument
});
statsd.TimerAsync("myapp.timeData", 400); //400ms
statsd.GaugeAsync("autotest.gaugeyo", 422);
statsd.GaugeDeltaAsync("autotest.gaugeyo", -10);
statsd.SetAsync("autotest.setyo", 888);

```

## Logging

Like most statsd clients, this client tries to avoid throwing exceptions at all costs. Any errors/exceptions created will be logged ass a Systems.Diagnostics.Trace message.

You can pass lambda into the StatsdOptions class to be passed exceptions and log messages.


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

var stats = Statsd.New(a=>a.OnExceptionGenerated = (exception) => { 
		/* handle exception */ });
```

## Buffering metrics

By setting the `BufferMetrics` property in the options object to true, the metrics will be buffered thus sending packets. This uses a Concurrent Queue to Queue up the metrics and a BackgroundWorker to peal metrics off the Queue and send them along aggregated.


## Awaiting metrics

By default the various logging metric functions return Tasks. **You do not need to await on these** If you await on these and you have buffered metrics off , you will return after the bytes have been added to the network stream. If you await, and buffered metrics are on then your await will return when your metric has been added to the Queue of metrics to be sent.

