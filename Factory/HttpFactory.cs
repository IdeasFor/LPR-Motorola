using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lpr.Factory
{
    public class HttpFactory
    {
        /// <summary>
        /// Realiza um POST para a url especificada
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeOut">tempo de espera em segundos, -1 = default</param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, string body, Dictionary<string, string> headers = null, string contentType = "application/json", int timeOut = -1)
        {
            string responseBody = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();

                // se timer um timeout
                if (timeOut != -1) httpClient.Timeout = TimeSpan.FromSeconds(timeOut);

                // adiciona os demais cabeçalhos
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                    }
                }
                var httpContentBoby = new StringContent(body, Encoding.UTF8, contentType);
                using (HttpResponseMessage responseMessage = await httpClient.PostAsync(url, httpContentBoby))
                using (HttpContent responseContent = responseMessage.Content)
                {
                    responseBody = await responseContent.ReadAsStringAsync();
                }
            }

            return responseBody;
        }

        /// <summary>
        /// Realiza um PUT para a url especificada
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<string> PutAsync(string url, string body, Dictionary<string, string> headers = null, string contentType = "application/json", int timeOut = -1)
        {
            string responseBody = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();

                // se timer um timeout
                if (timeOut != -1) httpClient.Timeout = TimeSpan.FromSeconds(timeOut);

                // adiciona os demais cabeçalhos
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                    }
                }
                var httpContentBoby = new StringContent(body, Encoding.UTF8, contentType);
                using (HttpResponseMessage responseMessage = await httpClient.PutAsync(url, httpContentBoby))
                using (HttpContent responseContent = responseMessage.Content)
                {
                    responseBody = await responseContent.ReadAsStringAsync();
                }
            }
            return responseBody;
        }

        /// <summary>
        /// Realiza um GET para a url espeficada
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parametros"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url, Dictionary<string, string> parametros = null, Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded", int timeOut = -1, bool v2 = true)
        {
            string respnseString = null;

            // cria os parâmetros 
            if (parametros != null)
            {
                bool mais = false;
                foreach (var p in parametros)
                {
                    if (v2)
                    {
                        url = url.Replace($":{p.Key}", p.Value);
                    }
                    else
                    {
                        if (mais) url += "&";
                        url += string.Format("?{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value));
                        mais = true;
                    }
                }
            }

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient
                    .DefaultRequestHeaders
                    .Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                // se timer um timeout
                if (timeOut != -1) httpClient.Timeout = TimeSpan.FromSeconds(timeOut);

                // adiciona os demais cabeçalhos
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                    }
                }
                using (HttpResponseMessage responseMessage = await httpClient.GetAsync(url))
                using (HttpContent responseContent = responseMessage.Content)
                {
                    respnseString = await responseContent.ReadAsStringAsync();
                }
            }
            return respnseString;

        }

    }
}
