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
using Nigrimmist.Modules.Commands;

namespace Test
{
    class Program
    {
        private static Random r = new Random();
        public static List<string> Jokes = new List<string>(); 
        static void Main(string[] args)
        {
            List<string> s2 = new List<string>() { "while(true)Out(1);" };

            foreach (var v in s2)
            {
                new Advice().HandleMessage(v, null, s =>
                    Console.WriteLine(s));
            }
            Console.ReadLine();
        }
    }
}
