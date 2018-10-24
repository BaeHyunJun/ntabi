using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCompany.Models
{
    [Serializable]
    public class review
    {
        public long post_ID { get; set; }

        public string CU_YY { get; set; }

        public int? CU_SEQ { get; set; }

        public string name { get; set; }

        public string post_content { get; set; }

        public long? post_parent { get; set; }

        public string ist_date { get; set; }
    }
}