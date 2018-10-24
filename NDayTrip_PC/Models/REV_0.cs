namespace NDayTrip_PC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class REV_0
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

        public int? REV_CNT { get; set; }

        [StringLength(2)]
        public string REV_STATE { get; set; }

        [StringLength(10)]
        public string REV_STARTDAY { get; set; }

        public int? IST_EMP_NO { get; set; }

        [StringLength(10)]
        public string IST_DATE { get; set; }

        public int? UDT_EMP_NO { get; set; }

        [StringLength(10)]
        public string UDT_DATE { get; set; }

        public int? CHG_EMP_NO { get; set; }

        public int? SEL_EMP_NO { get; set; }

        [StringLength(10)]
        public string SEL_DATE { get; set; }

        public int? ADULT_CNT { get; set; }

        public int? CHILD_CNT { get; set; }

        public int? BABY_CNT { get; set; }

        public int? REV_PRICE { get; set; }

        [StringLength(1)]
        public string REV_CHK_PRICE { get; set; }

        [StringLength(2)]
        public string PDT_TYPE_CODE { get; set; }

        public int? PDT_IST_EMP_NO { get; set; }

        [StringLength(2)]
        public string PDT_YY { get; set; }

        public int? PDT_SEQ { get; set; }

        [Column(TypeName = "text")]
        public string PDT_TITLE { get; set; }

        [StringLength(4)]
        public string PDT_DAYS_CODE { get; set; }

        [StringLength(4)]
        public string CU_YY { get; set; }

        public int? CU_SEQ { get; set; }

        [StringLength(20)]
        public string CU_NAME { get; set; }

        [Column(TypeName = "text")]
        public string REV_CONTENT { get; set; }

        [StringLength(1)]
        public string CHK_VOU { get; set; }
    }
}
