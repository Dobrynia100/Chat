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
        Socket handler;
     int num=0;
          server serv;
        int[] clients = new int[10];
        
        public myThread( int num1, List<Socket> handler1,server server1) //Конструктор получает имя функции и номер, до которого ведется счет
    {
            
            handler = handler1[num1];
       
            serv = server1;
            clients[num] = num1;
            num = num1+1;
        }

    public void func()//Функция потока, передаем параметр всегда object
    {
        StringBuilder builder = new StringBuilder();
        int bytes = 0; // количество полученных байтов за 1 раз
        int kol_bytes = 0;//количество полученных байтов
        byte[] data = new byte[255]; // буфер для получаемых данных

            string message;
            try
            {
                message = "\r \n"+num + " вошел в чат \n";

                serv.Broadcast(message, num - 1);
                while (true)
                {
                    

                    do
                    {
                       

                            bytes = handler.Receive(data);  // получаем сообщение
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            kol_bytes += bytes;
                      //  Console.WriteLine("num5-" + num);


                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + " client " + num + ": "+ builder.ToString());
                    

                    Console.WriteLine(kol_bytes + "bytes");
                    // отправляем ответ
                    message = "\r \n "+num+": "+ builder.ToString();
                    // закрываем сокет
                    if (builder.ToString() == "close")
                    {
                        message = "\r \n соединение с пользователем " + num + " закрыто.\n";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        serv.RemoveConnection(num - 1);
                        handler.Close();
                     //   Console.WriteLine("num4-" + num);
                        Console.WriteLine(" соединение с пользователем " + num + " закрыто.\n");
                      //  break;

                    }

                    serv.Broadcast(message,num-1);

                 

                    builder.Clear();


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
       

    }

 
   public class server
    {
        static int port = 3817; // порт для приема входящих запросов
        //static int[] clients = new int[10];
       // Socket[] handler = new Socket[10];
        List<Socket> handler = new List<Socket>();
        static int[] clients = new int[10];
        int num = 0;
        int last = 0;
        protected internal void RemoveConnection(int nom)
        {
            // получаем по id закрытое подключение
            Socket client = handler.ElementAt(nom);
            // и удаляем его из списка подключений
           // if (handler[] == null)
                handler.Remove(client);
            num--;
           // Console.WriteLine("num1-" + num);
        }
        internal void start()
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
            
            clients[0] = 1;
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
                   // clients[num] = num;
                    handler.Add(listenSocket.Accept());
                    if(last<num) last = num;
                    myThread t1 = new myThread( last, handler,this);
                    Thread thread = new Thread(new ThreadStart(t1.func));
                    num++;



                    // Console.WriteLine("num2-" + num);



                    thread.Start();
    

                }
            }



            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Broadcast(string message,int nom)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            
            for (int i = 0; i < num; i++)
            {




                // if (clients[i] != nom)
                // {
               // Console.WriteLine("num3-" + num);
                data = Encoding.Unicode.GetBytes(message);
                    handler[i].Send(data);

               // }
            }
        }
    }
    


class serv
{
    static server server; // сервер
    static Thread Thread; // потока для прослушивания
    static void Main(string[] args)
    {
        try
        {
            server = new server();
            Thread = new Thread(new ThreadStart(server.start));
            Thread.Start(); //старт потока
        }
        catch (Exception ex)
        {
            
            Console.WriteLine(ex.Message);
        }
    }
}


}