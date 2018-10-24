using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCompany.Models
{
    [Serializable]
    public class User
    {
        public string Login { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public string PER_CODE { get; set; }

        public string POS_CODE { get; set; }
    }
}