using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HelloBotCommunication;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class LangExecuter : IActionHandler
    {
        public List<string> CallCommandList { get { return new List<string>() { "execute" }; } }
        public string CommandDescription { get { return "Выполняет код на C#. Добавьте help для вызова справки."; } }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            if (args.StartsWith("help"))
            {
                sendMessageFunc(GetHelpText());
            }
            else
            {
                string templateCode = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rextester
{{
    public class Program
    {{
        public static void Main(string[] args)
        {{
            {0}
        }}
        public static void Out(object obj){{
            Console.WriteLine(obj);
        }}
    }}
}}";

                HtmlReaderManager hrm = new HtmlReaderManager();

                hrm.Post("http://rextester.com/rundotnet/Run", string.Format("LanguageChoiceWrapper=1&EditorChoiceWrapper=1&Program={0}&Input=&ShowWarnings=false&Title=&SavedOutput=&WholeError=&WholeWarning=&StatsToSave=&CodeGuid=&IsInEditMode=False&IsLive=False"
                    , HttpUtility.UrlEncode(string.Format(templateCode, args))));
                var response = JsonConvert.DeserializeObject<dynamic>(hrm.Html);
                string toReturn = response.Result.ToString();

                if (string.IsNullOrEmpty(toReturn))
                {
                    toReturn = response.Errors.ToString();
                }

                if (!string.IsNullOrEmpty(toReturn))
                {
                    toReturn = toReturn.Replace(Environment.NewLine," ").Trim();
                    sendMessageFunc(toReturn.Length > 200 ? toReturn.Substring(0,50) + "..." : toReturn);
                }
                
                
            }
        }

        public string GetHelpText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("За основу взят сайт http://rextester.com/runcode.");
            sb.AppendLine("Поддерживает многострочность. Для вывода использовать Out() метод. Например, Out(1+2);");
            sb.AppendLine("Вывод ограничен 50 символами.");
            return sb.ToString();
        }
    }
}
