namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EVE_0
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string EVE_REG_DATE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EVE_SEQ { get; set; }

        [StringLength(20)]
        public string EVE_NAME { get; set; }

        [StringLength(20)]
        public string EVE_TEL { get; set; }

        [StringLength(50)]
        public string EVE_EMAIL { get; set; }

        [StringLength(3)]
        public string EVE_OFFICE_CODE { get; set; }

        public int? EVE_CNT { get; set; }

        [Column(TypeName = "text")]
        public string EVE_ETC { get; set; }

        [StringLength(3)]
        public string EVE_AREA_CODE { get; set; }

        [StringLength(100)]
        public string EVE_TITLE { get; set; }

        public int? EVE_EMP_NO { get; set; }

        [StringLength(10)]
        public string EVE_DATE { get; set; }

        [StringLength(50)]
        public string OFFICE_SEQ { get; set; }

        [StringLength(1)]
        public string EVE_SET { get; set; }

        public int? EVE_PRICE { get; set; }

        [StringLength(1)]
        public string EVE_CHKMAIL { get; set; }

        [StringLength(10)]
        public string EVE_IST_DATE { get; set; }

        [StringLength(10)]
        public string EVE_UDT_DATE { get; set; }

        public int? EVE_IST_EMP_NO { get; set; }

        public int? EVE_UDT_EMP_NO { get; set; }
    }
}
