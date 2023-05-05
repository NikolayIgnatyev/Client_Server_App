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
using System.Windows.Markup;

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
        byte[] avatar = new byte[2097152];
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
                    if (cbColumnName.SelectedItem.ToString() == "age" | 
                        cbColumnName.SelectedItem.ToString() == "sale" |
                        cbColumnName.SelectedItem.ToString() == "level")
                    {
                        int.Parse(tbsender.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    (bytes, bytesRec) = Sender.SendMessageFromSocket("SEARCHNOTIMAGE;" + cbColumnName.SelectedItem.ToString() + "," + tbsender.Text);
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
                        lstVw.Items.Clear();
                        
                        string[] dataReply = reply.Split(';');
                        for (int i = 0; i < dataReply.Length; i++)
                        {
                            string[] user = dataReply[i].Split(',');
                            lstVw.Items.Add(new User { Name = user[0], Age = user[1], Nickname = user[2], Level = user[3], Discount = user[4] });
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
            try
            {
                (bytes, bytesRec) = Sender.SendMessageFromSocket("COLUMN_NAME;null");
                reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                string[] dataReply = reply.Split(',');
                Array.Reverse(dataReply);
                for (int i = 1; i < dataReply.Length; i++)
                {
                    if (!dataReply[i].Contains("avatar"))
                    {
                        cbColumnName.Items.Add(dataReply[i]);
                    }
                }
                cbColumnName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message + "\nПовторить попытку подлючения?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error)
                    == MessageBoxResult.Yes)
                {
                    Window_Loaded(this, null);
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void lstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstVw.Items.Count > 0)
            {
                User selectItem = (User)lstVw.SelectedItem;
                tblName.Text = selectItem.Name;
                tblAge.Text = selectItem.Age;
                tblNick.Text = selectItem.Nickname;
                tblLevel.Text = selectItem.Level;
                tblSale.Text = selectItem.Discount;

                (avatar, bytesRec) = Sender.SendMessageFromSocket("SEARCHIMAGE;" + "nickname" + "," + selectItem.Nickname);
                if (!Encoding.UTF8.GetString(avatar, 0, bytesRec).Contains("NOFOTO"))
                {
                    BitmapImage avatarImage = new BitmapImage();
                    avatarImage.BeginInit();
                    avatarImage.StreamSource = new MemoryStream(avatar);
                    avatarImage.EndInit();
                    imgAvatar.Source = avatarImage;
                }
                else
                {
                    imgAvatar.Source = Converter.ConvertToBitmapImage(Properties.Resources.ImageNotFound);
                }
            }
        }
    }
}
