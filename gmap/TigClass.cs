using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace gmap
{
    public struct Device
   {
        public string Mcc;
        public string Mnc;
        public string Ssi;
        public string Name;
        public double Rssi;
        public double Speed;
        public double Course;
        public double Altitude;
        public double Error;
        public double Lat;
        public double Lng;
   }
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
                MessageBox.Show("Server not found","Error",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
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
                // MessageBox.Show("Server not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
               // MessageBox.Show(e.ToString());
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
            string xml = Encoding.UTF8.GetString(received);

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xml);

                Device d = new Device();

                string xpath = "Tig/Subscriber.Location";
                var nodes = xmlDoc.SelectNodes(xpath);

                if (nodes.Count == 1)
                {
                    foreach (XmlNode node in xmlDoc.GetElementsByTagName("Tetra"))
                    {
                        d.Mcc = node.Attributes["Mcc"].Value;
                        d.Mnc = node.Attributes["Mnc"].Value;
                        d.Ssi = node.Attributes["Ssi"].Value;
                    }

                    foreach (XmlNode node in xmlDoc.GetElementsByTagName("Name"))
                    {
                        d.Name = node.Attributes["Name"].Value;
                    }

                    foreach (XmlNode node in xmlDoc.GetElementsByTagName("Uplink"))
                    {
                        d.Rssi = Convert.ToDouble(node.Attributes["Rssi"].Value);
                    }

                    foreach (XmlNode node in xmlDoc.GetElementsByTagName("PositionFix"))
                    {
                        d.Speed = Convert.ToDouble(node.Attributes["Speed"].Value);
                        d.Course = Convert.ToDouble(node.Attributes["Course"].Value);
                        d.Altitude = Convert.ToDouble(node.Attributes["Altitude"].Value);
                        d.Error = Convert.ToDouble(node.Attributes["MaximumPositionError"].Value);
                    }

                    foreach (XmlNode node in xmlDoc.GetElementsByTagName("Latitude"))
                    {
                        d.Lat = ConvertDegreeAngleToDouble(Convert.ToDouble(node.Attributes["Degrees"].Value),
                            Convert.ToDouble(node.Attributes["Minutes"].Value), Convert.ToDouble(node.Attributes["Seconds"].Value));
                    }

                    foreach (XmlNode node in xmlDoc.GetElementsByTagName("Longitude"))
                    {
                        d.Lng = ConvertDegreeAngleToDouble(Convert.ToDouble(node.Attributes["Degrees"].Value),
                            Convert.ToDouble(node.Attributes["Minutes"].Value), Convert.ToDouble(node.Attributes["Seconds"].Value));
                    }

                }

                logDevice(d);
            }
            catch (Exception err)
            {
                //MessageBox.Show("Server not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            
            
            udpClient.BeginReceive(new AsyncCallback(recv), null);
        }

        public double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds)
        {
            //Decimal degrees = 
            //   whole number of degrees, 
            //   plus minutes divided by 60, 
            //   plus seconds divided by 3600

            return degrees + (minutes / 60) + (seconds / 3600);
        }

        public void logDevice(Device device )
        {
            DataTable dt = SqliteDal.getData(string.Format("SELECT id FROM devices WHERE mcc='{0}' AND mnc='{1}' AND ssi='{2}' LIMIT 1",device.Mcc,device.Mnc,device.Ssi));
            if (dt.Rows.Count > 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(config.DataSource))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        conn.Open();
                        cmd.Connection = conn;

                        SQLiteHelper sh = new SQLiteHelper(cmd);

                        try
                        {
                            var d = new Dictionary<string, object>();
                            d["device_id"] = Int32.Parse( dt.Rows[0]["id"].ToString());
                            d["subscriber_name"] = device.Name;
                            d["uplink"] = device.Rssi;
                            d["speed"] = device.Speed;
                            d["course"] = device.Course;
                            d["altitude"] = device.Altitude;
                            d["max_position_error"] = device.Error;
                            d["lat"] = device.Lat;
                            d["lng"] = device.Lng;
                            d["created_at"] = DateTime.Now;
                            sh.Insert("logs", d);


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        conn.Close();
                    }
                }
            }
            
        }
    }
}
