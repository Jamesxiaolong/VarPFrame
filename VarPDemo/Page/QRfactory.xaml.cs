using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

//二维码工厂
namespace VarPDemo.Page
{
    /// <summary>
    /// 生成和解析二维码，可以在二维码中添加logo.
    /// 生成二维码时，可一设定二维码的版本和大小，以及设置logo的大小.
    /// 生成成功后，可以通过解析二维码来验证二维码是否有效(有时候logo的大小不合适会造成生成的二维码无效)。
    /// </summary>
    public partial class QRfactory : UserControl
    {
        private Bitmap bimg = null; //保存生成的二维码.方便后面保存
        private string logoImagepath = string.Empty; //存储Logo的路径
        private string decodePath = string.Empty;    //解码图片路口
        private int QRCodeScale; //
        /// 二维码一共有 40 个尺寸。官方叫版本 VersionVersion 1 是 21 x 21 的矩阵，Version 2 是 25 x 25 的矩阵
        /// 每增加一个 version，就会增加 4 的尺寸,最高 Version 40
        private int QRCodeVersion;


        public QRfactory()
        {
            InitializeComponent();


        }

        private void GenerateBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ShowQRCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("生成二维码出错！:{0}", ex.Message), "系统提示");
                return;
            }
        }

        /// <summary>
        /// 显示生成的二维码
        /// </summary>
        public void ShowQRCode()
        {
            if (txtQRCodeContent.Text.Trim().Length <= 0)
            {
                MessageBox.Show("二维码内容不能为空，请输入内容！", "系统提示");
                txtQRCodeContent.Focus();
                return;
            }
            QRCodeScale = 4;
            QRCodeVersion = 3;
            bimg = CreateQRCode(txtQRCodeContent.Text);
            QrImg.Source = BitmapToBitmapImage(bimg);
        }

        /// <summary>
        /// 生成二维码，如果有Logo，则在二维码中添加Logo
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Bitmap CreateQRCode(string content)
        {
            QRCodeEncoder qrEncoder = new QRCodeEncoder();
            qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrEncoder.QRCodeScale = QRCodeScale;
            qrEncoder.QRCodeVersion = QRCodeVersion;
            qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            try
            {
                var qrcode = qrEncoder.Encode(content, Encoding.UTF8);
                if (!logoImagepath.Equals(string.Empty))
                {
                    Graphics g = Graphics.FromImage(qrcode);
                    Bitmap bitmapLogo = new Bitmap(logoImagepath);
                    int logosize = 30;
                    bitmapLogo = new Bitmap(bitmapLogo, new System.Drawing.Size(logosize, logosize));
                    PointF point = new PointF(qrcode.Width / 2 - logosize / 2, qrcode.Height / 2 - logosize / 2);
                    g.DrawImage(bitmapLogo, point);
                }
                return qrcode;
            }
            catch (IndexOutOfRangeException e)
            {
                //二维码版本智能的后面涨
                if (QRCodeVersion < 35)
                {
                    QRCodeVersion++;
                    return CreateQRCode(txtQRCodeContent.Text);
                }
                MessageBox.Show(string.Format("超出当前二维码版本的容量上限，请选择更高的二维码版本！{0}", e.Message), "系统提示");
                return new Bitmap(100, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("生成二维码出错！", ex.Message), "系统提示");
                return new Bitmap(100, 100);
            }
        }


        /// <summary>
        /// 将Bitmap转换成BitmapImage,使其能够在Image控件中显示
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bImage.BeginInit();
                bImage.StreamSource = new MemoryStream(ms.ToArray());
                bImage.EndInit();
                return bImage;
            }
        }



        /// <summary>
        /// 添加logo按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "图片文件|*.jpg;*.png;*.gif|All files(*.*)|*.*";
            if (openDialog.ShowDialog() == true)
            {
                logoImagepath = openDialog.FileName;
                Bitmap bImg = new Bitmap(logoImagepath);
                logoImg.Source = new BitmapImage(new Uri(openDialog.FileName));
                ResetImageStrethch(logoImg, bImg);
            }
        }


        /// <summary>
        /// 充值Image的Strethch属性
        /// 当图片小于size显示原图
        /// 当图片大于size缩放比例,让图标全显示
        /// </summary>
        /// <param name="img"></param>
        /// <param name="bImg"></param>
        private void ResetImageStrethch(System.Windows.Controls.Image img, Bitmap bImg)
        {
            if (bImg.Width <= img.Width)
            {
                img.Stretch = Stretch.None;
            }
            else
            {
                img.Stretch = Stretch.Fill;
            }

        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Png文件(*.Png)|*.png|All files(*.*)|*.*";
            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    SaveQRCode(saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("保存二维码出错！{0}", ex.Message), "系统提示");
                    return;
                }
            }
        }


        /// <summary>
        /// 保存二维码，并为二维码添加白色背景。
        /// </summary>
        /// <param name="Path"></param>
        public void SaveQRCode(string Path)
        {
            if (bimg != null)
            {
                Bitmap bitmap = new Bitmap(bimg.Width + 30, bimg.Height + 30);
                Graphics g = Graphics.FromImage(bitmap);
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                g.DrawImage(bimg, new PointF(15, 15));
                bitmap.Save(Path, System.Drawing.Imaging.ImageFormat.Png);
                bitmap.Dispose();
            }
        }

        private void DecodeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DecodeContent.Text = "";
                DecodeContent.Text = DecodeQRCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}", ex.Message));
            }
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == false)
            {
                return;
            }
            decodeImg.Source = new BitmapImage(new Uri(dlg.FileName, UriKind.Absolute));
            decodePath = dlg.FileName;
        }


        /// <summary>
        /// 二维码解码
        /// </summary>
        /// <returns></returns>
        public string DecodeQRCode()
        {
            if (decodeImg.Source != null)
            {
                QRCodeDecoder decode = new QRCodeDecoder();
                QRCodeImage qimg = new QRCodeBitmapImage(new Bitmap(decodePath));
                return decode.decode(qimg, Encoding.UTF8);

            }
            return "没有找到要解码的图片!";
        }





    }//end class
}
