using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using HelloBotCommunication;
using HelloBotCommunication.ClientDataInterfaces;
using HelloBotModuleHelper;
using HtmlAgilityPack;
using Nigrimmist.Modules.Helpers;

namespace Nigrimmist.Modules.Commands
{
    public class SkypeChatSyncModule : IActionHandler
    {
        public List<string> CallCommandList{get { return new List<string>(){"get id"}; }}
        public string CommandDescription { get { return @"Возвращает id чата."; } }
        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            var skypeData = clientData as ISkypeData;
            if (skypeData != null)
            {
                sendMessageFunc(skypeData.ChatId);
            }
        }
    }
}
