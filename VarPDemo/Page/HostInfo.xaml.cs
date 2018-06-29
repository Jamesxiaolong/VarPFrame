using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VarPDemo.Common;
using VarPDemo.Helper;

namespace VarPDemo.Page
{
    /// <summary>
    /// HostInfo.xaml 的交互逻辑
    /// </summary>
    public partial class HostInfo : UserControl
    {

        //硬盘使用
        double DirSurplus;
        double MemSurplus;
        ICollection<string> dirNameList;
        double diskSize;
        double memSize;

        private IAsynNotify Asyn1;
        private IAsynNotify Asyn2;
        private IAsynNotify Asyn3;
        private IAsynNotify Asyn4;

        public HostInfo()
        {
            InitializeComponent();

            this.Asyn1 = new DefaultAsynNotify();
            this.pro1.DataContext = this.Asyn1;

            this.Asyn2 = new DefaultAsynNotify();
            this.pro2.DataContext = this.Asyn2;

            this.Asyn3 = new DefaultAsynNotify();
            this.pro3.DataContext = this.Asyn3;

            this.Asyn4 = new DefaultAsynNotify();
            this.pro4.DataContext = this.Asyn4;

            GetSysInfo();
        }


        private void GetSysInfo()
        {
            try
            {
                /* 
                 //CPU
                 item1.Num = Computer.Instance().GetLogicCpuCount().ToString();
                 //BrushConverter brushConverter = new BrushConverter();
                 //item1.Grid1.Background = (Brush)brushConverter.ConvertFromString("#67CBCC");
                 //item1.img.Source = new BitmapImage(new Uri("../Resource/Images/Hostinfo/CPU.png", UriKind.Relative));
                
                 //内存
                 item2.Desc.Text = "物理内存";
                 memSize = long.Parse(Computer.Instance().GetTotalPhysicalMemory()) / 1024 / 1024;
                 item2.Num.Text = memSize.ToString();
                 item2.Num.Text += "MB";
                 item2.Grid1.Background = (Brush)brushConverter.ConvertFromString("#FF6D5D");
                 item2.img.Source = new BitmapImage(new Uri("../Resource/Images/Hostinfo/mem.png", UriKind.Relative));
                 * 
                 //硬盘大小
                 item3.Desc.Text = "硬盘容量";
                 diskSize = long.Parse(Computer.Instance().GetSizeOfDisk()) / 1024 / 1024 / 1024;
                 item3.Num.Text = diskSize.ToString();
                 item3.Num.Text += "GB";
                 item3.Grid1.Background = (Brush)brushConverter.ConvertFromString("#FBD948");
                 item3.img.Source = new BitmapImage(new Uri("../Resource/Images/Hostinfo/disk.png", UriKind.Relative));

                 //分区数量
                 dirNameList = new List<string>();
                 dirNameList = Computer.Instance().GetDirNames();
                 item4.Desc.Text = "硬盘分区";
                 item4.Num.Text = dirNameList.Count.ToString();
                 item4.Grid1.Background = (Brush)brushConverter.ConvertFromString("#60CEEB");
                 item4.img.Source = new BitmapImage(new Uri("../Resource/imgs/Hostinfo/disk.png", UriKind.Relative));
              */

                item1.Num = Computer.Instance().GetLogicCpuCount().ToString();

                memSize = long.Parse(Computer.Instance().GetTotalPhysicalMemory()) / 1024 / 1024;
                item2.Num = memSize.ToString() + "MB";

                diskSize = long.Parse(Computer.Instance().GetSizeOfDisk()) / 1024 / 1024 / 1024;
                item3.Num = diskSize.ToString() + "GB";

                dirNameList = new List<string>();
                dirNameList = Computer.Instance().GetDirNames();
                item4.Num = dirNameList.Count.ToString();

            }
            catch (Exception e)
            {
                MessageBox.Show("异常", e.Message);
            }



        }//end HostInfo


        private void GetProInfo()
        {
            //获取进度条需要的信息

            //获取硬盘使用量
            //再次获得硬盘使用率 变量磁盘数量 获得每个盘剩余容量
            /* 这里会出现一个异常
            string nameStr;
            double _DiskSize = 0;
           /* foreach (string name in dirNameList)
            {
                nameStr = name;
                nameStr = nameStr.Replace(":\\", "");
                //获取该磁盘剩余带下
                _DiskSize += Computer.Instance().GetHardDiskFreeSpace(nameStr);
            }
            DirSurplus = (_DiskSize / diskSize) * 100.0;*/
            DirSurplus = 62.5;


            //获取内存已用多少 获得使用率
            double keyong = Computer.Instance().MemoryAvailable() / 1024 / 1024;
            keyong = memSize - keyong;//获得可用 
            MemSurplus = (keyong / memSize) * 100.0; //转成百分比


        }


