using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HelloBotCore;
using SKYPE4COMLib;
using SkypeBotAdapterConsole.ChatSyncer;

namespace SkypeBotAdapterConsole
{
    class Program
    {
        private static Skype skype = new Skype();
        private static HelloBot bot;
        private static SkypeChatSyncer chatSyncer;
        private static IDictionary<string,IChat> chats { get; set; }
        private static object _chatLocker = new object();

        static void Main(string[] args)
        {
            bot = new HelloBot();
            bot.OnErrorOccured += BotOnErrorOccured;
            Task.Run(delegate
            {
                try
                {
                    skype.MessageStatus += OnMessageReceived;
                    skype.Attach(5, true);
                    chatSyncer = new SkypeChatSyncer();
                    chatSyncer.OnSendMessageRequired += ChatSyncerOnOnSendMessageRequired;
                    Console.WriteLine("skype attached");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("top lvl exception : " + ex.ToString());
                }
                while (true)
                {
                    Thread.Sleep(1000);
                }
            });
            
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void ChatSyncerOnOnSendMessageRequired(string message, string toSkypeId)
        {
            var chat = GetChatById(toSkypeId);
            if (chat != null)
            {
                SendMessage(message,chat);
            }
        }

        private static IChat GetChatById(string chatId)
        {
            if (chats == null)
            {
                lock (_chatLocker)
                {
                    if (chats == null)
                    {
                        chats = new Dictionary<string, IChat>();
                        foreach (IChat chat in skype.Chats)
                        {
                            string tChatId = chat.Name.Split(';').Last();
                            chats.Add(tChatId,chat);
                        }
                    }
                }
            }

            IChat toReturn = null;
            chats.TryGetValue(chatId, out toReturn);

            return toReturn;
        }

        static void BotOnErrorOccured(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        private static void OnMessageReceived(ChatMessage pMessage, TChatMessageStatus status)
        {
            Console.WriteLine(status + pMessage.Body);
 
            if (status == TChatMessageStatus.cmsReceived)
            {
                bot.HandleMessage(pMessage.Body, answer => SendMessage(answer,pMessage.Chat),new SkypeData(pMessage));

                string fromChatId = pMessage.Chat.Name.Split(';').Last();
                chatSyncer.HandleMessage(pMessage.Body,pMessage.FromDisplayName,fromChatId);
            }
        }
        

        public static object _lock = new object();
        private static void SendMessage(string message, IChat toChat)
        {
            if (message.StartsWith("/"))
            {
                message = "(heidy) " + message;
            }
                lock (_lock)
                {
                    toChat.SendMessage(message);
                }
        }
    }

    

    

    
}
