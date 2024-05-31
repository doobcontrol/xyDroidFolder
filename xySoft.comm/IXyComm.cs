using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xySoft.comm
{
    public interface IXyComm
    {
        Task<string> sendForResponseAsync(string sendData);
        void startListen();
        void set(Dictionary<string, string> setDic);
    }

    public delegate string
        XyCommRequestHandler(string sendData);
}
