using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class Map : IActionHandler
    {
        public List<string> CallCommandList
        {
            get { return new List<string>() {"карта", "map"}; }
        }

        public string CommandDescription
        {
            get { return @"Генерирует ссылку на карту по адресу. Добавьте help для просмотра справки."; }
        }

       
        private IDictionary<string, string> mapUrlProviders = new Dictionary<string, string>()
        {
            {"g", "http://maps.google.com/?q={0}"},
            {"y", "http://maps.yandex.ru/?text={0}"},
        };

        private IDictionary<string, string> mapDirectionProviders = new Dictionary<string, string>()
        {
            {"g", "https://www.google.com/maps/dir/{0}/{1}"},
            {"y", "https://maps.yandex.ru/?rtext={0}~{1}"},
        };

        private const string defaultProvider = "g";
        private const string fromToDelimeter = "->";

        private string helpMsg = string.Format(@"""!map <опционально:поисковик> <адрес>"", где поисковик может быть y(yandex) или g(google).
Проложить маршрут : ""!map <опционально:поисковик> <из>{0}<в>""", fromToDelimeter);

       public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
       {
           

            if (!string.IsNullOrEmpty(args))
            {
                string inputProvider = args.Split(' ').First();

                if (inputProvider == "help")
                {
                    sendMessageFunc(helpMsg);
                }
                else if (args.Contains(fromToDelimeter))
                {
                    var addressParts = args.Split(new []{fromToDelimeter},StringSplitOptions.RemoveEmptyEntries);
                    if (addressParts.Count() == 2)
                    {
                        var leftPart = addressParts[0];
                        var rightPart = addressParts[1];
                        string foundProvider;
                        if (!mapDirectionProviders.TryGetValue(inputProvider, out foundProvider))
                        {
                            inputProvider = defaultProvider;
                            foundProvider = mapDirectionProviders[inputProvider];
                        }
                        else
                        {
                            leftPart = leftPart.Substring(inputProvider.Length).Trim();
                        }
                        string url = string.Format(foundProvider, HttpUtility.UrlEncode(leftPart), HttpUtility.UrlEncode(rightPart));
                        sendMessageFunc(PrepareUrl(url));
                    }
                }
                else
                {
                    string foundProvider;
                    string address = args;
                    if (!mapUrlProviders.TryGetValue(inputProvider, out foundProvider))
                    {
                        inputProvider = defaultProvider;
                        foundProvider = mapUrlProviders[inputProvider];
                    }
                    else
                    {
                        address = args.Substring(inputProvider.Length).Trim();    
                    }
                    string url = string.Format(foundProvider, HttpUtility.UrlEncode(address));
                    sendMessageFunc(PrepareUrl(url));
                }
            }
        }

        private string PrepareUrl(string url)
        {
            string shortenerPostUrl = "https://www.googleapis.com/urlshortener/v1/url";
            string postData = string.Format(@"{{""longUrl"": ""{0}""}}", url);
            HtmlReaderManager hrm = new HtmlReaderManager();
            hrm.ContentType = "application/json";
            hrm.Post(shortenerPostUrl, postData);

            var response = JsonConvert.DeserializeObject<dynamic>(hrm.Html);
            return response.id;
        }
    }
}
