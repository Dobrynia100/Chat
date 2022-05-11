using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpServer
{

    //отдельный класс нужен, чтоб запускать в потоке функцию с параметрами
    class myThread
{
    Thread thread;
    Socket[] handler=new Socket[10];

    int num=0;
          server serv;
        int[] clients = new int[10];
        
        public myThread( int num1, Socket handler1) //Конструктор получает имя функции и номер, до которого ведется счет
    {
            //  thread.Name = name;
            //thread.Start(num);//передача параметра в поток
          
            handler[num1] = handler1;
        num = num1;
            //serv = server;
            clients[num] = num;
    }

    public void func()//Функция потока, передаем параметр всегда object
    {
        StringBuilder builder = new StringBuilder();
        int bytes = 0; // количество полученных байтов за 1 раз
        int kol_bytes = 0;//количество полученных байтов
        byte[] data = new byte[255]; // буфер для получаемых данных

            /*   for (int i = 0; i < num; i++)
               {
                   if (clients[i] != num)
                   {
                       message = clients[i] + ": ";
                       data = Encoding.Unicode.GetBytes(message);
                       handler.Send(data);

                   }
               }*/
            string message = num + " вошел в чат \n";
            while (true)
        {
                
                serv.Broadcast(message, num, handler);
                do
            {


                bytes = handler[num].Receive(data);  // получаем сообщение
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                kol_bytes += bytes;
               
            }
            while (handler[num].Available > 0);

            Console.WriteLine(" client " + num + ":\n");
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

            Console.WriteLine(kol_bytes + "bytes\n");
            // отправляем ответ
            
            data = Encoding.Unicode.GetBytes(message);
          
            // закрываем сокет
            if (builder.ToString() == "cl")
            {
                message = "соединение закрыто\n";
                data = Encoding.Unicode.GetBytes(message);
                handler[num].Send(data);

                handler[num].Close();

                Console.WriteLine("соединение закрыто.\n");


            }
           

            builder.Clear();


        }
    }
       

    }

 
    class server
    {
        static int port = 3817; // порт для приема входящих запросов
        static int[] clients = new int[10];
        static void Main(string[] args)
        {

            String Host = Dns.GetHostName();
            Console.WriteLine("Comp name = " + Host);
            IPAddress[] IPs;
            IPs = Dns.GetHostAddresses(Host);
            foreach (IPAddress ip1 in IPs)
                Console.WriteLine(ip1);
            
            //получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            
            // создаем сокет сервера
            int num = 0;
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {


                    // готовимся  получать  сообщение
                    Socket handler = listenSocket.Accept();
                    myThread t1 = new myThread( num, handler);
                    Thread thread = new Thread(new ThreadStart(t1.func));
                    clients[num] = num;
                                                        

                    thread.Start();
                    num++;





                }
            }



            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Broadcast(string message, int num, Socket[] handler)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < num; i++)
            {
                if (clients[i] == num)
                {
                    // message = clients[i] + ": ";
                    data = Encoding.Unicode.GetBytes(message);
                    handler[i].Send(data);

                }
            }
        }
    }
    

}
