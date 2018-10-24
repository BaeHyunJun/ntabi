namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_Guide
    {
        [Key]
        [StringLength(10)]
        public string GuideNo { get; set; }

        [StringLength(20)]
        public string GuidePw { get; set; }

        [StringLength(50)]
        public string GuideName { get; set; }

        [StringLength(1)]
        public string UseFlag { get; set; }

        public DateTime? InsDate { get; set; }

        [StringLength(30)]
        public string InsId { get; set; }

        public DateTime? UpdDate { get; set; }

        [StringLength(30)]
        public string UpdId { get; set; }

        public DateTime? DelDate { get; set; }

        [StringLength(30)]
        public string DelId { get; set; }

        [StringLength(200)]
        public string Desciption { get; set; }

        [StringLength(20)]
        public string GuideTel { get; set; }

        [StringLength(30)]
        public string GuideMsg { get; set; }
    }
}
