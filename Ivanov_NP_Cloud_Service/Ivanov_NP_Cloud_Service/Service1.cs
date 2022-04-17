using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Drawing;

namespace Ivanov_NP_Cloud_Service
{
    public class Service1 : IService1
    {
        CloudUsersEntities1 context = new CloudUsersEntities1();
        List<FilesInfo> currentFileList = new List<FilesInfo>();
        List<string> foldersList = new List<string>();
        List<string> filesList = new List<string>();

        //check registrarion
        public bool CheckRegistrarion(string login, string password)
        {
            bool flag = false;

            var result = from t in context.Users
                         select new UserInfo
                         {
                             Login = t.login,
                             Password = t.password,
                             FolderName = t.folder
                         };

            if (result == null)
                throw new FaultException("Can't get data from database!");

            foreach (var user in result)
            {
                if (user.Login == login && user.Password == password)
                {
                    flag = true;
                    break;
                }
                else
                    flag = false;
            }
            return flag;
        }


        //register new user
        public void RegisterNewUser(string login, string password)
        {
            //добавляем user'a в базу данных
            Users newUser = new Users
            {
                login = login,
                password = password,
                folder = $@"C:\\IvanovCloud\\{login}"
            };
            context.Users.Add(newUser);
            context.SaveChanges();

            //создаём для user'а папку
            DirectoryInfo newDirectory = new DirectoryInfo($@"C:\IvanovCloud\{login}");
            if (!newDirectory.Exists)
                newDirectory.Create();
        }


        //получить имя корневой папки из БД
        public string GetBaseFolderPath(string login, string password)
        {
            string baseFolder = "";
            var result = from t in context.Users
                         select new UserInfo
                         {
                             Login = t.login,
                             Password = t.password,
                             FolderName = t.folder
                         };

            if (result == null) throw new FaultException("Can't get data from database!");

            foreach (var user in result)
            {
                if (user.Login == login && user.Password == password)
                {
                    baseFolder = user.FolderName;
                    break;
                }
            }
            return baseFolder;
        }


        //show files from base directory
        public List<FilesInfo> ShowFilesFromBaseDirectory(string login, string password)
        {
            currentFileList.Clear();
            ShowFiles(GetBaseFolderPath(login, password));
            if (currentFileList == null) throw new FaultException("Can't get the list of files from server");
            return currentFileList;
        }

