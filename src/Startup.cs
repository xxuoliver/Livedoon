using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Funq;
using HashGenerator.Configurations;
using RabbitMQ.Client;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;
using ServiceStack.Templates;
using ServiceStack.Text;

// Internal usings
using Livedoon.Api;
using ServiceStack.Web;

namespace Livedoon
{
    public class Startup
    {

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            // define Rabbit MQ server.
            var mqServer = new RabbitMqServer(RabbitConfig.Host);
            mqServer.RegisterHandler<Api.HashGenerator>(m =>
           {

               var request = m.GetBody();

               try
               {
                   // Use WebClient class to make the who procedure clean and simple.
                   // This handler should be running on a different thread, so that no
                   // async/await is needed.
                   using (var webClient = new WebClient()) // WebClient class inherits IDisposable
                   {
                       var url = request.Url;
                       var htmlContent = webClient.DownloadString(url);

                       // then create MD5 by .NET crypto library
                       var hashValue = string.Join("", MD5.Create().ComputeHash(
                                                              Encoding.ASCII.GetBytes(htmlContent))
                                                          .Select(s => s.ToString("x2")));
                       return new HashResponse()
                       {
                           Succeed = true,
                           Subscriber = request.Subscriber,
                           Url = request.Url,
                           Result = hashValue
                       };
                   }
               }
               catch (Exception ex)
               {
                   return new HashResponse()
                   {
                       Succeed = false,
                       Subscriber = request.Subscriber,
                       Url = request.Url,
                       Result = ex.Message
                   };
               }

           });

            mqServer.Start();

            app.UseServiceStack(new AppHost());
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("Page MD5 Hash Tool", typeof(HashServices).GetAssembly()) { }

        public override void Configure(Container container)
        {
            Plugins.Add(new TemplatePagesFeature());
            Plugins.Add(new ServerEventsFeature());
            container.Register<IServerEvents>(c => new MemoryServerEvents());
        }
    }
}