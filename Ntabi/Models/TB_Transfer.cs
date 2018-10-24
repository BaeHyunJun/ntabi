namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_Transfer
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string ComCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(5)]
        public string TransCode { get; set; }

        [StringLength(10)]
        public string BSPCode { get; set; }

        public int? SortNo { get; set; }

        [StringLength(30)]
        public string TransName_Kor { get; set; }

        [StringLength(30)]
        public string TransName_Eng { get; set; }

        [StringLength(10)]
        public string Property { get; set; }

        [StringLength(1)]
        public string UseFlag { get; set; }
    }
}
