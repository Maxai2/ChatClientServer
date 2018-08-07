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
        static List<Client> clients;

        //---------------------------------------------------------------------

        static void Main(string[] args)
        {
            clients = new List<Client>();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

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

                    if (length != 0)
                    {
                        var sendArr = ParseMsg(msg);

                        socket.SendTo(sendArr, client);
                    }
                    else
                        break;
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
                    if (clients.Count == 0)
                        clients.Add(new Client() { NickName = variable });
                    else
                    {
                        var result = clients.FirstOrDefault((c) => c.NickName == variable);

                        if (result == null)
                            clients.Add(new Client() { NickName = variable });
                    }
                    break;
                case "Send":
                    var nk = variable.Substring(0, variable.LastIndexOf(':'));
                    var text = variable.Substring(variable.LastIndexOf(':') + 1);

                    var client = clients.FirstOrDefault((c) => c.NickName == nk);

                    if (client.Messages == null)
                        client.Messages = new List<string>() { text };
                    else
                        client.Messages.Add(text);

                    break;
            }

            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();

            binFormatter.Serialize(mStream, clients);

            return mStream.ToArray();
        }
    }
}
