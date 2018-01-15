using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Http
{
    public static class HttpUtil
    {
        private static HttpClient client;

        public static HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient();
                }
                return client;
            }
        }

        public static async Task<T> JsonGet<T>(String url)
        {
            String json = await Client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<String> StringGet(String url)
        {
            return await Client.GetStringAsync(url);
        }

        private static int BoolTotal(bool[] flags)
        {
            int total = 0;
            foreach (bool b in flags)
            {
                if (b) total++;
            }
            return total;

        }

        public static async Task<T> JsonPost<T>(String url, IEnumerable<KeyValuePair<String, String>> headers = null, String body = null, HttpContent content = null,
            IEnumerable<KeyValuePair<String, String>> form = null, object data = null)
        {
            HttpContent returnContent = CreateContent(headers, body, content, form, data);
            return await JsonPostContent<T>(url, returnContent);
        }

        public static async Task<String> StringPost(String url, IEnumerable<KeyValuePair<String, String>> headers = null, String body = null, HttpContent content = null,
            IEnumerable<KeyValuePair<String, String>> form = null, object data = null)
        {

            HttpContent returnContent = CreateContent(headers, body, content, form, data);
            return await StringPostContent(url, returnContent);

        }

        public static HttpContent CreateContent(IEnumerable<KeyValuePair<String, String>> headers = null, String body = null, HttpContent content = null,
            IEnumerable<KeyValuePair<String, String>> form = null, object data = null)
        {
            HttpContent returnContent = null;

            if (BoolTotal(new bool[] { content != null, form != null, body != null, data != null }) > 1)
            {
                throw new ArgumentException("Too many post options.  Please use only content, form, body, or data");
            }

            if (data != null)
            {
                String json = JsonConvert.SerializeObject(data);
                returnContent = new StringContent(json, Encoding.UTF8, "application/json");
                
            }
            else if (content != null)
            {
                returnContent = content;
            }
            else if (form != null)
            {
                returnContent = new FormUrlEncodedContent(form);
            }
            else
            {
                returnContent = new StringContent(body == null ? "" : body);
            }

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    returnContent.Headers.Add(kv.Key, kv.Value);
                }
            }

            return returnContent;
        }

        public static async Task<String> StringPostContent(String url, HttpContent content)
        {
            HttpResponseMessage message = await Client.PostAsync(url, content);

            return await message.Content.ReadAsStringAsync();
        }

        public static async Task<T> JsonPostContent<T>(String url, HttpContent content)
        {
            String json = await StringPostContent(url, content);
            return JsonConvert.DeserializeObject<T>(json);

        }
    }


    /*public static T MakeJsonRequest<T>(String url)
    {
        HttpWebResponse wr = MakeRequest(url);
        return wr.GetJsonData<T>();

    }

    public static async Task<T> MakeJsonRequestAsync<T>(String url)
    {
        HttpWebResponse wr = await MakeRequestAsync(url);
        return await wr.GetJsonDataAsync<T>();

    }


    public static HttpWebResponse MakeRequest(String url)
    {
        HttpWebRequest wr = HttpWebRequest.Create(url) as HttpWebRequest;
        return wr.GetResponseNoException();

    }
    public static async Task<HttpWebResponse> MakeRequestAsync(String url)
    {
        HttpWebRequest wr = HttpWebRequest.Create(url) as HttpWebRequest;
        return await wr.GetResponseNoExceptionAsync();

    }


    public static async Task<HttpWebResponse> MakePostRequestAsync(String url, String body)
    {
        HttpWebRequest wr = HttpWebRequest.Create(url) as HttpWebRequest;

        return await wr.GetResponseNoExceptionAsync();

    }

    public static async Task<T> GetJsonDataAsync<T>(this HttpWebResponse resp)
    {
        Stream stream = resp.GetResponseStream();
        try
        {
            String data = await new StreamReader(stream).ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(data);
        }
        catch (Exception)
        {
            return default(T);
        }
    }
    public static T GetJsonData<T>(this HttpWebResponse resp)
    {
        Stream stream = resp.GetResponseStream();
        try
        {
            String data = new StreamReader(stream).ReadToEnd();
            return JsonConvert.DeserializeObject<T>(data);
        }
        catch (Exception)
        {
            return default(T);
        }
    }


    public static HttpWebResponse GetResponseNoException(this HttpWebRequest req)
    {
        try
        {
            return (HttpWebResponse)req.GetResponse();
        }
        catch (WebException we)
        {
            var resp = we.Response as HttpWebResponse;
            if (resp == null)
                throw;
            return resp;
        }

    }
    public static async Task<HttpWebResponse> GetResponseNoExceptionAsync(this HttpWebRequest req)
    {
        try
        {
            return (HttpWebResponse)await req.GetResponseAsync();
        }
        catch (WebException we)
        {
            var resp = we.Response as HttpWebResponse;
            if (resp == null)
                throw;
            return resp;
        }

    }*/
}

