using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using HtmlAgilityPack;

namespace Test
{
    class Program
    {
        private static Random r = new Random();
        public static List<string> Jokes = new List<string>(); 
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var token = cts.Token;
            //todo:убивать таску после n секунд исполнения
            Task.Run(() =>
            {
                using (cts.Token.Register(Thread.CurrentThread.Abort))
                {
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine(DateTime.Now);
                            Thread.Sleep(1000);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }, token);


            //new Timer(state =>
            //{
            //    Console.WriteLine("timer");
            //    cts.Cancel();
                
            //}, null, 3000, 0);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
