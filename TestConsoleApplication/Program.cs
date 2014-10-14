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
using HelloBotCore;
using Nigrimmist.Modules.Commands;

namespace Test
{
    class Program
    {
        private static Random r = new Random();
        public static List<string> Jokes = new List<string>(); 
        static void Main(string[] args)
        {

            HelloBot bot = new HelloBot();
            bot.HandleMessage("!погода", s =>
            {
                Console.WriteLine(s);
            },null);
            

            //List<string> s2 = new List<string>() { "Сиськи" };

            //foreach (var v in s2)
            //{
            //    new Quote().HandleMessage(v, null, s =>
            //        Console.WriteLine(s));
            //}
            Console.ReadLine();
        }
    }
}
