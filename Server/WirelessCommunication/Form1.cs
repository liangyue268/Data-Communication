using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.IO;

namespace WirelessCommunication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        int times;//循环次数，对其进行限制可避免进入死循环

        # region 编辑参数配置中的数据
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)     //调出数据修改框，修改数据
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem TheListview = listView1.SelectedItems[0];        //获得选中项
                int index = TheListview.Index;
                int x = TheListview.SubItems[1].Bounds.Left;                  //获得指定区域的首坐标
                int y = TheListview.SubItems[1].Bounds.Right;                 //获得指定区域的末坐标
                if (e.X > x && e.X < y)
                {
                    switch (index)
                    {
                        case 0: textBox3.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                textBox3.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;    //单元格宽度，下同
                                textBox3.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;  //单元格高度，下同
                                textBox3.Text = listView1.Items[0].SubItems[1].Text;  //显示ip地址，仅供复制使用，不对listView1中的值修改
                                textBox3.Visible = true; textBox3.Focus(); break;//显示控件并获得焦点，下同
                        case 1: comboBox6.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                comboBox6.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;
                                comboBox6.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;
                                comboBox6.Visible = true; comboBox6.Focus(); break;
                        case 2: numericUpDown1.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                numericUpDown1.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;
                                numericUpDown1.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;
                                numericUpDown1.Visible = true; numericUpDown1.Focus(); break;
                        case 3: textBox6.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                textBox6.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;
                                textBox6.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;
                                textBox6.Visible = true; textBox6.Focus(); break;
                        default:
                            break;
                    }
                }
            }
        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)    //实际修改ListView1中的数据，下同
        {
            listView1.Items[1].SubItems[1].Text = comboBox6.Text;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            listView1.Items[2].SubItems[1].Text = numericUpDown1.Value.ToString();
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            listView1.Items[3].SubItems[1].Text = textBox6.Text;
        }
        private void listView1_MouseDown(object sender, MouseEventArgs e)   //隐藏修改窗口
        {
            textBox3.Visible = comboBox6.Visible = numericUpDown1.Visible = textBox6.Visible = false;
        }

        # endregion

        #region 串口操作
        private void button1_Click(object sender, EventArgs e)     //打开串口
        {
            serialPort1.PortName = comboBox1.Text;                             //端口号
            serialPort1.Encoding = Encoding.UTF8;                              //UTF8编码
            serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);            //波特率
            serialPort1.DataBits = Convert.ToInt32(comboBox3.Text);            //数据位
            switch (comboBox4.Text)                                            //校验位
            {
                case "无":   serialPort1.Parity = Parity.None; break;
                case "奇":   serialPort1.Parity = Parity.Odd; break;
                case "偶":   serialPort1.Parity = Parity.Even; break;
                case "标志": serialPort1.Parity = Parity.Mark; break;
                case "空格": serialPort1.Parity = Parity.Space; break;
                default:     serialPort1.Parity = Parity.None; break;
            }
            switch (comboBox5.Text)                                            //停止位
            {
                case "1":   serialPort1.StopBits = StopBits.One; break;
                case "1.5": serialPort1.StopBits = StopBits.OnePointFive; break;
                case "2":   serialPort1.StopBits = StopBits.Two; break;
                default:    serialPort1.StopBits = StopBits.None; break;
            }
            try
            {
                serialPort1.Open();
                textBox4.AppendText(">>打开串口成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();//光标自动显示至最后一行，下同
                初始化ToolStripMenuItem.Enabled = true;  //启用初始化，可以进行下一步
                //开关串口按钮
                button1.Enabled = false;
                button2.Enabled = true;
                comboBox1.Enabled = comboBox2.Enabled = comboBox3.Enabled = comboBox4.Enabled = comboBox5.Enabled = false;    //禁用下拉框修改串口参数
            }
            catch                      //检查端口是否被占用
            {
                textBox4.Text += ">>打开串口失败！" + Environment.NewLine;
            }    
        }
        private void button2_Click(object sender, EventArgs e)     //关闭串口
        {
            serialPort1.Close();
            textBox4.AppendText(">>关闭串口成功！" + Environment.NewLine);
            textBox4.ScrollToCaret();
            //禁用任何程序，除参数修改
            comboBox1.Enabled = comboBox2.Enabled = comboBox3.Enabled = comboBox4.Enabled = comboBox5.Enabled = true;      //使用下拉框修改串口参数
            初始化ToolStripMenuItem.Enabled = 断开连接ToolStripMenuItem.Enabled = 建立连接ToolStripMenuItem.Enabled = false;
            //开关串口按钮
            button1.Enabled = true;
            button2.Enabled = false;
        }   

        # endregion

        # region 公共操作
        int DataLength, LastTotalDatalength = 0; //用于测速
        Disconnection disconnect = new Disconnection();
        private void Form1_Load(object sender, EventArgs e)        //获取串口端口号+时间显示+初始化接收数据空间
        {
            string[] ports;
            ports = SerialPort.GetPortNames();       //自动获取端口号
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            comboBox1.SelectedIndex = 0;             //设置下拉框默认值
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            //            
            Thread time = new Thread(new ThreadStart(refresh));
            time.Start();
            //
            reg[0] = new ArrayList(1024 * 1024);           //初始化接收数据空间（1MB）
            reg[1] = new ArrayList(1024 * 1024);
            reg[2] = new ArrayList(1024 * 1024);
            reg[3] = new ArrayList(1024 * 1024);
            reg[4] = new ArrayList(1024 * 1024);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)  //关闭窗体
        {
            DialogResult dt = MessageBox.Show("确认退出？", "系统消息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if(dt==DialogResult.No)
            {
                e.Cancel = true;     //取消关闭窗口
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)    //重启设备，为下一次运行做准备，可避免插拔电源
        {
            //
            # region 重启设备
            if(serialPort1.IsOpen)
            {
                disconnect.Reset(serialPort1);
            }           
            # endregion
            //
            serialPort1.Close();//关闭串口
        }
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)   //关于
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }
        private void button5_Click(object sender, EventArgs e)   //清除状态文本框文本
        {
            textBox4.Clear();
        }
        private void button6_Click(object sender, EventArgs e)   //清除数据文本框文本
        {
            textBox5.Clear(); textBox7.Clear(); textBox8.Clear(); textBox9.Clear(); textBox10.Clear();
            textBox1.Clear(); textBox2.Clear(); textBox11.Clear(); textBox12.Clear(); textBox13.Clear();
            reg[0].Clear(); reg[1].Clear(); reg[2].Clear(); reg[3].Clear(); reg[4].Clear();
            LastTotalDatalength = 0;//清空数据统计
        }
        private void refresh()   //用于刷新时间+测速
        {
            while (Application.AllowQuit)
            {
                toolStripStatusLabel3.Text = "当前系统时间：" + DateTime.Now.ToString();//显示当前时间
                DataLength = reg[0].Count + reg[1].Count + reg[2].Count + reg[3].Count + reg[4].Count - LastTotalDatalength;
                LastTotalDatalength = reg[0].Count + reg[1].Count + reg[2].Count + reg[3].Count + reg[4].Count;
                toolStripStatusLabel1.Text = "当前传输速度：" + (((double)DataLength / 1000) / (double)1).ToString("F4") + "KB/s";//显示当前接收速度
                Thread.Sleep(1000);//1000毫秒后再显示一次，实现刷新
            }
        }

        # endregion

        # region 初始化
        Initialization initial = new Initialization();
        bool detection_state, echo_state;
        private void 初始化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items[1].SubItems[1].Text == "" || listView1.Items[2].SubItems[1].Text == "" || listView1.Items[3].SubItems[1].Text == "")//存在未配置的参数
            {
                textBox4.AppendText(">>无法建立连接,请确保参数配置完成！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            else
            {
                comboBox6.Enabled = numericUpDown1.Enabled = textBox6.Enabled = false;//停止配置参数（允许获取ip地址）
                初始化ToolStripMenuItem.Enabled = false;
                //
                # region 检测板卡函数
                times = 0;
                do
                {
                    if (times == 3)//连续3次失败
                    {
                        textBox4.AppendText(">>检测板卡失败！" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                        break;
                    }
                    times++;
                    detection_state = initial.AtDetection(serialPort1);
                }
                while (!detection_state);
                if (detection_state)
                {
                    textBox4.AppendText(">>检测板卡成功！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
                # endregion
                //
                # region 关闭回显函数
                times = 0;
                do
                {
                    if (times == 3)//连续3次失败
                    {
                        textBox4.AppendText(">>关闭回显失败！" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                        break;
                    }
                    times++;
                    echo_state = initial.StopEcho(serialPort1);
                }
                while (!echo_state);
                if (echo_state)
                {
                    textBox4.AppendText(">>关闭回显成功！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
                # endregion 
                //
                if (detection_state && echo_state)
                {
                    textBox4.AppendText(">>初始化成功！等待客户端建立连接……<<" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
                else
                {
                    textBox4.AppendText(">>初始化失败！<<" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
                //
                flag = 0;//等待读取短信
                this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            }
        }

        # endregion

        # region 公用串口接收数据事件
        int flag;//0为读短信，1为收数据
        SMS message = new SMS();
        string index, msg;
        ReceiveData receive = new ReceiveData();
        byte[][] ReceivedData;
        ArrayList[] reg = new ArrayList[5];
        byte[] temp = new byte[0], Data, IncompleteData = new byte[0], RealData;//temp为串口接收的不完整数据的临时缓存
        bool close;//断开连接判断标志
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if(flag == 0)
            {
                index = message.ArrivalIndication(serialPort1);//收短信提醒函数
                if (index != "-1")
                {
                    DisplayStatus(0);
                    msg = message.ReadShortMessage(serialPort1, index, listView1.Items[3].SubItems[1].Text);//读取短信函数
                    if (msg != "")
                    {
                        DisplayStatus(1);
                        this.serialPort1.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);//接收短信完成，移除串口接收数据事件
                        this.Invoke(new EventHandler(Connection));
                    }
                }
            }
            else
            {
                //
                # region 接收数据函数组
                //接收
                ReceivedData = receive.ReceiveFromSerialPort(serialPort1, ref temp);
                //解析
                Data = receive.Parse(ReceivedData);
                //存储
                RealData = new byte[Data.Length + IncompleteData.Length];
                IncompleteData.CopyTo(RealData, 0);
                Data.CopyTo(RealData, IncompleteData.Length);
                IncompleteData = receive.ConvertToOriginalData(RealData, ref reg, ref close);
                # endregion             
                //
                this.Invoke(new EventHandler(DisplayData));
                if(close)
                {
                    this.serialPort1.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
                    this.Invoke(new EventHandler(DisConnection));
                }
            }
        }
        public void DisplayStatus(int i)   //显示状态
        {
            if (i == 0)
            {
                textBox4.AppendText(">>收到短信提醒！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            else
            {
                textBox4.AppendText(">>读取短信成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
        }

        # endregion

        # region 建立连接
        Connection connect = new Connection();
        bool SendMessage_state, DeleteMessage_state, InitialConnection_state, IpListening_state;
        private void Connection(object sender, EventArgs e)  //接收端响应发送端连接请求
        {
            //
            # region 删除短信函数（内部事件，不做状态反馈，删除失败也无所谓）
            times = 0;
            do
            {
                if (times == 3)//连续3次失败
                {
                    break;
                }
                times++;
                DeleteMessage_state = message.DeleteShortMessage(serialPort1);
            }
            while (!DeleteMessage_state);
            # endregion
            //
            # region 网络初始化函数
            times = 0;
            do
            {
                if (times == 3)//连续3次失败
                {
                    textBox4.AppendText(">>申请ip失败！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                    break;
                }
                times++;
                InitialConnection_state = initial.InitialConnection(serialPort1);
            }
            while (!InitialConnection_state);
            if (InitialConnection_state)
            {
                textBox4.AppendText(">>申请ip成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            # endregion
            //
            # region 获取ip地址函数
            listView1.Items[0].SubItems[1].Text = initial.GetIpAddress(serialPort1);//在参数配置中显示ip地址
            textBox4.AppendText(">>获取ip地址成功！" + Environment.NewLine);
            textBox4.ScrollToCaret();
            # endregion
            //
            # region ip监听函数
            times = 0;
            do
            {
                if (times == 3)//连续3次失败
                {
                    textBox4.AppendText(">>ip监听失败！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                    break;
                }
                times++;
                IpListening_state = connect.IpListening(serialPort1, listView1.Items[1].SubItems[1].Text, listView1.Items[2].SubItems[1].Text);
            }
            while (!IpListening_state);
            if (IpListening_state)
            {
                textBox4.AppendText(">>ip监听成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            # endregion
            //
            # region 发送短信函数
            times = 0;
            do
            {
                if (times == 3)//连续3次失败
                {
                    textBox4.AppendText(">>发送短信失败！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                    break;
                }
                times++;
                SendMessage_state = message.SendShortMessage(serialPort1, listView1.Items[3].SubItems[1].Text, listView1.Items[0].SubItems[1].Text + ':' + listView1.Items[2].SubItems[1].Text);
            }
            while (!SendMessage_state);
            if (SendMessage_state)
            {
                textBox4.AppendText(">>发送短信成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            # endregion
            //
            if(SendMessage_state && InitialConnection_state && IpListening_state)
            {
                textBox4.AppendText(">>建立连接成功！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            else
            {
                textBox4.AppendText(">>建立连接失败！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            //
            flag = 1;//准备接收数据
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
        }

        # endregion

        # region 接收数据（显示+保存）
        StringBuilder sb1 = new StringBuilder(1024 * 1024);
        StringBuilder sb2 = new StringBuilder(1024 * 1024);
        StringBuilder sb3 = new StringBuilder(1024 * 1024);
        StringBuilder sb4 = new StringBuilder(1024 * 1024);
        StringBuilder sb5 = new StringBuilder(1024 * 1024);
        private void DisplayData(object sender, EventArgs e)  //显示数据（接收数据和radiobutton同时引用）
        {
            byte[] display1 = (byte[])reg[0].ToArray(typeof(byte));
            byte[] display2 = (byte[])reg[1].ToArray(typeof(byte));
            byte[] display3 = (byte[])reg[2].ToArray(typeof(byte));
            byte[] display4 = (byte[])reg[3].ToArray(typeof(byte));
            byte[] display5 = (byte[])reg[4].ToArray(typeof(byte));
            //显示文件大小
            textBox1.Text = display1.Length.ToString();
            textBox2.Text = display2.Length.ToString();
            textBox11.Text = display3.Length.ToString();
            textBox12.Text = display4.Length.ToString();
            textBox13.Text = display5.Length.ToString();
            if(checkBox1.Checked)//判断是否在文本框中显示内容
            {
                //显示文件内容
                if(radioButton2.Checked)
                {
                    textBox5.Text = Encoding.UTF8.GetString(display1);
                    textBox7.Text = Encoding.UTF8.GetString(display2);
                    textBox8.Text = Encoding.UTF8.GetString(display3);
                    textBox9.Text = Encoding.UTF8.GetString(display4);
                    textBox10.Text = Encoding.UTF8.GetString(display5);
                }
                else
                {
                    foreach(byte b in display1)
                    {
                        sb1.Append(b.ToString("X2") + " ");
                    }
                    foreach (byte b in display2)
                    {
                        sb2.Append(b.ToString("X2") + " ");
                    }
                    foreach (byte b in display3)
                    {
                        sb3.Append(b.ToString("X2") + " ");
                    }
                    foreach (byte b in display4)
                    {
                        sb4.Append(b.ToString("X2") + " ");
                    }
                    foreach (byte b in display5)
                    {
                        sb5.Append(b.ToString("X2") + " ");
                    }
                    textBox5.Text = sb1.ToString();
                    textBox7.Text = sb2.ToString();
                    textBox8.Text = sb3.ToString();
                    textBox9.Text = sb4.ToString();
                    textBox10.Text = sb5.ToString();
                    sb1.Clear(); sb2.Clear(); sb3.Clear(); sb4.Clear(); sb5.Clear();//显示完成清除所有字符
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)    //保存接收到的数据
        {
            byte[] save1 = (byte[])reg[0].ToArray(typeof(byte));
            byte[] save2 = (byte[])reg[1].ToArray(typeof(byte));
            byte[] save3 = (byte[])reg[2].ToArray(typeof(byte));
            byte[] save4 = (byte[])reg[3].ToArray(typeof(byte));
            byte[] save5 = (byte[])reg[4].ToArray(typeof(byte));
            DialogResult dt = folderBrowserDialog1.ShowDialog();
            if (dt == DialogResult.OK)
            {
                if (radioButton1.Checked)//存储为二进制文件
                {                    
                    BinaryWriter writer1 = new BinaryWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\1.dat", FileMode.Create, FileAccess.Write));
                    BinaryWriter writer2 = new BinaryWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\2.dat", FileMode.Create, FileAccess.Write));
                    BinaryWriter writer3 = new BinaryWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\3.dat", FileMode.Create, FileAccess.Write));
                    BinaryWriter writer4 = new BinaryWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\4.dat", FileMode.Create, FileAccess.Write));
                    BinaryWriter writer5 = new BinaryWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\5.dat", FileMode.Create, FileAccess.Write));
                    # region 存储数据1
                    int index = 0;
                    for (int i = 0; i < save1.Length / 1024; i++)
                    {
                        writer1.Write(save1, index, 1024);
                        index += 1024;
                    }
                    writer1.Write(save1, index, save1.Length - index);
                    writer1.Close();
                    # endregion
                    # region 存储数据2
                    index = 0;
                    for (int i = 0; i < save2.Length / 1024; i++)
                    {
                        writer2.Write(save2, index, 1024);
                        index += 1024;
                    }
                    writer2.Write(save2, index, save2.Length - index);
                    writer2.Close();
                    # endregion
                    # region 存储数据3
                    index = 0;
                    for (int i = 0; i < save3.Length / 1024; i++)
                    {
                        writer3.Write(save3, index, 1024);
                        index += 1024;
                    }
                    writer3.Write(save3, index, save3.Length - index);
                    writer3.Close();
                    # endregion
                    # region 存储数据4
                    for (int i = 0; i < save4.Length / 1024; i++)
                    {
                        writer4.Write(save4, index, 1024);
                        index += 1024;
                    }
                    writer4.Write(save4, index, save4.Length - index);
                    writer4.Close();
                    # endregion
                    # region 存储数据5
                    index = 0;
                    for (int i = 0; i < save5.Length / 1024; i++)
                    {
                        writer5.Write(save5, index, 1024);
                        index += 1024;
                    }
                    writer5.Write(save1, index, save5.Length - index);
                    writer5.Close();
                    # endregion
                }
                else
                {
                    char[] savetext1 = Encoding.UTF8.GetChars(save1);
                    char[] savetext2 = Encoding.UTF8.GetChars(save2);
                    char[] savetext3 = Encoding.UTF8.GetChars(save3);
                    char[] savetext4 = Encoding.UTF8.GetChars(save4);
                    char[] savetext5 = Encoding.UTF8.GetChars(save5);
                    StreamWriter writer1 = new StreamWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\1.txt", FileMode.Create, FileAccess.Write), Encoding.Default);
                    StreamWriter writer2 = new StreamWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\2.txt", FileMode.Create, FileAccess.Write), Encoding.Default);
                    StreamWriter writer3 = new StreamWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\3.txt", FileMode.Create, FileAccess.Write), Encoding.Default);
                    StreamWriter writer4 = new StreamWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\4.txt", FileMode.Create, FileAccess.Write), Encoding.Default);
                    StreamWriter writer5 = new StreamWriter(new FileStream(folderBrowserDialog1.SelectedPath + "\\5.txt", FileMode.Create, FileAccess.Write), Encoding.Default);
                    # region 存储数据1
                    int index = 0;
                    for (int i = 0; i < savetext1.Length / 1024; i++)
                    {
                        writer1.Write(savetext1, index, 1024);
                        index += 1024;
                    }
                    writer1.Write(savetext1, index, savetext1.Length - index);
                    writer1.Close();
                    # endregion
                    # region 存储数据2
                    index = 0;
                    for (int i = 0; i < savetext2.Length / 1024; i++)
                    {
                        writer2.Write(savetext2, index, 1024);
                        index += 1024;
                    }
                    writer2.Write(savetext2, index, savetext2.Length - index);
                    writer2.Close();
                    # endregion
                    # region 存储数据3
                    index = 0;
                    for (int i = 0; i < savetext3.Length / 1024; i++)
                    {
                        writer3.Write(savetext3, index, 1024);
                        index += 1024;
                    }
                    writer3.Write(savetext3, index, savetext3.Length - index);
                    writer3.Close();
                    # endregion
                    # region 存储数据4
                    index = 0;
                    for (int i = 0; i < savetext4.Length / 1024; i++)
                    {
                        writer4.Write(savetext4, index, 1024);
                        index += 1024;
                    }
                    writer4.Write(savetext4, index, savetext4.Length - index);
                    writer4.Close();
                    # endregion
                    # region 存储数据5
                    index = 0;
                    for (int i = 0; i < savetext5.Length / 1024; i++)
                    {
                        writer5.Write(savetext5, index, 1024);
                        index += 1024;
                    }
                    writer5.Write(savetext5, index, savetext5.Length - index);
                    writer5.Close();
                    # endregion
                }
            }
        }
        
        # endregion

        # region 断开连接
        bool disconnect_state;
        private void DisConnection(object sender, EventArgs e)
        {
            close = false;//变更断开连接判断标志
            //
            # region 断开连接函数
            times = 0;
            do
            {
                if (times == 3)//连续3次失败
                {
                    textBox4.AppendText(">>断开连接失败！<<" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                    break;
                }
                times++;
                disconnect_state = disconnect.DisconnectAll(serialPort1);
            }
            while (!disconnect_state);
            if (disconnect_state)
            {
                textBox4.AppendText(">>断开连接成功！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            # endregion
            //
            flag = 0;//等待读取短信
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            listView1.Items[0].SubItems[1].Text = "";//清除本次ip地址
            textBox4.AppendText(">>空闲！等待客户端建立连接……" + Environment.NewLine);
            textBox4.ScrollToCaret();            
        }

        # endregion

    }
}