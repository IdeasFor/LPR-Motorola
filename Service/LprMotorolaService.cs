using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Net.WebRequestMethods;
using Lpr.Factory;

namespace Lpr.Service
{
    public class LprMotorolaService
    {
        private string _url;
        private string _login;
        private string _senha;
        private string _basicAuth;
        private HttpFactory _http;
        private int _shortPort;
        private int _longPort;

        private Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

        private static LprMotorolaService _instance = null;
        
        protected LprMotorolaService(HttpFactory http, string urlBase, string login, string senha, int shortPort, int longPort)
        {
            _url = urlBase;
            _shortPort = shortPort;
            _longPort = longPort;
            _http = http;
            _login = login;
            _senha = senha;
            _basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{senha}"));
            keyValuePairs.Add("Authorization", $"Basic {_basicAuth}");
            keyValuePairs.Add("Keep-Alive", "30000");

        }

        /// <summary>
        /// Singleton: inicia uma nova instância
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void Init(HttpFactory http, string urlBase, string login, string senha, int shortPort, int longPort)
        {            
            _instance = new LprMotorolaService(http, urlBase, login, senha, shortPort, longPort);
        }

        public static LprMotorolaService Instance()
        {
            return _instance;            
        }

        public string GetDeviceInfo()
        {
            var response = _http.GetAsync(url: $"{_url}:{_shortPort}/GetDeviceInfo", headers: keyValuePairs);
            response.Wait();
            return response.Result;
        }

        public string SetSubscribe(int keepAlive)
        {
            string body = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<config version=\"1.7\" xmlns=\"http://www.ipc.com/ver10\">\r\n<types>\r\n<openAlramObj>\r\n<enum>MOTION</enum>\r\n<enum>SENSOR</enum>\r\n<enum>PEA</enum>\r\n<enum>AVD</enum>\r\n<enum>OSC</enum>\r\n<enum>CPC</enum>\r\n<enum>CDD</enum>\r\n<enum>IPD</enum>\r\n<enum>VFD</enum>\r\n<enum>VFD_MATCH</enum>\r\n<enum>VEHICE</enum>\r\n<enum>AOIENTRY</enum>\r\n<enum>AOILEAVE</enum>\r\n<enum>PASSLINECOUNT</enum>\r\n<enum>TRAFFIC</enum>\r\n</openAlramObj>\r\n<subscribeRelation>\r\n<enum>ALARM</enum>\r\n<enum>FEATURE_RESULT</enum>\r\n<enum>ALARM_FEATURE</enum>\r\n</subscribeRelation>\r\n<subscribeTypes>\r\n<enum>BASE_SUBSCRIBE</enum>\r\n<enum>REALTIME_SUBSCRIBE</enum>\r\n<enum>STREAM_SUBSCRIBE</enum>\r\n</subscribeTypes>\r\n</types>\r\n<channelID type=\"uint32\">0</channelID>\r\n<initTermTime type=\"uint32\">0</initTermTime>\r\n<subscribeFlag type=\"subscribeTypes\">BASE_SUBSCRIBE</subscribeFlag>\r\n<subscribeList type=\"list\" count=\"1\">\r\n<item>\r\n<smartType type=\"openAlramObj\">VEHICE</smartType>\r\n<subscribeRelation type=\"subscribeRelation\">ALARM_FEATURE</subscribeRelation>\r\n</item>\r\n</subscribeList>\r\n</config>\r\n";

            var response = _http.PostAsync(url: $"{_url}:{_longPort}/SetSubscribe", body, keyValuePairs, "applicaton/xml");
            response.Wait();
            return response.Result;
        }

        public string GetEvent(string urlEvent)
        {
            var response = _http.GetAsync($"{urlEvent}", keyValuePairs);
            response.Wait();
            return response.Result;
        }
    }
}
