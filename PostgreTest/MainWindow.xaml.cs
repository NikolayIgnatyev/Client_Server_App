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

namespace ClientServerApp
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

        private void ButtonData_Click(object sender, RoutedEventArgs e)
        {
            if (tbsender.Text != "")
            {
                try
                {
                    (bytes, bytesRec) = Sender.SendMessageFromSocket("SEARCH;" + cbColumnName.SelectedItem.ToString() + "," + tbsender.Text);
                    reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    if (reply.Contains("Error 404"))
                    {
                        if (MessageBox.Show("Таких данных нет в таблице.\nХотите добавить?", "Нет данных", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            WindowInsert windowInsert = new WindowInsert(tbsender.Text, cbColumnName.SelectedItem.ToString());
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
                        if (reply.Contains("IMAGE"))
                        {
                            try
                            {
                                avatar = Sender.SendMessageFromSocket("SEARCHIMAGE;" + cbColumnName.SelectedItem.ToString() + "," + tbsender.Text).bytes;
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
                            avatarImage.StreamSource = new MemoryStream(avatar);
                            avatarImage.EndInit();
                            imgAvatar.Source = avatarImage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Заполните поле!!!");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (bytes, bytesRec) = Sender.SendMessageFromSocket("COLUMN_NAME;null");
            reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            string[] dataReply = reply.Split(',');
            Array.Reverse(dataReply);
            for (int i = 1; i < dataReply.Length; i++)
            {
                cbColumnName.Items.Add(dataReply[i]);
            }
            cbColumnName.SelectedIndex = 0;
        }
    }
}
