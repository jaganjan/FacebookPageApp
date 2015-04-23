using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace FacebookApp.Lib
{
    public class JsonServiceConsumer
    {
        #region Public Methods

        public static object Get(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetJsonResponse(Base.ServiceConsumer.Get(requestUrl, timeoutInSeconds));
        }

        public static object Post(string requestUrl, object content, string contentType = null, int timeoutInSeconds = 100)
        {
            contentType = string.IsNullOrWhiteSpace(contentType) ? "application/json; charset=utf-8" : contentType;
            return GetJsonResponse(Base.ServiceConsumer.Post(requestUrl, content, contentType, timeoutInSeconds));
        }

        public static object Head(string requestUrl, int timeoutInSeconds = 100)
        {
            return GetJsonResponse(Base.ServiceConsumer.Head(requestUrl, timeoutInSeconds));
        }

        public static object GetJsonResponse(HttpWebResponse response)
        {
            object JsonResponse = null;

            if (response != null)
            {
                using (response)
                {
                    using (StreamReader ResponseReader = new StreamReader(response.GetResponseStream()))
                    {
                        JsonResponse = JsonConvert.DeserializeObject(ResponseReader.ReadToEnd());
                        ResponseReader.Close();
                    }
                    response.Close();
                }
            }

            return JsonResponse;
        }

        #endregion
    }
}