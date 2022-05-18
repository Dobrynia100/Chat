using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
namespace chat_form
{



    public partial class Form1 : Form
    {
        static int port = 3817; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
        //создаем сокет
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        bool alive = false;//активен ли поток приема
     
        public Form1()
        {
            InitializeComponent();
            //создаем сокет

            button3.Enabled = true; // кнопка входа
            button2.Enabled = false; // кнопка выхода
            button1.Enabled = false; // кнопка отправки
            textBox2.ReadOnly = true; // поле для сообщений
         
        }
      
        private void button2_Click(object sender, EventArgs e)//выйти
        {
            string message ="close";
            byte[] data = Encoding.Unicode.GetBytes(message);
            socket.Send(data);
            alive = false;       

            Application.Exit();
        }
   
      
        private void button3_Click(object sender, EventArgs e)//присоединится 
        {
            
                socket.Connect(ipPoint);
            


            textBox2.Text = "Введите сообщение:";
            Task receiveTask = new Task(receive);
            receiveTask.Start();
            button3.Enabled = false; // кнопка входа
            button2.Enabled = true; // кнопка выхода
            button1.Enabled = true; // кнопка отправки
        }
        void receive()
        {
            alive = true;
            try
            {

                byte[] data = new byte[1064]; // буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                while (alive)
                {



                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));


                    string message = builder.ToString();
                  
                        textBox2.Text = textBox2.Text+ "\r" + "\n"+ message ;//вывод сообщения
                                                               
                    builder.Clear();
                }
            }
            catch (ObjectDisposedException)
            {
                if (!alive)
                    return;
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);


            }

        }
        private void button1_Click(object sender, EventArgs e)//отправить
        {
            try
            {

                string message = textBox1.Text;

            byte[] data = Encoding.Unicode.GetBytes(message);
          
            //посылаем сообщение
            socket.Send(data);
            // готовимся получить ответ
            StringBuilder builder = new StringBuilder();//ответ сервера
            char[] reply = new char[2000];//сообщения пользователей
           

            builder.Clear();
               textBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
    
}
