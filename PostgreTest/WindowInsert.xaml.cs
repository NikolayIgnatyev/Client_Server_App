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
            tbName.Text = name;
            tbName.IsReadOnly = true;
        }

        void SendMessageFromSocket(int port)
        {
            byte[] bytes = new byte[1024];
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipEndPoint);

            string message = "insert;" + tbName.Text + "," + tbAge.Text;

            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            string reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            MessageBox.Show(reply);
            if (reply.Contains("inserted"))
            {
                this.Close();
            }
            else
            {
                tbAge.Text = "";
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
    }
}
