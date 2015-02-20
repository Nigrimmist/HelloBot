using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HelloBotCommunication;

namespace Nigrimmist.Modules.WindowsCommands
{
    public class BrowserUrlsOpenModule: IActionHandler
    {

        //public List<CallCommandInfo> CallCommandList { get;/*{ return new List<string>(){"open"}; }*/ private set; }

        public List<CallCommandInfo> CallCommandList{get; private set;}

        public string CommandDescription { get { return "Открывает ссылку в браузере"; } }
        private IDictionary<string, string> _commandUrlDictionary = new Dictionary<string, string>();
        public BrowserUrlsOpenModule()
        {
           var configurationData = File.ReadAllText("ModuleConfiguration/BrowserUrlsOpenModule.txt");
            if (!string.IsNullOrEmpty(configurationData))
            {
                var keyValues = configurationData.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Select(line => line.Split(';')).Select(x => new KeyValuePair<string, string>(x[0], x[1]));

                foreach (KeyValuePair<string, string> keyValue in keyValues)
                {
                    _commandUrlDictionary.Add(keyValue);
                }

                CallCommandList = _commandUrlDictionary.Select(x => new CallCommandInfo(x.Key)).ToList();
            }
        }

        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            
               // string subCommand = args.Split(' ').First();
                //args = args.Substring(subCommand.Length).TrimStart();
                if (_commandUrlDictionary.ContainsKey(command))
                {
                    var url = _commandUrlDictionary[command];
                    if (string.IsNullOrEmpty(args.Trim()))
                    {
                        Uri uri;
                        if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                        {
                            url = uri.Scheme + "://" + uri.Host;
                        }
                    }
                    sendMessageFunc(string.Format(url, args), AnswerBehaviourType.Link);
                }
            
        }
    }
}
