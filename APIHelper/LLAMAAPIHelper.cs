using DataGen.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace DataGen.APIHelper
{
        public static class LLAMAAPIHelper
        {
            private static string UserName;
            private static string Password;
            private static string authenticationUrl;
            private static string apiUrl;
            private static IConfiguration _configuration;
            static HttpClient client = new HttpClient();
            static void  Initializer()
            {
            
                UserName = _configuration.GetSection("aPIUsername").Value;
                Password = _configuration.GetSection("aPIPassword").Value;
                authenticationUrl = _configuration.GetSection("apiAuthenticationUrl").Value;
                apiUrl = _configuration.GetSection("apiLLAMAUrl").Value;
            }
            public static async Task<string> getToken()
            {
                string? responseToken = String.Empty;

                FormUrlEncodedContent formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", UserName),
                    new KeyValuePair<string, string>("password", Password)
                });
                
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, authenticationUrl))
                {
                    request.Headers.Accept.Clear();
                    request.Content = formContent;

                    HttpResponseMessage responseMessage =   await client.SendAsync(request);
                    responseMessage.EnsureSuccessStatusCode();

                    responseToken = await responseMessage.Content.ReadAsStringAsync();

                    Root root = JsonConvert.DeserializeObject<Root>(responseToken) ?? new Root();

                    responseToken = root.access_token;
                }

                return responseToken??"" ;
            }

        public static async Task<string> PromptingLLAMA(string prompt, string token)
        {
            string answer = String.Empty;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var formData = new FormUrlEncodedContent(new[]
            {
               new KeyValuePair<string,string>("prompt",prompt)
            });

            using(HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl))
            {
                requestMessage.Headers.Accept.Clear();
                requestMessage.Content = formData;

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
                responseMessage.EnsureSuccessStatusCode();

                answer = await responseMessage.Content.ReadAsStringAsync();

            }
            

            return answer;
        }

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            Initializer();
        }
    }
}
