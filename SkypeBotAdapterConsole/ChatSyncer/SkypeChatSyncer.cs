using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeBotAdapterConsole.ChatSyncer
{
    public class SkypeChatSyncer
    {
        private List<ChatSyncerRelation> chatSyncRelations { get; set; }

        public delegate void OnSendMessageRequiredDelegate(string message, string toSkypeId);
        public event OnSendMessageRequiredDelegate OnSendMessageRequired;

        public SkypeChatSyncer()
        {
            chatSyncRelations = new List<ChatSyncerRelation>();
            LoadRelations();
        }

        private void LoadRelations()
        {
            string configValue = ConfigurationManager.AppSettings["ChatSyncRelations"];
            if (!string.IsNullOrEmpty(configValue))
            {
                var relations = configValue.Split(';');
                foreach (string relation in relations)
                {
                    var relationParts = relation.Split('|');
                    if (relationParts.Length == 3)
                    {
                         
                        var fromChatIdParts = relationParts[0];
                        var toChatIdParts = relationParts[2];
                        
                        string relationOperator = relationParts[1];
                        if (relationOperator == "from")
                        {
                            string tempChatId = fromChatIdParts;
                            fromChatIdParts = toChatIdParts;
                            toChatIdParts = tempChatId;
                        }
                        chatSyncRelations.Add(new ChatSyncerRelation(fromChatIdParts, toChatIdParts));

                        if (relationOperator == "both")
                        {
                            //reverse
                            chatSyncRelations.Add(new ChatSyncerRelation(toChatIdParts,fromChatIdParts));
                        }
                    }
                }
            }
        }

        public void HandleMessage(string message, string fromName, string fromChatId)
        {
            var sendToChats = chatSyncRelations.Where(x => x.FromChatId == fromChatId).ToList();
            foreach (var chat in sendToChats)
            {
                string formattedMessage = string.Format("[{0}]{1} : {2}", chat.FromChatName, fromName, message);
                if (OnSendMessageRequired != null)
                {
                    OnSendMessageRequired(formattedMessage, chat.TochatId);
                }
            }
            
        }
    }
}
