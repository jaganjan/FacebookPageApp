using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace FacebookApp.Lib.Base
{
    /// <summary>
    /// Class ServiceConsumer.
    /// </summary>
    public class ServiceConsumer
    {
        /// <summary>
        /// Enum ServiceRequestType
        /// </summary>
        private enum ServiceRequestType
        {
            /// <summary>
            /// The get
            /// </summary>
            GET,
            /// <summary>
            /// The post
            /// </summary>
            POST,
            /// <summary>
            /// The head
            /// </summary>
            HEAD
        }

        /// <summary>
        /// Gets the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>HttpWebResponse.</returns>
        /// <exception cref="System.Exception"></exception>
        public static HttpWebResponse Get(string requestUrl, int timeoutInSeconds = 100)
        {
            HttpWebResponse response = null;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;

                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                if (request != null)
                {
                    request.Method = ServiceRequestType.GET.ToString();
                    if (timeoutInSeconds != 100)
                    {
                        request.Timeout = timeoutInSeconds*1000;
                    }

                    response = request.GetResponse() as HttpWebResponse;
                }
                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");


                    /* throw new MXServiceConsumerException(Response.StatusCode, Response.StatusDescription, requestURL, ServiceRequestType.GET.ToString()); */
                }
            }
            catch (Exception ex)
            {
                response?.Close();
                throw ex;


                /* throw new MXServiceConsumerException(HttpStatusCode.InternalServerError, ex.Message, requestURL, ServiceRequestType.GET.ToString(), inner: ex); */
            }

            return response;
        }

        /// <summary>
        /// Posts the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="content">The content.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>HttpWebResponse.</returns>
        /// <exception cref="System.Exception"></exception>
        public static HttpWebResponse Post(string requestUrl, object content, string contentType = null,
            int timeoutInSeconds = 100)
        {
            HttpWebResponse response = null;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;
                ServicePointManager.DefaultConnectionLimit = 50;
                ServicePointManager.Expect100Continue = false;

                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                if (request != null)
                {
                    request.Proxy = null;
                    request.Method = ServiceRequestType.POST.ToString();
                    if (!string.IsNullOrWhiteSpace(contentType))
                    {
                        request.ContentType = contentType;
                    }
                    if (timeoutInSeconds != 100)
                    {
                        request.Timeout = timeoutInSeconds*1000;
                    }

                    if (content != null)
                    {
                        var isContentSerialized = content is string;
                        using (var writer = new StreamWriter(request.GetRequestStream()))
                        {
                            writer.Write(isContentSerialized ? content : JsonConvert.SerializeObject(content));
                            writer.Close();
                        }
                    }
                    else
                    {
                        request.ContentLength = 0;
                    }

                    response = (HttpWebResponse) request.GetResponse();
                }
                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");


                    /* throw new MXServiceConsumerException(Response.StatusCode, Response.StatusDescription, requestURL, ServiceRequestType.POST.ToString(), content, contentType); */
                }
            }
            catch (Exception ex)
            {
                response?.Close();
                throw ex;


                /* throw new MXServiceConsumerException(HttpStatusCode.InternalServerError, ex.Message, requestURL, ServiceRequestType.POST.ToString(), content, contentType, inner: ex); */
            }

            return response;
        }

        /// <summary>
        /// Heads the specified request URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>HttpWebResponse.</returns>
        /// <exception cref="System.Exception"></exception>
        public static HttpWebResponse Head(string requestUrl, int timeoutInSeconds = 100)
        {
            HttpWebResponse response = null;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;

                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                if (request != null)
                {
                    request.Method = ServiceRequestType.HEAD.ToString();

                    if (timeoutInSeconds != 100)
                    {
                        request.Timeout = timeoutInSeconds*1000;
                    }
                    response = request.GetResponse() as HttpWebResponse;
                }
                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");


                    /* throw new MXServiceConsumerException(Response.StatusCode, Response.StatusDescription, requestURL, ServiceRequestType.HEAD.ToString()); */
                }
            }
            catch (Exception ex)
            {
                response?.Close();
                throw ex;


                /* throw new MXServiceConsumerException(HttpStatusCode.InternalServerError,ex.Message, requestURL, ServiceRequestType.HEAD.ToString(),inner:ex); */
            }

            return response;
        }
    }
}