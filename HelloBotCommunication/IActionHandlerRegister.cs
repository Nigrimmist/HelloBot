using System.Collections.Generic;


namespace HelloBotCommunication
{
    public interface IActionHandlerRegister
    {
        List<IActionHandler> GetHandlers();
    }
}
