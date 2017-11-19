using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HashGenerator.Configurations
{
    public class RabbitConfig
    {
        public static string QueueName = "Livedoon.MD5HashQueue";
        public static string Host = "localhost";
    }
}
