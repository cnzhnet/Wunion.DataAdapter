using System;
using System.Collections.Generic;
using System.Linq;

namespace Wunion.DataAdapter.NetCore.Demo.Models
{
    public class ResultMessage
    {
        public ResultMessage()
        { }

        public int ResultCode
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string DataFormat
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }
    }
}
