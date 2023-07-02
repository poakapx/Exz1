using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace WpfApp1
{
    internal class OpenUserViewModel
    {
        private async void UploadButton_Click(object sender, RoutedEventArgs e)
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
                        await blob.UploadFromStreamAsync(fileStream);
                    }

                    TextBlock documentTextBlock = new TextBlock();
                    documentTextBlock.Text = fileName;

                    Button downloadButton = new Button();
                    downloadButton.Content = "Скачать документ";
                    downloadButton.Click += (s, args) => DownloadDocument(blob);

                    Button deleteButton = new Button();
                    deleteButton.Content = "Удалить документ";
                    deleteButton.Click += (s, args) => DeleteDocument(blob, documentTextBlock, downloadButton, deleteButton);

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
