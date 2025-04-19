using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Delta_PLC_Tool
{
    public partial class Form1 : Form
    {

        private SerialPort port;
        private SerialPort port_2;
        private byte[] by = new byte[17];  //PLC命令， 全局调用

        private byte[] by_2 = new byte[17];  //PLC2命令， 全局调用
        static string pathMeter = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MeterConfig.ini");
        Configini configini = new Configini();
        private Thread _custmnThread;
        private Thread _custmnThread_2;

        /// <summary>
        /// 红外1信号变化
        /// </summary>
        int i_HW1_State = 0;
        int i_Goods = 0;

        /// <summary>
        /// 红外2信号变化
        /// </summary>
        int i_HW_2_State = 0;
        int i_Big = 0;
        int i_ServerRec = 0;
        int i_SaveOK = 0;
        int i_Intput_0 = 0;
        int i_Intput_1 = 0;
        int i_Intput_2 = 0;
        int i_Intput_3 = 0;
        int i_Intput_4 = 0;
        int i_Intput_5 = 0;
        int i_Intput_6 = 0;
        int i_Intput_7 = 0;

        int i_Output_0 = 0;
        int i_Output_1 = 0;
        int i_Output_2 = 0;
        int i_Output_3 = 0;
        int i_Output_4 = 0;
        int i_Output_5 = 0;
        int i_Output_6 = 0;
        int i_Output_7 = 0;



        int i_Intput_8 = 0;
        int i_Intput_9 = 0;
        int i_Intput_10 = 0;
        int i_Intput_11 = 0;
        int i_Intput_12 = 0;
        int i_Intput_13 = 0;
        int i_Intput_14 = 0;
        int i_Intput_15 = 0;

        int i_Output_8 = 0;
        int i_Output_9 = 0;
        int i_Output_10 = 0;
        int i_Output_11 = 0;
        int i_Output_12 = 0;
        int i_Output_13 = 0;
        int i_Output_14 = 0;
        int i_Output_15 = 0;


        public Form1()
        {
            InitializeComponent();
        }

        private void btn_OpenCom_Click(object sender, EventArgs e)
        {
            if (configini == null)
            {
                MessageBox.Show("配置文件为空！请检查");
                return;
            }
            string msg = "";
            if (port == null) port = new SerialPort();
            port.PortName = cmbPort.Text.ToString();
            port.BaudRate = int.Parse(configini.Baud);
            port.Parity = (Parity)Enum.Parse(typeof(Parity), configini.Parity);
            port.DataBits = int.Parse(configini.Databit);
            port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), configini.Stopbit);
            port.WriteTimeout = 300;
            port.ReadTimeout = 300;

            if (!port.IsOpen)
            {
                try
                {
                    _custmnThread = new Thread(CustumeDataCollection);
                    _custmnThread.IsBackground = true;
                    port.Open();
                    if (port.IsOpen)
                    {
                        port.DataReceived += Port_DataReceived;
                        _custmnThread.Start();
                    }
                    MessageBox.Show(string.Format("串口{0},打开成功", port.PortName));
                    SysLog.Info(string.Format("串口{0},打开成功", port.PortName));
                    
                    cmbPort.Enabled = false;
                    btn_OpenCom.Enabled = false;
                    //Aotu_PD();//开启皮带秤转动
                    Thread.Sleep(50);
                    timer1.Start();
                }
                catch (Exception ex)
                {
                    msg = ex.Source + ex.Message;
                    if (port.IsOpen) port.Close();
                    MessageBox.Show(string.Format("串口{0},打开失败，请检查！", port.PortName));
                    port.Dispose();
                    port = null;
                }
            }
        }


        string str_Function = "02";
        string str_XY, str_IO_status;
        private void CustumeDataCollection()
        {
            try
            {
                foreach (string recdata in _blockingDataCollection_1.GetConsumingEnumerable())
                {
                    str_Function = str_IO_status = str_XY = "";
                    str_Function = recdata.Substring(3, 2);
                    str_XY = recdata.Substring(5, 2);  //02 为 X, 01 为 Y
                    if (str_XY == "02" && str_XY.Length > 15)
                        str_IO_status = recdata.Substring(9, 2);   // X 
                    else
                        str_IO_status = recdata.Substring(7, 2);   // Y
                    int aa = Convert.ToInt32(str_IO_status, 16);
                    string item = Convert.ToString(aa, 2).PadLeft(8, '0');
                    //if(str_XY == "04") //X
                    if (str_XY == "02" && recdata.Length > 12)
                    {
                        if (item[7] == '1') btn_Input_0.BackColor = Color.GreenYellow; else btn_Input_0.BackColor = SystemColors.Window;
                        if (item[6] == '1') btn_Input_1.BackColor = Color.GreenYellow; else btn_Input_1.BackColor = SystemColors.Window;
                        if (item[5] == '1') btn_Input_2.BackColor = Color.GreenYellow; else btn_Input_2.BackColor = SystemColors.Window;
                        if (item[4] == '1') btn_Input_3.BackColor = Color.GreenYellow; else btn_Input_3.BackColor = SystemColors.Window;
                        if (item[3] == '1') btn_Input_4.BackColor = Color.GreenYellow; else btn_Input_4.BackColor = SystemColors.Window;
                        if (item[2] == '1') btn_Input_5.BackColor = Color.GreenYellow; else btn_Input_5.BackColor = SystemColors.Window;
                        if (item[1] == '1') btn_Input_6.BackColor = Color.GreenYellow; else btn_Input_6.BackColor = SystemColors.Window;
                        if (item[0] == '1') btn_Input_7.BackColor = Color.GreenYellow; else btn_Input_7.BackColor = SystemColors.Window;

                        if (item[7] == '1') i_Intput_0 = 1; else i_Intput_0 = 0;
                        if (item[6] == '1') i_Intput_1 = 1; else i_Intput_1 = 0;
                        if (item[5] == '1') i_Intput_2 = 1; else i_Intput_2 = 0;
                        if (item[4] == '1') i_Intput_3 = 1; else i_Intput_3 = 0;
                        if (item[3] == '1') i_Intput_4 = 1; else i_Intput_4 = 0;
                        if (item[2] == '1') i_Intput_5 = 1; else i_Intput_5 = 0;
                        if (item[1] == '1') i_Intput_6 = 1; else i_Intput_6 = 0;
                        if (item[0] == '1') i_Intput_7 = 1; else i_Intput_7 = 0;
                        //BigOrSmall();
                        IsGoods();

                    }
                    else
                    {
                        if (item[7] == '1') Output_0.BackColor = Color.GreenYellow; else Output_0.BackColor = SystemColors.Window;
                        if (item[6] == '1') Output_1.BackColor = Color.GreenYellow; else Output_1.BackColor = SystemColors.Window;
                        if (item[5] == '1') Output_2.BackColor = Color.GreenYellow; else Output_2.BackColor = SystemColors.Window;
                        if (item[4] == '1') Output_3.BackColor = Color.GreenYellow; else Output_3.BackColor = SystemColors.Window;
                        if (item[3] == '1') Output_4.BackColor = Color.GreenYellow; else Output_4.BackColor = SystemColors.Window;
                        if (item[2] == '1') Output_5.BackColor = Color.GreenYellow; else Output_5.BackColor = SystemColors.Window;
                        if (item[1] == '1') Output_6.BackColor = Color.GreenYellow; else Output_6.BackColor = SystemColors.Window;
                        if (item[0] == '1') Output_7.BackColor = Color.GreenYellow; else Output_7.BackColor = SystemColors.Window;

                        if (item[7] == '1') i_Output_0 = 1; else i_Output_0 = 0;
                        if (item[6] == '1') i_Output_1 = 1; else i_Output_1 = 0;
                        if (item[5] == '1') i_Output_2 = 1; else i_Output_2 = 0;
                        if (item[4] == '1') i_Output_3 = 1; else i_Output_3 = 0;
                        if (item[3] == '1') i_Output_4 = 1; else i_Output_4 = 0;
                        if (item[2] == '1') i_Output_5 = 1; else i_Output_5 = 0;
                        if (item[1] == '1') i_Output_6 = 1; else i_Output_6 = 0;
                        if (item[0] == '1') i_Output_7 = 1; else i_Output_7 = 0;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误：" + ex.StackTrace);
            }
        }


        string str_Function_2 = "02";
        string str_XY_2, str_IO_status_2;
        private void CustumeDataCollection_2()
        {
            try
            {
                foreach (string recdata in _blockingDataCollection_1.GetConsumingEnumerable())
                {
                    str_Function_2 = str_IO_status_2 = str_XY_2 = "";
                    str_Function_2 = recdata.Substring(3, 2);
                    str_XY_2 = recdata.Substring(5, 2);  //02 为 X, 01 为 Y
                    if (str_XY_2 == "02" && str_XY_2.Length > 15)
                        str_IO_status_2 = recdata.Substring(9, 2);   // X 
                    else
                        str_IO_status_2 = recdata.Substring(7, 2);   // Y
                    int aa = Convert.ToInt32(str_IO_status_2, 16);
                    string item = Convert.ToString(aa, 2).PadLeft(8, '0');
                    //if(str_XY == "04") //X
                    if (str_XY_2 == "02" && recdata.Length > 12)
                    {
                        if (item[7] == '1') btn_Input_8.BackColor = Color.GreenYellow; else btn_Input_8.BackColor = SystemColors.Window;
                        if (item[6] == '1') btn_Input_9.BackColor = Color.GreenYellow; else btn_Input_9.BackColor = SystemColors.Window;
                        if (item[5] == '1') btn_Input_10.BackColor = Color.GreenYellow; else btn_Input_10.BackColor = SystemColors.Window;
                        if (item[4] == '1') btn_Input_11.BackColor = Color.GreenYellow; else btn_Input_11.BackColor = SystemColors.Window;
                        if (item[3] == '1') btn_Input_12.BackColor = Color.GreenYellow; else btn_Input_12.BackColor = SystemColors.Window;
                        if (item[2] == '1') btn_Input_13.BackColor = Color.GreenYellow; else btn_Input_13.BackColor = SystemColors.Window;
                        if (item[1] == '1') btn_Input_14.BackColor = Color.GreenYellow; else btn_Input_14.BackColor = SystemColors.Window;
                        if (item[0] == '1') btn_Input_15.BackColor = Color.GreenYellow; else btn_Input_15.BackColor = SystemColors.Window;

                        if (item[7] == '1') i_Intput_8 = 1; else i_Intput_8 = 0;
                        if (item[6] == '1') i_Intput_9 = 1; else i_Intput_9 = 0;
                        if (item[5] == '1') i_Intput_10 = 1; else i_Intput_10 = 0;
                        if (item[4] == '1') i_Intput_11 = 1; else i_Intput_11 = 0;
                        if (item[3] == '1') i_Intput_12 = 1; else i_Intput_12 = 0;
                        if (item[2] == '1') i_Intput_13 = 1; else i_Intput_13 = 0;
                        if (item[1] == '1') i_Intput_14 = 1; else i_Intput_14 = 0;
                        if (item[0] == '1') i_Intput_15 = 1; else i_Intput_15 = 0;
                        //BigOrSmall();
                        //IsGoods();

                    }
                    else
                    {
                        if (item[7] == '1') Output_8.BackColor = Color.GreenYellow; else Output_8.BackColor = SystemColors.Window;
                        if (item[6] == '1') Output_9.BackColor = Color.GreenYellow; else Output_9.BackColor = SystemColors.Window;
                        if (item[5] == '1') Output_10.BackColor = Color.GreenYellow; else Output_10.BackColor = SystemColors.Window;
                        if (item[4] == '1') Output_11.BackColor = Color.GreenYellow; else Output_11.BackColor = SystemColors.Window;
                        if (item[3] == '1') Output_12.BackColor = Color.GreenYellow; else Output_12.BackColor = SystemColors.Window;
                        if (item[2] == '1') Output_13.BackColor = Color.GreenYellow; else Output_13.BackColor = SystemColors.Window;
                        if (item[1] == '1') Output_14.BackColor = Color.GreenYellow; else Output_14.BackColor = SystemColors.Window;
                        if (item[0] == '1') Output_15.BackColor = Color.GreenYellow; else Output_15.BackColor = SystemColors.Window;

                        if (item[7] == '1') i_Output_8 = 1; else i_Output_8 = 0;
                        if (item[6] == '1') i_Output_9 = 1; else i_Output_9 = 0;
                        if (item[5] == '1') i_Output_10 = 1; else i_Output_10 = 0;
                        if (item[4] == '1') i_Output_11 = 1; else i_Output_11 = 0;
                        if (item[3] == '1') i_Output_12 = 1; else i_Output_12 = 0;
                        if (item[2] == '1') i_Output_13 = 1; else i_Output_13 = 0;
                        if (item[1] == '1') i_Output_14 = 1; else i_Output_14 = 0;
                        if (item[0] == '1') i_Output_15 = 1; else i_Output_15 = 0;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误：" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 0 表示已接收，  1 表示已发送
        /// </summary>
        int Socket_hz = 0; 
        private void BigOrSmall()
        {
            
            if (i_ServerRec == 0 && i_SaveOK == 0 && i_HW_2_State == 0)
            {
                if (i_HW1_State == 0 && i_Intput_0 == 0)
                {
                    i_HW1_State = 0;
                }

                if (i_HW1_State == 0 && i_Intput_0 == 1)
                {
                    i_HW1_State = 1;
                }

                if (i_HW1_State == 1 && i_Intput_0 == 0 && i_Intput_1 == 0 )
                {
                    i_HW1_State = 2;   //从无变有再变无，表示小。
                }
                if (i_Intput_0 == 1 && i_Intput_1 == 1 && i_Big <2 ) //X0和X1同时有信号，为大，停止皮带秤
                {
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A };
                    SysLog.Info(string.Format("X0:{0},X1:{1},物体'{2}'", "1", "1", "大，停止皮带秤"));
                    if (Socket_hz % 3 ==0)
                    {
                        sendServer_Big();
                        Socket_hz = 1;
                    }else
                    {
                        Socket_hz = Socket_hz + 1;
                    }
                    i_Big = 1;
                }

                if (i_HW1_State == 2)
                {
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A };
                    SysLog.Info(string.Format("X0:{0},X1:{1},物体'{2}'", "1", "1", "小，停止皮带秤"));
                    //sendServer_Small();
                    if (Socket_hz % 3 == 0)
                    {
                        sendServer_Small();
                        Socket_hz = 1;
                    }
                    else
                    {
                        Socket_hz = Socket_hz + 1;
                    }
                }
            }

            if(i_ServerRec == 1 && i_SaveOK == 1 && i_Intput_1 == 1 && i_HW_2_State == 0)
            {
                i_HW_2_State = 1;
            }

            if (i_ServerRec == 1 && i_SaveOK == 1 && i_Intput_1 == 0 && i_HW_2_State == 1)
            {
                i_HW_2_State = 2;
            }

            if (i_HW_2_State == 2 && i_ServerRec == 1 && i_SaveOK == 1)
            {
                i_ServerRec = 0;
                i_SaveOK = 0;
                i_Big = 0;
                i_HW_2_State = 0;
                i_HW1_State = 0;
            }

        }

        private void IsGoods()
        {
            if (i_ServerRec == 0 && i_SaveOK == 0)
            {
                if (i_Goods == 0 && i_Intput_0 == 0)
                {
                    i_Goods = 0;
                }
                if (i_Goods == 0 && i_Intput_0 == 1)
                {
                    i_Goods = 1;
                }
                if (i_Goods == 1)
                {
                    PLC_Count = 3;  //设置发送次数
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A };
                    SysLog.Info(string.Format("X0:{0},物体'{1}'", "1", " 有，停止皮带秤"));
                    //sendServer_Small();
                    if (Socket_hz  == 0)
                    {
                        sendServer_Small();
                        Socket_hz = 1;
                    }
                }
            }

            if (i_Intput_0 == 0 && i_ServerRec == 1 && i_SaveOK == 1)
            {
                i_ServerRec = 0;
                i_SaveOK = 0;
                i_Goods = 0;
                Socket_hz = 0;
            }
        }

        private readonly BlockingCollection<byte[]> _blockingDataCollection = new BlockingCollection<byte[]>();
        private readonly BlockingCollection<string> _blockingDataCollection_1 = new BlockingCollection<string>();
        private List<byte> bufferlist = new List<byte>();
        private readonly object lock_bufferlist = new object();
        private List<byte> tempList = new List<byte>();

        public byte[] GetData(ref List<byte> bufferlist)
        {
            if (bufferlist.Count < 12) return null;
            byte[] newestData = null;
            int startIndex = -1;

            for (int i = 0; i < bufferlist.Count; i++)
            {
                if (bufferlist[i] == 0x3A)
                {
                    if ((i + 13 - 1) < bufferlist.Count && bufferlist[i + 12 - 1] == 0x0D && bufferlist[i + 13 - 1] == 0x0A) //Y 回复
                    {
                        startIndex = i;
                        newestData = new byte[13];
                        Array.Copy(bufferlist.ToArray(), startIndex, newestData, 0, 13);
                        break;
                    }

                    if ((i + 15 - 1) < bufferlist.Count && bufferlist[i + 14 - 1] == 0x0D && bufferlist[i + 15 - 1] == 0x0A) //X 回复
                    {
                        startIndex = i;
                        newestData = new byte[15];
                        Array.Copy(bufferlist.ToArray(), startIndex, newestData, 0, 15);
                        break;
                    }
                }
            }
            if (startIndex >= 0)
            {
                bufferlist.RemoveRange(0, startIndex + newestData.Length);
            }
            return newestData;
        }
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int n = port.BytesToRead;
                byte[] buf = new byte[n];
                int len = port.Read(buf, 0, n);
                byte[] recdata = null;


                lock (lock_bufferlist)
                {
                    if (bufferlist.Count > 3000)
                    {
                        Console.WriteLine("仪表数据一直未被成功解析，数据移除");
                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in bufferlist) sb.Append(string.Format("0x{0},", b.ToString("X2")));
                        Console.WriteLine("获取未被解析的数据" + sb);
                        bufferlist.Clear();
                        port.DiscardInBuffer();
                    }
                    bufferlist.AddRange(buf);
                    recdata = GetData(ref bufferlist);
                    string Recmsg = System.Text.Encoding.UTF8.GetString(recdata);
                    SysLog.Info(string.Format("Rec:{0}", Recmsg));
                    if (Recmsg.Contains(":") && Recmsg.Contains("\r\n"))
                    {
                        string temp = Recmsg.Substring(Recmsg.IndexOf(":"));
                        int endIndex = temp.IndexOf("\r\n");
                        temp = temp.Substring(0, endIndex);
                        _blockingDataCollection_1.Add(temp);
                    }
                    else
                    {
                        SysLog.Info(string.Format("指令解析错误,指令内容:{0}", Recmsg));
                    }
                }

                //if (recdata != null) _blockingDataCollection.Add(recdata);  //队列中添加数据
            }
            catch (Exception ex)
            {
                Console.WriteLine("串口数据接收异常:" + ex.StackTrace);
            }
        }

        private void Port_DataReceived_2(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int n = port_2.BytesToRead;
                byte[] buf = new byte[n];
                int len = port_2.Read(buf, 0, n);
                byte[] recdata = null;


                lock (lock_bufferlist)
                {
                    if (bufferlist.Count > 3000)
                    {
                        Console.WriteLine("仪表数据一直未被成功解析，数据移除");
                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in bufferlist) sb.Append(string.Format("0x{0},", b.ToString("X2")));
                        Console.WriteLine("获取未被解析的数据" + sb);
                        bufferlist.Clear();
                        port_2.DiscardInBuffer();
                    }
                    bufferlist.AddRange(buf);
                    recdata = GetData(ref bufferlist);
                    string Recmsg = System.Text.Encoding.UTF8.GetString(recdata);
                    SysLog.Info(string.Format("Rec:{0}", Recmsg));
                    if (Recmsg.Contains(":") && Recmsg.Contains("\r\n"))
                    {
                        string temp = Recmsg.Substring(Recmsg.IndexOf(":"));
                        int endIndex = temp.IndexOf("\r\n");
                        temp = temp.Substring(0, endIndex);
                        _blockingDataCollection_1.Add(temp);
                    }
                    else
                    {
                        SysLog.Info(string.Format("PLC_2 指令解析错误,指令内容:{0}", Recmsg));
                    }
                }

                //if (recdata != null) _blockingDataCollection.Add(recdata);  //队列中添加数据
            }
            catch (Exception ex)
            {
                Console.WriteLine("串口数据接收异常:" + ex.StackTrace);
            }
        }

        bool isX = true;
        int PLC_Count = 1; //命令发送次数

        int CheckSocket = 1;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (port.IsOpen)
            {
                /*string str = "ABC123456";
                        port.Write(str);*/
                
                portWrite(by);
                if (PLC_Count > 1)
                {
                    PLC_Count = PLC_Count - 1;
                    SysLog.Info("PLC发送命令次数：" + PLC_Count);
                    return;
                }
                if (isX)
                {
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x32, 0x30, 0x34, 0x30, 0x30, 0x30, 0x30, 0x31, 0x30, 0x45, 0x39, 0x0D, 0x0A };  //X 0~10
                    isX = false;
                }
                else
                {
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x32, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x38, 0x46, 0x30, 0x0D, 0x0A };  //Y 0~8
                    isX = true;
                }

            }

            if (port_2.IsOpen)
            {
                /*string str = "ABC123456";
                        port.Write(str);*/

                portWrite_2(by);
                if (PLC_Count > 1)
                {
                    PLC_Count = PLC_Count - 1;
                    SysLog.Info("PLC发送命令次数：" + PLC_Count);
                    return;
                }
                if (isX)
                {
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x32, 0x30, 0x34, 0x30, 0x30, 0x30, 0x30, 0x31, 0x30, 0x45, 0x39, 0x0D, 0x0A };  //X 0~10
                    isX = false;
                }
                else
                {
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x32, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x38, 0x46, 0x30, 0x0D, 0x0A };  //Y 0~8
                    isX = true;
                }

            }

            CheckSocket = CheckSocket + 1;
            if(CheckSocket > 19)
            {
                CheckSocket = 0;
                if (!client.IsSocketConnected())
                {
                    SysLog.Info(string.Format("SocketClient 连接失败，重新连接...."));
                    Open_Socket();
                }
            }
        }

        object portWriteLock = new object();
        private void portWrite(byte[] by)
        {
            lock (portWriteLock)
            {
                port.Write(by, 0, by.Length);
            }

        }

        private void portWrite_2(byte[] by)
        {
            lock (portWriteLock)
            {
                port_2.Write(by, 0, by.Length);
            }

        }

        private void btn_CloseCom_Click(object sender, EventArgs e)
        {
            if (port == null) return;

            if (timer1 != null)
            {
                timer1.Stop();
                
            }
            SysLog.Info(string.Format("关闭串口，关闭皮带秤"));

            if (port.IsOpen)
            {
                Thread.Sleep(50);
                by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A };
                portWrite(by);
                Thread.Sleep(50);
                portWrite(by);
                Output_0.BackColor = SystemColors.Window;
            }
            cmbPort.Enabled = true;
            btn_OpenCom.Enabled = true;
            port.Close();
            SysLog.Info(string.Format("关闭串口"));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            configini.PortName = cmbPort.Text.ToString();
            IniDeserialize iniDeserialize = new IniDeserialize(pathMeter, configini);
            iniDeserialize.Serialize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] protNames = SerialPort.GetPortNames();
            for (int i = 0; i < protNames.Count(); i++)
            {
                cmbPort.Items.Add(protNames[i]);
                cmbPort_2.Items.Add(protNames[i]);
            }

            IniDeserialize iniDeserialize = new IniDeserialize(pathMeter, configini);
            iniDeserialize.Deserialize(out configini);
            cmbPort.Text = configini.PortName;
            cmbPort_2.Text = configini.PortName_2;

            Open_Socket();
        }

        private void btn_Ouput_0_Click(object sender, EventArgs e)
        {
            //byte[] by = { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x46, 0x46, 0x30, 0x30, 0x46, 0x36, 0x0D, 0x0A };
            //byte[] by = new byte[17];
            Button btn_Temp = sender as Button;
            string str = "On";
            string btnTag = Convert.ToString(btn_Temp.Tag);
            if (btn_Temp.BackColor == SystemColors.Control)  // 置 1 On  FF00
            {
                str = "On";
                btn_Temp.BackColor = SystemColors.GrayText;
                if (btnTag == "0") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x46, 0x46, 0x30, 0x30, 0x46, 0x36, 0x0D, 0x0A }; }
                if (btnTag == "1") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x31, 0x46, 0x46, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A }; }
                if (btnTag == "2") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x32, 0x46, 0x46, 0x30, 0x30, 0x46, 0x34, 0x0D, 0x0A }; }
                if (btnTag == "3") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x33, 0x46, 0x46, 0x30, 0x30, 0x46, 0x33, 0x0D, 0x0A }; }
                if (btnTag == "4") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x34, 0x46, 0x46, 0x30, 0x30, 0x46, 0x32, 0x0D, 0x0A }; }
                if (btnTag == "5") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x35, 0x46, 0x46, 0x30, 0x30, 0x46, 0x31, 0x0D, 0x0A }; }
                if (btnTag == "6") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x36, 0x46, 0x46, 0x30, 0x30, 0x46, 0x30, 0x0D, 0x0A }; }
                if (btnTag == "7") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x37, 0x46, 0x46, 0x30, 0x30, 0x45, 0x46, 0x0D, 0x0A }; }

                if (btnTag == "8") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x46, 0x46, 0x30, 0x30, 0x46, 0x36, 0x0D, 0x0A }; }
                if (btnTag == "9") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x31, 0x46, 0x46, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A }; }
                if (btnTag == "10") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x32, 0x46, 0x46, 0x30, 0x30, 0x46, 0x34, 0x0D, 0x0A }; }
                if (btnTag == "11") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x33, 0x46, 0x46, 0x30, 0x30, 0x46, 0x33, 0x0D, 0x0A }; }
                if (btnTag == "12") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x34, 0x46, 0x46, 0x30, 0x30, 0x46, 0x32, 0x0D, 0x0A }; }
                if (btnTag == "13") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x35, 0x46, 0x46, 0x30, 0x30, 0x46, 0x31, 0x0D, 0x0A }; }
                if (btnTag == "14") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x36, 0x46, 0x46, 0x30, 0x30, 0x46, 0x30, 0x0D, 0x0A }; }
                if (btnTag == "15") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x37, 0x46, 0x46, 0x30, 0x30, 0x45, 0x46, 0x0D, 0x0A }; }
            }
            else                                            // 置 0  Off 0000
            {
                str = "Off";
                btn_Temp.BackColor = SystemColors.Control;
                if (btnTag == "0") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A }; }
                if (btnTag == "1") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x31, 0x30, 0x30, 0x30, 0x30, 0x46, 0x34, 0x0D, 0x0A }; }
                if (btnTag == "2") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x32, 0x30, 0x30, 0x30, 0x30, 0x46, 0x33, 0x0D, 0x0A }; }
                if (btnTag == "3") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x33, 0x30, 0x30, 0x30, 0x30, 0x46, 0x32, 0x0D, 0x0A }; }
                if (btnTag == "4") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x34, 0x30, 0x30, 0x30, 0x30, 0x46, 0x31, 0x0D, 0x0A }; }
                if (btnTag == "5") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x46, 0x30, 0x0D, 0x0A }; }
                if (btnTag == "6") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x36, 0x30, 0x30, 0x30, 0x30, 0x45, 0x46, 0x0D, 0x0A }; }
                if (btnTag == "7") { by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x37, 0x30, 0x30, 0x30, 0x30, 0x45, 0x45, 0x0D, 0x0A }; }

                if (btnTag == "8") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A }; }
                if (btnTag == "9") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x31, 0x30, 0x30, 0x30, 0x30, 0x46, 0x34, 0x0D, 0x0A }; }
                if (btnTag == "10") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x32, 0x30, 0x30, 0x30, 0x30, 0x46, 0x33, 0x0D, 0x0A }; }
                if (btnTag == "11") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x33, 0x30, 0x30, 0x30, 0x30, 0x46, 0x32, 0x0D, 0x0A }; }
                if (btnTag == "12") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x34, 0x30, 0x30, 0x30, 0x30, 0x46, 0x31, 0x0D, 0x0A }; }
                if (btnTag == "13") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x46, 0x30, 0x0D, 0x0A }; }
                if (btnTag == "14") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x36, 0x30, 0x30, 0x30, 0x30, 0x45, 0x46, 0x0D, 0x0A }; }
                if (btnTag == "15") { by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x37, 0x30, 0x30, 0x30, 0x30, 0x45, 0x45, 0x0D, 0x0A }; }
            }
            if (port != null)
            {
                if (port.IsOpen)
                    portWrite(by);
                //port.Write(by, 0, by.Length);
            }

            if (port_2 != null)
            {
                if (port_2.IsOpen)
                    portWrite_2(by_2);
                //port.Write(by, 0, by.Length);
            }
            int tag = Convert.ToInt32(btnTag);
            string cmd = "";
            if(tag > 8)
            {
                cmd = System.Text.Encoding.UTF8.GetString(by_2);
            }
            else
            {
                cmd = System.Text.Encoding.UTF8.GetString(by);
            }
            
            SysLog.Info(string.Format("按钮:{0},内容:{2},指令'{1}'", btnTag, cmd, str));
        }

        private void btn_OpenSocket_Click(object sender, EventArgs e)
        {
            Open_Socket();
        }
        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close_Socket();
        }

        SocketClient client;
        private void Open_Socket()
        {
            string ip = configini.IP;
            int port = Convert.ToInt32(configini.Port);
            try
            {
                client = new SocketClient(ip, port);
                client.HandleRecMsg = (bytes, theClient) =>
                {
                    var task = new Task(() =>
                    {
                        var msg = Encoding.UTF8.GetString(bytes);
                        Console.WriteLine("收到内容:" + msg);
                        SysLog.Info("Socket接收:" + msg);
                        //var mags = msg.Split(new[] { "\r\n" }, StringSplitOptions.None);
                        RecMsg(msg);

                    });
                    task.Start();
                };
                client.HandleClientClose = thecilent =>
                {
                    Console.WriteLine("SocketClient 关闭！");
                    Invoke(new Action(() => {
                        labSocketInfo.Text = "Socket Closed.... ";
                        btn_OpenSocket.Enabled = true;
                    }));
                    
                };
                client.HandleException = ex =>
                {
                    Console.WriteLine("SocketClient 异常！");
                    SysLog.Info("SocketClient 异常：" + ex.Message);
                };

                client.HandleClientStarted = theClient =>
                {
                    Invoke(new Action(() => { 
                        labSocketInfo.Text = "Socket Open Success";
                        btn_OpenSocket.Enabled = false;
                    }));
                    
                    
                    SysLog.Info(string.Format("Socket,IP:{0},Port:{1},打开成功", configini.IP, configini.Port));
                };
                client.StartClient();
                
            }
            catch (Exception ex)
            {
                labSocketInfo.Text = "Socket Open Fail";
                SysLog.Info(string.Format("Socket,IP:{0},Port:{1},打开失败，{3}", configini.IP, configini.Port,ex.Message));
            }
            
        }

        private void RecMsg(string msg)
        {
            SocketRec rec = new SocketRec();

            JObject jo = (JObject)JsonConvert.DeserializeObject(msg);
            string strCMD = jo["cmd"].ToString();
            if(strCMD == "ProductInfo")
            {
                if (configini.para1 == jo["para1"].ToString())
                {
                    Socket_hz = 0;
                    i_ServerRec = 1;
                }
            }

            if(strCMD == "SaveOK")
            {
                //if("1" == jo["para1"].ToString())  //202401022 讨论后只判断SaveOK，不判断para1参数，并且不再回复服务端的SaveOK
                {
                    //i_HW1_State = 0;
                    i_ServerRec = 1;
                    Socket_hz = 0;
                    i_SaveOK = 1;
                    if (i_Big == 1) i_Big = 2;
                    PLC_Count = 4;  //设置发送次数
                    by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x46, 0x46, 0x30, 0x30, 0x46, 0x36, 0x0D, 0x0A };
                    //if (client.IsSocketConnected())
                    //{
                    //    string strMsg = "#{\"cmd\":\"SaveOK\",\"para1\":\"1\",\"para2\":\"\"}%";
                    //    client.Send(strMsg);
                    //}
                }
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            sendServer_Small();
        }

        private void btnBig_Click(object sender, EventArgs e)
        {
            //sendServer_Big();
            SysLog.Debug(string.Format("X0:{0},物体'{1}'", "1", " 有，停止皮带秤"));
            //SysLog.Info(string.Format("X0:{0},物体'{1}'", "1", " 有，停止皮带秤"));
        }
        private void sendServer_Big()
        {
            if (client.IsSocketConnected())
            {
                string strMsg = "#{\"cmd\":\"ProductInfo\",\"para1\":\"2\",\"para2\":\"\"}%";
                client.Send(strMsg);
                
            }
        }

        private void sendServer_Small()
        {
            if (client.IsSocketConnected())
            {
                string strMsg = "#{\"cmd\":\"ProductInfo\",\"para1\":\"1\",\"para2\":\"\"}%";
                client.Send(strMsg);
            }
        }

        private void Close_Socket()
        {
            client.Close();
            //labSocketInfo.Text = "Socket Closed";
            SysLog.Info(string.Format("Socket,IP:{0},Port:{1},关闭成功", configini.IP, configini.Port));
            //btn_Close.Enabled = false;
        }

        private void btn_OpenCom_2_Click(object sender, EventArgs e)
        {
            if (configini == null)
            {
                MessageBox.Show("配置文件为空！请检查");
                return;
            }
            string msg = "";
            if (port_2 == null) port_2 = new SerialPort();
            port_2.PortName = cmbPort_2.Text.ToString();
            port_2.BaudRate = int.Parse(configini.Baud_2);
            port_2.Parity = (Parity)Enum.Parse(typeof(Parity), configini.Parity_2);
            port_2.DataBits = int.Parse(configini.Databit_2);
            port_2.StopBits = (StopBits)Enum.Parse(typeof(StopBits), configini.Stopbit_2);
            port_2.WriteTimeout = 300;
            port_2.ReadTimeout = 300;

            if (!port_2.IsOpen)
            {
                try
                {
                    _custmnThread_2 = new Thread(CustumeDataCollection_2);
                    _custmnThread_2.IsBackground = true;
                    port_2.Open();
                    if (port_2.IsOpen)
                    {
                        port_2.DataReceived += Port_DataReceived_2;
                        _custmnThread_2.Start();
                    }
                    MessageBox.Show(string.Format("串口2{0},打开成功", port_2.PortName));
                    SysLog.Info(string.Format("串口2{0},打开成功", port_2.PortName));

                    cmbPort_2.Enabled = false;
                    btn_OpenCom_2.Enabled = false;
                    //Aotu_PD();//开启皮带秤转动
                    Thread.Sleep(50);

                    if(!timer1.Enabled)
                        timer1.Start();
                }
                catch (Exception ex)
                {
                    msg = ex.Source + ex.Message;
                    if (port_2.IsOpen) port_2.Close();
                    MessageBox.Show(string.Format("串口{0},打开失败，请检查！", port_2.PortName));
                    port.Dispose();
                    port = null;
                }
            }
        }

        private void btn_CloseCom_2_Click(object sender, EventArgs e)
        {
            if (port_2 == null) return;

            if (timer1 != null)
            {
                timer1.Stop();

            }
            SysLog.Info(string.Format("关闭串口222"));

            if (port_2.IsOpen)
            {
                Thread.Sleep(50);
                by_2 = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A };
                portWrite_2(by_2);
                Thread.Sleep(50);
                portWrite_2(by_2);
                Output_0.BackColor = SystemColors.Window;
            }
            cmbPort_2.Enabled = true;
            btn_OpenCom_2.Enabled = true;
            port_2.Close();
            SysLog.Info(string.Format("关闭串口222"));
        }

        private void btnSave_2_Click(object sender, EventArgs e)
        {
            configini.PortName_2 = cmbPort_2.Text.ToString();
            IniDeserialize iniDeserialize = new IniDeserialize(pathMeter, configini);
            iniDeserialize.Serialize();
        }

        private void Aotu_PD()
        {
            by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x46, 0x46, 0x30, 0x30, 0x46, 0x36, 0x0D, 0x0A };
            if (port != null)
            {
                if (port.IsOpen)
                {
                    portWrite(by);
                    Thread.Sleep(50);
                    portWrite(by);
                }
                    
            }
            string cmd = System.Text.Encoding.UTF8.GetString(by);
            SysLog.Info(string.Format("开启皮带秤:{0}", cmd));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port == null) return;

            if (timer1 != null)
            {
                timer1.Stop();
            }
            SysLog.Info(string.Format("关闭程序，关闭皮带秤"));

            if (port.IsOpen)
            {
                /*string str = "ABC123456";
                        port.Write(str);*/
                Thread.Sleep(50);
                by = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x35, 0x0D, 0x0A };
                portWrite(by);
                Thread.Sleep(50);
                portWrite(by);
                Output_0.BackColor = SystemColors.Window;

            }
            Thread.Sleep(50);

            Close_Socket();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //BigOrSmall();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (i_Intput_0 == 0)
            {
                i_Intput_0 = 1;
                button3.BackColor = Color.Yellow;
            }
            else if (i_Intput_0 == 1)
            {
                i_Intput_0 = 0;
                button3.BackColor = Color.Gray;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (i_Intput_1 == 0)
            {
                i_Intput_1 = 1;
                button4.BackColor = Color.Yellow;
            }
            else if (i_Intput_1 == 1)
            {
                i_Intput_1 = 0;
                button4.BackColor = Color.Gray;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            i_Intput_0 = 0;
            i_Intput_1 = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            i_ServerRec = 1;
            i_SaveOK = 1;
            if (i_Big == 1) i_Big = 2;
        }


    }
}
