using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class ConnectionDB
    {
        readonly static string cs = "Host=localhost;Username=postgres;Password=123;Database=postgres";
        readonly static NpgsqlConnection con = new NpgsqlConnection(cs);
        
        public static void OpenConnection()
        {
            con.Open();
        }

        public static void CloseConncetion()
        {
            con.Close();
        }

        public static byte[] SearchImage(string data)
        {
            string[] strArr = data.Split(',');
            data = strArr[1];
            byte[] avatar = new byte[1024];
            using (NpgsqlCommand command = new NpgsqlCommand($"SELECT avatar FROM people WHERE nickname='{data}';", con))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader[0]);
                    avatar = (byte[])reader[0];
                }
                reader.Close();
            }
            return avatar;
        }

        public static string Search(string data)
        {
            string reply = "";
            using (NpgsqlCommand command = new NpgsqlCommand($"SELECT * FROM people WHERE nickname='{data}';", con))
            {
                byte[] avatar = new byte[1024];
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    reply = ($"{reader.GetString(0)},{reader.GetInt32(1)},{reader.GetString(2)},{reader.GetInt32(3)},{reader.GetInt32(4)}");
                    avatar = (byte[])reader.GetValue(5);
                    
                }
                reader.Close();
                Console.WriteLine($"{avatar[0]} {avatar[1]} {avatar[2]}");
                if (avatar[0] != 0)
                {
                    Console.WriteLine(avatar.Length);
                    reply += ",IMAGE";
                    Console.WriteLine(reply);
                }
                if (reply == "")
                {
                    reply = "Error 404 Not Found!";
                }
            }
            return reply;
        }

        public static string Insert(string data, byte[] image)
        {
            string reply = "";
            try
            {
                string[] dataIns = data.Split(',');
                try
                {
                    int.Parse(dataIns[1]);
                    int.Parse(dataIns[3]);
                    int.Parse(dataIns[4]);
                }
                catch (Exception ex)
                {
                    reply = ex.Message;
                }
                using (NpgsqlCommand command = new NpgsqlCommand($"INSERT INTO people (name, age, nickname, level, sale, avatar) VALUES (@n, @a, @nick, @lvl, @sale, @avatar)", con))
                {
                    command.Parameters.AddWithValue("n", dataIns[0]);
                    command.Parameters.AddWithValue("a", int.Parse(dataIns[1]));
                    command.Parameters.AddWithValue("nick", dataIns[2]);
                    command.Parameters.AddWithValue("lvl", int.Parse(dataIns[3]));
                    command.Parameters.AddWithValue("sale", int.Parse(dataIns[4]));
                    command.Parameters.AddWithValue("avatar", image);

                    int nRows = command.ExecuteNonQuery();
                    Console.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                    reply = String.Format("Number of rows inserted={0}", nRows);
                }
            }
            catch (Exception ex)
            {
                reply = ex.Message;
            }
            return reply;
        }

        public static string InsertNotImage(string data)
        {
            string reply = "";
            try
            {
                string[] dataIns = data.Split(',');
                try
                {
                    int.Parse(dataIns[1]);
                    int.Parse(dataIns[3]);
                    int.Parse(dataIns[4]);
                }
                catch (Exception ex)
                {
                    reply = ex.Message;
                }
                using (NpgsqlCommand command = new NpgsqlCommand($"INSERT INTO people (name, age, nickname, level, sale, avatar) VALUES (@n, @a, @nick, @lvl, @sale, @avatar)", con))
                {
                    command.Parameters.AddWithValue("n", dataIns[0]);
                    command.Parameters.AddWithValue("a", int.Parse(dataIns[1]));
                    command.Parameters.AddWithValue("nick", dataIns[2]);
                    command.Parameters.AddWithValue("lvl", int.Parse(dataIns[3]));
                    command.Parameters.AddWithValue("sale", int.Parse(dataIns[4]));
                    command.Parameters.AddWithValue("avatar", DBNull.Value);

                    int nRows = command.ExecuteNonQuery();
                    Console.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                    reply = String.Format("Number of rows inserted={0}", nRows);
                }
            }
            catch (Exception ex)
            {
                reply = ex.Message;
            }
            return reply;
        }
    }
}
