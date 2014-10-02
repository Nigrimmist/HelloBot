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

        private string defaultProvider = "g";

        private string helpMsg = @"Общий формат запроса : ""!map <опционально:поисковик> <адрес>"", где поисковик может быть y(yandex) или g(google). Например !map g Минск Ул Якуба Коласа 6";

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
                else
                {
                    string foundProvider;
                    if (!mapUrlProviders.TryGetValue(inputProvider, out foundProvider))
                    {
                        inputProvider = defaultProvider;
                        foundProvider = mapUrlProviders[inputProvider];
                    }

                    string address = args.Substring(inputProvider.Length).Trim();
                    sendMessageFunc(string.Format(foundProvider, HttpUtility.UrlEncode(address)));
                }
            }
        }


    }
}
