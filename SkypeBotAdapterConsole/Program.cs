using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HelloBotCore;
using SKYPE4COMLib;

namespace SkypeBotAdapterConsole
{
    class Program
    {
        private static Skype skype = new Skype();
        private static HelloBot bot;

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

        static void BotOnErrorOccured(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        private static void OnMessageReceived(ChatMessage pMessage, TChatMessageStatus status)
        {
            Console.WriteLine(status + pMessage.Body);
 
            if (status == TChatMessageStatus.cmsReceived)
            {
                bot.HandleMessage(pMessage.Body, answer => SendMessage(answer,pMessage.Chat),
                    new SkypeData(){FromName = pMessage.FromDisplayName});
            }
        }
        

        public static object _lock = new object();
        private static void SendMessage(string message, Chat toChat)
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
