using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Utility.ToolboxProgram;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Mail;

namespace ToolboxProgram
{
    public partial class TrayMinimizerForm : Form
    {
        bool firstTime = true;
        ModifyRegistry reg = new ModifyRegistry();
        bool enableReg = false;

        public TrayMinimizerForm()
        {
            InitializeComponent();
            try
            {
                checkBox1.Checked = bool.Parse(reg.Read("autoreboot_on"));
            }
            catch { }
            try
            {
                checkBox2.Checked = bool.Parse(reg.Read("checkprogramvitality_on"));
            }
            catch { }
            try
            {
                 dateTimePicker1.Value = DateTime.Parse(reg.Read("autoreboot_time"));
            }
            catch { }
            try
            {
                textBox1.Text = reg.Read("checkprogramvitality_programpath");
            }
            catch { }
            try
            {
                String ip = reg.Read("keepconnctionwith_ip1");
                if (!String.IsNullOrEmpty(ip))
                    textBox2.Text = ip;
            }
            catch { }
            try
            {
                String ip = reg.Read("keepconnctionwith_ip2");
                if (!String.IsNullOrEmpty(ip))
                    textBox3.Text = ip;
            }
            catch { }
            try
            {
                String ip = reg.Read("keepconnctionwith_ip3");
                if (!String.IsNullOrEmpty(ip))
                    textBox4.Text = ip;
            }
            catch { }
            try
            {
                String ip = reg.Read("keepconnctionwith_ip4");
                if (!String.IsNullOrEmpty(ip))
                    textBox5.Text = ip;
            }
            catch { }
            try
            {
                String ip = reg.Read("keepconnctionwith_ip5");
                if (!String.IsNullOrEmpty(ip))
                    textBox6.Text = ip;
            }
            catch { }
            try
            {
                String ip = reg.Read("keepconnctionwith_ip6");
                if (!String.IsNullOrEmpty(ip))
                    textBox7.Text = ip;
            }
            catch { }
            try
            {
                checkBox3.Checked = bool.Parse(reg.Read("keepconnctionwith_on"));
            }
            catch { }
            enableReg = true;
        }

        private void TrayMinimizerForm_Resize(object sender, EventArgs e)
        {

            notifyIcon1.BalloonTipTitle = "Toolbox program";
            notifyIcon1.BalloonTipText = "The program will be hide";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();    
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("autoreboot_on", checkBox1.Checked);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (enableReg) 
                reg.Write("checkprogramvitality_on", checkBox2.Checked);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (enableReg) 
                reg.Write("autoreboot_time", dateTimePicker1.Value.ToLongTimeString());
            checkBox1.Checked = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("checkprogramvitality_programpath", textBox1.Text);
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            checkBox2.Checked = true;
        }

        private void TrayMinimizerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            
            notifyIcon1.BalloonTipTitle = "Toolbox program";
            notifyIcon1.BalloonTipText = "The program will be hide";

            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(500);
            this.Hide();    
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (firstTime)
            {
                notifyIcon1.BalloonTipTitle = "Toolbox program";
                notifyIcon1.BalloonTipText = "The program will be hide";

                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
                firstTime = false;
            }
            if (checkBox1.Checked)
            {
                try
                {
                    if (Math.Abs(DateTime.Now.TimeOfDay.TotalSeconds - dateTimePicker1.Value.TimeOfDay.TotalSeconds) < 2)
                    {
                        ProcessStartInfo proc = new ProcessStartInfo();
                        proc.FileName = "cmd";
                        proc.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.Arguments = "/C shutdown -f -r -t 5";
                        Process.Start(proc);
                    }
                }
                catch
                {
                    checkBox1.Checked = false;
                }
            }
            if (checkBox2.Checked)
            {
                try
                {
                    FileInfo fi = new FileInfo(textBox1.Text);
                    if (fi.Exists && (fi.Extension.ToLower() == ".exe"))
                    {
                        System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(fi.Name.ToLower().Replace(".exe",""));
                        if (procs.Length > 0)
                        {
                            //MessageBox.Show(String.Format("{0}is  running!", sProcessName), sProcessName);
                        }
                        else
                        {
                            //MessageBox.Show(String.Format("{0}is not running!", sProcessName), sProcessName);
                            // start your process
                            ProcessStartInfo proc = new ProcessStartInfo();
                            proc.FileName = fi.FullName;
                            Process.Start(proc);
                        }
                    }
                }
                catch
                {
                    checkBox2.Checked = false;
                }
            }
            timer1.Enabled = true;
        }

