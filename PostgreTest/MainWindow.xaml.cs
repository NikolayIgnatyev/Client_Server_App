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
        string reply;
        byte[] bytes = new byte[262144];
        int bytesRec;

        public MainWindow()
        {
            InitializeComponent();
        }



        public (byte[] bytes, int bytesRec) SendMessageFromSocket(int port, string message)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[262144];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return (bytes, bytesRec);
        }

        private void ButtonData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Console.WriteLine(cbColumnName.SelectedItem.ToString());
                bytes = SendMessageFromSocket(11000, "SEARCH;" + cbColumnName.SelectedItem.ToString() + "," + tbsender.Text).bytes;
                bytesRec = SendMessageFromSocket(11000, "SEARCH;" + cbColumnName.SelectedItem.ToString() + "," + tbsender.Text).bytesRec;
                reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
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
                    byte[] avatar = new byte[262144];
                    string[] dataReply = reply.Split(',');
                    tblName.Text = dataReply[0];
                    tblAge.Text = dataReply[1];
                    tblNick.Text = dataReply[2];
                    tblLevel.Text = dataReply[3];
                    tblSale.Text = dataReply[4];
                    if (dataReply[5] == "IMAGE")
                    {

                        try
                        {
                            avatar = SendMessageFromSocket(11000, "SEARCH;IMAGE," + cbColumnName.SelectedItem.ToString() + "," + tbsender.Text).bytes;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            Console.ReadLine();
                        }
                        BitmapImage avatarImage = new BitmapImage();
                        avatarImage.BeginInit();
                        avatarImage.StreamSource = new System.IO.MemoryStream(avatar);
                        avatarImage.EndInit();
                        imgAvatar.Source = avatarImage;
                    }
                }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bytes = SendMessageFromSocket(11000, "COLUMN_NAME;null").bytes;
            bytesRec = SendMessageFromSocket(11000, "COLUMN_NAME;null").bytesRec;
            reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            string[] dataReply = reply.Split(',');
            for (int i = 0; i < dataReply.Length - 1; i++)
            {
                cbColumnName.Items.Add(dataReply[i]);
            }
            cbColumnName.SelectedIndex = 0;
        }
    }
}
