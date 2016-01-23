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
                        case 3: numericUpDown2.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                numericUpDown2.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;
                                numericUpDown2.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;
                                numericUpDown2.Visible = true; numericUpDown2.Focus(); break;
                        case 4: comboBox7.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                comboBox7.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;
                                comboBox7.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;
                                comboBox7.Visible = true; comboBox7.Focus(); break;
                        case 5: comboBox8.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                comboBox8.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;
                                comboBox8.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;
                                comboBox8.Visible = true; comboBox8.Focus(); break;
                        case 6: numericUpDown4.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
                                numericUpDown4.Width = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Width;
                                numericUpDown4.Height = TheListview.GetSubItemAt(e.X, e.Y).Bounds.Height;
                                numericUpDown4.Visible = true; numericUpDown4.Focus(); break;
                        case 7: textBox6.Location = new Point(TheListview.SubItems[1].Bounds.Location.X + listView1.Bounds.Left + 2, TheListview.SubItems[1].Bounds.Location.Y + listView1.Bounds.Top + 2);
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
            if (comboBox6.Text == "UDP")
            {
                listView1.Items[4].SubItems[1].Text = "否";
                comboBox7.SelectedIndex = 1;
                comboBox7.Enabled = false;
            }
            else
            {
                comboBox7.Enabled = true;
            }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            listView1.Items[2].SubItems[1].Text = numericUpDown1.Value.ToString();
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            listView1.Items[3].SubItems[1].Text = numericUpDown2.Value.ToString();
        }
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items[4].SubItems[1].Text = comboBox7.Text;
            if(comboBox7.Text == "是")
            {
                listView1.Items[1].SubItems[1].Text = "TCP";
                comboBox6.SelectedIndex = 0;
                comboBox6.Enabled = false;
            }
            else
            {
                comboBox6.Enabled = true;
            }
        }
        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items[5].SubItems[1].Text = comboBox8.Text;
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            listView1.Items[6].SubItems[1].Text = numericUpDown4.Value.ToString();
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            listView1.Items[7].SubItems[1].Text = textBox6.Text;
        }
        private void listView1_MouseDown(object sender, MouseEventArgs e)  //隐藏修改窗口
        {
            textBox3.Visible = comboBox6.Visible = numericUpDown1.Visible = numericUpDown2.Visible = comboBox7.Visible = comboBox8.Visible = numericUpDown4.Visible = textBox6.Visible = false;
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
                textBox4.AppendText(">>打开串口失败！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }    
        }
        private void button2_Click(object sender, EventArgs e)     //关闭串口
        {
            serialPort1.Close();
            textBox4.AppendText(">>关闭串口成功！" + Environment.NewLine);
            textBox4.ScrollToCaret();
            //禁用任何按钮，除参数修改
            comboBox1.Enabled = comboBox2.Enabled = comboBox3.Enabled = comboBox4.Enabled = comboBox5.Enabled = true;      //使用下拉框修改串口参数
            初始化ToolStripMenuItem.Enabled = 断开连接ToolStripMenuItem.Enabled = 建立连接ToolStripMenuItem.Enabled = false;
            button3.Enabled = button4.Enabled = checkBox1.Enabled = false;
            //开关串口按钮
            button1.Enabled = true;
            button2.Enabled = false;
        }

        # endregion

        # region 公共操作
        int datainfo = 0;//统计已发送字节信息
        Disconnection disconnect = new Disconnection();
        private void Form1_Load(object sender, EventArgs e)        //获取串口端口号+时间显示
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
            ThreadStart ts = new ThreadStart(refresh);
            Thread t = new Thread(ts);
            t.Start();
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
        private void refresh()   //用于刷新时间+统计发送的字节数
        {
            while (Application.AllowQuit)
            {
                toolStripStatusLabel3.Text = "当前系统时间：" + DateTime.Now.ToString();//显示当前时间
                toolStripStatusLabel1.Text = "已发送字节数：" + datainfo.ToString();//显示已发送字节数
                Thread.Sleep(1000);//1000毫秒后再显示一次，实现刷新
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)   //改变发送形式时清除文本
        {
            textBox1.Clear();
        }
        private void button6_Click(object sender, EventArgs e)   //清空统计
        {
            datainfo = 0;
        }

        # endregion

        # region 初始化
        Initialization initial= new Initialization();
        bool detection_state, echo_state;
        private void 初始化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            if(detection_state)
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
            if(detection_state && echo_state)
            {
                textBox4.AppendText(">>初始化成功！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            else
            {
                textBox4.AppendText(">>初始化失败！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            //
            建立连接ToolStripMenuItem.Enabled = true;  //初始化完成，进入下一步
        }

        # endregion

        # region 建立连接
        SMS message = new SMS();
        Connection connect = new Connection();
        SendData send = new SendData();
        bool SendMessage_state, DeleteMessage_state, InitialConnection_state, ConnectionRequest_state, transparent_state;
        string index,msg;
        private void 建立连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items[1].SubItems[1].Text == "" || listView1.Items[2].SubItems[1].Text == "" || listView1.Items[3].SubItems[1].Text == "" || listView1.Items[4].SubItems[1].Text == "" || listView1.Items[5].SubItems[1].Text == "" || listView1.Items[6].SubItems[1].Text == "" || listView1.Items[7].SubItems[1].Text == "")//存在未配置的参数
            {
                textBox4.AppendText(">>无法建立连接,请确保参数配置完成！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            else
            {
                comboBox6.Enabled = comboBox7.Enabled = numericUpDown1.Enabled = numericUpDown2.Enabled = numericUpDown4.Enabled = textBox6.Enabled = false;//停止配置参数（允许更改设备ID以及获取ip地址）
                建立连接ToolStripMenuItem.Enabled = false;
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
                    SendMessage_state = message.SendShortMessage(serialPort1, listView1.Items[7].SubItems[1].Text, "start");
                }
                while (!SendMessage_state);
                if (SendMessage_state)
                {
                    textBox4.AppendText(">>发送短信成功！等待服务器响应……" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
                # endregion
                //
                this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);//添加串口接收数据事件，接收服务器端发来的短信
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            index = message.ArrivalIndication(serialPort1);//收短信提醒函数
            if (index != "-1")
            {
                DisplayStatus(0);
                msg = message.ReadShortMessage(serialPort1, index, listView1.Items[7].SubItems[1].Text);//读取短信函数
                if(msg != "")
                {
                    DisplayStatus(1);
                    this.serialPort1.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);//接收短信完成，移除串口接收数据事件
                    this.Invoke(new EventHandler(Connection));
                }
            }
        }
        private void Connection(object sender, EventArgs e)  //发送端主动建立连接
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
            # region 发起TCP或UDP连接函数
            string[] ipANDport;
            ipANDport = message.IpParse(msg);
            times = 0;
            do
            {
                if (times == 5)//连续5次失败（相比其他的指令该指令失败几率较大，故增加2次失败的可能提高成功率）
                {
                    textBox4.AppendText(">>连接服务器失败！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                    break;
                }
                times++;
                ConnectionRequest_state = connect.ConnectionRequest(serialPort1, listView1.Items[1].SubItems[1].Text, ipANDport[0], ipANDport[1], listView1.Items[6].SubItems[1].Text);
            }
            while (!ConnectionRequest_state);
            if (ConnectionRequest_state)
            {
                textBox4.AppendText(">>连接服务器成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }            
            # endregion
            //
            # region 设置传输模式函数
            if (listView1.Items[4].SubItems[1].Text == "是")
            {
                times = 0;
                do
                {
                    if (times == 3)//连续3次失败
                    {
                        textBox4.AppendText(">>进入透明传输失败！" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                        break;
                    }
                    times++;
                    transparent_state = connect.EnterTransparentTransmissionMode(serialPort1);
                }
                while (!transparent_state);
                if (transparent_state)
                {
                    textBox4.AppendText(">>进入透明传输成功！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
            }
            # endregion
            //
            # region 开启心跳包timer
            timerForHeartbeat.Interval = Convert.ToInt32(listView1.Items[2].SubItems[1].Text)*1000;//心跳间隔
            timerForHeartbeat.Start();
            textBox4.AppendText(">>心跳包开启！" + Environment.NewLine);
            textBox4.ScrollToCaret();
            # endregion
            //
            if(SendMessage_state && InitialConnection_state && ConnectionRequest_state)
            {
                if(listView1.Items[4].SubItems[1].Text == "是")
                {
                    if (transparent_state)
                    {
                        textBox4.AppendText(">>建立连接成功！<<" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                    }
                    else
                    {
                        textBox4.AppendText(">>建立连接失败！<<" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                    }
                }
                else
                {
                    textBox4.AppendText(">>建立连接成功！<<" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
            }
            else
            {
                textBox4.AppendText(">>建立连接失败！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            //启用发送数据和断开连接按钮
            button3.Enabled = button4.Enabled = checkBox1.Enabled = true;
            if (listView1.Items[4].SubItems[1].Text == "是")//非透明传输对传输16进制数据支持不好
                button4.Enabled = true;
            断开连接ToolStripMenuItem.Enabled = true;
        }
        private void timerForHeartbeat_Tick(object sender, EventArgs e)  //发送心跳包
        {
            send.SendHeartbeatPacket(serialPort1, listView1.Items[4].SubItems[1].Text);
        }
        public void DisplayStatus(int i)   //显示状态
        {
            if(i==0)
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

        # region 发送数据
        public byte[] string2hex(string text)//16进制发送时将字符串转换成真正的字节数组
        {
            string[] temp;
            char[] separator = { ' ' };
            temp = text.Split(separator,StringSplitOptions.RemoveEmptyEntries);//分割每个字符
            int length = temp.Length;
            byte[] result = new byte[length];
            try
            {
                for (int i = 0; i < length; i++)
                {
                    result[i] = Convert.ToByte(temp[i], 16);
                }
            }
            catch(Exception)
            {
                MessageBox.Show("输入数据不能被正确解析！"+Environment.NewLine+"请确保输入的是16进制数据并用空格分隔", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result=new byte[0];
            }
            return result;
        }
        private void button3_Click(object sender, EventArgs e)  //发送文本框数据
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("输入不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (checkBox1.CheckState == CheckState.Unchecked)
                {
                    //
                    # region 发送数据函数
                    byte[] OriginalData;
                    if(radioButton1.Checked)
                    {
                        OriginalData = string2hex(textBox1.Text);
                    }
                    else
                    {
                        OriginalData = Encoding.UTF8.GetBytes(textBox1.Text);
                    }
                    datainfo += OriginalData.Length;//已发送字节
                    byte[] DataPacket = send.MakeDataPacket(OriginalData, Convert.ToInt32(listView1.Items[5].SubItems[1].Text), Convert.ToInt32(listView1.Items[3].SubItems[1].Text));
                    send.SendText(serialPort1, DataPacket, listView1.Items[4].SubItems[1].Text);
                    if(OriginalData.Length==0)
                    {
                        textBox4.AppendText(">>发送数据失败！" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                    }
                    else
                    {
                        textBox4.AppendText(">>发送数据成功！" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                    }                   
                    # endregion
                    //                    
                }
                else
                {
                    textBox1.ReadOnly = true;//停止变更数据防止发送出错
                    radioButton1.Enabled = radioButton2.Enabled = false;
                    timerForSend.Start();
                }
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)  //定时发送文本框数据时间设置
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                timerForSend.Interval = Convert.ToInt32(textBox2.Text);
                textBox2.ReadOnly = true;
            }
            else
            {
                textBox2.ReadOnly = false;
                timerForSend.Stop();
                textBox1.ReadOnly = false;//允许变更数据
                radioButton1.Enabled = radioButton2.Enabled = true;
            }
        }
        private void timerForSend_Tick(object sender, EventArgs e)  //定时发送文本框数据
        {
            //
            # region 发送数据函数
            byte[] OriginalData;
            if (radioButton1.Checked)
            {
                OriginalData = string2hex(textBox1.Text);
            }
            else
            {
                OriginalData = Encoding.UTF8.GetBytes(textBox1.Text);
            }
            datainfo += OriginalData.Length;
            byte[] DataPacket = send.MakeDataPacket(OriginalData, Convert.ToInt32(listView1.Items[5].SubItems[1].Text), Convert.ToInt32(listView1.Items[3].SubItems[1].Text));
            send.SendText(serialPort1, DataPacket, listView1.Items[4].SubItems[1].Text);
            if (OriginalData.Length == 0)
            {
                textBox4.AppendText(">>发送数据失败！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            else
            {
                textBox4.AppendText(">>发送数据成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            # endregion
            //
        }
        private void button4_Click(object sender, EventArgs e)  //发送txt或dat文件中的数据
        {
            openFileDialog1.Filter = "文本文件(*.txt)|*.txt|二进制文件(*.dat)|*.dat";
            DialogResult dt = openFileDialog1.ShowDialog();
            if (dt == DialogResult.OK)
            {
                dt = MessageBox.Show("信息获取完毕，是否确认发送？", "系统消息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dt == DialogResult.Yes)
                {
                    //
                    # region 发送数据函数
                    int length;
                    send.SendFromFile(serialPort1, openFileDialog1.FileName, listView1.Items[4].SubItems[1].Text, Convert.ToInt32(listView1.Items[5].SubItems[1].Text), Convert.ToInt32(listView1.Items[3].SubItems[1].Text),out length);
                    datainfo += length;
                    textBox4.AppendText(">>发送数据成功！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                    # endregion
                    //
                }
            }
        }

        # endregion

        # region 断开连接
        bool disconnect_state, QuitTransparent_state;
        private void 断开连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3.Enabled = button4.Enabled = checkBox1.Enabled = false;
            断开连接ToolStripMenuItem.Enabled = false;
            //
            # region 停止心跳包timer
            timerForHeartbeat.Stop();
            textBox4.AppendText(">>心跳包停止！" + Environment.NewLine);
            textBox4.ScrollToCaret();
            # endregion
            //
            # region 发送注销包函数
            send.SendLogoutPacket(serialPort1, listView1.Items[4].SubItems[1].Text);
            textBox4.AppendText(">>正在与服务器断开连接……" + Environment.NewLine);
            textBox4.ScrollToCaret();
            # endregion
            //
            # region 退出透明传输（如果是的话）
            Thread.Sleep(1000);//正确识别退出透明传输指令的关键
            if (listView1.Items[4].SubItems[1].Text == "是")
            {
                times = 0;
                do
                {
                    if (times == 3)//连续3次失败
                    {
                        textBox4.AppendText(">>退出透明传输失败！" + Environment.NewLine);
                        textBox4.ScrollToCaret();
                        break;
                    }
                    times++;
                    QuitTransparent_state = connect.QuitTransparentTransmissionMode(serialPort1);
                }
                while (!QuitTransparent_state);
                if (QuitTransparent_state)
                {
                    textBox4.AppendText(">>退出透明传输成功！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                }
            }
            # endregion
            //
            # region 断开ip连接函数
            times = 0;
            do
            {
                if (times == 3)//连续3次失败
                {
                    textBox4.AppendText(">>断开ip连接失败！" + Environment.NewLine);
                    textBox4.ScrollToCaret();
                    break;
                }
                times++;
                disconnect_state = disconnect.DisconnectAll(serialPort1);
            }
            while (!disconnect_state);
            if (disconnect_state)
            {
                textBox4.AppendText(">>断开ip连接成功！" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            # endregion
            //
            if(disconnect_state)  //断开ip连接成功则退出透明传输必定成功（如果存在的话）
            {
                textBox4.AppendText(">>断开连接成功！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            else
            {
                textBox4.AppendText(">>断开连接失败！<<" + Environment.NewLine);
                textBox4.ScrollToCaret();
            }
            //
            listView1.Items[0].SubItems[1].Text = "";//清除本次ip地址
            comboBox6.Enabled = comboBox7.Enabled = numericUpDown1.Enabled = numericUpDown2.Enabled = numericUpDown4.Enabled = textBox6.Enabled = true;//启用参数配置
            建立连接ToolStripMenuItem.Enabled = true;//允许进行下一次的建立连接
        }

        # endregion
 
    }
}