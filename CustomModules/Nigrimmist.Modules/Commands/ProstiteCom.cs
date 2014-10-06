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
        private Random r = new Random();

        public List<string> CallCommandList
        {
            get { return new List<string>() { "простите", "prostite" }; }
        }
        public string CommandDescription { get { return @"Случайная история с http://prostite.com"; } }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {

            HtmlReaderManager hrm = new HtmlReaderManager();

            hrm.Get("https://prostite.com");
            string html = hrm.Html;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var div = htmlDoc.DocumentNode.SelectSingleNode(@"//./div[@class='text']");
            sendMessageFunc(HttpUtility.HtmlDecode(div.InnerHtml));
        }
    }
}
