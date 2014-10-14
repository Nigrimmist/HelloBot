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
        public List<string> CallCommandList
        {
            get { return new List<string>() { "сократи"}; }
        }
        public string CommandDescription { get { return @"Случайная цитата"; } }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            sendMessageFunc(args.ToShortUrl());
        }
    }
}
