using System.Collections.Generic;
using HelloBotCommunication;


namespace SmartAssHandlerLib
{
    public class VariousHandlersRegister : IActionHandlerRegister
    {
        public List<IActionHandler> GetHandlers()
        {
            return new List<IActionHandler>()
            {
                new SmartAssStuffHandler(),
                new YesNoHandler(),
                new FckinWeatherHandler()
            };
        }
    }
}
