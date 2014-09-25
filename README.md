HelloBot
========

.NET/C# Personal/group program assistant with any kind of client integration (Skype for example).

#Description

HelloBot is a small code-behind project that contains C# action modules integrated with any kind of client (Skype). One module = one action that will be called by command with output result in text format. Sound simple, right? You can write your own client using HelloBot core and our documentation behind.

#Getting Started
####Adding modules and commands


For adding your own command handlers you should implement two interfaces in your dll : **IActionHandler** and **IActionHandlerRegister**. They are placed in HelloBotCommunication.dll (HelloBotCommunication project).

**1.**  Implementing IActionHandler you should define CallCommandList, CommandDescription properties and HandleMessage method. CallCommandList should return List of pre-defined command names. Each defined command name will fire your HandleMessage method.

Handled message (your answer) should be returned using sendMessageFunc callback. clientData param will contains additional data from client. All possible client interfaces placed in HelloBotCommunication/ClientDataInterfaces folder.


**2.** Implementing IActionHandlerRegister you should define list of your IActionHandler modules.

- **Sample of Calculator command**
```C#
//Calculate command handler
public class Calculator : IActionHandler
    {

        public List<string> CallCommandList { get { return new List<string>() { "calc","calculator" }; } }
        public string CommandDescription { get { return "Clever calculator using NCalc library"; }  }
        
        //that method will be fired when anybody will call your command from client
        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            
            //calculate expression passing arguments to Expression NCalc's class constructor.
            //args can be, for example, "1+2/3". You should check it for valid format in context of you command.
            Expression expr = new Expression(args);
            
            var exprAnswer = expr.Evaluate();
            string messageAuthor = string.Empty;
            string answer = string.Empty;
            
            //check for clientData if you require some specific data from client
            var skypeData = clientData as ISkypeData;
            if (skypeData!=null)
            {
                messageAuthor = skypeData.FromName;
            }

            if (!string.IsNullOrEmpty(messageAuthor))
            {
                answer = string.Format("Mr {0}, answer is : {1}", messageAuthor, exprAnswer);
            }
            else
            {
                answer = string.Format("Answer is : {0}", exprAnswer);
            }
            
            //send answer back to client
            sendMessageFunc(answer);
        }
    }
```

And another one class for register your calculator handler :

```C#
//That class will register your modules
   public class DllRegister : IActionHandlerRegister
   {
       public List<IActionHandler> GetHandlers()
       {
           return new List<IActionHandler>()
           {
               new Calculator(),
               //and others if exist
           };
       }
   }
```

After that you can simply put your dll to bin folder of client application and it should work.

- **Adding client functionality**

Client is a program, that will use HelloBot with modules. Let's show example with skype client. 

####Sample of Skype client integration (console application)
```C#
    class Program
    {
        
        private static Skype skype = new Skype();
        private static HelloBot bot;

        static void Main(string[] args)
        {
            
            bot = new HelloBot(); //init HelloBot
            bot.OnErrorOccured += BotOnErrorOccured; //event subscribing for handling any unexpectable exceptions
            
            //running in separate thread for unlock ui thread
            Task.Run(delegate
            {
                try
                {
                    
                    skype.MessageStatus += OnMessageReceived; //subscribe to skype message status change
                    
                    skype.Attach(5, true); //attach to ui version of skype. Note : now, Microsoft was remove api                                                   support, it's work only with old portable version of skype. 
                    Console.WriteLine("skype attached");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("top lvl exception : " + ex.ToString());
                }
                
                //freeze thread
                while (true)
                {
                    Thread.Sleep(1000);
                }
            });
            
            //freeze main thread
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
                //get answer from HelloBot. Answer will be received by SendMessage method.
                bot.HandleMessage(pMessage.Body, answer => SendMessage(answer,pMessage.Chat),
                    new SkypeData(){FromName = pMessage.FromDisplayName});
            }
        }
        

        public static object _lock = new object();
        private static void SendMessage(string message, Chat toChat)
        {
                lock (_lock)
                {
                    toChat.SendMessage(message);
                }
        }
    }
```

###Supported system commands
- {commandPrefix}help - show list of system commands
- {commandPrefix}modules - show custom module list
