using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mNtabi.Models
{
    [Serializable]
    public class User
    {
        public string Login { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }
    }
}