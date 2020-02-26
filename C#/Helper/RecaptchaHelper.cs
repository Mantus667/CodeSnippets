using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RecaptchaHelper
{
    public static class RecaptchaHelper
    {
        public const string FormParamResponse = "g-recaptcha-response";

        public class RecaptchaVerifyResponse
        {
            public bool Success { get; set; }

            [JsonProperty("challenge_ts")]
            public DateTime ChallengeTimestamp { get; set; }

            public string HostName { get; set; }
        }

        public static RecaptchaVerifyResponse VerifyVisitor(string response, string secretKey, string ipAddress = null)
        {
            var values = new Dictionary<string, string>
            {
               { "secret", secretKey },
               { "response", response },
               { "remoteip", ipAddress }
            };
            var content = new FormUrlEncodedContent(values);

            string responseString = null;
            using (var httpClient = new HttpClient())
            {
                Task.Run(async () =>
                {
                    var responseMessage = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                    responseString = await responseMessage.Content.ReadAsStringAsync();
                }).Wait();
            }

            return JsonConvert.DeserializeObject<RecaptchaVerifyResponse>(responseString);
        }
    }
}