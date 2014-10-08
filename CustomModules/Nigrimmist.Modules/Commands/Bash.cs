using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class Bash : IActionHandler
    {
        public List<string> Jokes = new List<string>();
        private Random r = new Random();

        public List<string> CallCommandList{get { return new List<string>(){"баш","bash"}; }
        }
        public string CommandDescription { get { return @"Случайная цитата с башорга"; } }
        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            if (!Jokes.Any())
            {
                HtmlReaderManager hrm = new HtmlReaderManager();
                hrm.Encoding = Encoding.GetEncoding(1251);
                hrm.Get("http://bash.im/random");
                string html = hrm.Html;
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var divs = htmlDoc.DocumentNode.SelectNodes(@"//./div[@class='text']");

                foreach (var div in divs)
                {
                    Jokes.Add(HttpUtility.HtmlDecode(div.InnerHtml.Replace("<br>", Environment.NewLine)));
                }
            }
            int rPos = r.Next(0, Jokes.Count - 1);
            string joke = Jokes[rPos];
            Jokes.RemoveAt(rPos);
            sendMessageFunc(joke);
        }
    }
}
