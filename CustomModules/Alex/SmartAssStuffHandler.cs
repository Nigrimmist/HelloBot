using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HelloBotCommunication;


namespace SmartAssHandlerLib
{
    public class SmartAssStuffHandler : IActionHandler
    {
        private const string Query =
            "http://referats.yandex.ru/referats/write/?t=astronomy+geology+gyroscope+literature+marketing+mathematics+music+polit+agrobiologia+law+psychology+geography+physics+philosophy+chemistry+estetica";

        public List<string> CallCommandList
        {
            get { return new List<string>() { "жги" }; }
        }

        public string CommandDescription
        {
            get { return "Безумная заумь небольшими дозами. Добавьте слово \"напалмом\" к команде, чтобы получить порцию зауми побольше. "; }
        }

        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            var needLotsOfStuff = !string.IsNullOrEmpty(args) && args.Contains("напалмом");

            sendMessageFunc(RetrieveSmartAssStuff(needLotsOfStuff), AnswerBehaviourType.Text);
        }

        private string RetrieveSmartAssStuff(bool needLotsOfStuff)
        {
            var result = "ААААРРРГХ!!! Что-то в моей голове! В МОЕЙ ГОЛОВЕ!!!";

            try
            {
                var referat = GetReferatText();

                result = GetReferatPart(referat, needLotsOfStuff);
            }
            catch (Exception ex) { }

            return result;
        }

        private string GetReferatText()
        {
            var client = new WebClient();
            var rawResult = client.DownloadData(Query);
            var chars = new char[rawResult.Length];

            Encoding.UTF8.GetDecoder().GetChars(rawResult, 0, rawResult.Length, chars, 0);

            return new string(chars);
        }

        private string GetReferatPart(string referatText, bool isLong)
        {
            var parts = referatText.Split(new[] { "<p>" }, StringSplitOptions.RemoveEmptyEntries);
            var entry = parts[1].Replace("</p>", string.Empty);

            return isLong ? entry : entry.Split('.').First();
        }
    }
}
