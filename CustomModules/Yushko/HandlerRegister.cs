using System.Collections.Generic;
using HelloBotCommunication;
using Yushko.Commands;

namespace Yushko
{
    public class HandlerRegister: IActionHandlerRegister
    {
        public List<IActionHandler> GetHandlers()
        {
            return new List<IActionHandler>()
            {
                new ExchangeRate(),
                new Sorry(),
                new Horoscope(),
                new Moon(),
                new Kstati()
            };
        }
    }
}
