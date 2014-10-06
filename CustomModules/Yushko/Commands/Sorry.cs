using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using HelloBotCommunication;

namespace Yushko.Commands
{
    class SlugPost {
        public SlugPost() {}
        public String slug { get; set; }
        public String text { get; set; }
        public String html { get; set; }
        public int viewsCount { get; set; }
        public bool isApproved { get; set; }
        public bool isInVk { get; set; }
        public bool result { get; set; }
        public String title { get; set; }
        public String socialTitle { get; set; }
    }

     class Slug {
        public String indexSlug { get; set; }
        public SlugPost post;
        public String nextSlug { get; set; }
    }

    public class Sorry : IActionHandler
    {
        public List<string> CallCommandList {
            get { return new List<string>() { "простите" }; }
        }
        private string NextSlug;

        /// <summary>
        /// Sending GET request.
        /// </summary>
        /// <param name="Url">Request Url.</param>
        /// <param name="Data">Data for request.</param>
        /// <returns>Response body.</returns>
        public static string HTTP_GET(string Url, string Data)
        {
            string Out = String.Empty;
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + (string.IsNullOrEmpty(Data) ? "" : "?" + Data));
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            try
            {
                System.Net.WebResponse resp = req.GetResponse();
                using (System.IO.Stream stream = resp.GetResponseStream())
                {
                    
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, Encoding.ASCII))
                    {
                        Out = sr.ReadToEnd();
                        sr.Close();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Out = string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message);
            }
            catch (WebException ex)
            {
                Out = string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Out = string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message);
            }

            return Out;
        }

        public string CommandDescription { get { return @"казнить нельзя помиловать"; } }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            string url = "https://prostite.com";
            string resp = HTTP_GET(url + "/" + NextSlug,"");
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            Slug slugobj= json_serializer.Deserialize<Slug>(resp);
            NextSlug = slugobj.nextSlug;
            sendMessageFunc(slugobj.post.text);
        }
    }
}
