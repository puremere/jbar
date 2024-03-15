using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;

namespace jbar.Classes
{
    public static class methods
    {
        private static string baseServer = "https://localhost:44389/api/app";// "https://jbar.app/api/app";
        public static string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static async  Task<T2>  PostData<T,T2> (T model, T2 model2,string url, string token) 
        {
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseServer +url);
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
                string content = model != null ? JsonConvert.SerializeObject(model) : "";
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var rsp = await client.PostAsync("", byteContent).ConfigureAwait(false);
                string final =  await rsp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T2>(final);
            }
        }
        
    }
}