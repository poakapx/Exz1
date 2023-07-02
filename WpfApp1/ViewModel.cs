using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;
using WpfApp1;
using System.IO;
using Microsoft.Win32;
using System.Data.SqlClient;

namespace WpfApp1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        private async void Login(object parameter)
        {
            bool isValidUser = await ValidateUser(Username, Password);
            if (isValidUser)
            {
                OpenUserWindow();
            }
            else
            {
                MessageBox.Show("Неверные логин или пароль");
            }
        }

        private async void Register(object parameter)
        {
            bool isRegistered = await RegisterUser(Username, Password);
            if (isRegistered)
            {
                OpenUserWindow();
            }
            else
            {
                MessageBox.Show("Не удалось зарегистрировать пользователя");
            }
        }

        private void OpenUserWindow()
        {
            OpenUserWindow openUserWindow = new OpenUserWindow();
            openUserWindow.Show();
            Close();
        }

        private async Task<bool> ValidateUser(string username, string password)
        {
            private async Task<bool> ValidateUser(string username, string password)
            {
                using (var dbConnection = new SqlConnection("DefaultEndpointsProtocol=https;AccountName=pavlo_test;AccountKey=TEST1TEST1TEST1TEST1;EndpointSuffix=core.windows.net\r\n";))
                {
                    await dbConnection.OpenAsync();

                    var query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";

                    using (var command = new SqlCommand(query, dbConnection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        int count = (int)await command.ExecuteScalarAsync();

                        if (count == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

        }

    private async Task<bool> RegisterUser(string username, string password)
    {
        using (var dbConnection = new SqlConnection("YourConnectionString"))
        {
            await dbConnection.OpenAsync();

            var checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            using (var checkCommand = new SqlCommand(checkQuery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@Username", username);

                int existingUserCount = (int)await checkCommand.ExecuteScalarAsync();

                if (existingUserCount > 0)
                {
                    return false;
                }
            }

            var registerQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            using (var registerCommand = new SqlCommand(registerQuery, dbConnection))
            {
                registerCommand.Parameters.AddWithValue("@Username", username);
                registerCommand.Parameters.AddWithValue("@Password", password);

                int rowsAffected = await registerCommand.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }
    }

}

public class OpenUserWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Document> _documents;

        public ObservableCollection<Document> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public OpenUserWindowViewModel()
        {
            UploadDocumentCommand = new RelayCommand(UploadDocument);
            DownloadDocumentCommand = new RelayCommand(DownloadDocument);
            DeleteDocumentCommand = new RelayCommand(DeleteDocument);

            Documents = new ObservableCollection<Document>();
        }

        public ICommand UploadDocumentCommand { get; }
        public ICommand DownloadDocumentCommand { get; }
        public ICommand DeleteDocumentCommand { get; }

        private async void UploadDocument(object parameter)
        {
            string filePath = GetDocumentPathFromUser();
            if (!string.IsNullOrEmpty(filePath))
            {
                string fileName = Path.GetFileName(filePath);

                bool isUploaded = await UploadToBlobStorage(filePath, fileName);
                if (isUploaded)
                {
                    Document document = new Document { FileName = fileName, FilePath = filePath };

                    Documents.Add(document);
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить документ");
                }
            }
        }

        private void DownloadDocument(object parameter)
        {
            Document document = parameter as Document;
            if (document != null)
            {
                string filePath = document.FilePath;
                string fileName = document.FileName;

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string destinationPath = Path.Combine(desktopPath, fileName);
                File.Copy(filePath, destinationPath, true);
            }
        }

        private async void DeleteDocument(object parameter)
        {
            Document document = parameter as Document;
            if (document != null)
            {
                bool isDeleted = await DeleteFromBlobStorage(document.FilePath);
                if (isDeleted)
                {
                    Documents.Remove(document);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить документ");
                }
            }
        }

        private async Task<bool> UploadToBlobStorage(string filePath, string fileName)
        {
            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=pavlo_test;AccountKey=TEST1TEST1TEST1TEST1;EndpointSuffix=core.windows.net\r\n";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("documents");

                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
                await blob.UploadFromFileAsync(filePath);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> DeleteFromBlobStorage(string filePath)
        {
            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=pavlo_test;AccountKey=TEST1TEST1TEST1TEST1;EndpointSuffix=core.windows.net\r\n";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("documents");

                string fileName = Path.GetFileName(filePath);
                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
                await blob.DeleteIfExistsAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GetDocumentPathFromUser()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Документы Word (*.doc;*.docx)|*.doc;*.docx|Все файлы (*.*)|*.*";
            openFileDialog.Title = "Выберите документ";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

    }

    public class Document
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}