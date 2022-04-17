using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ivanov_NP_Cloud_Service
{
    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string FolderName { get; set; }
    }
}
