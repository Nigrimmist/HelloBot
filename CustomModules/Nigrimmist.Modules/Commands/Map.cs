using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HelloBotCommunication;
using HtmlAgilityPack;
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

        private Random r = new Random();

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

        private string helpMsg = string.Format(@"Общий формат запроса : ""!map <опционально:поисковик> <адрес>"", где поисковик может быть y(yandex) или g(google). Например !map g Минск Ул Якуба Коласа 6
        Чтобы проложить маршрут из точки А в точку Б составьте запрос вида : ""!map <опционально:поисковик> <из>{0}<в>""", fromToDelimeter);

        public List<string> Images = new List<string>();

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
                        
                        sendMessageFunc(string.Format(foundProvider, HttpUtility.UrlEncode(leftPart), HttpUtility.UrlEncode(rightPart)));
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
                    
                    sendMessageFunc(string.Format(foundProvider, HttpUtility.UrlEncode(address)));
                }
            }
        }


    }
}
