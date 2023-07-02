using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class OpenUserWindow : Window
    {
        private CloudBlobContainer container;

        public OpenUserWindow()
        {
            InitializeComponent();

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=pavlo_test;AccountKey=TEST1TEST1TEST1TEST1;EndpointSuffix=core.windows.net\r\n";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("documents");
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".doc";
            openFileDialog.Filter = "Документы Word (*.doc)|*.doc|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = openFileDialog.SafeFileName;

                if (Path.GetExtension(filePath) == ".doc")
                {
                    CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        blob.UploadFromStream(fileStream);
                    }

                    TextBlock documentTextBlock = new TextBlock();
                    documentTextBlock.Text = fileName;

                    Button downloadButton = new Button();
                    downloadButton.Content = "Скачать документ";
                    downloadButton.Command = ((ViewModelLocator)App.Current.Resources["Locator"]).OpenUserViewModel.DownloadDocumentCommand;
                    downloadButton.CommandParameter = blob;

                    Button deleteButton = new Button();
                    deleteButton.Content = "Удалить документ";
                    deleteButton.Command = ((ViewModelLocator)App.Current.Resources["Locator"]).OpenUserViewModel.DeleteDocumentCommand;
                    deleteButton.CommandParameter = blob;

                    StackPanel documentPanel = new StackPanel();
                    documentPanel.Children.Add(documentTextBlock);
                    documentPanel.Children.Add(downloadButton);
                    documentPanel.Children.Add(deleteButton);

                    DocumentsStackPanel.Children.Add(documentPanel);
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите документ формата .doc");
                }
            }
        }
    }
}
