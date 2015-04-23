using System.IO;
using System.Net;
using Newtonsoft.Json;
using System;

namespace FacebookApp.Lib.Base
{
    public class ServiceConsumer
    {
        private enum ServiceRequestType { GET, POST, HEAD }

        #region Public Methods

        public static HttpWebResponse Get(string requestURL, int timeoutInSeconds = 100)
        {
            HttpWebResponse Response = null;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                HttpWebRequest Request = WebRequest.Create(requestURL) as HttpWebRequest;
                Request.Method = ServiceRequestType.GET.ToString();
                if (timeoutInSeconds != 100) { Request.Timeout = timeoutInSeconds * 1000; }

                Response = Request.GetResponse() as HttpWebResponse;
                if (Response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", Response.StatusCode, Response.StatusDescription));

                   
                    /* throw new MXServiceConsumerException(Response.StatusCode, Response.StatusDescription, requestURL, ServiceRequestType.GET.ToString()); */
                }
            }
            catch (Exception ex)
            {
                if (Response != null)
                {
                    Response.Close();
                }
                throw ex;

              
                /* throw new MXServiceConsumerException(HttpStatusCode.InternalServerError, ex.Message, requestURL, ServiceRequestType.GET.ToString(), inner: ex); */
            }

            return Response;
        }

        public static HttpWebResponse Post(string requestURL, object content, string contentType = null, int timeoutInSeconds = 100)
        {
            HttpWebResponse Response = null;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                ServicePointManager.DefaultConnectionLimit = 50;
                ServicePointManager.Expect100Continue = false;

                HttpWebRequest Request = WebRequest.Create(requestURL) as HttpWebRequest;
                Request.Proxy = null;
                Request.Method = ServiceRequestType.POST.ToString();
                if (!string.IsNullOrWhiteSpace(contentType)) { Request.ContentType = contentType; }
                if (timeoutInSeconds != 100) { Request.Timeout = timeoutInSeconds * 1000; }

                if (content != null)
                {
                    bool IsContentSerialized = (content.GetType() == typeof(System.String)) ? true : false;
                    using (StreamWriter Writer = new StreamWriter(Request.GetRequestStream()))
                    {
                        Writer.Write(IsContentSerialized ? content : JsonConvert.SerializeObject(content));
                        Writer.Close();
                    }
                }
                else
                {
                    Request.ContentLength = 0;
                }

                Response = (HttpWebResponse)Request.GetResponse();
                if (Response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", Response.StatusCode, Response.StatusDescription));

                   
                    /* throw new MXServiceConsumerException(Response.StatusCode, Response.StatusDescription, requestURL, ServiceRequestType.POST.ToString(), content, contentType); */
                }
            }
            catch (Exception ex)
            {
                if (Response != null)
                {
                    Response.Close();
                }
                throw ex;

               
                /* throw new MXServiceConsumerException(HttpStatusCode.InternalServerError, ex.Message, requestURL, ServiceRequestType.POST.ToString(), content, contentType, inner: ex); */
            }

            return Response;
        }

        public static HttpWebResponse Head(string requestURL, int timeoutInSeconds = 100)
        {
            HttpWebResponse Response = null;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                HttpWebRequest Request = WebRequest.Create(requestURL) as HttpWebRequest;
                Request.Method = ServiceRequestType.HEAD.ToString();

                if (timeoutInSeconds != 100) { Request.Timeout = timeoutInSeconds * 1000; }
                Response = Request.GetResponse() as HttpWebResponse;
                if (Response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", Response.StatusCode, Response.StatusDescription));

                   
                    /* throw new MXServiceConsumerException(Response.StatusCode, Response.StatusDescription, requestURL, ServiceRequestType.HEAD.ToString()); */
                }
            }
            catch (Exception ex)
            {
                if (Response != null)
                {
                    Response.Close();
                }
                throw ex;

                
                /* throw new MXServiceConsumerException(HttpStatusCode.InternalServerError,ex.Message, requestURL, ServiceRequestType.HEAD.ToString(),inner:ex); */
            }

            return Response;
        }

        #endregion
    }
}