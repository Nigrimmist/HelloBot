using System;
using System.Collections.Generic;

using HelloBotCommunication;
using NCalc;

namespace Nigrimmist.Modules.Commands
{
    public class Calculator : IActionHandler
    {

        public List<string> CallCommandList { get { return new List<string>() { "calc" }; } }
        public string CommandDescription { get { return "Умный калькулятор. Реализация NCalc библиотеки"; }  }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            Expression s = new Expression(args);
            sendMessageFunc(string.Format("Мсье {0}, ответ равен : {1}", clientData, s.Evaluate()));
            
        }
    }
}