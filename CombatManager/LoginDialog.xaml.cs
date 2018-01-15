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
using System.Windows.Shapes;

using CombatManager.Google;
using CombatManager.Service;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {

        public LoginDialog()
        {
            InitializeComponent();
        }

        public Action<LoginResult> Callback
        {
            get; set;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginResult res = new LoginResult();
            res.Service = "Google";

            String token = await GoogleLogin.Login(this);
            if (token != null)
            {

                TokenData data = await LoginService.LoginAsync(token);

                if (data != null)
                {
                    res.Id = data.loginid;
                    res.Token = data.token;
                }
            }
            Callback?.Invoke(res);

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public class LoginResult
        {
            public bool Succeeded;
            public string Token;
            public string Service;
            public string Id;

            
        }
        
    }
}
