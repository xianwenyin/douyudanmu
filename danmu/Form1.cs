using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace danmu
{


    public partial class Form1 : Form
    {
        bool isConnected = false;
        bool isEmptyText = true;
        String roomId = "";
        public Socket sckClient;            //客户端套接字
        int bufferLength = 1024;
        byte[] blength = new byte[4];
        byte[] ReceiveBuf = null;  //接收缓冲区
        byte[] SendBuf = null;     //发送缓冲区
        delegate void SetTextCallback(string text);
        Dictionary<string, string> config = new Dictionary<string, string>();

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.txtContent.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.txtContent.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.txtContent.Disposing || this.txtContent.IsDisposed)
                        return;
                }
                SetTextCallback d = new SetTextCallback(SetText);
                this.txtContent.Invoke(d, new object[] { text });
            }
            else
            {
                if (text == null)
                {
                    txtContent.Text = "";
                    isEmptyText = true;
                    return;
                }
                if (isEmptyText)
                {
                    txtContent.AppendText(text);
                    isEmptyText = false;
                }
                else
                {
                    txtContent.AppendText("\r\n" + text);
                    txtContent.ScrollToCaret();
                }
                

            }
        }



        public Form1()
        {
            InitializeComponent();
            ReceiveBuf = new byte[bufferLength];
            SendBuf = new byte[bufferLength];
        }


        private void btn_ok_Click(object sender, EventArgs e)
        {
            roomId = this.txtRoom.Text;
            SetText(null);
            SetText("正在进入房间"+roomId+"...");
            IPAddress myIp = IPAddress.Parse(config["dmServerIp"]);
            IPEndPoint iep = new IPEndPoint(myIp, Convert.ToInt32(config["dmServerPort"]));

            //创建客户端套接字
            sckClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //连接服务器
            sckClient.BeginConnect(iep, new AsyncCallback(ConnectCallback), sckClient);

        }

        /// <summary>
        /// 发送数据回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            Socket sckSend = (Socket)ar.AsyncState;
            int sendLen = sckSend.EndSend(ar);
            Console.WriteLine("数据发送" + sendLen);
        }

        /// <summary>
        /// 连接服务器回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            Socket sckConnect = (Socket)ar.AsyncState;
            sckConnect.EndConnect(ar);
            Console.WriteLine(roomId);

            //清空接收数据缓冲区
            Array.Clear(ReceiveBuf, 0, bufferLength);
            SendBuf = packMsg("type@=loginreq/username@=auto_KRLJbE8mZM/password@=1234567890123456/roomid@=" + roomId + "/");
            sckClient.BeginSend(SendBuf, 0, SendBuf.Length, SocketFlags.None, new AsyncCallback(SendCallback), sckClient);

            SendBuf = packMsg("type@=joingroup/rid@=" + roomId + "/gid@=" + config["groupId"] + "/");
            sckClient.BeginSend(SendBuf, 0, SendBuf.Length, SocketFlags.None, new AsyncCallback(SendCallback), sckClient);
            //接收数据

            /*sckClient.Receive(blength);
            int msgLength = bufferLength;
            try
            {
                 msgLength = (int)(blength[0] | blength[1] << 8 | blength[2] << 16 | blength[3] << 24);
            }
            catch (Exception ex) {

            }
            Console.WriteLine(msgLength);*/
            sckClient.BeginReceive(ReceiveBuf, 0, bufferLength, SocketFlags.None, new AsyncCallback(ReceiveCallback), sckClient);

            //sckClient.BeginReceive()
            isConnected = true;
            Console.WriteLine("连接成功！");

            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (isConnected)
                {
                    Thread.Sleep(45000);
                    SendBuf = packMsg("type@=keeplive/tick@=70/");
                    sckClient.BeginSend(SendBuf, 0, SendBuf.Length, SocketFlags.None, new AsyncCallback(SendCallback), sckClient);

                }
            }));
            thread.IsBackground = true;
            thread.Start();  //心跳线程


        }

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            //Console.WriteLine("数据接收中。。");
            Socket sckReceive = (Socket)ar.AsyncState;
            int revLength = sckReceive.EndReceive(ar);
            int skipLength = 12;
            byte[] tmp = ReceiveBuf.Skip(skipLength).Take(bufferLength - 1 - skipLength).ToArray();

            //把接收到的数据转成字符串显示到界面

            string strReceive = System.Text.Encoding.UTF8.GetString(tmp, 0, bufferLength - 1 - skipLength);
            Dictionary<string, string> data = msg_decode(strReceive);
            if (data.ContainsKey("type")) {
                switch (data["type"]) {
                    case "loginres":
                        this.SetText("已连接上服务器...");
                        break;
                    case "chatmsg":
                        this.SetText(data["nn"]+":"+data["txt"]);
                        break;
                    case "keeplive":
                        //this.SetText("心跳信息");
                        break;
                    case "dgb":
                        if (!data.ContainsKey("gfcnt")) {
                            data.Add("gfcnt", "1");
                        }
                        if (!data.ContainsKey("hits"))
                        {
                            data.Add("hits", "1");
                        }
                        this.SetText(data["nn"]+"送的礼物(gfid="+ data["gfid"] + ") 数量"+data["gfcnt"]+",连击"+data["hits"]);
                        break;
                    case "uenter":
                        this.SetText(data["nn"]+" 进入直播间");
                        break;
                    default:
                        break;

                }

            }

     

            try
            {
                //再次接收数据
                /*
                sckClient.Receive(blength);
                int msgLength = bufferLength;
                try
                {
                    msgLength = (int)(blength[0] | blength[1] << 8 | blength[2] << 16 | blength[3] << 24);
                }
                catch (Exception ex)
                {

                }
                Console.WriteLine(msgLength);*/
                Array.Clear(ReceiveBuf, 0, bufferLength);
                sckClient.BeginReceive(ReceiveBuf, 0, bufferLength, SocketFlags.None, new AsyncCallback(ReceiveCallback), sckClient);
            }
            catch (Exception ex)
            {

            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            IPAddress[] ips = Dns.GetHostAddresses("openbarrage.douyutv.com");
            Console.WriteLine(ips[0]);


            config.Add("dmServerIp", ips[0].ToString()); //Dns.GetHostAddresses("openbarrage.douyutv.com").ToString()
            config.Add("dmServerPort", "8601");
            config.Add("groupId", "-9999");
            config.Add("gidServerIp", "119.90.49.110");
            config.Add("gidServerPort", "8046");
            //txtRoom.Text = "594528";

        }

        private byte[] packMsg(string msg)
        {
            byte[] sarr = System.Text.Encoding.Default.GetBytes(msg);
            uint length = Convert.ToUInt32(4 + 4 + sarr.Length + 1);

            byte[] lengthByte = BitConverter.GetBytes(length);
            byte[] buffer = new byte[length + 4];
            byte[] magic = new byte[] { 0xb1, 0x02, 0x00, 0x00 };

            lengthByte.CopyTo(buffer, 0);
            lengthByte.CopyTo(buffer, 4);
            magic.CopyTo(buffer, 4 + 4);

            sarr.CopyTo(buffer, 4 + 4 + magic.Length);

            byte[] end = new byte[] { 0x00 };

            end.CopyTo(buffer, 4 + 4 + magic.Length + sarr.Length);
            Console.WriteLine(System.Text.Encoding.Default.GetString(buffer));
            return buffer;
        }

        private void disposeData()
        {
            int skipLength = 12;
            byte[] tmp = ReceiveBuf.Skip(skipLength).Take(bufferLength - 1 - skipLength).ToArray();

            //把接收到的数据转成字符串显示到界面
            string strReceive = System.Text.Encoding.UTF8.GetString(tmp, 0, bufferLength - 1 - skipLength);
        }

        private Dictionary<string, string> msg_decode(string str)
        {
            if (str == "" || str == null)
            {
                return null;    
            }
            Dictionary<string, string> data = new Dictionary<string, string>();
            string key = "";
            string val = "";

            if (str.Substring(str.Length - 1, 1) != "/")
            {
                str += "/";
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (str.Substring(i, 1) == "/")
                {

                    if (!data.ContainsKey(key))
                    {
                        data.Add(key, val);

                    }
                    else {
                        data[key] = val;
                    }
                        
                    if (val.Contains("@S/"))
                    {
                        /** 
                        string[] arr = Regex.Split(val, "js", RegexOptions.IgnoreCase);
                        foreach (string ss in arr) {

                        } 
                        */
                    }
                    key = val = "";

                }
                else {
                    if (str.Substring(i, 1) == "@")
                    {
                        i++;
                        if (str.Substring(i, 1) == "A")
                        {
                            val += "@";
                        }
                        else
                        {
                            if (str.Substring(i, 1) == "S")
                            {
                                val += "/";
                            }
                            else
                            {
                                if (str.Substring(i, 1) == "=")
                                {
                                    key = val;
                                    val = "";
                                }
                            }
                        }
                    }
                    else {
                        val += str.Substring(i, 1);
                    }
                        
                }
            }

            return data;
        }

    }
}
