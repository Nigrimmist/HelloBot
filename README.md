HelloBot
========

Personal/group assistant with skype integration. .NET/C#

#Description

HelloBot is a small code-behind project that contains C# action modules integrated with client example (skype). One module = one action/command with output result in text format. Sound simple, right? You can write your own client using HelloBot core and our documentation behind.


#Getting Started

- **Adding modules and commands**

For adding your own command handlers you should implement two interfaces in your dll : **IActionHandler** and **IActionHandlerRegister**. They are placed in HelloBotCommunication.dll (HelloBotCommunication project).

**1.**  Implementing IActionHandler you should define CallCommandList, CommandDescription properties and HandleMessage method. CallCommandList should return List of pre-defined command names. Each defined command name will fire you HandleMessage method.

Handled message (your answer) should be returned using sendMessageFunc callback. clientData param will contains additional data from client. All possible client interfaces placed in HelloBotCommunication/ClientDataInterfaces folder.


**2.** Implementing IActionHandlerRegister you should define list of your IActionHandler modules.

####Sample of Calculator command
```C#
//Calculate command handler
public class Calculator : IActionHandler
    {

        public List<string> CallCommandList { get { return new List<string>() { "calc","calculator" }; } }
        public string CommandDescription { get { return "Clever calculator using NCalc library"; }  }
        
        //that method will fired when anybody will call your command from client
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
            
             //That class will register your modules

        }
    }
    
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
