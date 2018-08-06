using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDLL
{
    [Serializable]
    public class Client
    {
        public string NickName { get; set; }
        public List<string> Messages { get; set; }
    }
}
