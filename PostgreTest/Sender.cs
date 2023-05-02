using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerApp
{
    /// <summary>
    /// Класс предоставляющий методы для работы с сокетом.
    /// </summary>
    internal static class Sender
    {
        // Буфер для входящих данных
        static byte[] bytes = new byte[262144];
        static Socket sender;

        public static void OpenSocketConnection(int port)
        {
            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);
        }

        public static void CloseSocketConnection()
        {
            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        public static (byte[] bytes, int bytesRec) SendMessageFromSocket(string message)
        {
            OpenSocketConnection(11000);
            // Кодирование входящих данных в byte[]
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);
            CloseSocketConnection();
            return (bytes, bytesRec);
        }

        public static (byte[] bytes, int bytesRec) SendMessageFromOneSocket(string message)
        {
            // Кодирование входящих данных в byte[]
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);
            return (bytes, bytesRec);
        }


        public static (byte[] bytes, int bytesRec) SendMessageFromSocket(byte[] message)
        {
            OpenSocketConnection(11000);
            // Отправляем данные через сокет
            int bytesSent = sender.Send(message);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);
            CloseSocketConnection();
            return (bytes, bytesRec);
        }

        public static (byte[] bytes, int bytesRec) SendMessageFromOneSocket(byte[] message)
        {
            // Отправляем данные через сокет
            int bytesSent = sender.Send(message);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);
            return (bytes, bytesRec);
        }
    }
}
