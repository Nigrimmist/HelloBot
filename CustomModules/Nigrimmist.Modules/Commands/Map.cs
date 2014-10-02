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
        public List<string> CallCommandList { get { return new List<string>() { "карта","map" }; } }
        public string CommandDescription { get { return @"Генерирует ссылку на карту по адресу. Добавьте help для просмотра доп параметров."; } }
        private Random r = new Random();

       private IDictionary<string,string> mapUrlProviders = new Dictionary<string, string>()
       {
            {"g","http://maps.google.com/?q={0}"},
            {"y","http://maps.yandex.ru/?text={0}"},
       };

       
       
        public List<string> Images = new List<string>(); 

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {

            foreach (KeyValuePair<string, string> provider in mapUrlProviders)
            {
                if (args.StartsWith(provider.Key))
                {
                    string address = args.Substring(provider.Key.Length).Trim();
                    sendMessageFunc(string.Format(provider.Value, HttpUtility.HtmlEncode(address)));
                    break;
                }
            }
        }


    }
}
