using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nigrimmist.Modules.Helpers
{
    /// <summary>
    /// Encapsulates WebReader functions.
    /// </summary>
    public class HtmlReaderManager
    {
        public HtmlReaderManager()
        {
        }

        public HtmlReaderManager(string baseUri)
        {
            BaseUri = baseUri;
        }

        private X509Certificate _certificate;

        public X509Certificate Certificate
        {
            get { return _certificate; }
            set { _certificate = value; }
        }

        private string _baseUri = string.Empty;

        private float _pageSize;

        public float PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public string LastPostLocation { get; set; }

        public string BaseUri
        {
            get { return _baseUri; }
            set { _baseUri = value; }
        }

        private string _previousUri;

        public string PreviousUri
        {
            get { return _previousUri; }
            set { _previousUri = value; }
        }

        private CookieContainer _cookieContainer = new CookieContainer();

        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set { _cookieContainer = value; }
        }

        private string _userAgent =
            @"Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9.1.2) Gecko/20090729 Firefox/3.5.2";

        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        private string _accept =
            @"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/vnd.ms-powerpoint, */*";

        public string Accept
        {
            get { return _accept; }
            set { _accept = value; }
        }

        private Uri _requestUri;

        public Uri RequestUri
        {
            get { return _requestUri; }
            set { _requestUri = value; }
        }

        private string _responseUri;

        public string ResponseUri
        {
            get { return _responseUri; }
            set { _responseUri = value; }
        }

        private string _contentType = string.Empty;

        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        private IWebProxy _proxy;

        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        private string _html;

        public string Html
        {
            get { return _html; }
        }

        private Hashtable _headers = new Hashtable();

        public Hashtable Headers
        {
            get { return _headers; }
        }

        private string _location;

        public string Location
        {
            get { return _location; }
        }

        private bool _sendReferer = true;

        public bool SendReferer
        {
            get { return _sendReferer; }
            set { _sendReferer = value; }
        }

        private HttpStatusCode _statusCode;

        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        private int _timeout;

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public Encoding Encoding { get; set; }

        public HttpStatusCode Request(string requestUri, string method, string postData)
        {
            string uri = requestUri;

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);

            if (Proxy != null)
                request.Proxy = Proxy;

            request.CookieContainer = CookieContainer;
            request.UserAgent = UserAgent;
            request.Accept = Accept;
            request.Headers.Add("Accept-Language", "ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3");

            request.Method = method;
            request.KeepAlive = true;

            if (SendReferer)
                request.Referer = PreviousUri != null ? PreviousUri : uri;

            foreach (string key in Headers.Keys)
                request.Headers.Add(key, Headers[key].ToString());

            if (method == "POST")
            {
                request.ContentType = string.IsNullOrEmpty(ContentType) ? "application/x-www-form-urlencoded" : ContentType;
                request.AllowAutoRedirect = false;
            }
            else
            {
                request.ContentType = ContentType;
                request.AllowAutoRedirect = true;
            }

            PreviousUri = uri;

            if (Certificate != null)
                request.ClientCertificates.Add(Certificate);

            if (Timeout != 0)
                request.Timeout = Timeout;

            if (postData != null)
            {
                using (Stream st = request.GetRequestStream())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postData);
                    st.Write(bytes, 0, bytes.Length);
                }
            }

            _html = "";

            using (HttpWebResponse resp = (HttpWebResponse) request.GetResponse())
            using (Stream sm = resp.GetResponseStream())
            using (StreamReader sr = new StreamReader(sm, Encoding??Encoding.UTF8))
            {
                _statusCode = resp.StatusCode;
                _location = resp.Headers["Location"];


                _html = sr.ReadToEnd();
                //UTF8Encoding encoding = new UTF8Encoding();

                //Byte[] bytes = encoding.GetBytes(_html);
                //float pageSizeInMb = (bytes.Length/1024f)/1024f;
                //PageSize = pageSizeInMb;


                if (resp.ResponseUri.AbsoluteUri.StartsWith(BaseUri) == false)
                    BaseUri = resp.ResponseUri.Scheme + "://" + resp.ResponseUri.Host;

                _responseUri = resp.ResponseUri.ToString();

                CookieCollection cc = request.CookieContainer.GetCookies(request.RequestUri);

                // This code fixes the situation when a server sets a cookie without the 'path'.
                // IE takes this as the root ('/') value,
                // the HttpWebRequest class as the RequestUri.AbsolutePath value.
                //
                foreach (Cookie c in cc)
                {
                    if (c.Path == request.RequestUri.AbsolutePath)
                    {
                        CookieContainer.Add(new Cookie(c.Name, c.Value, "/", c.Domain));
                    }
                }
            }

            RequestUri = request.RequestUri;

            return StatusCode;
        }

        public HttpStatusCode Get(string requestUri)
        {
            return Request(requestUri, "GET", null);
        }

        public HttpStatusCode Post(string requestUri, string postData)
        {
            Request(requestUri, "POST", postData);

            for (int i = 0; i < 10; i++)
            {
                bool post = false;

                switch (StatusCode)
                {
                    case HttpStatusCode.MultipleChoices: // 300
                    case HttpStatusCode.MovedPermanently: // 301
                    case HttpStatusCode.Found: // 302
                    case HttpStatusCode.SeeOther: // 303
                        break;

                    case HttpStatusCode.TemporaryRedirect: // 307
                        post = true;
                        break;

                    default:
                        return StatusCode;
                }
                if (Location != null)
                    LastPostLocation = Location;
                if (Location == null)
                    break;

                Uri uri = new Uri(new Uri(PreviousUri), Location);

                //BaseUri = uri.Scheme + "://" + uri.Host;
                //requestUri = uri.AbsolutePath + uri.Query;

                Request(requestUri, post ? "POST" : "GET", post ? postData : null);
            }

            return StatusCode;
        }

        public void LoadCertificate(string fileName)
        {
            Certificate = X509Certificate.CreateFromCertFile(fileName);
        }

        public bool TryCheckFileSizeByUrl(string url,int maxFileSizeBytes, out int contentLength, out string contentType, string _methodToCheck = "HEAD")
        {
            contentType = string.Empty;
            contentLength = 0;
            try
            {
                HttpWebRequest req = System.Net.HttpWebRequest.Create(url) as HttpWebRequest;
                req.Method = _methodToCheck;
                req.CookieContainer = CookieContainer;
                req.UserAgent = UserAgent;
                req.Accept = Accept;
                req.Headers.Add("Accept-Language", "ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3");
                
                
                req.KeepAlive = true;
                switch (_methodToCheck)
                {
                    case "HEAD" :
                    {
                        using (System.Net.WebResponse resp = req.GetResponse())
                        {
                            contentType = resp.Headers.Get("Content-Type");
                            int.TryParse(resp.Headers.Get("Content-Length"), out contentLength);
                        }
                        break;
                    }

                    case "GET":
                    {
                        //get first n bytes
                        req.AddRange(0, maxFileSizeBytes - 1 + 1);
                        using (System.Net.WebResponse resp = req.GetResponse())
                        {
                            using (Stream stream = resp.GetResponseStream())
                            {
                                byte[] buffer = new byte[1024];
                                int totalBytesRead = 0;
                                if (stream != null)
                                {
                                    int read;
                                    do
                                    {
                                        read = stream.Read(buffer, 0, 1024);
                                        totalBytesRead += read;
                                        if (totalBytesRead >= maxFileSizeBytes)
                                        {
                                            contentLength = maxFileSizeBytes+1;
                                            return false;
                                        }
                                    } while (read!=0);
                                    contentLength = totalBytesRead;
                                    contentType = "image/gif";
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        break;
                    }
                }
                
            }
            catch(Exception ex)
            {
                var err = ex.ToString().ToLower();
                if ((err.Contains("method not allowed") || err.Contains("(403) forbidden")) && _methodToCheck == "HEAD")
                {
                    return TryCheckFileSizeByUrl(url,maxFileSizeBytes, out contentLength, out contentType, "GET");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="str">output stream. must be in using</param>
        /// <returns></returns>
        public  bool TryDownloadFileByUrl(string url, Stream str, out Exception ex)
        {
            ex = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response;
            request.Timeout = 1000 * 15; //15s
            
            request.CookieContainer = CookieContainer;
            request.UserAgent = UserAgent;
            request.Accept = Accept;
            request.Headers.Add("Accept-Language", "ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3");
            try
            {
                response = (HttpWebResponse) request.GetResponse();
            }
            catch (Exception exc)
            {
                ex = exc;
                return false;
            }

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                 response.StatusCode == HttpStatusCode.Moved ||
                 response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download it
                using (Stream inputStream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        str.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
                return true;
            }
            else
                return false;
        }


    }
}
