using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using HelloBotCommunication;
using HtmlAgilityPack;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    /// <summary>
    /// Fun staff from prostite.com
    /// </summary>
    public class ProstiteCom : IActionHandler
    {
        public List<string> Jokes = new List<string>();
        private Random r = new Random();

        public List<string> CallCommandList
        {
            get { return new List<string>() { "простите", "prostite" }; }
        }
        public string CommandDescription { get { return @"Случайная история с http://prostite.com"; } }
        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            if (!Jokes.Any())
            {
                HtmlReaderManager hrm = new HtmlReaderManager();

                hrm.Get("https://prostite.com");
                string html = hrm.Html;
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var divs = htmlDoc.DocumentNode.SelectNodes(@"//./div[@class='text']");

                foreach (var div in divs)
                {
                    Jokes.Add(HttpUtility.HtmlDecode(div.InnerHtml).Trim());
                }
            }
            int rPos = r.Next(0, Jokes.Count - 1);
            string joke = Jokes[rPos];
            Jokes.RemoveAt(rPos);
            sendMessageFunc(joke);
        }
    }
}
