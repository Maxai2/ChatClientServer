using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatClientServer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string conDisConIp;
        public string ConDisConIp
        {
            get { return conDisConIp; }
            set { conDisConIp = value; OnChanged(); }
        }

        private string textMessage;
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

        Dictionary<bool, SolidColorBrush> color;
        Dictionary<bool, HorizontalAlignment> alignment;

        bool rightLeft = false;

        //----------------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

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

            lbMessages.ScrollIntoView(lbMessages.Items.Cast<ListBoxItem>().Last());

            rightLeft = !rightLeft;

            tbUser.Clear();
        }

        //----------------------------------------------------------------------------

        

        //----------------------------------------------------------------------------

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

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
