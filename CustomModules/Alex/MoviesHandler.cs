using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HelloBotCommunication;


namespace SmartAssHandlerLib
{
    public class MoviesHandler : IActionHandler
    {
        public List<CallCommandInfo> CallCommandList
        {
            get
            {
                return new List<CallCommandInfo>()
                {
                    new CallCommandInfo("кино" )
                };
            }
        }

        public string CommandDescription
        {
            get { return string.Empty; }
        }

        public void HandleMessage(string command, string args, object clientData, Action<string, AnswerBehaviourType> sendMessageFunc)
        {
            var request = BuildRequestUrl(args);
            var rawData = GetRawData(request);
            var description = ParseMovieData(rawData);

            sendMessageFunc(description.ToString(), AnswerBehaviourType.Text);
        }

        private MovieDescription ParseMovieData(string rawData)
        {
            return new MovieDescription();
        }

        private string GetRawData(string location)
        {
            var client = new WebClient();

            var rawResult = client.DownloadData(location);
            var chars = new char[rawResult.Length];

            Encoding.UTF8.GetDecoder().GetChars(rawResult, 0, rawResult.Length, chars, 0);

            return new string(chars);
        }

        private string BuildRequestUrl(string args)
        {
            return string.Empty;
        }

        private class MovieDescription
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Genre { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                return string.Format("Название: {0}{1}Год выпуска: {2}{3}Жанр: {4}{5}Описание: \"{6}\"",
                    Title,
                    Environment.NewLine,
                    Year,
                    Environment.NewLine,
                    Genre,
                    Environment.NewLine,
                    Description);
            }
        }
    }
}
