using System;
using System.Collections.Generic;
using System.Text;
using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;


namespace Yushko.Commands
{
    public class Moon : IActionHandler
    {
        public List<string> CallCommandList {
            get { return new List<string>() { "луна", "moon"}; }
        }

        public string CommandDescription { get { return @"лунный календарь"; } }


        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            StringBuilder result = new StringBuilder();
            HtmlReaderManager hrm = new HtmlReaderManager();
            hrm.Encoding = Encoding.GetEncoding(1251);
            hrm.Get(@"http://www.goroskop.org/luna/049/segodnya.shtml");
            string html = hrm.Html;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            //title
            result.Append(htmlDoc.DocumentNode.SelectSingleNode(@"//./span[@class='tabl-header']").InnerText.Trim());
            result.Append(Environment.NewLine);
            //info
            HtmlNodeCollection tblSpans = htmlDoc.DocumentNode.SelectNodes(@"//./span[@class='tabl-content']");
            if ((tblSpans != null) && (tblSpans.Count >= 1)) { 
                HtmlNodeCollection tds = tblSpans[1].SelectNodes(@".//./table[1]/tr/td");
                if ((tds != null) && (tds.Count >= 2)) {
                    result.Append(tds[1].InnerText);//.InnerHtml.Replace("<br>", Environment.NewLine).RemoveAllTags().Trim();
                }
            }
            sendMessageFunc(result.ToString());
        }
    }
}
