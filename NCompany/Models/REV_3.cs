namespace NCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class REV_3
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
        public int REV_ACC_SEQ { get; set; }

        [StringLength(8)]
        public string REV_ACC_DATE { get; set; }

        [StringLength(8)]
        public string REV_ACC_REG_DATE { get; set; }

        public int? REV_ACC_IST_EMP_NO { get; set; }

        [StringLength(4)]
        public string REV_ACC_GB_CODE { get; set; }

        [StringLength(20)]
        public string REV_ACC_DET_SEQ { get; set; }

        public int? REV_ACC_PRICE { get; set; }

        [StringLength(20)]
        public string REV_ACC_NAME { get; set; }

        [StringLength(100)]
        public string REV_ACC_CONTENT { get; set; }

        [StringLength(8)]
        public string ACC_DAY { get; set; }

        public int? ACC_SEQ { get; set; }

        public int? ACC_SUB_SEQ { get; set; }
    }
}
