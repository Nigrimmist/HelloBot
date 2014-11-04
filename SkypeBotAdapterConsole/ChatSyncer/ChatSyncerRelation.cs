using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeBotAdapterConsole.ChatSyncer
{
    public class ChatSyncerRelation
    {
        public string FromChatId { get; set; }
        public string TochatId { get; set;}
        public string FromChatName { get; set; }

        public ChatSyncerRelation(string fromChatId, string tochatId)
        {
            var fromChatParts = fromChatId.Split(':');
            var toChatParts = tochatId.Split(':');
            FromChatId = fromChatParts[0];
            TochatId = toChatParts[0];
            FromChatName = fromChatParts[1];
        }
    }
}
