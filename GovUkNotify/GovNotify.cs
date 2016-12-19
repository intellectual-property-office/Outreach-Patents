using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace GovUkNotify
{
    public class GovNotify : INotifications
    {
        private const string BASE_URL = "https://api.notifications.service.gov.uk";
        public string Secret { get; private set; }
        public string ServiceId { get; private set; }

        /// <summary>
        /// Use https://jwt.io/ for debugging
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="serviceId"></param>
        public GovNotify(string apiKey, string serviceId)
        {
            Secret = apiKey;
            ServiceId = serviceId;
        }

        /// <summary>
        /// </summary>
        /// <param name="json">json string</param>
        /// <returns></returns>
        public PostResponse SendSms(string json)
        {
            const string url = BASE_URL + "/notifications/sms";

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + GetAuthToken();

                var result = client.UploadString(url, "POST", json);
                return JsonConvert.DeserializeObject<PostResponse>(result);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>GovGetResponse object</returns>
        public GetResponse GetNotification(string id)
        {
            var url = BASE_URL + "/notifications/" + id;

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + GetAuthToken();

                var result = client.DownloadString(url);
                return JsonConvert.DeserializeObject<GetResponse>(result);
            }
        }

        #region Json Web Token

        private string GetAuthToken()
        {
            // Payload
            const string header = "{\"typ\":\"JWT\",\"alg\":\"HS256\"}";
            var claims = "{\"iss\":\"" + ServiceId + "\",\"iat\":" + ToUnixTime(DateTime.Now) + "}";
            var b64Header = B64EncodedString(header);
            var b64Claims = B64EncodedString(claims);
            var payload = b64Header + "." + b64Claims;

            // Generate signature
            var key = Encoding.UTF8.GetBytes(Secret);
            var message = Encoding.UTF8.GetBytes(payload);
            var sig = Convert.ToBase64String(HashHmac(key, message))
                 .Replace('+', '-')
                 .Replace('/', '_')
                 .Replace("=", "");

            return payload + "." + sig;
        }

        private static string B64EncodedString(string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data))
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        private static byte[] HashHmac(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        private static long ToUnixTime(DateTime dateTime)
        {
            return (int)(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        #endregion
    }
}
