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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VarPDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //启动动画
            Storyboard board = (Storyboard)this.FindResource("zhankai");
            if (null != board)
            {
                board.Begin();
            }


        }

        int i = 0;
        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            //支持双击放大
            i += 1;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 360);
            timer.Tick += (s, e1) => { timer.IsEnabled = false; i = 0; };
            timer.IsEnabled = true;
            if (i % 2 == 0)
            {
                timer.IsEnabled = false;
                i = 0;
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }


        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            System.Environment.Exit(0);
        }

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MoreBtn_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.IsOpen = true;
        }

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //选择右边的栏目 获取索引
            TabControl tab = sender as TabControl;
            if (tab.SelectedIndex == 0)
            {
                HostInfoPage.StartProcess();
            }
        }

        private void OpenAbout(object sender, RoutedEventArgs e)
        {
            PrintDialog dlg = new PrintDialog();
            dlg.ShowDialog();

        }



    }//end MainWindow Class
}