        public object lockobj = new object();
        public bool isStartProcess = true;//是否可以执行动画
        public void StartProcess()
        {
            lock (lockobj)
            {
                if (isStartProcess)
                {
                    /* System.Utility.SingleThread _Thread;
                     _Thread = new SingleThread(); 
                     _Thread.Start(ProcessThreadFunc);*/
                    isStartProcess = false; //动画在运行其他不能执行
                    Task task = new Task(ProcessThreadFunc);
                    task.Start();

                }
            }
        }


        private void ProcessThreadFunc()
        {

            GetProInfo();
            this.Asyn1.Start(100);
            this.Asyn2.Start(100);

            //找到值最大的循环
            int max = 0;
            if (DirSurplus > MemSurplus)
                max = (int)DirSurplus;
            else
                max = (int)MemSurplus;
            max += 1;

            for (int i = 0; i < 100; i++)
            {
                if (i < (int)DirSurplus)
                {
                    this.Asyn1.Advance(1);
                }
                else if (i == (int)DirSurplus)
                {
                    //截取后面两位小数
                    this.Asyn1.Advance((DirSurplus - (int)DirSurplus));
                }

                if (i < (int)MemSurplus)
                {
                    this.Asyn2.Advance(1);
                }
                else if (i == (int)MemSurplus)
                {
                    this.Asyn2.Advance((MemSurplus - (int)MemSurplus));
                }
                System.Threading.Thread.Sleep(30);
            }
            isStartProcess = true;


        }


        public double prevByte1 = 0; //记录上次的
        private void UpdateText1(long bytesCopied, long? totalBytes, HttpClientHelper.HttpDownloadState state, string failInfo)
        {
            if (state == HttpClientHelper.HttpDownloadState.Loading)
            {
                //当前复制字节 总复制字节  需要获得上次增加了多少将总的减去上次的
                double currByte = bytesCopied - prevByte1;
                currByte = (currByte / (double)totalBytes) * 100.0;
                prevByte1 = bytesCopied;
                //Trace.WriteLine("正在下载");
                double tatal = (double)bytesCopied / (double)totalBytes;
                //Trace.WriteLine(string.Format("复制字节{0}:总长度{1}:进度{2}:百分比{3}", bytesCopied, totalBytes, tatal,tatal*100.0));
                this.Asyn3.Advance(currByte);
            }
            else if (state == HttpClientHelper.HttpDownloadState.Success)
            {
                this.Asyn3.Advance(100);
                //Trace.WriteLine("下载成功");
            }
            else if (state == HttpClientHelper.HttpDownloadState.ERROR)
            {
                MessageBox.Show(string.Format("下载失败:{0}", failInfo));
                //Trace.WriteLine(string.Format("下载失败:{0}",failInfo));
            }
        }


        public double prevByte2 = 0; //记录上次的
        private void UpdateText2(long bytesCopied, long? totalBytes, HttpClientHelper.HttpDownloadState state, string failInfo)
        {
            if (state == HttpClientHelper.HttpDownloadState.Loading)
            {
                //当前复制字节 总复制字节  需要获得上次增加了多少将总的减去上次的
                double currByte = bytesCopied - prevByte2;
                currByte = (currByte / (double)totalBytes) * 100.0;
                prevByte2 = bytesCopied;
                //Trace.WriteLine("正在下载");
                double tatal = (double)bytesCopied / (double)totalBytes;
                //Trace.WriteLine(string.Format("复制字节{0}:总长度{1}:进度{2}:百分比{3}", bytesCopied, totalBytes, tatal, tatal * 100.0));    
                this.Asyn4.Advance(currByte);
            }
            else if (state == HttpClientHelper.HttpDownloadState.Success)
            {
                //this.Asyn4.Advance(100);
                //MessageBox.Show("下载成功");
                //Trace.WriteLine("下载成功");
            }
            else if (state == HttpClientHelper.HttpDownloadState.ERROR)
            {
                MessageBox.Show(string.Format("回调:下载失败:{0}", failInfo));
                //Trace.WriteLine(string.Format("下载失败:{0}", failInfo));
            }
        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            this.Asyn4.Start(100);
            HttpClientHelper http2 = new HttpClientHelper();
            http2.DownloadMutilThread("http://pc.xzstatic.com/2018/06/15/The.Quest.RIP.rar", "c:/The.Quest.RIP.rar", 6, 2048, UpdateText2);
            //http2.DownloadMutilThread("https://www.vipkes.cn/images/logo.png", "c:/logo.png", 6, 2048, UpdateText2);

            //this.Asyn3.Start(100);
            //HttpClientHelper http1 = new HttpClientHelper();
            //http1.Download("http://pc.xzstatic.com/2018/06/15/The.Quest.RIP.rar", "c:/The.Quest.RIP HTTP.rar", UpdateText1);

        }


    }//ebd HostInfo
}
