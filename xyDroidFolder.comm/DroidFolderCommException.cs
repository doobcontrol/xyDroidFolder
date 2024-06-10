using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xySoft.comm;

namespace xyDroidFolder.comm
{
    public class DroidFolderCommException : Exception
    {
        private DroidFolderCommErrorCode _errorCode;

        public DroidFolderCommException(DroidFolderCommErrorCode errorCode, 
            string message) : base(message)
        {
            _errorCode = errorCode;
        }

        public DroidFolderCommErrorCode ErrorCode => _errorCode;
    }
}
