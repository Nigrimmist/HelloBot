using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class Weather : IActionHandler
    {
        public List<string> CallCommandList {
            get { return new List<string>() { "погода" }; }
        }
        public string CommandDescription { get { return @"Погода на сегодня и завтра. !погода {город}"; } }
        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            if (string.IsNullOrEmpty(args))
                args = "минск";

            HtmlReaderManager hrm = new HtmlReaderManager();
            hrm.Get(string.Format("http://pogoda.yandex.by/{0}", HttpUtility.UrlEncode(args)));
            string html = hrm.Html;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var todayContainers = htmlDoc.DocumentNode.SelectNodes(@"//./div[@class='b-thermometer']");
            StringBuilder sb = new StringBuilder();

            string currTemp = todayContainers[0].SelectSingleNode(".//.//div[@class='b-thermometer__now']").InnerText;
            string currTempDescr = todayContainers[0].SelectSingleNode(".//.//div[@class='b-info-item b-info-item_type_fact-big']").InnerText;
            string windSpeed = htmlDoc.DocumentNode.SelectNodes(".//.//div[@class='b-thermometer-info__line']")[1].InnerText;
            windSpeed = ClearText(windSpeed);

            sb.AppendLine(string.Format("За окном : {0} {1} {2}", currTemp, currTempDescr, windSpeed));

            for (int i = 1; i < todayContainers.Count; i++)
            {
                var container = todayContainers[i];

                string timeName = container.SelectSingleNode(".//./div[@class='b-thermometer__name']").InnerText;
                string temp = container.SelectSingleNode(".//./div[@class='b-thermometer__small-temp']").InnerText;
                sb.AppendLine(string.Format("{0} {1}", ClearText(timeName), ClearText(temp)));
            }

            var dateTds = htmlDoc.DocumentNode.SelectNodes("//./div[@class='b-forecast-detailed__data']/tbody/tr");
            for (int i = 0; i < dateTds.Count; i+=5)
            {
                var morningTd = dateTds[i];
                var dayTd = dateTds[i + 1];
                var eveningTd = dateTds[i + 2];
                var nightTd = dateTds[i + 3];

            }
            sendMessageFunc(sb.ToString().Replace("&deg;", "°"));
        }

        private string ClearText(string str)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            return regex.Replace(str, @" ").Replace("\n", "").Trim();
        }
    }
}
