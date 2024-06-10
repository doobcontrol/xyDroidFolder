using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xySoft.comm
{
    public class XySoftCommException : Exception
    {
        private XyCommErrorCode _errorCode;

        public XySoftCommException(XyCommErrorCode errorCode, string message)
            : base(message)
        {
            _errorCode = errorCode;
        }

        public XyCommErrorCode ErrorCode => _errorCode;
    }
}
