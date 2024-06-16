using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public static class CommStringEncode
    {
        static Dictionary<string, string> encodeDic = new Dictionary<string, string>()
        {
            {",", "xyCommA" },
            { "=", "xyEquaL" }
        };

        static Dictionary<string, string> dncodeDic = new Dictionary<string, string>()
        {
            {"xyCommA", "," },
            { "xyEquaL", "=" }
        };

        static public string encodeParString(string parString)
        {
            return stringReplace(parString, encodeDic);
        }
        static public string decodeParString(string parString)
        {
            return stringReplace(parString, dncodeDic);
        }
        static public string stringReplace(string rString,
            Dictionary<string, string> rDic)
        {
            string retString = rString;

            foreach (string key in rDic.Keys)
            {
                retString = retString.Replace(key, rDic[key]);
            }

            return retString;
        }
    }
}
