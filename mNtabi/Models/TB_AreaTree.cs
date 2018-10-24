namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_AreaTree
    {
        [Key]
        [StringLength(5)]
        public string City { get; set; }

        [StringLength(5)]
        public string Area { get; set; }

        [StringLength(5)]
        public string Nation { get; set; }

        [StringLength(1)]
        public string UseFlag { get; set; }

        public DateTime? InsDate { get; set; }

        public DateTime? UpdDate { get; set; }

        public DateTime? DelDate { get; set; }

        [StringLength(20)]
        public string InsUser { get; set; }

        [StringLength(20)]
        public string UpdUser { get; set; }

        [StringLength(20)]
        public string DelUser { get; set; }
    }
}
