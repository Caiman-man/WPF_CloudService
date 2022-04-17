﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Ivanov_NP_Cloud_Service
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [FaultContract(typeof(string))]
        bool CheckRegistrarion(string login, string password);

        [OperationContract]
        void RegisterNewUser(string login, string password);

        [OperationContract]
        [FaultContract(typeof(string))]
        string GetBaseFolderPath(string login, string password);

        [OperationContract]
        [FaultContract(typeof(string))]
        List<FilesInfo> ShowFilesFromBaseDirectory(string login, string password);

        [OperationContract]
        [FaultContract(typeof(string))]
        List<FilesInfo> RefreshFiles(string path);

        [OperationContract]
        [FaultContract(typeof(string))]
        List<FilesInfo> SearchFiles(string path, string name);

        [OperationContract]
        void CreateFolder(string path);

        [OperationContract]
        [FaultContract(typeof(string))]
        void DeleteFile(string path);

        [OperationContract]
        byte[] GetArray(string path);

        [OperationContract]
        List<string> GetFolders(string sourcePath);

        [OperationContract]
        List<string> GetFilesInsideFolders(string sourcePath);

        [OperationContract]
        [FaultContract(typeof(string))]
        void UploadFiles(byte[] arr, string destPath, string fileName);

        [OperationContract]
        void UploadFolders(string[] foldersList, string[] filesList, string destPath);

        [OperationContract]
        void UploadFilesInsideFolders(byte[] arr, string destPath, string filePath, int index);
    }
}
