using System;
using System.Collections.Generic;
using System.Text;
using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;


namespace Yushko.Commands
{
    public class Horoscope : IActionHandler
    {
        
        public List<CallCommandInfo> CallCommandList
        {
            get
            {
                return new List<CallCommandInfo>()
                {
                    
                    new CallCommandInfo("гороскоп" )
                };
            }
        }
        public string CommandDescription { get { return @"гороскоп <знак зодиака>"; } }

        private IDictionary<string, string> Signs = new Dictionary<string, string>()
        {
            {"ОВЕН", "aries/"},
            {"ТЕЛЕЦ", "taurus/"},
            {"БЛИЗНЕЦЫ", "gemini/"},
            {"РАК", "cancer/"},
            {"ЛЕВ", "leo/"},
            {"ДЕВА", "virgo/"},
            {"ВЕСЫ", "libra/"},
            {"СКОРПИОН", "scorpio/"},
            {"СТРЕЛЕЦ", "sagittarius/"},
            {"КОЗЕРОГ", "capricorn/"},
            {"ВОДОЛЕЙ", "aquarius/"},
            {"РЫБЫ", "pisces"}
        };
        private IDictionary<string, string> Terms = new Dictionary<string, string>()
        {   
            {"ВЧЕРА", "yesterday"},
            {"СЕГОДНЯ", "today"},
            {"ЗАВТРА", "tomorrow"},
            {"НЕДЕЛЯ", "week"},
            {"МЕСЯЦ", "month"},
            {"ГОД", "year"}
        };

        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            string[] arg = args.Split(' ');
            string url = "http://goroskop.open.by/pda/";
            StringBuilder result = new StringBuilder();
            string sign, term="today", category="ОБЩИЙ";
            string help = "!гороскоп <знак зодиака> [общий/эротический/антигороскоп/бизнес/любовный/здоровья/кулинарный/мобильный] [сегодня/завтра/неделя/месяц/год]";
            if (arg.Length == 0)
            {
                sendMessageFunc(help, AnswerBehaviourType.Text);
                return;
            }

            if ((arg.Length >= 1) && (Signs.TryGetValue(arg[0].ToUpper(), out sign)))
            {
                url += sign;
            }
            else
            {
                if (string.IsNullOrEmpty(arg[0])){
                    sendMessageFunc(help, AnswerBehaviourType.Text);
                }else{
                    sendMessageFunc(arg[0] + " - неверный знак зодиака", AnswerBehaviourType.Text);
                }
                return;
            }

            if (arg.Length == 2) {
               if (!Terms.TryGetValue(arg[1].ToUpper(), out term))
                    category = arg[1].ToUpper();
            }else{
                if (arg.Length >= 2)
                {
                        category = arg[1].ToUpper();
                }

                if ((arg.Length >= 3) && (!Terms.TryGetValue(arg[2].ToUpper(), out term)))
                {
                    result.Append(arg[2].ToUpper() + " - неверно задан срок. Возможные варианты: [сегодня/завтра/неделя/месяц/год] ");
                    result.Append(Environment.NewLine);
                }
            }
            url += term;



            HtmlReaderManager hrm = new HtmlReaderManager();
            hrm.Get(url);
            string html = hrm.Html;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            //title
            result.Append(arg[0].ToUpper()+" ");
            result.Append(htmlDoc.DocumentNode.SelectSingleNode(@"//./p[@class='horoTitleName']/span").InnerText);
            result.Append(Environment.NewLine);

            //select proper horoscope category
            HtmlNodeCollection horoCategories = htmlDoc.DocumentNode.SelectNodes(@"//./p[@class='categoryName' or @class='categoryText']");
            if ((horoCategories != null)&&(horoCategories.Count >= 2))
            {
                bool categoryFound = false;
                for (int i = 0; i < horoCategories.Count; i = i + 2)
                {
                    if (horoCategories[i].InnerText.ToUpper() == category)
                    {
                        result.Append(category);
                        result.Append(Environment.NewLine);
                        result.Append(horoCategories[i + 1].InnerText);
                        categoryFound = true;
                        break;
                    }
                }
                if (!categoryFound) {
                    result.Append(category.ToUpper() + " - не удалось найти гороскоп такого типа.");
                    result.Append(Environment.NewLine);
                    result.Append(horoCategories[0].InnerText.ToUpper());
                    result.Append(Environment.NewLine);
                    result.Append(horoCategories[1].InnerText);
                }
            }
            else {
                result.Append("На запрашиваемый Вами период гороскоп отсутствует");
            }
            sendMessageFunc(result.ToString(), AnswerBehaviourType.Text);
        }
    }
}
