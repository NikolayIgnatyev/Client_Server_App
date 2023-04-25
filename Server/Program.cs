﻿using System;
using System.Text;
using Npgsql;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {


            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            int port = 11000;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string[] dataedit = data.Split(new char[] { ';', });
                    string work = dataedit[0];
                    string dataDB = dataedit[1];
                    Console.WriteLine($"Входящие данные: {data}");
                    CConnectionDB.OpenConnection();
                    string reply = "";
                    switch (work)
                    {
                        case "search":
                            reply = CConnectionDB.Search(dataDB);
                            break;
                        case "insert":
                            reply = CConnectionDB.Insert(dataDB);
                            break;
                    }

                    // Отправляем ответ клиенту\
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);
                    CConnectionDB.CloseConncetion();
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
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
    }
}