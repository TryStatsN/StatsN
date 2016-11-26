## StastN


StastN is a modern high performance Stastd client for dotnet core. StatsN supports both TCP and UDP, although UDP is recommended.


## Getting started

You can get a new IStatsd with a few different ways, and then you can log metrics. Here are some examples.


```
IStatsd statsd = Statsd.New<Udp>(a=>a.hostOrIp = "10.22.2.1");
IStatsd statsd = Statsd.New<Tcp>(a=>a.hostOrIp = "10.22.2.1");
IStatsd statsd = Statsd.New(a=>a.hostOrIp = "10.22.2.1"); //defaults to udp
IStatsd statsd = Statsd.New(new StatsdOptions(){ hostOrIp = "127.0.0.1"}); //defaults to udp
IStatsd statsd = new Stastd(new StatsdOptions(){ hostOrIp = "127.0.0.1"});  //defaults to udp
IStatsd statsd = new Stastd(new StatsdOptions(){ hostOrIp = "127.0.0.1"}, new Udp());


statsd.Count("myapp.counterstat");
statsd.Timer("myapp.timeMyFunction", ()=>{
 //code to instrument
});
statsd.Timer("myapp.timeData", 400); //400ms
statsd.Gauge("autotest.gaugeyo", 422);
statsd.GaugeDelta("autotest.gaugeyo", -10);
statsd.Set("autotest.setyo", 888);

```

## Logging

Like most statsd clients, this client tries to avoid throwing exceptions at all costs. Any errors/exceptions created will be logged ass a Systems.Diagnostics.Trace message.

The Statsd Options 