# PageHashGenerator
A tool generates the hash from a given url

Prequsitions:
- .NET Core 2.0
- ServiceStack v4.5.14 
- RabbitMQ Server

The front-end is built by jQuery and Bootstrap 4.0 As the [document from ServiceStack](http://docs.servicestack.net/javascript-server-events-client) provides a jQuery plugin. Thus it would be natural to just use these primitive front-end framework.

Please see the file below to configure the rabbitMQ server.
~~~
src/Configurations/RabbitConfig.cs
~~~

The server is hosted at: http://localhost:5000/

# Tests

The test is currently missing, but I did some general edge test e.g. invalid hostname, etc.

# CI intergation

It is possible ti intergate the project to a CI. I am open to discuss this oppounity, as it was not required in the specification.
