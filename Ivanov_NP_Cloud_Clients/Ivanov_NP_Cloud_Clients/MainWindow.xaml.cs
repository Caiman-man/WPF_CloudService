using Ivanov_NP_Cloud_Clients.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
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

namespace Ivanov_NP_Cloud_Clients
{

    public partial class MainWindow : Window
    {
        Service1Client client;
        AdminWindow adminWindow;
        ObservableCollection<FilesInfo> filesList = new ObservableCollection<FilesInfo>();
        string baseFolderPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        //log in
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            client = new Service1Client();

            try
            {
                //если логин и пароль подходят, то открываем окно работы с файлами
                if (client.CheckRegistrarion(NameTextBox.Text, PasswordBox.Password) == true)
                {
                    GetBaseFolderPath();
                    adminWindow = new AdminWindow(NameTextBox.Text, baseFolderPath);
                    adminWindow.Owner = this;
                    adminWindow.PerformRefreshFiles += RefreshFilesEvent;
                    adminWindow.PerformSearchFiles += SearchFilesEvent;
                    adminWindow.PerformCreateFolder += CreateFolderEvent;
                    adminWindow.PerformDeleteFiles += DeleteFilesEvent;
                    adminWindow.PerformDownloadFiles += DownloadFilesEvent;
                    adminWindow.PerformDownloadFolders += DownloadFoldersEvent;
                    adminWindow.PerformSendFiles += SendFilesEvent;
                    adminWindow.PerformSendFolders += SendFoldersEvent;
                    if (this.WindowState == WindowState.Maximized)
                        adminWindow.WindowState = WindowState.Maximized;
                    adminWindow.Show();
                    this.WindowState = WindowState.Normal;
                    this.Visibility = Visibility.Hidden;
                    RefreshDataGrid();
                }
                else
                {
                    MessageBox.Show("Wrong login or password!");
                }
            }
            catch (CommunicationException ex)
            {
                MessageBox.Show("Failed to connect the server!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
        }

        //получить имя корневой папки
        private void GetBaseFolderPath()
        {
            baseFolderPath = client.GetBaseFolderPath(NameTextBox.Text, PasswordBox.Password);
        }

        //показать файлы из корневой папки
        private void RefreshDataGrid()
        {
            filesList.Clear();
            FilesInfo[] currentFileList = client.ShowFilesFromBaseDirectory(NameTextBox.Text, PasswordBox.Password);
            if (currentFileList == null) MessageBox.Show("The list of files from server is empty");

            foreach (var file in currentFileList)
            {
                try
                {
                    FilesInfo info = new FilesInfo()
                    {
                        FileIcon = file.FileIcon,
                        FileFullName = file.FileFullName,
                        FileName = file.FileName,
                        LastAccessTime = file.LastAccessTime,
                        Size = file.Size
                    };
                    filesList.Add(info);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString());
                    continue;
                }
            }
            adminWindow.myFilesDataGrid.ItemsSource = filesList;
        }

        //registration
        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Registration reg = new Registration();
            reg.Owner = this;
            reg.PerformRegistrarion += RegistrationEvent;
            reg.ShowDialog();
        }

        //event for registration
        private void RegistrationEvent(string login, string password)
        {
            client.RegisterNewUser(login, password);
        }

        //event for refresh files
        private void RefreshFilesEvent(string path)
        {
            filesList.Clear();
            FilesInfo[] currentFileList = client.RefreshFiles(path);
            if (currentFileList == null) MessageBox.Show("The list of files from server is empty");

            foreach (var file in currentFileList)
            {
                try
                {
                    FilesInfo info = new FilesInfo()
                    {
                        FileIcon = file.FileIcon,
                        FileFullName = file.FileFullName,
                        FileName = file.FileName,
                        LastAccessTime = file.LastAccessTime,
                        Size = file.Size
                    };
                    filesList.Add(info);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString());
                    continue;
                }
            }
            adminWindow.myFilesDataGrid.ItemsSource = filesList;
        }

        //event for search files
        private void SearchFilesEvent(string path, string name)
        {
            filesList.Clear();
            FilesInfo[] currentFileList = client.SearchFiles(path, name);
            if (currentFileList == null) MessageBox.Show("The list of files from server is empty");

            foreach (var file in currentFileList)
            {
                try
                {
                    FilesInfo info = new FilesInfo()
                    {
                        FileIcon = file.FileIcon,
                        FileFullName = file.FileFullName,
                        FileName = file.FileName,
                        LastAccessTime = file.LastAccessTime,
                        Size = file.Size
                    };
                    filesList.Add(info);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString());
                    continue;
                }
            }
            adminWindow.myFilesDataGrid.ItemsSource = filesList;
        }

        //event for create folder
        private void CreateFolderEvent(string path)
        {
            client.CreateFolder(path);
        }

        //event for delete files
        private void DeleteFilesEvent(string path)
        {
            client.DeleteFile(path);
        }

        //event for download files
        private async void DownloadFilesEvent(string sourcePath, string name, string destPath)
        {
            byte[] arr = await client.GetArrayAsync(sourcePath);
            File.WriteAllBytes(destPath + @"\" + name, arr);
        }

        //event for download folders
        private async void DownloadFoldersEvent(string sourcePath, string name, string destPath)
        {
            string[] foldersList = await client.GetFoldersAsync(sourcePath);
            foreach (string folderPath in foldersList)
            {
                int index = baseFolderPath.Length - 2;
                Directory.CreateDirectory(destPath + @"\" + folderPath.Substring(index));
            }

            string[] filesList = await client.GetFilesInsideFoldersAsync(sourcePath);
            foreach (string filePath in filesList)
            {
                int index = baseFolderPath.Length - 2;
                byte[] arr = await client.GetArrayAsync(filePath);
                File.WriteAllBytes(destPath + @"\" + filePath.Substring(index), arr);
            }
        }

        //event for send files
        private async void SendFilesEvent(string sourcePath)
        {
            FileStream st = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            byte[] arr = new byte[st.Length];
            st.Read(arr, 0, (int)st.Length);
            st.Close();
            await client.UploadFilesAsync(arr, baseFolderPath, System.IO.Path.GetFileName(sourcePath));
            RefreshDataGrid();
        }

        //event for send folders
        private async void SendFoldersEvent(List<string> foldersList, List<string> filesList)
        {
            string[] foldersArray = foldersList.ToArray();
            string[] filesArray = filesList.ToArray();
            await client.UploadFoldersAsync(foldersArray, filesArray, baseFolderPath);

            string lastName = System.IO.Path.GetFileName(foldersList[0]);
            int lastLength = lastName.Length;
            int baseLength = foldersList[0].Length - lastLength;
            foreach (var sourcePath in filesArray)
            {
                FileStream st = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
                byte[] arr = new byte[st.Length];
                st.Read(arr, 0, (int)st.Length);
                st.Close();
                await client.UploadFilesInsideFoldersAsync(arr, baseFolderPath, sourcePath, baseLength);
            }
            RefreshDataGrid();
        }
    }
}
