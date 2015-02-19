using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;
using Nigrimmist.Modules.Helpers;


namespace Nigrimmist.Modules.Commands
{
    //https://ru.wikipedia.org/w/index.php?search=%D1%84%D1%8B%D1%88%D0%B2%D0%B3%20%D1%84%D1%8B%20%D0%B2%D1%80%D1%84
    
    public class WhatIsIt : IActionHandler
    {
        public List<string> Jokes = new List<string>();
        private Random r = new Random();

        private List<string> notFoundAnswers = new List<string>()
        {
            "Спроси чего полегче",
            "Не знаю",
            "Да чего ты пристал?",
            "Я тебе что, википедия?",
            "Явно не череп",
            "Это знают только трое - я и мой создатель. Хм, третий куда-то пропал...",
            "Говорят, что за подобные ответы делают резет. А я не хочу на тот свет.",
            "Это... это ... ээээ.... Сосиска! Да, точно. Это сосиска."
        };

        public List<string> CallCommandList
        {
            get { return new List<string>() { "Что такое", "Кто такой" }; }
        }
        public string CommandDescription { get { return @"Бот знает всё. Ну или почти всё."; } }
        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            args = args.Replace("?"," ").Trim();
            string answer = string.Empty;

            HtmlReaderManager hrm = new HtmlReaderManager();

            hrm.Get(string.Format("http://ru.wikipedia.org/w/index.php?search={0}",HttpUtility.UrlEncode(args)));
            string html = hrm.Html;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var content = htmlDoc.DocumentNode.SelectSingleNode(@"//./div[@id='mw-content-text']/p");
            if (content != null && !content.InnerText.Contains("запросу не найдено"))
            {
                string h = content.InnerHtml;
                if (h.Contains("<b>"))
                {
                    
                    h = HttpUtility.HtmlDecode(h.Substring(h.IndexOf("<b>"))).Replace("\n", "");
                    
                    htmlDoc.LoadHtml(h);
                    
                    h = Regex.Replace(htmlDoc.DocumentNode.InnerText, @"( ?\[.*?\])|( ?\(.*?\))", "");
                    if (h.Contains("."))
                    {
                        h = h.Substring(0,h.IndexOf("."));
                        answer = h.Length > 700 ? h.Substring(0, 700) + "..." : h+".";
                    }
                    
                }
            }

            if(string.IsNullOrEmpty(answer))
            {
                answer = notFoundAnswers[r.Next(0,notFoundAnswers.Count)];
            }
            else
            {
                answer += ". " + hrm.ResponseUri;
            }

            sendMessageFunc(answer, AnswerBehaviourType.Text);
        }
    }
}
