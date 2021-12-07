using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUPERCHAT
{
    public partial class Telespamm : Form
    {
        bool alive = false;
        UdpClient client;
        int temp = 0;
        const int LOCALPORT = 10101; // порт отправки
        const int REMOTEPORT = 8001; // порт приема
        const int TTL = 20;
        const string HOST = "235.5.5.1"; // хост для групповой рассылки
        IPAddress groupAddress; // адрес для групповой рассылк
        static IPAddress address = IPAddress.Parse("26.45.37.25");
        //TcpListener ServerListener = new TcpListener(address, LOCALPORT);
        //TcpClient clientSocket = default(TcpClient);
        
        
       

        string userName; // имя пользователя в чате
        public Telespamm()
        {
            InitializeComponent();
            LoginButton.Enabled = true; // кнопка входа
            LogoutButton.Enabled = false; // кнопка выхода
            sendButton.Enabled = false; // кнопка отправки
            chatTextBox.ReadOnly = true; // поле для сообщений

            groupAddress = IPAddress.Parse(HOST);
        }

        private void ReceiveMessages()
         {
             alive = true;
             try
             {
                 while (alive)
                 {
                     IPEndPoint remoteIp = null;
                     byte[] data = client.Receive(ref remoteIp);
                     string message = Encoding.Unicode.GetString(data);

                     this.Invoke(new MethodInvoker(() =>
                     {
                         string time = DateTime.Now.ToShortTimeString();
                         chatTextBox.Text = time + " " + message + "\r\n" + chatTextBox.Text;
                     }));
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

     private void Form1_Load(object sender, EventArgs e)
     {
         
     }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (userNameTextBox.Text == "")
            {
                MessageBox.Show("Вы не ввели имя");
            }
            else
            {
                userName = userNameTextBox.Text;
                userNameTextBox.ReadOnly = true;
                Random rnd = new Random();
                temp = rnd.Next(10000, 11000);

                try
                {
                    client = new UdpClient(LOCALPORT);

                    client.JoinMulticastGroup(groupAddress, TTL);

                    // запускаем задачу на прием сообщений
                    Task receiveTask = new Task(ReceiveMessages);
                    receiveTask.Start();

                    // отправляем первое сообщение о входе нового пользователя
                    string message = userName + " вошел в чат. Поприветствуйте его!";
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    client.Send(data, data.Length, HOST, REMOTEPORT);
                    client.Send(data, data.Length, HOST, LOCALPORT);

                    LoginButton.Enabled = false;
                    LogoutButton.Enabled = true;
                    sendButton.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            ExitChat();
        }

        private void ChatTextBox_TextChanged(object sender, EventArgs e)
        {

        }

       private void ExitChat()
        {
            string message = userName + " покинул наш мир";
            byte[] data = Encoding.Unicode.GetBytes(message);
            client.Send(data, data.Length, HOST, REMOTEPORT);
            client.Send(data, data.Length, HOST, LOCALPORT);
            client.DropMulticastGroup(groupAddress);

            alive = false;
            client.Close();

            LoginButton.Enabled = true;
            LogoutButton.Enabled = false;
            sendButton.Enabled = false;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string message = String.Format("{0}: {1}", userName, messageTextBox.Text);
                byte[] data = Encoding.Unicode.GetBytes(message);
                client.Send(data, data.Length, HOST, REMOTEPORT);
                client.Send(data, data.Length, HOST, LOCALPORT);
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (alive)
                ExitChat();
        }

        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void MessageTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void Port_TextChanged(object sender, EventArgs e)
        {

        }

        private void Address_TextChanged(object sender, EventArgs e)
        {

        }

        private void Connection_Port_TextChanged(object sender, EventArgs e)
        {

        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            chatTextBox.Text = "";
        }
    }
}
