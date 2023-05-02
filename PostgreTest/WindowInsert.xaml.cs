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
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text.RegularExpressions;

namespace ClientServerApp
{
    /// <summary>
    /// Логика взаимодействия для WindowInsert.xaml
    /// </summary>
    public partial class WindowInsert : Window
    {
        public WindowInsert(string text, string columnName)
        {
            InitializeComponent();
            switch (columnName)
            {
                case "age":
                    tbAge.Text = text;
                    tbAge.IsReadOnly = true;
                    break;

                case "level":
                    tbLevel.Text = text;
                    tbLevel.IsReadOnly = true;
                    break;

                case "sale":
                    tbSale.Text = text;
                    tbSale.IsReadOnly = true;
                    break;

                case "name":
                    tbName.Text = text;
                    tbName.IsReadOnly = true;
                    break;

                case "nickname":
                    tbNickname.Text = text;
                    tbNickname.IsReadOnly = true;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name, nickname;
                int age, sale, level;


                byte[] avatarByte = new byte[262144];
                string message = null;
                int bytesRec;
                byte[] bytes = new byte[262144];

                if (imgAvatar.Source != null)
                {
                    avatarByte = Converter.ConvertFromImage(imgAvatar.Source as BitmapImage);
                    message = $"INSERT;{tbName.Text},{tbAge.Text},{tbNickname.Text},{tbLevel.Text},{tbSale.Text},INSIMAGE";
                }
                else
                {
                    message = $"INSERT;{tbName.Text},{tbAge.Text},{tbNickname.Text},{tbLevel.Text},{tbSale.Text}";
                }
                (bytes, bytesRec) = Sender.SendMessageFromSocket( message);
                string reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                if (reply.Contains("NEED_IMAGE"))
                {
                    (bytes, bytesRec) = Sender.SendMessageFromSocket(avatarByte);
                    reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                }

                else if (reply.Contains("INSERTED"))
                {
                    MessageBox.Show("Inserted complete");
                    this.Close();
                }
                else
                {
                    MessageBox.Show(reply);
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png"
            };
            if (op.ShowDialog() == true)
            {
                imgAvatar.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void tbAge_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbAge.Text = Regex.Replace(tbAge.Text, "[^0-9]+", "", RegexOptions.ECMAScript);
        }

        private void tbLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbLevel.Text = Regex.Replace(tbLevel.Text, "[^0-9]+", "", RegexOptions.ECMAScript);
        }

        private void tbSale_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbSale.Text = Regex.Replace(tbSale.Text, "[^0-9]+", "", RegexOptions.ECMAScript);
        }
    }
}
