using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _registerUsername;
        private string _registerPassword;
        private string _loginUsername;
        private string _loginPassword;

        public string RegisterUsername
        {
            get => _registerUsername;
            set
            {
                _registerUsername = value;
                OnPropertyChanged();
            }
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set
            {
                _registerPassword = value;
                OnPropertyChanged();
            }
        }

        public string LoginUsername
        {
            get => _loginUsername;
            set
            {
                _loginUsername = value;
                OnPropertyChanged();
            }
        }

        public string LoginPassword
        {
            get => _loginPassword;
            set
            {
                _loginPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }

        public MainViewModel()
        {
            RegisterCommand = new RelayCommand(Register);
            LoginCommand = new RelayCommand(Login);
        }

        private void Register(object parameter)
        {
            string username = RegisterUsername;
            string password = RegisterPassword;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя и пароль");
                return;
            }

            RegisterUsername = string.Empty;
            RegisterPassword = string.Empty;

            MessageBox.Show("Пользователь успешно зарегистрирован");
        }


        private void Login(object parameter)
        {
            string username = LoginUsername;
            string password = LoginPassword;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя и пароль");
                return;
            }
            LoginUsername = string.Empty;
            LoginPassword = string.Empty;

            MessageBox.Show("Пользователь успешно вошел в систему");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
