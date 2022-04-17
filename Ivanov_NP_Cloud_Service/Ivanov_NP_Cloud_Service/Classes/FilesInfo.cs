using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ivanov_NP_Cloud_Service
{
    [DataContract]
    public class FilesInfo
    {
        [DataMember]
        public string FileIcon { get; set; }
        [DataMember]
        public string FileFullName { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string LastAccessTime { get; set; }
        [DataMember]
        public long Size { get; set; }
    }
}
