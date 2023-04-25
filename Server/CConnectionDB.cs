﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class CConnectionDB
    {
        static string cs = "Host=localhost;Username=postgres;Password=123;Database=postgres";
        static NpgsqlConnection con = new NpgsqlConnection(cs);
        
        public static void OpenConnection()
        {
            con.Open();
        }

        public static void CloseConncetion()
        {
            con.Close();
        }

        public static string Search(string data)
        {
            string reply = "";
            using (NpgsqlCommand command = new NpgsqlCommand($"SELECT * FROM newtable WHERE name='{data}';", con))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    reply += ($"Name: {reader.GetString(0)}, Age: {reader.GetString(1)}");
                }
                reader.Close();
                if (reply == "")
                {
                    reply = "Error 404 Not Found!";
                }
            }
            return reply;
        }

        public static string Insert(string data)
        {
            string reply = "";
            try
            {
                string[] dataIns = data.Split(',');
                using (NpgsqlCommand command = new NpgsqlCommand($"INSERT INTO newtable (name, age) VALUES (@n, @a)", con))
                {
                    command.Parameters.AddWithValue("n", dataIns[0]);
                    command.Parameters.AddWithValue("a", dataIns[1]);

                    int nRows = command.ExecuteNonQuery();
                    Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                    reply =String.Format("Number of rows inserted={0}", nRows);
                }
            }
            catch (Exception ex)
            {
                reply += ex.ToString();
            }
            return reply;
        }
    }
}
