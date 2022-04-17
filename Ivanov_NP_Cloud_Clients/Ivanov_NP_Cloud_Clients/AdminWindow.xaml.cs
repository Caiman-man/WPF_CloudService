using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ivanov_NP_Cloud_Clients.ServiceReference1;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ivanov_NP_Cloud_Clients
{
    public delegate void RefreshFilesDelegate(string path);
    public delegate void SearchFilesDelegate(string path, string name);
    public delegate void CreateFolderDelegate(string path);
    public delegate void DeleteFilesDelegate(string path);
    public delegate void DownloadFilesDelegate(string sourcePath, string name, string destPath);
    public delegate void DownloadFoldersDelegate(string sourcePath, string name, string destPath);
    public delegate void SendFilesDelegate(string sourcePath);
    public delegate void SendFoldersDelegate(List<string> foldersList, List<string> filesList);

    public partial class AdminWindow : Window
    {
        public event RefreshFilesDelegate PerformRefreshFiles;
        public event SearchFilesDelegate PerformSearchFiles;
        public event CreateFolderDelegate PerformCreateFolder;
        public event DeleteFilesDelegate PerformDeleteFiles;
        public event DownloadFilesDelegate PerformDownloadFiles;
        public event DownloadFoldersDelegate PerformDownloadFolders;
        public event SendFilesDelegate PerformSendFiles;
        public event SendFoldersDelegate PerformSendFolders;

        string baseFolderPath;
        string currentFolderPath;
        string parentFolderPath;

        bool isLogOut = false;

        List<string> foldersList = new List<string>();
        List<string> filesList = new List<string>();

        public AdminWindow(string login, string path)
        {
            InitializeComponent();
            baseFolderPath = path;
            currentFolderPath = path;
            parentFolderPath = path;
            userNameLabel.Content = login;
        }
    
        //log out
        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to log out?", "Log out", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                isLogOut = true;
                this.Close();
            }
        }

        //close
        private void Window_Closed(object sender, EventArgs e)
        {
            if (isLogOut == true)
            {
                this.Close();
                var obj = this.Owner as MainWindow;
                if (this.WindowState == WindowState.Maximized)
                    obj.WindowState = WindowState.Maximized;
                obj.Visibility = Visibility.Visible;
            }
            else
            {
                this.Close();
                var obj = this.Owner as MainWindow;
                obj.Close();
            }
        }

        //change tabs
        private void menuListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (menuListBox.SelectedIndex == 0)
                filesTabControl.SelectedIndex = 0;
            else if (menuListBox.SelectedIndex == 1)
                filesTabControl.SelectedIndex = 1;
            else if (menuListBox.SelectedIndex == 2)
                filesTabControl.SelectedIndex = 2;
        }

        //open folder
        private void myFilesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string path = "";
            if (myFilesDataGrid.SelectedItem != null)
            {
                foreach (FilesInfo row in myFilesDataGrid.SelectedItems)
                {
                    if (row.Size == 0)
                    {
                        path = row.FileFullName;
                    }
                }
                if (path != "")
                {
                    parentFolderPath = currentFolderPath;
                    currentFolderPath = path;
                    PerformRefreshFiles.Invoke(currentFolderPath);
                }
            }
        }

        //home
        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            parentFolderPath = baseFolderPath;
            currentFolderPath = baseFolderPath;
            PerformRefreshFiles.Invoke(baseFolderPath);
        }

        //back
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Directory.GetParent(currentFolderPath).ToString();
            if (path == "C:\\IvanovCloud")
            {
                PerformRefreshFiles.Invoke(baseFolderPath);
            }
            else
            {
                parentFolderPath = path;
                currentFolderPath = parentFolderPath;
                PerformRefreshFiles.Invoke(parentFolderPath);
            }
        }

        //search
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearchFiles.Invoke(baseFolderPath, searchTextBox.Text);
        }
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PerformSearchFiles.Invoke(baseFolderPath, searchTextBox.Text);
        }

        //create folder
        private void createFolderButton_Click(object sender, RoutedEventArgs e)
        {
            PerformCreateFolder.Invoke(currentFolderPath);
            PerformRefreshFiles.Invoke(currentFolderPath);
        }

        //delete files
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to delete files?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string path = "";
                if (myFilesDataGrid.SelectedItem != null)
                {
                    foreach (FilesInfo row in myFilesDataGrid.SelectedItems)
                    {
                        path = row.FileFullName;
                        if (path != "")
                            PerformDeleteFiles.Invoke(path);
                    }
                    PerformRefreshFiles.Invoke(currentFolderPath);
                }
            }
        }

        //download
        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = "";
            string destPath = "";
            string name = "";

            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.InitialDirectory = @"c:\";
                CommonFileDialogResult result = dialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                    destPath = dialog.FileName;
            }
            
            if (myFilesDataGrid.SelectedItem != null)
            {
                foreach (FilesInfo row in myFilesDataGrid.SelectedItems)
                {
                    if (row.Size == 0)
                    {
                        sourcePath = row.FileFullName;
                        name = row.FileName;
                        if (sourcePath != "")
                            PerformDownloadFolders.Invoke(sourcePath, name, destPath);
                    }
                    else
                    {
                        sourcePath = row.FileFullName;
                        name = row.FileName;
                        if (sourcePath != "")
                            PerformDownloadFiles.Invoke(sourcePath, name, destPath);
                    }
                }
            }
        }

        //send files
        private void sendFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                foreach (string sourcePath in dlg.FileNames)
                    PerformSendFiles.Invoke(sourcePath);
            }
            PerformRefreshFiles.Invoke(baseFolderPath);
            //homeButton_Click(sender, e);
        }

        //send folders
        private void sendFolderButton_Click(object sender, RoutedEventArgs e)
        {
            foldersList.Clear();
            filesList.Clear();
            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                foldersList.Add(fbd.SelectedPath);
                foreach (string dirPath in Directory.GetDirectories(fbd.SelectedPath, "*", SearchOption.AllDirectories))
                    foldersList.Add(dirPath);

                foreach (string filePath in Directory.GetFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories))
                    filesList.Add(filePath);

                PerformSendFolders.Invoke(foldersList, filesList);
            }
            PerformRefreshFiles.Invoke(currentFolderPath);
        }

        //keys
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Home)
                homeButton_Click(sender, e);
            if (e.Key == Key.Back)
                backButton_Click(sender, e);
        }
    }
}
