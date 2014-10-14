using System;
using System.Collections.Generic;
using System.Text;
using HelloBotCommunication;
using HtmlAgilityPack;
using HelloBotModuleHelper;

namespace Yushko.Commands
{
    public class Kstati : IActionHandler
    {
        public List<string> CallCommandList {
            get { return new List<string>() { "кстати", ""}; }
        }

        public string CommandDescription { get { return @"Интересный факт одной строкой"; } }


        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            string url = "http://know-that.ru/randomizer.php";
            String result = string.Empty;

            HtmlReaderManager hrm = new HtmlReaderManager();
            hrm.Encoding = Encoding.GetEncoding(1251);
            hrm.Get(url);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(hrm.Html);
            HtmlNodeCollection factList = htmlDoc.DocumentNode.SelectNodes(@"//./center/table");
            if (factList != null)
            {
                Random random = new Random();
                int showResult = random.Next(factList.Count-1);
                HtmlNodeCollection tds = factList[showResult].SelectNodes(".//./td");//.InnerText.Trim();
                if ((tds != null) && (tds.Count >= 3))
                {
                    result = tds[2].InnerText.Trim();
                }
                else {
                    result = "Факт сломался... :(";
                }
            }
            else {
                result = "Факты кончились... :(";
            }
            sendMessageFunc(result);
        }
    }
}
