using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;

namespace Nigrimmist.Modules.Commands
{
    public class OrModule : IActionHandler
    {
        public List<CallCommandInfo> CallCommandList
        {
            get
            {
                return new List<CallCommandInfo>()
                {
                    new CallCommandInfo("Выбери" )
                };
            }
        }

        public string CommandDescription { get { return @"Выбирает между чем-то. Использует ""Или"""; } }
        private Random r = new Random();
        private const int ChanceOfSpecialAnswer = 30;

        public List<string> cusstomMessages = new List<string>()
        {
            "Думаю {0}",
            "Определенно {0}",
            "{0}. К гадалке не ходи",
            "Не нужно быть семи пядей во лбу, чтобы понять, что {0} тут единственно верный вариант",
            "Возможно {0}",
            "Я выбираю {0}",
            "Пусть будет {0}",
            "Эники бэники... {0}!"
        };

        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            var variants = Regex.Split(args, "или", RegexOptions.IgnoreCase);
            string answer = "А что, есть выбор?";
            if (variants.Any())
            {
                answer = variants[r.Next(0, variants.Count())];
            }
            if (r.Next(1, 101) < ChanceOfSpecialAnswer)
                answer = string.Format(cusstomMessages[r.Next(0, cusstomMessages.Count)], answer);
            sendMessageFunc(answer, AnswerBehaviourType.Text);
        }


    }
}
