using System.Collections.Generic;


using HelloBotCommunication;
using Nigrimmist.Modules.Commands;
using Nigrimmist.Modules.WindowsCommands;

namespace Nigrimmist.Modules
{

    public class DllRegister : IActionHandlerRegister
    {
        public List<IActionHandler> GetHandlers()
        {
            return new List<IActionHandler>()
            {
                new Calculator(),
                new Weather(),
                new Boobs(),
                new Say(),
                new Translate(),
                new Bash(),
                new ItHappens(),
                new WhatIsIt(),
                new Map(),
                new LangExecuter(),
                new Advice(),
                new OrModule(),
                new Quote(),
                new ShortLink(),
                new SkypeChatSyncModule(),
                new BrowserUrlsOpenModule()
            };
        }
    }
}
