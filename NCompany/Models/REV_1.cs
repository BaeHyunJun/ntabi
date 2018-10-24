namespace NCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class REV_1
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(8)]
        public string REV_DAY { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int REV_SEQ { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int REV_PRC_SEQ { get; set; }

        public int? REV_PRC_PRICE { get; set; }

        public int? REV_PRC_DISCOUNT { get; set; }

        public int? REV_PRC_ADDITIONAL_PRICE { get; set; }

        public int? REV_PRC_CNT { get; set; }

        public int? REV_PRC_TOTAL_PRICE { get; set; }

        [StringLength(50)]
        public string REV_PRC_CONTENT { get; set; }

        [StringLength(1)]
        public string REV_PRC_GB { get; set; }

        [StringLength(2)]
        public string PDT_TYPE_CODE { get; set; }

        public int? PDT_IST_EMP_NO { get; set; }

        [StringLength(2)]
        public string PDT_YY { get; set; }

        public int? PDT_SEQ { get; set; }

        public int? PRC_SEQ { get; set; }

        [StringLength(50)]
        public string PRC_CONTENT { get; set; }

        [StringLength(3)]
        public string PRC_CURRENCY_CODE { get; set; }
    }
}
