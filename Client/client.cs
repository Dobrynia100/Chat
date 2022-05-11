using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace SocketTcpClient
{
    class myThread
    {
        Thread thread;
        Socket socket;
        public myThread(Socket sock) //Конструктор получает имя функции и номер,     до которого ведется счет
        {
            thread = new Thread(this.receive);
            //thread.Name = name;
            socket = sock;
            // thread.Start(num);//передача параметра в поток
            thread.Start();

        }

        void receive()
    {
        while (true)
        {
            try
            {
                byte[] data = new byte[1064]; // буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                string message = builder.ToString();
                Console.WriteLine(message);//вывод сообщения
            }
            catch
            {
                Console.WriteLine("Подключение прервано!"); //соединение было прервано
                Console.ReadLine();
               
            }
        }
    }


}

class Client
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 3817; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера
        static void Main(string[] args)
        {
            bool check = false;
            try
            {    //создаем конечную точку

                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                //создаем сокет
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);

                StringBuilder builder = new StringBuilder();//ответ сервера
                while (check == false)
                {

                    Console.Write("Введите сообщение:");
                    string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);

                    //посылаем сообщение
                    socket.Send(data);
                    // готовимся получить ответ
                    data = new byte[256]; // буфер для ответа
                    int bytes = 0; // количество полученных байт
                                 // получаем ответ

                    char[] reply = new char[2000];//сообщения пользователей
                    myThread t1 = new myThread(socket);
                    

                    builder.Clear();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
       

    }

}
