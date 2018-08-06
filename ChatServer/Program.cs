using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ClientDLL;

namespace ChatServer
{
    class Program
    {
        static Socket socket;
        static EndPoint ep;
        static List<Client> clients = new List<Client>();

        //---------------------------------------------------------------------

        static void Main(string[] args)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

            socket.Bind(ep);

            var SocketLength = socket.ReceiveBufferSize - 100;

            var bytes = new byte[SocketLength];

            while (true)
            {
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    var length = socket.ReceiveFrom(bytes, ref client);
                    var msg = Encoding.Default.GetString(bytes, 0, length);

                    var sendArr = ParseMsg(msg);

                    socket.SendTo(sendArr, client);
                }
            }
        }

        //---------------------------------------------------------------------

        static byte[] ParseMsg(string msg)
        {
            var mode = msg.Substring(0, msg.IndexOf(':'));

            var variable = msg.Substring(msg.IndexOf(':') + 1);

            switch (mode)
            {
                case "Connect":
                    clients.Add(new Client()
                    {
                        NickName = variable
                    });

                    var binFormatter = new BinaryFormatter();
                    var mStream = new MemoryStream();

                    binFormatter.Serialize(mStream, clients);

                    return mStream.ToArray();
                case "Send":

                    return null;
            }

            return null;
        }
    }
}
