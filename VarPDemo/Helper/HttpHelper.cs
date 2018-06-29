using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKick.HttpLib;
using JumpKick.HttpLib.Builder;
using VarPDemo.Helper.HttpMutilDownload;
using System.Net;
using System.IO;
using System.Windows;
using System.Threading;
using VarPDemo.Common;


namespace VarPDemo.Helper
{
    internal class HttpClientHelper
    {

        public enum HttpDownloadState
        {
            ERROR = -1,
            Success = 0,
            Loading = 1,
            InitSuccess = 2,
        }

        /// <summary>
        /// 下载进度委托事件
        /// </summary>
        /// <param name="bytesCopied">已经复制字节长度</param>
        /// <param name="totalBytes">总长度</param>
        /// <param name="state">状态-1失败,1在下载,0下载成功</param>
        /// <param name="failInfo">错误信息</param>
        public delegate void HttpDownloadProgress(long bytesCopied, long? totalBytes, HttpDownloadState state, string failInfo);


        /// <summary>
        /// HTTP下载性能优先
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="fileName">保存本地路劲</param>
        /// <param name="progress">进度的委托事件</param>
        public void Download(string url, string fileName, HttpDownloadProgress progress)
        {
            RequestBuilder request = Http.Get(url).DownloadTo(fileName, onProgressChanged: (bytesCopied, totalBytes) =>
            {
                progress(bytesCopied, totalBytes, HttpDownloadState.Loading, null);
            },
            onSuccess: (headers) =>
            {
                progress(0, 0, HttpDownloadState.Success, null);
            });

            request.OnFail(action =>
            {
                progress(0, 0, HttpDownloadState.ERROR, action.Message);//这里发生错误异常
            }).Go();
        }


        /// <summary>
        /// HTTP多线程下载,急速提升下载速度,但很消耗性能
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="fileName">保存本地路劲</param>
        /// <param name="ThreadNum">线程数量(建议6个)</param>
        /// <param name="progress"></param>
        public void DownloadMutilThread(string url, string fileName, int threadNum, int bufSize, HttpDownloadProgress progress)
        {
            HttpMutilThreadDownload httpMTD = new HttpMutilThreadDownload(url, fileName, threadNum, bufSize, progress);
            httpMTD.StartDownload();
        }

    }//end HttpHelper
}



namespace VarPDemo.Helper.HttpMutilDownload
{
    internal class HttpMutilThreadDownload
    {
        public string Url { get; set; }          //接收文件的URL
        public string FileName { get; set; }     //文件名
        public int ThreadNum { get; set; }       //线程数量
        public long FileSizeAll;                 //文件长
        public int BufferSize;                   //缓冲区大小

        public bool ThreadStatus { get; set; }   //每个线程结束标志
        public string[] FileNames { get; set; }  //线程文件名数组 
        public int[] StartPos { get; set; }      //线程启动位置
        public int[] FileSizes { get; set; }     //线程需要下载的大小
        public int HasMerge { get; set; }        //文件合并标志
        public int CpuLogicCount { get; set; }   //CPU逻辑处理器数量 
        public long bytesCopiedLength = 0;       //已经复制多长了
        public Queue<long> byteCopiedQueue;      //字节赋值长度队列
        System.Threading.Thread checkThread;     //队列维护线程


        private static readonly object syncObject = new object(); //同步对象
        public Semaphore threadSeam; //线程信号量

        HttpClientHelper.HttpDownloadProgress downloadProgress; //下载进度回调给用户回调的


        public HttpMutilThreadDownload(string url, string fileName, int threadNum, int bufsize, HttpClientHelper.HttpDownloadProgress progress)
        {
            this.Url = url;
            this.FileName = fileName;
            this.downloadProgress = progress;
            this.BufferSize = bufsize;
            this.ThreadNum = threadNum;
            HasMerge = 0;
            ThreadStatus = false;
            CpuLogicCount = Computer.Instance().GetLogicCpuCount() + 1;
            threadSeam = new Semaphore(CpuLogicCount, CpuLogicCount);
            byteCopiedQueue = new Queue<long>();

            try//这里Get同步
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Url);
                WebResponse resp = request.GetResponse();//获得响应头             
                FileSizeAll = resp.ContentLength; //文件长
                if (FileSizeAll <= 0)
                    progress(0, 0, HttpClientHelper.HttpDownloadState.ERROR, "没有获得到文件长度,网络出现波动.");
                request.Abort(); //请求终止
            }
            catch (WebException e)
            {
                progress(0, 0, HttpClientHelper.HttpDownloadState.ERROR, e.Message.ToString());
                return;
            }
            Init(FileSizeAll); //初始化

        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// 每个线程分配文件评价大小,多余的部分给最后一个线程处理
        /// <param name="filesize"></param>
        private void Init(long filesize)
        {
            //ThreadStatus = new bool[ThreadNum];
            FileNames = new string[ThreadNum];
            StartPos = new int[ThreadNum];
            FileSizes = new int[ThreadNum];
            int filethread = (int)filesize / ThreadNum; //线程分配大小
            int filethreade = filethread + (int)filesize % ThreadNum;//最后一个线程分配大小
            string name = Path.GetFileName(FileName);

            for (int i = 0; i < ThreadNum; i++)
            {
                //ThreadStatus[i] = false;  //线程结束标志false
                FileNames[i] = name + "_" + i.ToString() + ".data_progress";//通过索引起名字
                //给线程最后一个分配剩余的文件部分
                StartPos[i] = filethread * i;
                if (i < ThreadNum - 1)
                {
                    FileSizes[i] = filethread - 1;
                }
                else
                {
                    FileSizes[i] = filethreade - 1;
                }
            }
        }//end Init


