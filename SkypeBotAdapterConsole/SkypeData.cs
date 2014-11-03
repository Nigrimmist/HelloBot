using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloBotCommunication.ClientDataInterfaces;
using SKYPE4COMLib;

namespace SkypeBotAdapterConsole
{
    public class SkypeData : ISkypeData
    {
        public string FromName { get; set; }
        public string ChatId { get; set; }

        public SkypeData(ChatMessage pMessage)
        {
            FromName = pMessage.FromDisplayName;
            ChatId = pMessage.Chat.Name.Split(';').Last();
        }
    }
}
