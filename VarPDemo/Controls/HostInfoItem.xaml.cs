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

namespace VarPDemo.Controls
{
    /// <summary>
    /// HostInfoItem.xaml 的交互逻辑
    /// </summary>
    public partial class HostInfoItem : UserControl
    {

        public HostInfoItem()
        {
            InitializeComponent();
        }


        //依赖属性 外界直接改变这个属性即可改变里面的值
        public string Desc
        {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }
        public static readonly DependencyProperty DescProperty = DependencyProperty
            .Register("Desc", typeof(string), typeof(HostInfoItem));

        public string Num
        {
            get { return (string)GetValue(NumProperty); }
            set { SetValue(NumProperty, value); }
        }
        public static readonly DependencyProperty NumProperty = DependencyProperty
            .Register("Num", typeof(string), typeof(HostInfoItem));


        public Brush BackBrush
        {
            get { return (Brush)GetValue(BackBrushProperty); }
            set { SetValue(BackBrushProperty, value); }
        }
        public static readonly DependencyProperty BackBrushProperty = DependencyProperty
            .Register("BackBrush", typeof(Brush), typeof(HostInfoItem), new PropertyMetadata(Brushes.AliceBlue));


        public static readonly DependencyProperty ImgProperty = DependencyProperty.Register("Img", typeof(string), typeof(HostInfoItem));
        public string Img
        {
            get { return (string)GetValue(ImgProperty); }
            set { SetValue(ImgProperty, value); }
        }


    }

}
