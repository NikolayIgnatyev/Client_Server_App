using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace PostgreTest
{
    /// <summary>
    /// Логика взаимодействия для WindowInsert.xaml
    /// </summary>
    public partial class WindowInsert : Window
    {
        public WindowInsert(string name)
        {
            InitializeComponent();
            tbNickname.Text = name;
            tbNickname.IsReadOnly = true;
        }

        void SendMessageFromSocket(int port)
        {
            byte[] bytes = new byte[1024];
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipEndPoint);
            byte[] avatarByte = null;
            string message = null;
            if (imgAvatar.Source != null)
            {
                avatarByte = Converter.ConvertFromImage(imgAvatar.Source as BitmapImage);
                message = $"INSERT;{tbName.Text},{tbAge.Text},{tbNickname.Text},{tbLevel.Text},{tbSale.Text},image";
            }
            else
            {
                message = $"INSERT;{tbName.Text},{tbAge.Text},{tbNickname.Text},{tbLevel.Text},{tbSale.Text}";
            }


            byte[] msg = Encoding.UTF8.GetBytes(message);

            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            string reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            if (reply.Contains("NEED_IMAGE"))
            {
                sender.Send(avatarByte);
            }
            bytesRec = sender.Receive(bytes);

            reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine(reply);
            if (reply.Contains("inserted"))
            {
                MessageBox.Show("Inserted complete");
                this.Close();
            }
            else
            {
                MessageBox.Show(reply);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendMessageFromSocket(11000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgAvatar.Source = new BitmapImage(new Uri(op.FileName));
            }
        }
    }
}
