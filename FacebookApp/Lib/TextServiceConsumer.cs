using System.IO;
using System.Net;
using System.Text;
using FacebookApp.Lib.Base;

namespace FacebookApp.Lib
{
    /// <summary>
    /// Class TextServiceConsumer.
    /// </summary>
    public class TextServiceConsumer
    {
        /// <summary>
        /// Gets the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>System.String.</returns>
        public static string Get(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetTextResponse(ServiceConsumer.Get(requestUrl, timeoutInSeconds));
        }

        /// <summary>
        /// Posts the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="content">The content.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>System.String.</returns>
        public static string Post(string requestUrl, string content, string contentType = null,
            int timeoutInSeconds = 100)
        {
            contentType = string.IsNullOrWhiteSpace(contentType) ? "text/plain; charset=utf-8" : contentType;
            return GetTextResponse(ServiceConsumer.Post(requestUrl, content, contentType, timeoutInSeconds));
        }

        /// <summary>
        /// Heads the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>System.String.</returns>
        public static string Head(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetTextResponse(ServiceConsumer.Head(requestUrl, timeoutInSeconds));
        }

        /// <summary>
        /// Gets the text response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>System.String.</returns>
        public static string GetTextResponse(HttpWebResponse response)
        {
            string textResponse = null;

            if (response == null) return null;
            using (response)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                using (var responseReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    textResponse = responseReader.ReadToEnd();
                    responseReader.Close();
                }
                response.Close();
            }

            return textResponse;
        }
    }
}