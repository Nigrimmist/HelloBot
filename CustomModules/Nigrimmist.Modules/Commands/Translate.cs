using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

using HelloBotCommunication;
using HelloBotModuleHelper;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class Translate : IActionHandler
    {
        public List<string> CallCommandList { get { return new List<string>() { "t","translate"}; } }
        public string CommandDescription { get { return "Переводчик. Язык определяет автоматически, поддерживаются только русский/английский"; } }
        
        public List<string> Images = new List<string>();

        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            
            HtmlReaderManager hrm = new HtmlReaderManager();

            Regex r = new Regex("[а-яА-ЯЁё]+");
            bool isRu = r.IsMatch(args);
            string fromLang = isRu ? "ru" : "en";
            string toLang = isRu ? "en" : "ru";

            hrm.Get(string.Format("https://translate.google.ru/translate_a/single?client=t&sl={0}&tl={1}&hl=ru&dt=bd&dt=ex&dt=ld&dt=md&dt=qc&dt=rw&dt=rm&dt=ss&dt=t&dt=at&dt=sw&ie=UTF-8&oe=UTF-8&oc=1&otf=2&ssel=0&tsel=0&q=", fromLang, toLang) + HttpUtility.UrlEncode(args));
            string html = hrm.Html;
            string anwser = html.Substring(4, html.IndexOf(@""",""") - 4);
            sendMessageFunc(anwser, AnswerBehaviourType.Text);
        }

       
    }
}
