using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataCheck
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool check = true;
        private void button1_Click(object sender, EventArgs e)
        {
             DialogResult dt = openFileDialog1.ShowDialog();
             if (dt == DialogResult.OK)
             {
                 textBox1.Text = openFileDialog1.FileName;
             }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dt = openFileDialog1.ShowDialog();
            if (dt == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = button2.Enabled = button3.Enabled = false;
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("无法完成数据校验，请指定需要校验的文件！", "错误", MessageBoxButtons.OK);
            }
            else
            {
                backgroundWorker1.RunWorkerAsync();
                button4.Enabled = true;
            }
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo finfo1 = new FileInfo(textBox1.Text);
            FileInfo finfo2 = new FileInfo(textBox2.Text);
            if (finfo1.Extension.ToLower() != finfo2.Extension.ToLower())
            {
                MessageBox.Show("无法完成数据校验，请确保文件类型一致！", "错误", MessageBoxButtons.OK);
            }
            else
                if(finfo1.Extension.ToLower()==".dat")//读取二进制文件
                {
                    int readsize = 0;
                    BinaryReader reader1 = new BinaryReader(finfo1.Open(FileMode.Open, FileAccess.Read));
                    BinaryReader reader2 = new BinaryReader(finfo2.Open(FileMode.Open, FileAccess.Read));
                    byte[] buffer1 = new byte[1024];
                    byte[] buffer2 = new byte[1024];
                    if(finfo1.Length != finfo2.Length)
                    {
                        MessageBox.Show("文件大小不一样，校验失败！", "", MessageBoxButtons.OK);
                    }
                    else
                    {
                        int length = (int)finfo1.Length;
                        while (reader2.Read(buffer2, 0, 1024) > 0)
                        {
                            readsize += reader1.Read(buffer1, 0, 1024);
                            if (backgroundWorker1.CancellationPending)
                            {
                                e.Cancel = true;
                                break;
                            }
                            for(int i=0;i<1024;i++)
                            {
                                if (buffer1[i] != buffer2[i])
                                {
                                    check = false;
                                    break;
                                }   
                            }
                            backgroundWorker1.ReportProgress((int)((float)readsize / (float)length * 100));
                        }
                    }
                    reader1.Close(); reader2.Close();
                }
                else
                {
                    int readsize = 0;
                    StreamReader sr1 = new StreamReader(finfo1.Open(FileMode.Open, FileAccess.Read), Encoding.Default);
                    StreamReader sr2 = new StreamReader(finfo2.Open(FileMode.Open, FileAccess.Read), Encoding.Default);
                    char[] buffer1 = new char[1024];
                    char[] buffer2 = new char[1024];
                    if (finfo1.Length != finfo2.Length)
                    {
                        MessageBox.Show("文件大小不一样，校验失败！", "", MessageBoxButtons.OK);
                        check = false;
                    }
                    else
                    {
                        int length = (int)finfo1.Length;
                        while (sr2.Read(buffer2, 0, 1024) > 0)
                        {
                            readsize += sr1.Read(buffer1, 0, 1024);
                            if (backgroundWorker1.CancellationPending)
                            {
                                e.Cancel = true;
                                break;
                            }
                            for (int i = 0; i < 1024; i++)
                            {
                                if (buffer1[i] != buffer2[i])
                                {
                                    check = false;
                                    break;
                                }
                            }
                            //System.Threading.Thread.Sleep(1000);
                            backgroundWorker1.ReportProgress((int)((float)readsize / (float)length * 100));
                        }
                    }
                    sr1.Close(); sr2.Close();
                }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if(e.Cancelled)
            {
                MessageBox.Show("校验被取消");
            }
            else if(check)
            {
                MessageBox.Show("文件内容相同，校验成功！", "", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("文件内容不同，校验失败！", "", MessageBoxButtons.OK);
            }

            button1.Enabled = button2.Enabled = button3.Enabled = true;
            button4.Enabled = false;
            check = true; progressBar1.Value = 0;//恢复原始状态
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            button4.Enabled = false;
        }
    }
}
