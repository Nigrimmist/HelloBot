using System;
using System.Collections.Generic;

using HelloBotCommunication;

namespace Nigrimmist.Modules.Commands
{
    public class Say : IActionHandler
    {
        private Random r = new Random();
        

        public List<string> CallCommandList
        {
            get { return new List<string>() { "скажи", "say" }; }
        }
        public string CommandDescription { get { return @"Говорит что прикажете"; } }
        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            sendMessageFunc(args,AnswerBehaviourType.Text);
        }
    }
}
