using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Collections;

namespace WirelessCommunication
{
    
    # region 初始化类ok
    class Initialization
    {
        public bool AtDetection(SerialPort serialPort1)                 //AT检测
        {
            int FrontPosition, BackPosition;
            string Flag, Result;
            serialPort1.Write("AT\r");
            do
            {
                Thread.Sleep(50);     //延时50ms，确保在正常情况下可以一次收到所需数据，下同（详见指令响应时间测试）
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "");    //无反馈信息则等待
            FrontPosition = Flag.IndexOf("\r\n");     //获取需要的信息的位置
            BackPosition = Flag.LastIndexOf("\r\n");
            Result = Flag.Substring(FrontPosition + 2, BackPosition - FrontPosition - 2);  //得到有用的信息
            return Result == "OK" ? true : false;
        }
        public bool StopEcho(SerialPort serialPort1)                    //关闭回显
        {
            int FrontPosition, BackPosition;
            string Flag, Result;
            serialPort1.Write("ATE0\r");
            do
            {
                Thread.Sleep(50);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "");
            FrontPosition = Flag.IndexOf("\r\n");
            BackPosition = Flag.LastIndexOf("\r\n");
            Result = Flag.Substring(FrontPosition + 2, BackPosition - FrontPosition - 2);
            return Result == "OK" ? true : false;
        }
        public bool InitialConnection(SerialPort serialPort1)           //初始化链接，申请ip地址
        {
            int FrontPosition, BackPosition;
            string Flag, Result;
            serialPort1.Write("AT^IPINIT\r");
            do
            {
                Thread.Sleep(3000);     //因ip申请时间具有不确定性，不一定能够一次得到结果
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "");
            FrontPosition = Flag.IndexOf("\r\n");
            BackPosition = Flag.LastIndexOf("\r\n");
            Result = Flag.Substring(FrontPosition + 2, BackPosition - FrontPosition - 2);
            return Result == "OK" ? true : false;
        }
        public string GetIpAddress(SerialPort serialPort1)              //获取本机ip地址
        {
            int FrontPosition, BackPosition;
            string Flag, Result;
            serialPort1.Write("AT^IPINIT?\r");
            do
            {
                Thread.Sleep(60);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");  //申请ip后需要排除的“休眠”指令，下同
            FrontPosition = Flag.IndexOf(",");
            BackPosition = Flag.IndexOf(",", FrontPosition + 1);
            return Result = Flag.Substring(FrontPosition + 1, BackPosition - FrontPosition - 1);
        }
    }
    # endregion

