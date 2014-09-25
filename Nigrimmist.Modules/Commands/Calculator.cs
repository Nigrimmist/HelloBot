using System;
using System.Collections.Generic;

using HelloBotCommunication;
using HelloBotCommunication.ClientDataInterfaces;
using NCalc;

namespace Nigrimmist.Modules.Commands
{
    public class Calculator : IActionHandler
    {

        public List<string> CallCommandList { get { return new List<string>() { "calc" }; } }
        public string CommandDescription { get { return "Умный калькулятор. Реализация NCalc библиотеки"; }  }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            Expression expr = new Expression(args);
            var exprAnswer = expr.Evaluate();
            string messageAuthor = string.Empty;
            string answer = string.Empty;
            

            var skypeData = clientData as ISkypeData;
            if (skypeData!=null)
            {
                messageAuthor = skypeData.FromName;
            }

            if (!string.IsNullOrEmpty(messageAuthor))
            {
                answer = string.Format("Мсье {0}, ответ равен : {1}", messageAuthor, exprAnswer);
            }
            else
            {
                answer = string.Format("Ответ равен : {0}", exprAnswer);
            }

            sendMessageFunc(answer);
            
        }
    }
}