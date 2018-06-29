using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarPDemo.Dal;
using VarPDemo.Helper;
using VarPDemo.Models;

namespace VarPDemo
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {

            InitializeComponent();
            // InitializeCommand();
            DbHelper.InitDataBase();

        }



        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserLogin())
            {
                GoToMainView();
            }


        }
        private bool UserLogin()
        {
            if (string.IsNullOrEmpty(user.Text) || string.IsNullOrEmpty(pass.Password))
            {
                return false;
            }

            AccountModel acount = new AccountModel();
            AccountDao ado = new AccountDao(DbHelper.GetConnection());
            acount.UName = user.Text;

            ICollection<AccountModel> datas = new List<AccountModel>();
            datas = ado.SelectData(0, 1, acount);
            if (datas.Count <= 0)
            {
                MessageBox.Show("账号密码不正确,请确定!");
                return false;
            }
            acount.UPass = Md5Helper.MD5Encrypt32(pass.Password);
            int result = acount.UPass.CompareTo(datas.ElementAt(0).UPass);
            if (result != 0)
            {
                MessageBox.Show("密码错误!");
                return false;
            }

   

            return true;
        }



        private void registBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RegistAccount())
            {
                GoToMainView();
            }

        }

        private bool RegistAccount()
        {
            if (string.IsNullOrEmpty(regUser.Text) || string.IsNullOrEmpty(regPass.Password) || string.IsNullOrEmpty(regName.Text))
            {
                return false;
            }
            AccountModel acount = new AccountModel();
            AccountDao ado = new AccountDao(DbHelper.GetConnection());
            acount.UName = regUser.Text;
            if (ado.GetRecordCount(acount) > 0)
            {
                MessageBox.Show("该账号已经被注册了!");
                return false;
            }
            acount.UserName = regName.Text;
            acount.UPass = Md5Helper.MD5Encrypt32(regPass.Password);
            acount.ULevel = 0;
            acount.UState = 0;
            ICollection<AccountModel> datas = new List<AccountModel>();
            datas.Add(acount);
            ado.InsertData(datas);
            return true;
        }

        private void GoToMainView()
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        string useroldStr;
        private void user_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            Regex re = new Regex("[^0-9a-zA-Z.-]+");
            if (re.IsMatch(e.Text))
            {
                user.Text = useroldStr;
            }
            useroldStr = user.Text;

        }

        string regUseroldStr;
        private void regUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9a-zA-Z.-]+");
            if (re.IsMatch(e.Text))
            {
                regUser.Text = regUseroldStr;
            }
            regUseroldStr = regUser.Text;
        }

        private void root1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tb = sender as TabControl;
            if (tb.SelectedIndex == 0)
            {
                regName.Text = "";
                regPass.Password = "";
                regUser.Text = "";
            }
            else
            {
                user.Text = "";
                pass.Password = "";
            }

        }


    }


}


//废弃代码
/*
   //声明并定义命令  
        private RoutedCommand LoginCommand = new RoutedCommand("LOGINS", typeof(LoginWindow));
        private RoutedCommand RegistCommand = new RoutedCommand("REGISTS", typeof(LoginWindow));
        private void InitializeCommand()
        {

            this.loginBtn.Command = LoginCommand; //把命令赋值给命令源
            this.loginBtn.CommandTarget = loginBtn;//指定命令目标
            this.registBtn.Command = RegistCommand;
            this.registBtn.CommandTarget = registBtn;


            CommandBinding loginBinding = new CommandBinding(); //创建命令关联  
            loginBinding.Command = LoginCommand;//只关注与rouutedCommand相关的命令  
            loginBinding.CanExecute += new CanExecuteRoutedEventHandler(cb_Login);
            loginBinding.Executed += new ExecutedRoutedEventHandler(cb_Execute);

            CommandBinding registBinding = new CommandBinding();
            registBinding.Command = RegistCommand;//只关注与rouutedCommand相关的命令  
            registBinding.CanExecute += new CanExecuteRoutedEventHandler(cb_Regist);
            registBinding.Executed += new ExecutedRoutedEventHandler(cb_Execute);

            this.tb1.CommandBindings.Add(loginBinding);//把命令关联安置在外围控件上  
            this.tb2.CommandBindings.Add(registBinding);
        }

        private void cb_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            //当命令到达目标之后，此方法被调用  避免事件继续向上传递而降低程序性能  
            e.Handled = true;
        }

        private void cb_Login(object sender, CanExecuteRoutedEventArgs e)
        {
            //当探测命令是否可执行的时候该方法会被调用  
            if (string.IsNullOrEmpty(user.Text) || string.IsNullOrEmpty(pass.Password))
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
            e.Handled = true;//避免事件继续向上传递而降低程序性能  
        }

        private void cb_Regist(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(regName.Text) || string.IsNullOrEmpty(regUser.Text) ||
                string.IsNullOrEmpty(regPass.Password))
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

*/