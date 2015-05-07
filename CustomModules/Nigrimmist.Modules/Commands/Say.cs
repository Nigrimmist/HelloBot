using System;
using System.Collections.Generic;

using HelloBotCommunication;

namespace Nigrimmist.Modules.Commands
{
    public class Say : IActionHandler
    {
        private Random r = new Random();



        public List<CallCommandInfo> CallCommandList
        {
            get
            {
                return new List<CallCommandInfo>()
                {
                      new CallCommandInfo("скажи" ),
                      new CallCommandInfo("say" )
                };
            }
        }


        public string CommandDescription { get { return @"Говорит что прикажете"; } }
        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            if (!args.Contains(command))
                sendMessageFunc(args, AnswerBehaviourType.Text);
            else
                sendMessageFunc(args.Replace("!", ""), AnswerBehaviourType.Text);
        }
    }
}
