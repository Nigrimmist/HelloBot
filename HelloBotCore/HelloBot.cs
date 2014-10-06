using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HelloBotCommunication;

namespace HelloBotCore
{
    public class HelloBot
    {
        private  List<IActionHandler> handlers = new List<IActionHandler>();
        private IDictionary<string, Tuple<string, Func<string>>> systemCommands;
        private string dllMask { get; set; }
        private string botCommandPrefix;
        private int commandTimeoutSec;

        /// <summary>
        /// Bot costructor
        /// </summary>
        /// <param name="dllMask">File mask for retrieving client command dlls</param>
        /// <param name="botCommandPrefix">Prefix for bot commands. Only messages with that prefix will be handled</param>
        public HelloBot(string dllMask = "*.dll", string botCommandPrefix = "!")
        {
            this.dllMask = dllMask;
            this.botCommandPrefix = botCommandPrefix;
            this.commandTimeoutSec = 60;

            systemCommands = new Dictionary<string, Tuple<string, Func<string>>>()
            {
                {"help", new Tuple<string, Func<string>>("список системных команд", GetSystemCommands)},
                {"modules", new Tuple<string, Func<string>>("список кастомных модулей", GetUserDefinedCommands)},
            };
            RegisterModules();
        }

        private void RegisterModules()
        {
            handlers = GetHandlers();
        }

        protected virtual List<IActionHandler> GetHandlers()
        {
            List<IActionHandler> toReturn = new List<IActionHandler>();
            var dlls = Directory.GetFiles(".", dllMask);
            var i = typeof(IActionHandlerRegister);
            foreach (var dll in dlls)
            {
                var ass = Assembly.LoadFile(Environment.CurrentDirectory + dll);

                //get types from assembly
                var typesInAssembly = ass.GetTypes().Where(x => i.IsAssignableFrom(x)).ToList();

                foreach (Type type in typesInAssembly)
                {
                    object obj = Activator.CreateInstance(type);
                    var clientHandlers = ((IActionHandlerRegister)obj).GetHandlers();
                    foreach (IActionHandler handler in clientHandlers)
                    {
                        if (handler.CallCommandList.Any())
                        {
                            toReturn.Add(handler);
                        }
                    }
                }
            }
            
            return toReturn;
        }

        public void HandleMessage(string incomingMessage, Action<string> answerCallback, object data)
        {
            if (incomingMessage.StartsWith(botCommandPrefix))
            {
                incomingMessage = incomingMessage.Substring(botCommandPrefix.Length);
                var argsSpl = incomingMessage.Split(' ');

                var command = argsSpl[0];

                var systemCommandList = systemCommands.Where(x => x.Key.ToLower() == command.ToLower()).ToList();
                if (systemCommandList.Any())
                {
                    var systemComand = systemCommandList.First();
                    answerCallback(systemComand.Value.Item2());
                }
                else
                {
                    
                    IActionHandler handler = FindHandler(incomingMessage,out command);
                    
                    if (handler!=null)
                    {
                        string args = incomingMessage.Substring((command).Length).Trim();
                        
                        IActionHandler hnd = handler;
                        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(commandTimeoutSec));
                        var token = cts.Token;
                        
                        Task.Run(() =>
                        {
                            using (cts.Token.Register(Thread.CurrentThread.Abort))
                            {
                                try
                                {
                                    hnd.HandleMessage(args, data, answerCallback);
                                }
                                catch (Exception ex)
                                {
                                    if (OnErrorOccured != null)
                                    {
                                        OnErrorOccured(ex);
                                    }
                                    answerCallback(command + " пал смертью храбрых :(");
                                }
                            }
                            
                        },token);

                    }
                }
            }
        }

        public delegate void onErrorOccuredDelegate(Exception ex);
        public event onErrorOccuredDelegate OnErrorOccured;

        private IActionHandler FindHandler(string phrase, out string command)
        {
            IActionHandler toReturn = null;
            command = string.Empty;
            List<string> foundCommands = new List<string>();
            foreach (var actionHandler in handlers)
            {
                foreach (var com in actionHandler.CallCommandList)
                {
                    if (phrase.StartsWith(com, StringComparison.OrdinalIgnoreCase))
                    {
                        var args = phrase.Substring(com.Length);
                        if (string.IsNullOrEmpty(args) || args.StartsWith(" "))
                        foundCommands.Add(com);
                    }
                }
            }

            if (foundCommands.Any())
            {
                string foundCommand = foundCommands.OrderByDescending(x => x).First();
                toReturn = handlers.FirstOrDefault(x => x.CallCommandList.Contains(foundCommand,StringComparer.OrdinalIgnoreCase));
                if (toReturn != null)
                {
                    command = foundCommand;
                }
            }
            
            return toReturn;
        }

        private string GetSystemCommands()
        {
            return String.Join(Environment.NewLine, systemCommands.Select(x => String.Format("!{0} - {1}", x.Key, x.Value.Item1)).ToList());
        }

        private string GetUserDefinedCommands()
        {
            return String.Join(Environment.NewLine, handlers.Select(x => String.Format("{0} - {1}", string.Join(" / ", x.CallCommandList.Select(y => botCommandPrefix + y)), x.CommandDescription)).ToList());
        }
    }

   
}
