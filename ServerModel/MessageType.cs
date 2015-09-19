using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerModel
{
    public enum MessageType
    {
        ConnectionCheckFromServer = 1,
        ConnectionCheckFromClient = 2,
        FromServerWithTask = 3,
        FromClientToClient = 4,
        FromClientWithResult = 5
    }
}