        public void StartDownload()
        {
            //System.Threading.Thread[] threads = new System.Threading.Thread[ThreadNum];//定义线程数组
            DownloadRecvice[] downloadRecvice = new DownloadRecvice[ThreadNum]; //接收类

            //启动队列维护线程  这里也会创建多个线程
            checkThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckQueue));
            checkThread.Start();
            CancellationTokenSource cts = new CancellationTokenSource();
            TaskFactory tfDownload = new TaskFactory(); //使用任务工厂
            for (int i = 0; i < ThreadNum; i++) //启动线程
            {
                downloadRecvice[i] = new DownloadRecvice(i, this, downloadProgress);
                tfDownload.StartNew(downloadRecvice[i].OnReceive, cts);
                //使用线程池
                //System.Threading.ThreadPool.QueueUserWorkItem(downloadRecvice[i].OnReceive);
                //threads[i] = new System.Threading.Thread(new System.Threading.ThreadStart(downloadRecvice[i].OnReceive));         
                //threads[i].Start();
                //threads[i].Priority = System.Threading.ThreadPriority.Highest;
            }


        }//end StartDownload



        /// <summary>
        /// 检查队列和文件是否可合并的线程函数
        /// </summary>
        private void CheckQueue()
        {
            while (true)
            {
                if (ThreadStatus)
                {
                    for (int i = 0; i < byteCopiedQueue.Count; i++)
                    {
                        downloadProgress(byteCopiedQueue.Dequeue(), FileSizeAll, HttpClientHelper.HttpDownloadState.Loading, null);
                    }
                    checkThread.Abort();
                }
                if (byteCopiedQueue.Count > 0)
                {
                    int count = byteCopiedQueue.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (ThreadStatus)
                        {
                            for (int j = 0; j < byteCopiedQueue.Count; j++)
                            {
                                downloadProgress(byteCopiedQueue.Dequeue(), FileSizeAll, HttpClientHelper.HttpDownloadState.Loading, null);
                            }
                            checkThread.Abort();
                        }
                        downloadProgress(byteCopiedQueue.Dequeue(), FileSizeAll, HttpClientHelper.HttpDownloadState.Loading, null);
                    }
                }
                System.Threading.Thread.Sleep(60);
            }
        }


        public void MergeFile()
        {
            ++HasMerge;
            if (HasMerge < ThreadNum)
                return;
            ThreadStatus = true;
            //文件合并
            int readSize;
            string downloadFilePath = FileName.Trim();
            byte[] bytes = new byte[BufferSize * 2];
            FileStream fs = new FileStream(downloadFilePath, FileMode.Create);
            FileStream fsTmp = null;
            //变量线程创建的临时文件
            for (int k = 0; k < ThreadNum; k++)
            {
                //首先打开文件
                fsTmp = new FileStream(FileNames[k], FileMode.Open);
                while (true)
                {
                    readSize = fsTmp.Read(bytes, 0, BufferSize * 2);//把线程临时文件读取出来
                    if (readSize <= 0)
                        break;
                    fs.Write(bytes, 0, readSize); //写入到新文件
                }
                fsTmp.Close();
                //删除临时文件 
                FileHelper.DeleteFile(FileNames[k]);
            }
            fs.Close();
            downloadProgress(0, 0, HttpClientHelper.HttpDownloadState.Success, null);
        }

    }//end class




    //接收类
    class DownloadRecvice
    {
        private static readonly object syncObject = new object(); //同步对象
        public HttpMutilThreadDownload dataObj;     //需要这个对象里的数据
        public int ThreadId { set; get; }           //线程ID    
        HttpClientHelper.HttpDownloadProgress downloadProgress;


        public DownloadRecvice(int threadid, HttpMutilThreadDownload obj, HttpClientHelper.HttpDownloadProgress progress)
        {
            dataObj = obj;
            ThreadId = threadid;
            downloadProgress = progress;
        }

        /// <summary>
        /// //接收数据线程
        /// </summary>
        /// <param name="ThreadId"></param>
        public void OnReceive(object obj)
        {
            int threadId = ThreadId;
            string filename = dataObj.FileNames[threadId]; //线程临时文件
            byte[] buffer = new byte[dataObj.BufferSize];         // 接收缓冲区
            int readSize = 0;                              // 接收字节数
            FileStream fs = new FileStream(filename, System.IO.FileMode.Create);
            Stream ns = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(dataObj.Url);
                request.AddRange(dataObj.StartPos[threadId], dataObj.StartPos[threadId] + dataObj.FileSizes[threadId]);//请求添加范围
                ns = request.GetResponse().GetResponseStream();//获得流       
                //第一次读 如果读到就继续读,读不到才退出
                readSize = ns.Read(buffer, 0, dataObj.BufferSize);
                dataObj.bytesCopiedLength += readSize;
                dataObj.byteCopiedQueue.Enqueue(dataObj.bytesCopiedLength);
                //通过线程同步,同时运行的线程数量和CPU逻辑处理器相同
                dataObj.threadSeam.WaitOne();
                while (readSize > 0)
                {
                    //写入临时文件          
                    fs.Write(buffer, 0, readSize);
                    lock (syncObject)
                    {
                        dataObj.bytesCopiedLength += readSize;
                        dataObj.byteCopiedQueue.Enqueue(dataObj.bytesCopiedLength);
                    }
                    readSize = ns.Read(buffer, 0, dataObj.BufferSize);
                }
                fs.Close();
                ns.Close();

            }
            catch (Exception er)
            {
                downloadProgress(0, 0, HttpClientHelper.HttpDownloadState.ERROR, er.ToString());
                fs.Close();
            }
            dataObj.threadSeam.Release();//退出信号量
            //线程停止调用合并文件
            dataObj.MergeFile();
        }//end OnReceive


    }//end DownloadRecvice


}
