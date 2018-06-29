using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace VarPControl.Controls
{
    /// <summary>
    /// PButton.xaml 的交互逻辑
    /// 集成了字体图标
    /// PButton继承自微软基础控件Button，没有什么逻辑代码，主要扩展了几个属性
    /// </summary>
    public partial class PButton : Button
    {
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(PButton), new PropertyMetadata(Brushes.DarkBlue));
        /// <summary>
        /// 鼠标按下背景样式
        /// </summary>
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedForegroundProperty =
            DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(PButton), new PropertyMetadata(Brushes.White));
        /// <summary>
        /// 鼠标按下前景样式（图标、文字）
        /// </summary>
        public Brush PressedForeground
        {
            get { return (Brush)GetValue(PressedForegroundProperty); }
            set { SetValue(PressedForegroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(PButton), new PropertyMetadata(Brushes.RoyalBlue));
        /// <summary>
        /// 鼠标进入背景样式
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(PButton), new PropertyMetadata(Brushes.White));
        /// <summary>
        /// 鼠标进入前景样式
        /// </summary>
        public Brush MouseOverForeground
        {
            get { return (Brush)GetValue(MouseOverForegroundProperty); }
            set { SetValue(MouseOverForegroundProperty, value); }
        }

        public static readonly DependencyProperty PIconProperty =
            DependencyProperty.Register("PIcon", typeof(string), typeof(PButton), new PropertyMetadata("\ue604"));
        /// <summary>
        /// 按钮字体图标编码
        /// </summary>
        public string PIcon
        {
            get { return (string)GetValue(PIconProperty); }
            set { SetValue(PIconProperty, value); }
        }

        public static readonly DependencyProperty PIconSizeProperty =
            DependencyProperty.Register("PIconSize", typeof(int), typeof(PButton), new PropertyMetadata(20));
        /// <summary>
        /// 按钮字体图标大小
        /// </summary>
        public int PIconSize
        {
            get { return (int)GetValue(PIconSizeProperty); }
            set { SetValue(PIconSizeProperty, value); }
        }

        public static readonly DependencyProperty PIconMarginProperty = DependencyProperty.Register(
            "PIconMargin", typeof(Thickness), typeof(PButton), new PropertyMetadata(new Thickness(0, 1, 3, 1)));
        /// <summary>
        /// 字体图标间距
        /// </summary>
        public Thickness PIconMargin
        {
            get { return (Thickness)GetValue(PIconMarginProperty); }
            set { SetValue(PIconMarginProperty, value); }
        }

        public static readonly DependencyProperty PIconVisibilityProperty = DependencyProperty.Register(
           "PIconVisibility", typeof(bool), typeof(PButton), new PropertyMetadata(false));
        /// <summary>
        /// 字体图标显示模式
        /// </summary>
        public bool PIconVisibility
        {
            get { return (bool)GetValue(PIconVisibilityProperty); }
            set { SetValue(PIconVisibilityProperty, value); }
        }


        public static readonly DependencyProperty PIconColorProperty =
            DependencyProperty.Register("PIconColor", typeof(Brush), typeof(PButton), new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// 按钮字体图标颜色
        /// </summary>
        public Brush PIconColor
        {
            get { return (Brush)GetValue(PIconColorProperty); }
            set { SetValue(PIconColorProperty, value); }
        }

        //FontFamily
        public static readonly DependencyProperty PIconFamilyProperty =  
            DependencyProperty.Register("PIconFamily", typeof(FontFamily), typeof(PButton));
        /// <summary>
        /// 字体图标库
        /// </summary>
        public FontFamily PIconFamily
        {
            get { return (FontFamily)GetValue(PIconFamilyProperty); }
            set { SetValue(PIconFamilyProperty, value); }
        }



        public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.Register(
            "AllowsAnimation", typeof(bool), typeof(PButton), new PropertyMetadata(true));
        /// <summary>
        /// 是否启用Ficon动画
        /// </summary>
        public bool AllowsAnimation
        {
            get { return (bool)GetValue(AllowsAnimationProperty); }
            set { SetValue(AllowsAnimationProperty, value); }
        }


        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PButton), new PropertyMetadata(new CornerRadius(2)));
        /// <summary>
        /// 按钮圆角大小,左上，右上，右下，左下
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty ContentDecorationsProperty = DependencyProperty.Register(
            "ContentDecorations", typeof(TextDecorationCollection), typeof(PButton), new PropertyMetadata(null));
        public TextDecorationCollection ContentDecorations
        {
            get { return (TextDecorationCollection)GetValue(ContentDecorationsProperty); }
            set { SetValue(ContentDecorationsProperty, value); }
        }

        static PButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PButton), new FrameworkPropertyMetadata(typeof(PButton)));
        }
    }
}
