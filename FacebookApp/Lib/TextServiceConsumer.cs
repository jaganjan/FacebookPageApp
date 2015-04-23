using System.IO;
using System.Net;
using System.Text;

namespace FacebookApp.Lib
{
    public class TextServiceConsumer
    {
        #region Public Methods

        public static string Get(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetTextResponse(Base.ServiceConsumer.Get(requestUrl, timeoutInSeconds));
        }

        public static string Post(string requestUrl, string content, string contentType = null, int timeoutInSeconds = 100)
        {
            contentType = string.IsNullOrWhiteSpace(contentType) ? "text/plain; charset=utf-8" : contentType;
            return GetTextResponse(Base.ServiceConsumer.Post(requestUrl, content, contentType, timeoutInSeconds));
        }

        public static string Head(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetTextResponse(Base.ServiceConsumer.Head(requestUrl, timeoutInSeconds));
        }

        public static string GetTextResponse(HttpWebResponse response)
        {
            string TextResponse = null;

            if (response != null)
            {
                using (response)
                {
                    using (StreamReader ResponseReader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                    {
                        TextResponse = ResponseReader.ReadToEnd();
                        ResponseReader.Close();
                    }
                    response.Close();
                }
            }

            return TextResponse;
        }

        #endregion
    }
}