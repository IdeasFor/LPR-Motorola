using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpr
{
    public class XmlParser
    {
        public static string GetCDATAValue(string xmlData)
        {
            int pi = xmlData.IndexOf("[CDATA[");
            int pf = xmlData.IndexOf("]]");
            string CDATA = xmlData.Substring(pi+7, (pf-pi)-7);
            
            return CDATA;
        }
    }
}
