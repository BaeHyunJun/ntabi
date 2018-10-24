namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_TelPack
    {
        [Key]
        public int Seq { get; set; }

        [Required]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Required]
        [StringLength(2)]
        public string PDT_TYPE_CODE { get; set; }

        public int PDT_IST_EMP_NO { get; set; }

        [Required]
        [StringLength(2)]
        public string PDT_YY { get; set; }

        public short PDT_SEQ { get; set; }

        public int HTL_SEQ { get; set; }

        [StringLength(2)]
        public string HTL_TYPE { get; set; }

        public short? SortNo { get; set; }

        [StringLength(1)]
        public string UseFlag { get; set; }

        [StringLength(50)]
        public string Temp { get; set; }

        public DateTime? InsDate { get; set; }

        [StringLength(20)]
        public string InsUser { get; set; }

        public DateTime? UpdDate { get; set; }

        [StringLength(20)]
        public string UpdUser { get; set; }

        public DateTime? DelDate { get; set; }

        [StringLength(20)]
        public string DelUser { get; set; }
    }
}
