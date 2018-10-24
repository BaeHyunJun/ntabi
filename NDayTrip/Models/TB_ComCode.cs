namespace NDayTrip.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_ComCode
    {
        [Key]
        [StringLength(5)]
        public string CatCode { get; set; }

        [StringLength(10)]
        public string ComCode { get; set; }

        [StringLength(50)]
        public string Code_Kor { get; set; }

        [StringLength(50)]
        public string Code_Eng { get; set; }

        [StringLength(1)]
        public string UseFlag { get; set; }

        public short? SortNo { get; set; }

        [StringLength(200)]
        public string Code_Desc { get; set; }

        public DateTime? InsDate { get; set; }

        public DateTime? UpdDate { get; set; }

        public DateTime? DelDate { get; set; }

        [StringLength(20)]
        public string InsUser { get; set; }

        [StringLength(20)]
        public string UpdUser { get; set; }

        [StringLength(20)]
        public string DelUser { get; set; }

        [StringLength(2)]
        public string AlterCode { get; set; }

        [StringLength(3)]
        public string Limit { get; set; }
    }
}