        private void TrayMinimizerForm_Load(object sender, EventArgs e)
        {
            timer2_Tick(null, null);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("keepconnctionwith_on", checkBox3.Checked);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("keepconnctionwith_ip1", ((TextBox)sender).Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("keepconnctionwith_ip2", ((TextBox)sender).Text);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("keepconnctionwith_ip4", ((TextBox)sender).Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("keepconnctionwith_ip3", ((TextBox)sender).Text);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("keepconnctionwith_ip6", ((TextBox)sender).Text);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (enableReg)
                reg.Write("keepconnctionwith_ip5", ((TextBox)sender).Text);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                string strOutput = "";
                try
                {
                    if (!String.IsNullOrEmpty(textBox2.Text))
                    {
                        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                        pProcess.StartInfo.FileName = "ping";
                        pProcess.StartInfo.Arguments = textBox2.Text + " -n 1";
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.RedirectStandardOutput = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.Start();
                        strOutput = pProcess.StandardOutput.ReadToEnd();
                        //pProcess.WaitForExit();
                    }
                }
                catch
                {
                }
                textBox2.BackColor = (strOutput.Contains("0% persi") || (strOutput.Contains("0% loss"))) ? Color.LightGreen : Color.OrangeRed;
                strOutput = "";
                try
                {
                    if (!String.IsNullOrEmpty(textBox3.Text))
                    {
                        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                        pProcess.StartInfo.FileName = "ping";
                        pProcess.StartInfo.Arguments = textBox3.Text + " -n 1";
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.RedirectStandardOutput = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.Start();
                        strOutput = pProcess.StandardOutput.ReadToEnd();
                        //pProcess.WaitForExit();
                    }
                }
                catch
                {
                }
                textBox3.BackColor = (strOutput.Contains("0% persi") || (strOutput.Contains("0% loss"))) ? Color.LightGreen : Color.OrangeRed;

                strOutput = "";
                try
                {
                    if (!String.IsNullOrEmpty(textBox4.Text))
                    {
                        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                        pProcess.StartInfo.FileName = "ping";
                        pProcess.StartInfo.Arguments = textBox4.Text + " -n 1";
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.RedirectStandardOutput = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.Start();
                        strOutput = pProcess.StandardOutput.ReadToEnd();
                        //pProcess.WaitForExit();
                    }
                }
                catch
                {
                }
                textBox4.BackColor = (strOutput.Contains("0% persi") || (strOutput.Contains("0% loss"))) ? Color.LightGreen : Color.OrangeRed;
                strOutput = "";
                try
                {
                    if (!String.IsNullOrEmpty(textBox5.Text))
                    {
                        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                        pProcess.StartInfo.FileName = "ping";
                        pProcess.StartInfo.Arguments = textBox5.Text + " -n 1";
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.RedirectStandardOutput = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.Start();
                        strOutput = pProcess.StandardOutput.ReadToEnd();
                        //pProcess.WaitForExit();
                    }
                }
                catch
                {
                }
                textBox5.BackColor = (strOutput.Contains("0% persi") || (strOutput.Contains("0% loss"))) ? Color.LightGreen : Color.OrangeRed;
                strOutput = "";
                try
                {
                    if (!String.IsNullOrEmpty(textBox6.Text))
                    {
                        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                        pProcess.StartInfo.FileName = "ping";
                        pProcess.StartInfo.Arguments = textBox6.Text + " -n 1";
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.RedirectStandardOutput = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.Start();
                        strOutput = pProcess.StandardOutput.ReadToEnd();
                        //pProcess.WaitForExit();
                    }
                }
                catch
                {
                }
                textBox6.BackColor = (strOutput.Contains("0% persi") || (strOutput.Contains("0% loss"))) ? Color.LightGreen : Color.OrangeRed;
                strOutput = "";
                try
                {
                    if (!String.IsNullOrEmpty(textBox7.Text))
                    {
                        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                        pProcess.StartInfo.FileName = "ping";
                        pProcess.StartInfo.Arguments = textBox7.Text + " -n 1";
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.RedirectStandardOutput = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.Start();
                        strOutput = pProcess.StandardOutput.ReadToEnd();
                        //pProcess.WaitForExit();
                    }
                }
                catch
                {
                }
                textBox7.BackColor = (strOutput.Contains("0% persi") || (strOutput.Contains("0% loss"))) ? Color.LightGreen : Color.OrangeRed;
            }
        }

        int calls = 0;
        Dictionary<IPAddress, DateTime> ipLastTimeSeen = new Dictionary<IPAddress, DateTime>();
        List<IPAddress> newConnected = new List<IPAddress>();
        DateTime firstPing = DateTime.MinValue;
        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Enabled = false;
            int cicliDiControllo = 10;
            if (calls == 0) //verifica la presenza di new entry o mancate risposte
            {
                if (firstPing == DateTime.MinValue)
                    firstPing = DateTime.Now;
            }

            if (ipLastTimeSeen.Count == 0) 
                //Generating 192.168.0.1/24 IP Range
                for (int i = 1; i < 255; i++)
                    //Obviously you'll want to safely parse user input to catch exceptions.
                    for (int j = 2; j <= 4; j++)
                        ipLastTimeSeen.Add(IPAddress.Parse("10.2.23" + j + "." + i), DateTime.MinValue);

            List<IPAddress> ips = new List<IPAddress>(ipLastTimeSeen.Keys);
            for (int k=0;k< ips.Count;k++)
            {
                IPAddress ip = ips[k];
            
                Thread thread = new Thread(() => SendArpRequest(ip, ipLastTimeSeen, newConnected));
                thread.Start();
            }

            if (calls == (cicliDiControllo-1)) //verifica la presenza di new entry o mancate risposte
            {
                bool somethingNew = false;
                DateTime nowo = DateTime.Now;

                String newIp = "Lista nuovi ip connessi:";
                foreach (IPAddress ipa in newConnected)
                {
                    newIp += "\n\r" + ipa.ToString();
                    somethingNew = true;
                }
                newConnected.Clear();

                String lostIp = "Lista ip disconnessi:";
                for (int k = 0; k < ips.Count; k++)
                {
                    IPAddress ip = ips[k];
                    if (ipLastTimeSeen[ip] != DateTime.MinValue && nowo.Subtract(ipLastTimeSeen[ip]).TotalMinutes > nowo.Subtract(firstPing).TotalMinutes)
                    {
                        lostIp += "\n\r" + ip.ToString();
                        somethingNew = true;
                        ipLastTimeSeen[ip] = DateTime.MinValue;
                    }
                }

                if (somethingNew)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("ricei0io@gmail.com", "Casa rice parla");
                    mail.To.Add(new MailAddress("rice103@gmail.com"));
                    mail.Subject = "Nuovi ip o ip persi negli ultimi 5 minuti";
                    mail.Body = newIp + "\n\r" + lostIp;

                    mail.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient();
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = "mail.smtp2go.com";
                    smtp.Credentials = new System.Net.NetworkCredential("rice103", "YXQwdDE2YTY0NjUw");
                    smtp.Port = 2525;
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        int pippo = 1;
                    }
                }

                firstPing = DateTime.MinValue;
            }
            calls = (calls + 1) % cicliDiControllo;
            timer3.Enabled = true;
        }


        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public static void SendArpRequest(IPAddress dst, Dictionary<IPAddress,DateTime> dictonary, List<IPAddress> newIp)
        {
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            int uintAddress = BitConverter.ToInt32(dst.GetAddressBytes(), 0);

            if (SendARP(uintAddress, 0, macAddr, ref macAddrLen) == 0)
            {
                if (dictonary.ContainsKey(dst))
                {
                    if (dictonary[dst] == DateTime.MinValue)
                        newIp.Add(dst);
                    dictonary[dst] = DateTime.Now;
                }
                //Console.WriteLine("{0} responded to ping", dst.ToString());
            }
        }
    }
}