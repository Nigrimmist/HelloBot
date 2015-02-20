using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using HelloBotCommunication;
using HelloBotModuleHelper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    /// <summary>
    /// Generate short link for argument url
    /// </summary>
    public class ShortLink : IActionHandler
    {
        public List<CallCommandInfo> CallCommandList
        {
            get
            {
                return new List<CallCommandInfo>()
                {
                    new CallCommandInfo("сократи" ),
                };
            }
        }

       
        public string CommandDescription { get { return @"Сокращалка ссылок"; } }

        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            string answer = args.ToShortUrl();
            sendMessageFunc(answer, AnswerBehaviourType.Text);
        }
    }
}
