using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClientDLL;

namespace ChatClientServer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string nickName = "";
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; OnChanged(); }
        }

        private string textMessage = "";
        public string TextMessage
        {
            get { return textMessage; }
            set { textMessage = value; OnChanged(); }
        }

        private Visibility conButVis;
        public Visibility ConButVis
        {
            get { return conButVis; }
            set { conButVis = value; OnChanged(); }
        }

        private Visibility disconButVis = Visibility.Collapsed;
        public Visibility DisconButVis
        {
            get { return disconButVis; }
            set { disconButVis = value; OnChanged(); }
        }


        private ICommand connectCom;
        public ICommand ConnectCom
        {
            get
            {
                if (connectCom is null)
                {
                    connectCom = new RelayCommand(
                        (param) =>
                        {
                            var msg = "Connect:" + NickName;
                            var data = Encoding.Default.GetBytes(msg);
                            socket.SendTo(data, ep);

                            var answer = new byte[socket.ReceiveBufferSize];

                            var length = socket.Receive(answer);

                            if (length != 0)
                            {
                                var mStream = new MemoryStream();
                                var binFormatter = new BinaryFormatter();

                                mStream.Write(answer, 0, length);
                                mStream.Position = 0;

                                var tempCol = binFormatter.Deserialize(mStream) as List<Client>;

                                Clients.Clear();

                                foreach (var item in tempCol)
                                {
                                    Clients.Add(item.NickName);
                                }
                            }
                        });
                }

                return connectCom;
            }
        }

        private ICommand disconnectCom;
        public ICommand DisconnectCom
        {
            get
            {
                if (disconnectCom is null)
                {
                    disconnectCom = new RelayCommand(
                        (param) =>
                        {

                        });
                }

                return disconnectCom;
            }
        }

        public ObservableCollection<object> MessageList { get; set; }
        public ObservableCollection<string> Clients { get; set; }

        Dictionary<bool, SolidColorBrush> color;
        Dictionary<bool, HorizontalAlignment> alignment;

        bool rightLeft = false;

        Socket socket;
        EndPoint ep;

        //----------------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

            MessageList = new ObservableCollection<object>();
            Clients = new ObservableCollection<string>();

            color = new Dictionary<bool, SolidColorBrush>()
            {
                { false, new SolidColorBrush(Colors.White)},
                { true, new SolidColorBrush(Color.FromRgb(220, 248, 198))}
            };

            alignment = new Dictionary<bool, HorizontalAlignment>()
            {
                { false, HorizontalAlignment.Left},
                { true, HorizontalAlignment.Right}
            };
        }

        //----------------------------------------------------------------------------

        void listFill(string str)
        {
            MessageList.Add(new ListBoxItem()
            {
                HorizontalAlignment = alignment[rightLeft],
                IsTabStop = false,
                Tag = str,

                Content = new Border()
                {
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(10),
                    Background = color[rightLeft],

                    Child = new TextBlock
                    {
                        Text = ' ' + str + ' ',
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 15,
                        FontFamily = new FontFamily("Segoe Print")
                    }
                }
            });

            //lbMessages.ScrollIntoView(lbMessages.Items.Cast<ListBoxItem>().Last());

            rightLeft = !rightLeft;

            TextMessage = "";
        }

        //----------------------------------------------------------------------------

        

        //----------------------------------------------------------------------------

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //----------------------------------------------------------------------

        private void CloseButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //----------------------------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
