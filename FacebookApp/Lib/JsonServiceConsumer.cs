using System.IO;
using System.Net;
using FacebookApp.Lib.Base;
using Newtonsoft.Json;

namespace FacebookApp.Lib
{
    /// <summary>
    /// Class JsonServiceConsumer.
    /// </summary>
    public class JsonServiceConsumer
    {
        /// <summary>
        /// Gets the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>System.Object.</returns>
        public static object Get(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetJsonResponse(ServiceConsumer.Get(requestUrl, timeoutInSeconds));
        }

        /// <summary>
        /// Posts the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="content">The content.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>System.Object.</returns>
        public static object Post(string requestUrl, object content, string contentType = null,
            int timeoutInSeconds = 100)
        {
            contentType = string.IsNullOrWhiteSpace(contentType) ? "application/json; charset=utf-8" : contentType;
            return GetJsonResponse(ServiceConsumer.Post(requestUrl, content, contentType, timeoutInSeconds));
        }

        /// <summary>
        /// Heads the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>System.Object.</returns>
        public static object Head(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetJsonResponse(ServiceConsumer.Head(requestUrl, timeoutInSeconds));
        }

        /// <summary>
        /// Gets the json response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>System.Object.</returns>
        public static object GetJsonResponse(HttpWebResponse response)
        {
            object jsonResponse = null;

            if (response == null) return null;
            using (response)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                using (var responseReader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResponse = JsonConvert.DeserializeObject(responseReader.ReadToEnd());
                    responseReader.Close();
                }
                response.Close();
            }

            return jsonResponse;
        }
    }
}