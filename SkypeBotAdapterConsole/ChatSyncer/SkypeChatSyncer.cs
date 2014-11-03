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
                        string fromChatId = relationParts[0];
                        string toChatId = relationParts[2];
                        string relationOperator = relationParts[1];
                        if (relationOperator == "<")
                        {
                            string tempChatId = fromChatId;
                            fromChatId = toChatId;
                            toChatId = tempChatId;
                        }
                        chatSyncRelations.Add(new ChatSyncerRelation()
                        {
                            FromChatId = fromChatId,
                            TochatId = toChatId
                        });

                        if (relationOperator == "<>")
                        {
                            //reverse
                            chatSyncRelations.Add(new ChatSyncerRelation()
                            {
                                FromChatId = toChatId,
                                TochatId = fromChatId
                            });
                        }
                    }
                }
            }
        }
    }
}