        public void ShowFiles(string path)
        {
            DirectoryInfo dinfo = new DirectoryInfo(path);

            if (dinfo.Exists)
            {
                try
                {
                    //получить массив подпапок в текущей папке
                    DirectoryInfo[] dirs = dinfo.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        FilesInfo info = new FilesInfo();
                        info.FileIcon = "Images/folder.png";
                        info.FileFullName = current.FullName;
                        info.FileName = current.Name;
                        info.LastAccessTime = current.LastAccessTime.ToString();
                        info.Size = 0;
                        currentFileList.Add(info);
                    }

                    //получить массив файлов в текущей папке
                    FileInfo[] files = dinfo.GetFiles("*.*");
                    foreach (FileInfo current in files)
                    {
                        FilesInfo info = new FilesInfo();
                        switch (current.Extension)
                        {
                            case ".avi": info.FileIcon = "Images/avi.png"; break;
                            case ".css": info.FileIcon = "Images/css.png"; break;
                            case ".dbf": info.FileIcon = "Images/dbf.png"; break;
                            case ".doc": info.FileIcon = "Images/doc.png"; break;
                            case ".exe": info.FileIcon = "Images/exe.png"; break;
                            case ".html": info.FileIcon = "Images/html.png"; break;
                            case ".iso": info.FileIcon = "Images/iso.png"; break;
                            case ".js": info.FileIcon = "Images/js.png"; break;
                            case ".json": info.FileIcon = "Images/json.png"; break;
                            case ".mp3": info.FileIcon = "Images/mp3.png"; break;
                            case ".mp4": info.FileIcon = "Images/mp4.png"; break;
                            case ".pdf": info.FileIcon = "Images/pdf.png"; break;
                            case ".txt": info.FileIcon = "Images/txt.png"; break;
                            case ".xls": info.FileIcon = "Images/xls.png"; break;
                            case ".xlsx": info.FileIcon = "Images/xls.png"; break;
                            case ".xlsm": info.FileIcon = "Images/xls.png"; break;
                            case ".xml": info.FileIcon = "Images/xml.png"; break;
                            case ".zip": info.FileIcon = "Images/zip.png"; break;
                            case ".jpg": info.FileIcon = "Images/image.png"; break;
                            case ".jpeg": info.FileIcon = "Images/image.png"; break;
                            case ".png": info.FileIcon = "Images/image.png"; break;
                            case ".bmp": info.FileIcon = "Images/image.png"; break;
                            case ".ico": info.FileIcon = "Images/image.png"; break;
                            default: info.FileIcon = "Images/file.png"; break;
                        }
                        info.FileFullName = current.FullName;
                        info.FileName = current.Name;
                        info.LastAccessTime = current.LastAccessTime.ToString();
                        info.Size = current.Length;
                        currentFileList.Add(info);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
                throw new FaultException("Path is not exist!");
        }

        //refresh 
        public List<FilesInfo> RefreshFiles(string path)
        {
            currentFileList.Clear();
            ShowFiles(path);
            if (currentFileList == null) throw new FaultException("Can't get the list of files from server");
            return currentFileList;
        }


        //search
        public List<FilesInfo> SearchFiles(string path, string name)
        {
            currentFileList.Clear();
            Search(path, name);
            if (currentFileList == null) throw new FaultException("Can't get the list of files from server");
            return currentFileList;
        }

        public void Search(string path, string name)
        {
            DirectoryInfo dinfo = new DirectoryInfo(path);

            if (dinfo.Exists)
            {
                try
                {
                    //получить массив подпапок в текущей папке
                    DirectoryInfo[] dirs = dinfo.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        if (current.Name.Contains(name))
                        {
                            FilesInfo info = new FilesInfo();
                            info.FileIcon = "Images/folder.png";
                            info.FileFullName = current.FullName;
                            info.FileName = current.Name;
                            info.LastAccessTime = current.LastAccessTime.ToString();
                            info.Size = 0;
                            currentFileList.Add(info);
                        }
                        Search(path + @"\" + current.Name, name);
                    }

                    //получить массив файлов в текущей папке
                    FileInfo[] files = dinfo.GetFiles("*.*");
                    foreach (FileInfo current in files)
                    {
                        if (current.Name.Contains(name))
                        {
                            FilesInfo info = new FilesInfo();
                            switch (current.Extension)
                            {
                                case ".avi": info.FileIcon = "Images/avi.png"; break;
                                case ".css": info.FileIcon = "Images/css.png"; break;
                                case ".dbf": info.FileIcon = "Images/dbf.png"; break;
                                case ".doc": info.FileIcon = "Images/doc.png"; break;
                                case ".exe": info.FileIcon = "Images/exe.png"; break;
                                case ".html": info.FileIcon = "Images/html.png"; break;
                                case ".iso": info.FileIcon = "Images/iso.png"; break;
                                case ".js": info.FileIcon = "Images/js.png"; break;
                                case ".json": info.FileIcon = "Images/json.png"; break;
                                case ".mp3": info.FileIcon = "Images/mp3.png"; break;
                                case ".mp4": info.FileIcon = "Images/mp4.png"; break;
                                case ".pdf": info.FileIcon = "Images/pdf.png"; break;
                                case ".txt": info.FileIcon = "Images/txt.png"; break;
                                case ".xls": info.FileIcon = "Images/xls.png"; break;
                                case ".xlsx": info.FileIcon = "Images/xls.png"; break;
                                case ".xlsm": info.FileIcon = "Images/xls.png"; break;
                                case ".xml": info.FileIcon = "Images/xml.png"; break;
                                case ".zip": info.FileIcon = "Images/zip.png"; break;
                                case ".jpg": info.FileIcon = "Images/image.png"; break;
                                case ".jpeg": info.FileIcon = "Images/image.png"; break;
                                case ".png": info.FileIcon = "Images/image.png"; break;
                                case ".bmp": info.FileIcon = "Images/image.png"; break;
                                case ".ico": info.FileIcon = "Images/image.png"; break;
                                default: info.FileIcon = "Images/file.png"; break;
                            }
                            info.FileFullName = current.FullName;
                            info.FileName = current.Name;
                            info.LastAccessTime = current.LastAccessTime.ToString();
                            info.Size = current.Length;
                            currentFileList.Add(info);
                        }
                    } 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
                throw new FaultException("Path is not exist!");
        }

        //create folder
        public void CreateFolder(string path)
        {
            string name = @"\New folder";
            string current = name;
            int i = 0;
            while (Directory.Exists(path + current))
            {
                i++;
                current = String.Format("{0} {1}", name, i);
            }
            Directory.CreateDirectory(path + current);
        }

        //delete file
        public void DeleteFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                Directory.Delete(path, true);

            if (File.Exists(path))
                File.Delete(path);
        }

        //download file
        public byte[] GetArray(string path)
        {
            FileStream st = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] arr = new byte[st.Length];
            st.Read(arr, 0, (int)st.Length);
            st.Close();
            return arr;
        }

        //download folders
        public List<string> GetFolders(string sourcePath)
        {
            foldersList.Clear();
            FileAttributes attr = File.GetAttributes(sourcePath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                    foldersList.Add(dirPath);

                if (foldersList.Count == 0)
                    foldersList.Add(sourcePath);
            }
            return foldersList;
        }

        //download files inside folders
        public List<string> GetFilesInsideFolders(string sourcePath)
        {
            filesList.Clear();
            FileAttributes attr = File.GetAttributes(sourcePath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                    filesList.Add(filePath);
            }
            return filesList;
        }

        //upload files
        public void UploadFiles(byte[] arr, string destPath, string fileName)
        {
            File.WriteAllBytes(destPath + @"\" + fileName, arr);
        }

        //upload folders
        public void UploadFolders(string[] foldersList, string[] filesList, string destPath)
        {
            string lastName = Path.GetFileName(foldersList[0]);
            int lastLength = lastName.Length;
            int baseLength = foldersList[0].Length - lastLength;
            foreach (string folderPath in foldersList)
            {
                Directory.CreateDirectory(destPath + @"\" + folderPath.Remove(0, baseLength));
            }
        }

        //upload files inside folders
        public void UploadFilesInsideFolders(byte[] arr, string destPath, string filePath, int index)
        {
            File.WriteAllBytes(destPath + @"\" + filePath.Remove(0, index), arr);
        }
    }
}
