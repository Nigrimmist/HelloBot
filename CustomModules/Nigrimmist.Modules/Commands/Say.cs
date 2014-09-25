using System;
using System.Collections.Generic;

using HelloBotCommunication;

namespace Nigrimmist.Modules.Commands
{
    public class Say : IActionHandler
    {
        private Random r = new Random();
        private List<string> answers = new List<string>()
        {
            "Друг, а хохо не хихи?",
            "Вот сам и скажи",
            "Чё, самый умный?",
            "Ищи дурака",
            "Зачем?",
            "5$",
            "Нет, спасибо",
        };

        public List<string> CallCommandList
        {
            get { return new List<string>() { "скажи", "say" }; }
        }
        public string CommandDescription { get { return @"Говорит что прикажете"; } }
        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            if (args.StartsWith("/"))
            {
                sendMessageFunc(answers[r.Next(0,answers.Count-1)]);
            }else
            sendMessageFunc(args);
        }
    }
}
