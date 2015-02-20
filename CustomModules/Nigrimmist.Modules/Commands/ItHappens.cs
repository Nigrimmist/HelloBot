using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class ItHappens : IActionHandler
    {
        public List<string> Jokes = new List<string>();
        private Random r = new Random();

       
        public List<CallCommandInfo> CallCommandList
        {
            get
            {
                return new List<CallCommandInfo>()
                {
                    
                    new CallCommandInfo("ithap" )
                };
            }
        }

        public string CommandDescription { get { return @"Случайная IT история с ithappens.me"; } }
        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            if (!Jokes.Any())
            {
                HtmlReaderManager hrm = new HtmlReaderManager();
                
                hrm.Get("http://ithappens.me/random");
                string html = hrm.Html;
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var divs = htmlDoc.DocumentNode.SelectNodes(@"//./div[@class='text']");

                foreach (var div in divs)
                {
                    Jokes.Add(div.InnerHtml.Replace("<p>", "").Replace("</p>", Environment.NewLine + Environment.NewLine).RemoveAllTags().Trim());
                }
            }
            int rPos = r.Next(0, Jokes.Count );
            string joke = Jokes[rPos];
            Jokes.RemoveAt(rPos);
            sendMessageFunc(joke, AnswerBehaviourType.Text);
        }
    }
}
