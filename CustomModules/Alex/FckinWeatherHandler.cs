using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using HelloBotCommunication;


namespace SmartAssHandlerLib
{
    public class FckinWeatherHandler : IActionHandler
    {
        private const string DefaultLocation = "minsk";
        private const string QueryTemaplate = "http://thefuckingweather.com/?where={0}";
        private const string FailedResult = "I CAN'T FIND THAT SHIT!";

        public List<string> CallCommandList { get { return new List<string>() {"weather"}; }  }
        public string CommandDescription { get { return "Shows fucking WEATHER!"; } }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            var result = FailedResult;

            try
            {
                var location = args.Split(' ').FirstOrDefault();
                var rawData = GetRawWeatherData(location);
                var forecast = ParseForecast(rawData);

                result = forecast.ToString();
            }
            catch (Exception ex) { }

            sendMessageFunc(result);
        }

        private Forecast ParseForecast(string rawWeatherHtml)
        {
            var encodedStr = HttpUtility.HtmlDecode(rawWeatherHtml);

            var form = encodedStr.Split(new[] { "<div class=\"content\">" }, StringSplitOptions.RemoveEmptyEntries).Last();

            form = form.Split(new[] { "<table" }, StringSplitOptions.RemoveEmptyEntries).First();
            form = Uri.UnescapeDataString(string.Format("<div class=\"content\">{0}</div>", form));
            
            var xml = XElement.Parse(form);

            var tempStr = (string) xml.XPathEvaluate("string(/p/span/@tempf)");
            var remark = (string) xml.XPathEvaluate("string(/div/p[text()])");
            var flavor = (string) xml.XPathEvaluate("string(/p[2][text()])");

            var celsiusTemp = ConvertToCelsius(double.Parse(tempStr));

            return new Forecast()
            {
                DegreeCels = celsiusTemp,
                Remark = remark,
                Flavor = flavor
            };
        }

        private double ConvertToCelsius(double fahrenheit)
        {
            return Math.Round((fahrenheit - 32.0)*5.0/9.0, 1);
        }

        private string GetRawWeatherData(string location)
        {
            var client = new WebClient();

            client.Headers.Add("User-Agent", "Fiddler");

            var rawResult = client.DownloadData(string.Format(QueryTemaplate, location ?? DefaultLocation));
            var chars = new char[rawResult.Length];

            Encoding.UTF8.GetDecoder().GetChars(rawResult, 0, rawResult.Length, chars, 0);

            return new string(chars);
        }

        private class Forecast
        {
            public double DegreeCels { get; set; }
            public string Remark { get; set; }
            public string Flavor { get; set; }

            public override string ToString()
            {
                return string.Format("{0}?!{1}{2}{3}{4}",
                    DegreeCels,
                    Environment.NewLine,
                    Remark,
                    Environment.NewLine,
                    Flavor);
            }
        }
    }
}