    # region 建立连接类ok
    class Connection
    {
        public bool IpListening(SerialPort serialPort1, string ConnectionType, string ListenPort)       //设置接收端的ip监听，tcp/udp可选
        {
            int Position;
            string Flag;
            if(ConnectionType == "TCP")
            {
                serialPort1.Write("AT^IPLISTEN=\"TCP\"," + ListenPort + "\r");
            }
            else
            {
                serialPort1.Write("AT^IPLISTEN=\"UDP\"," + ListenPort + "\r");
            }
            do
            {
                Thread.Sleep(60);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");
            Position = Flag.IndexOf("OK");
            if(Position!=-1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ConnectionRequest(SerialPort serialPort1, string ConnectionType, string dest_ip, string dest_port, string local_port)      //发起链接请求,tcp/udp可选
        {
            int Position;
            string Flag;
            if (ConnectionType == "TCP")
            {
                serialPort1.Write("AT^IPOPEN=1,\"TCP\",\"" + dest_ip + "\"," + dest_port + "," + local_port + "\r");//只需建立第一个链接
                do
                {
                    Thread.Sleep(1000);      //因建立连接时间具有不确定性，不能一次得到结果
                    Flag = serialPort1.ReadExisting();
                }
                while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");
                Position = Flag.IndexOf("OK");
                if (Position != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                serialPort1.Write("AT^IPOPEN=1,\"UDP\",\"" + dest_ip + "\"," + dest_port + "," + local_port + "\r");
                do
                {
                    Thread.Sleep(100);      //延时100ms，确保在正常情况下可以一次收到所需数据
                    Flag = serialPort1.ReadExisting();
                }
                while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");
                Position = Flag.IndexOf("OK");
                if (Position != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool EnterTransparentTransmissionMode(SerialPort serialPort1)            //进入透明传输模式
        {
            int Position;
            string Flag;
            serialPort1.Write("AT^IPENTRANS=1\r");
            do
            {
                Thread.Sleep(50);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");
            Position = Flag.IndexOf("OK");
            if (Position != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool QuitTransparentTransmissionMode(SerialPort serialPort1)             //退出透明传输模式
        {
            int Position;
            string Flag;
            serialPort1.Write("+++");
            do
            {
                Thread.Sleep(1500);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");
            Position = Flag.IndexOf("OK");
            if (Position != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    # endregion

    # region 短信模块ok
    class SMS
    {
        public string ArrivalIndication(SerialPort serialPort1)    //当短信到达时，获取其存储的位置
        {
            int Position;
            string Flag, index;
            do
            {
                Thread.Sleep(20);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "");
            Position = Flag.IndexOf("+CMTI");
            if(Position == -1)  //排除别的指令的干扰
            {
                return "-1";
            }
            index = Flag[Position + 11].ToString();
            return index;
        }
        public string ReadShortMessage(SerialPort serialPort1, string index, string PhoneNum)   //读取短信内容
        {
            int IdFrontPosition, IdBackPosition, MsgFrontPosition, MsgBackPosition;
            string Flag, CallerID, Msg;
            serialPort1.Write("AT^HCMGR=" + index + "\r");
            do
            {
                Thread.Sleep(40);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "");
            //解析电话号码
            IdFrontPosition = Flag.IndexOf("^HCMGR");
            IdBackPosition=Flag.IndexOf(",",IdFrontPosition);
            CallerID= Flag.Substring(IdFrontPosition+7,IdBackPosition-IdFrontPosition-7);
            if (CallerID != PhoneNum)  //排除垃圾短信干扰
            {
                return "";
            }
            //解析短信内容
            MsgFrontPosition = Flag.IndexOf("\r\n", IdFrontPosition);
            MsgBackPosition = Flag.IndexOf((char)0x1A);
            Msg = Flag.Substring(MsgFrontPosition+2,MsgBackPosition-MsgFrontPosition-2);
            return Msg;
        }
        public bool DeleteShortMessage(SerialPort serialPort1)     //删除收到的全部短信短信
        {
            int Position;
            string Flag;
            serialPort1.Write("AT+CMGD=0,4\r");
            do
            {
                Thread.Sleep(50);   //正常情况可以一次得到结果，如果删除的条数过多可能一次收不到结果
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "");
            Position = Flag.IndexOf("OK");
            if (Position != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SendShortMessage(SerialPort serialPort1, string PhoneNum, string msg)    //发送短信，内容自行制定
        {
            string Flag;
            serialPort1.Write("AT^HCMGS=\"" + PhoneNum + "\"\r");
            do
            {
                Thread.Sleep(40);
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");  //因为发送短信在申请ip后，所以需要考虑排除休眠指令
            serialPort1.Write(msg + (char)0x1A);
            return SendingReport(serialPort1);
        }
        protected bool SendingReport(SerialPort serialPort1)    //发送短信的状态监测
        {
            int Position;
            string Flag;
            do
            {
                Thread.Sleep(500);   //因为短信发送时间不确定，不能一次得到结果
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");   //因为发送短信在申请ip后，所以需要考虑排除休眠指令；同时，需要排除在短信发送后立即返回的"\r\n"
            Position = Flag.IndexOf("^HCMGSS");
            if(Position != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string[] IpParse(string msg)   //解析短信内容
        {
            int Position;
            string[] ipANDport = new string[2];
            Position = msg.IndexOf(':');
            ipANDport[0] = msg.Substring(0, Position);//ip地址
            ipANDport[1] = msg.Substring(Position + 1);//端口
            return ipANDport;
        }
    }
    # endregion

    # region 发送数据类ok
    class SendData
    {
        public byte[] MakeDataPacket(byte[] text, int id, int DataPacketlength)  //为数据添加帧头桢尾以及标志位，可以指定帧的长度
        {
            ArrayList Result = new ArrayList(10 * 1024);
            byte[] FrameHeader = { 0xaa, 0x55 }, FrameFooter = { 0xff, 0xff };   //定义帧头桢尾
            int length = text.Length, index = 0;
            for (int i = 0; i < length / DataPacketlength; i++)  //i为可以分割的次数
            {
                Result.AddRange(FrameHeader);
                Result.Add((byte)(id + 0x30));   //添加id标志位区分数据来源
                for (int j = index; j < index + DataPacketlength; j++)
                    Result.Add(text[j]);
                Result.AddRange(FrameFooter);
                index += DataPacketlength;
            }
            //处理剩余部分
            if (length % DataPacketlength > 0)
            {
                Result.AddRange(FrameHeader);
                Result.Add((byte)(id + 0x30));
                for (int j = index; j < length; j++)
                    Result.Add(text[j]);
                Result.AddRange(FrameFooter);
            }
            return (byte[])Result.ToArray(typeof(byte));
        }
        public void SendText(SerialPort serialPort1, byte[] text, string TransmissionMode)    //从文本框发送数据（应考虑16进制和ascii码，未完待续）
        {
            int length = text.Length;
            if (TransmissionMode == "是")   //透传时注意限制一次发送的大小，最大不能超过WriteBuffer的大小，现在设为2048
            {
                if (length > 2048)
                {
                    int index = 0;
                    for (int i = length / 2048; i > 0; i--)
                    {
                        serialPort1.Write(text, index, 2048);
                        index += 2048;
                    }
                    serialPort1.Write(text, index, length - index);
                }
                else
                {
                    serialPort1.Write(text, 0, length);
                }
            }
            else       //非透明传输时同样需要注意一次发送的大小，最大不能超过AT指令所限制的1500个字节，现在设为1400
            {
                if (length > 1400)
                {
                    int index = 0;
                    for (int i = length / 1400; i > 0; i--)
                    {
                        do
                        {
                            serialPort1.Write("AT^IPSEND=1,\"");
                            serialPort1.Write(text, index, 1400);
                            serialPort1.Write("\"\r");
                        }
                        while (!SendVerify(serialPort1));
                        index += 1400;
                    }
                    do
                    {
                        serialPort1.Write("AT^IPSEND=1,\"");
                        serialPort1.Write(text, index, length - index);
                        serialPort1.Write("\"\r");
                    }
                    while (!SendVerify(serialPort1));
                }
                else
                {
                    do
                    {
                        serialPort1.Write("AT^IPSEND=1,\"");
                        serialPort1.Write(text, 0, length);
                        serialPort1.Write("\"\r");
                    }
                    while (!SendVerify(serialPort1));
                }
            }
        }
        protected bool SendVerify(SerialPort serialPort1)     //验证ipsend指令是否发送成功
        {
            string Flag;
            int Position;
            do
            {
                Thread.Sleep(30);    //除第一次发送数据需要循环多次外，以后只需执行一次（效率也许比透明传输低，原因见指令响应时间测试）
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:1\r\n\r\n");
            Position = Flag.IndexOf("OK");
            if (Position != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void SendFromFile(SerialPort serialPort1, string path, string TransmissionMode, int id, int DataPacketlength,out int datainfo)    //从文本文件发送信息
        {
            FileInfo finfo = new FileInfo(path);
            datainfo = 0;
            if(finfo.Extension.ToLower()==".dat")//读取二进制文件
            {
                BinaryReader reader = new BinaryReader(finfo.Open(FileMode.Open, FileAccess.Read));
                byte[] buffer = new byte[1024];
                int Readsize;
                while ((Readsize = reader.Read(buffer, 0, 1024)) > 0)
                {
                    datainfo += Readsize;
                    byte[] BufferedData = buffer.Take(Readsize).ToArray();//返回真正读取到的有效的字节数组
                    BufferedData = MakeDataPacket(BufferedData, id, DataPacketlength);  //添加帧头桢尾
                    SendText(serialPort1, BufferedData, TransmissionMode);
                }
                reader.Close();
            }
            else//读取文本文件
            {
                char[] buffer = new char[1024];
                int Readsize;
                byte[] temp;
                StreamReader sr = new StreamReader(finfo.Open(FileMode.Open, FileAccess.Read), Encoding.Default);
                while ((Readsize = sr.Read(buffer, 0, 1024)) > 0)
                {
                    temp = Encoding.UTF8.GetBytes(buffer, 0, Readsize);   //将读取到的字符数组转换成字节数组发送
                    datainfo += temp.Length;
                    temp = MakeDataPacket(temp, id, DataPacketlength);    //添加帧头桢尾
                    SendText(serialPort1, temp, TransmissionMode);
                }
                sr.Close();
            }
        }
        public void SendHeartbeatPacket(SerialPort serialPort1, string TransmissionMode)   //发送心跳包
        {
            byte[] haertbeat = { 0xaa, 0x55, 0x30, 0x30, 0xff, 0xff };//格式：帧头+0（id头）+0（代表心跳）+桢尾（内容为空）
            SendText(serialPort1, haertbeat, TransmissionMode);
        }
        public void SendLogoutPacket(SerialPort serialPort1, string TransmissionMode)   //发送注销包
        {
            byte[] Logout = { 0xaa, 0x55, 0x30, 0x31, 0xff, 0xff };//格式：帧头+0（id头）+1（代表注销）+桢尾（内容为空）
            SendText(serialPort1, Logout, TransmissionMode);
        }
    }
    # endregion

    # region 接收数据类ok
    class ReceiveData
    {
        protected byte[][] split(byte[] data, byte[] Separator)     //分割字节数组
        {
            byte[][] result;
            //确定数组元素的个数
            int FrontPosition = 0, BackPosition = 0, i = 0;
            while ((BackPosition = GetIndexOf(data, Separator, FrontPosition)) != -1)
            {
                FrontPosition = BackPosition + 3;
                i++;
            }
            result = new byte[i + 1][];
            //对每个数组元素赋值
            FrontPosition = 0; BackPosition = 0; i = 0;
            while ((BackPosition = GetIndexOf(data, Separator, FrontPosition)) != -1)
            {
                result[i] = new byte[BackPosition - FrontPosition];
                Array.Copy(data, FrontPosition, result[i], 0, BackPosition - FrontPosition);
                i++; FrontPosition = BackPosition + 3;
            }
            result[i] = new byte[data.Length - FrontPosition];
            Array.Copy(data, FrontPosition, result[i], 0, data.Length - FrontPosition);
            //
            return result;
        }
        protected int GetIndexOf(byte[] b, byte[] bb, int startindex)   //查找所需的字节数组在一个字节数组中的位置
        {
            if (b == null || bb == null || b.Length == 0 || bb.Length == 0 || b.Length < bb.Length)
                return -1;

            int i, j;
            for (i = startindex; i < b.Length - bb.Length + 1; i++)
            {
                if (b[i] == bb[0])
                {
                    for (j = 1; j < bb.Length; j++)
                    {
                        if (b[i + j] != bb[j])
                            break;
                    }
                    if (j == bb.Length)
                        return i;
                }
            }
            return -1;
        }
        public byte[][] ReceiveFromSerialPort(SerialPort serialPort1, ref byte[] temp)   //接收数据（无论是否透明传输，接收到的数据格式都是一样的）
        {
            byte[] buffer;   
            byte[] Separator = { 0x0d,0x0a,0x0d };   //数据分割判断标准
            byte[][] Result;
            //读取数据
            Thread.Sleep(50);    //等待50ms确保读取的数据完整
            int readsize = serialPort1.BytesToRead;
            buffer = new byte[readsize];
            serialPort1.Read(buffer, 0, readsize);
            byte[] data = new byte[buffer.Length + temp.Length];
            temp.CopyTo(data, 0);   //将上一次的未处理部分赋给Flag
            buffer.CopyTo(data, temp.Length);
            //分割数据
            Result = split(data, Separator);    //接受分割后的最后一组数据为空或者不全
            temp = Result[Result.Length - 1];//将最后一个不完整或者为空的数据传递给temp供下次使用
            //返回结果
            byte[][] FinalResult = new byte[Result.Length - 1][];
            Array.Copy(Result, FinalResult, Result.Length - 1);
            return FinalResult;   //除去最后一组不可用外，返回所有立即可用的数据为最终结果，供下一步处理
        }
        public byte[] Parse(byte[][] SourceData)  //对返回的接收数据指令进行解析，获取有用的部分
        {
            int FrontPosition, BackPosition, Position;
            ArrayList DestinationData = new ArrayList(1024*5);  //初始大小设为5k，基本可以满足容纳一次所得的数据
            byte[] find = Encoding.ASCII.GetBytes("^IPDATA:");
            foreach (byte[] data in SourceData)
            {
                Position = GetIndexOf(data, find, 0);
                if (Position != -1)   //排除不是接收数据的指令（其他任何的指令，例如DSDORMANT）
                {
                    FrontPosition = Array.IndexOf(data, (byte)0x2c, Position + 8 + 2);
                    BackPosition = Array.LastIndexOf(data, (byte)0x0d);
                    for (int i = 0; i < BackPosition - FrontPosition - 1; i++)
                        DestinationData.Add(data[FrontPosition + 1 + i]);
                }
            }
            return (byte[])DestinationData.ToArray(typeof(byte));   //返回的全部是带帧头桢尾的数据，除了最后一组可能不全以外
        }
        public byte[] ConvertToOriginalData(byte[] RealData, ref ArrayList[] reg, ref bool close)  //用于处理所有可以立即处理的数据，即：数据完整，包含帧头桢尾的部分
        {
            int startindex = 0, rear;
            byte[] find = { 0xff, 0xff };
            while ((rear = GetIndexOf(RealData, find, startindex)) != -1)
            {
                int id = (int)RealData[startindex + 2] - 0x30;  //确定数据id
                if (id == 0)  //心跳包不做处理
                {
                    if (RealData[startindex + 3] == (byte)0x31)   //处理注销包
                    {
                        close = true;
                    }
                }
                else
                {
                    for (int i = startindex+3; i < rear; i++)
                        reg[id - 1].Add(RealData[i]);  //存入对应的存储区域中
                }
                startindex = rear + 2;
            }
            byte[] IncompleteData = new byte[RealData.Length - startindex];
            Array.Copy(RealData, startindex, IncompleteData, 0, RealData.Length - startindex);
            return IncompleteData;
        }
    }
    # endregion

    # region 断开连接类ok
    class Disconnection
    {
        public bool DisconnectOneLink(SerialPort serialPort1, string TransmissionMode, string id)   //发送端主动关闭连接——暂不使用，未测
        {
            int FrontPosition, BackPosition;
            string Flag, Result;
            //判断传输模式
            if (TransmissionMode=="是")
            {
                Connection connect = new Connection();
                while (!connect.QuitTransparentTransmissionMode(serialPort1)) ;
            }
            //断开连接
            serialPort1.Write("AT^IPCLOSE=" + id + "\r");
            do
            {
                Flag = serialPort1.ReadExisting();
                Thread.Sleep(20);
            }
            while (Flag == "");
            FrontPosition = Flag.IndexOf("\r\n");
            BackPosition = Flag.LastIndexOf("\r\n");
            Result = Flag.Substring(FrontPosition + 2, BackPosition - FrontPosition - 2);
            return Result == "OK" ? true : false;
        }
        public bool DisconnectServerListening(SerialPort serialPort1)   //接收端断开ip监听——暂不使用，未测
        {
            int Position;
            string Flag;
            serialPort1.Write("AT^IPCLOSE=6\r");
            do
            {
                Flag = serialPort1.ReadExisting();
                Thread.Sleep(20);
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n");  //断开监听时只可能有这一种情况需要排除
            Position = Flag.IndexOf("OK");
            if (Position != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DisconnectAll(SerialPort serialPort1)  //断开所有链接
        {
            int Position;
            string Flag;
            serialPort1.Write("AT^IPCLOSE=7\r");
            do
            {
                Thread.Sleep(1000);  //客户端执行时不能一次得到结果，会执行多次
                Flag = serialPort1.ReadExisting();
            }
            while (Flag == "" || Flag == "\r\n^DSDORMANT:0\r\n\r\n" || Flag == "\r\n^DSDORMANT:0\r\n\r\n");
            Position = Flag.IndexOf("OK");
            if (Position != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Reset(SerialPort serialPort1)  //重启设备，关闭软件前使用（无需验证成功与否）
        {
            serialPort1.Write("AT^RESET\r");
        }
    }
    # endregion

}