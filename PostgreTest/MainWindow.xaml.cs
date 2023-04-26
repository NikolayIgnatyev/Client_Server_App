using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using static System.Collections.Specialized.BitVector32;
using System.IO;

namespace PostgreTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void SendMessageFromSocket(int port)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];
            byte[] avatar = new byte[262144];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            string message = "SEARCH;" + tbsender.Text;

            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            string reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);


            if (reply.Contains("Error 404"))
            {
                if (MessageBox.Show("Таких данных нет в таблице.\nХотите добавить?", "Нет данных", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    WindowInsert windowInsert = new WindowInsert(tbsender.Text);
                    windowInsert.ShowDialog();
                }
            }
            else
            {
                
                string[] dataReply = reply.Split(',');
                tblName.Text = dataReply[0];
                tblAge.Text = dataReply[1];
                tblNick.Text = dataReply[2];
                tblLevel.Text = dataReply[3];
                tblSale.Text = dataReply[4];
                Console.WriteLine(dataReply[4]);
                if (dataReply[5] == "IMAGE")
                {
                    Console.WriteLine(dataReply[5]);
                    message = "IMAGE";

                    msg = Encoding.UTF8.GetBytes(message);
                    bytesSent = sender.Send(msg);

                    // Получаем ответ от сервера
                    bytesRec = sender.Receive(avatar);
                    BitmapImage avatarImage = new BitmapImage();
                    avatarImage.BeginInit();
                    avatarImage.StreamSource = new System.IO.MemoryStream(avatar);
                    avatarImage.EndInit();
                    imgAvatar.Source = avatarImage;
                }
            }
            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        private void ButtonData_Click(object sender, RoutedEventArgs e)
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
