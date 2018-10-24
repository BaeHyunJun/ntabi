namespace NDayTrip.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ACC_0
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(8)]
        public string ACC_DAY { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ACC_SEQ { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ACC_SUB_SEQ { get; set; }

        [StringLength(3)]
        public string ACC_BANK_CODE { get; set; }

        [StringLength(50)]
        public string ACC_NUMBER { get; set; }

        public int? ACC_PRICE { get; set; }

        [StringLength(50)]
        public string ACC_NAME { get; set; }

        public int? ACC_EMP_NO { get; set; }

        [StringLength(8)]
        public string ACC_IST_DAY { get; set; }

        public int? ACC_IST_EMP_NO { get; set; }

        public int? ACC_FIRST_PRICE { get; set; }

        [StringLength(100)]
        public string ACC_CONTENT { get; set; }
    }
}
