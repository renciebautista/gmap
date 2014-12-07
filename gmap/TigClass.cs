using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gmap
{
    class TigClass
    {
        UdpClient udpClient = new UdpClient();
        private string username = settingsClass.GetValue("tig_username");
        private string version = settingsClass.GetValue("tig_version");
        private string server = settingsClass.GetValue("tig_server");
        private int port = Int32.Parse(settingsClass.GetValue("server_port"));
        System.Timers.Timer timer1 = new System.Timers.Timer(4000);

        public TigClass()
        {
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // reuse port
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 30512));
        }
        private string Connect()
        {
            return string.Format("<?xml version=\"1.0\"?><Tig><Client.Connect Name=\"{0}\" Version=\"{1}\" /></Tig>", username, version);
        }

        public  void connect()
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(Connect());
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length, server, port); // send data to server via port 30511

                Thread thdUDPServer = new Thread(new ThreadStart(serverThread)); //start thread for recieving
                if (thdUDPServer.ThreadState != ThreadState.Running)
                {
                     thdUDPServer.Start();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }


        public void serverThread()
        {
            try
            {
                udpClient.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //CallBack
        private void recv(IAsyncResult res)
        {
            //IPAddress tnxip = IPAddress.Parse("192.168.1.180");
            //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.180"), 0);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(server), 0);
            byte[] received = udpClient.EndReceive(res, ref RemoteIpEndPoint);

            //Process codes

            //MessageBox.Show(Encoding.UTF8.GetString(received));
            //this.Invoke((MethodInvoker)(() => txtReply.AppendText(RemoteIpEndPoint + "=>" + Encoding.UTF8.GetString(received) + Environment.NewLine)));
            udpClient.BeginReceive(new AsyncCallback(recv), null);
        }
    }
}
