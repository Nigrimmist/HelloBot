using System;
using System.Collections.Generic;

namespace HelloBotCommunication
{
    public interface IActionHandler
    {
        /// <summary>
        /// Call command list.
        /// </summary>
        List <string> CallCommandList { get;}

        /// <summary>
        /// Will be displayed in !modules list
        /// </summary>
        string CommandDescription { get; }


        /// <summary>
        /// Event will be fired for your Command
        /// </summary>
        /// <param name="args">Command arguments, can be empty</param>
        /// <param name="clientData"></param>
        /// <param name="sendMessageFunc">Call it when your answer will be ready.</param>
        void HandleMessage(string args, object clientData, Action<string> sendMessageFunc);
    }
}