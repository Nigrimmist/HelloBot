using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HelloBotCommunication;
using HtmlAgilityPack;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class Boobs : IActionHandler
    {
        public List<string> CallCommandList { get { return new List<string>() { "сиськи","boobs","с" }; } }
        public string CommandDescription { get { return "Ну а что тут объяснять. Сиськи."; } }
        private Random r = new Random();
        public List<string> Images = new List<string>(); 

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            if (!Images.Any())
            {
                HtmlReaderManager hrm = new HtmlReaderManager();
                hrm.Get("http://boobs-selfshots.tumblr.com/page/" + r.Next(1, 600));
                string html = hrm.Html;
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var divs = htmlDoc.DocumentNode.SelectNodes(@"//./div[@class='photo_post']/a/img");
                StringBuilder sb = new StringBuilder();
                foreach (var div in divs)
                {
                    Images.Add(div.Attributes["src"].Value);
                }
            }
            int rPos = r.Next(0, Images.Count - 1);
            string url = Images[rPos];
            Images.RemoveAt(rPos);
            sendMessageFunc("Только тссс... никому. Ок? "+url);
        }


    }
}
