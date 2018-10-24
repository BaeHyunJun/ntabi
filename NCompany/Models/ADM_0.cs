namespace NCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ADM_0
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CORP_NO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string CORP_NAME { get; set; }

        [StringLength(4)]
        public string CORP_TEL1 { get; set; }

        [StringLength(4)]
        public string CORP_TEL2 { get; set; }

        [StringLength(4)]
        public string CORP_TEL3 { get; set; }

        [StringLength(50)]
        public string CORP_ADDRESS { get; set; }

        [StringLength(20)]
        public string CORP_REG_NO { get; set; }

        [StringLength(10)]
        public string CORP_CEO_NAME { get; set; }

        [StringLength(20)]
        public string CORP_BUSI_CDTS { get; set; }

        [StringLength(20)]
        public string CORP_BUSI_TYPE { get; set; }

        [StringLength(4)]
        public string CORP_FAX1 { get; set; }

        [StringLength(4)]
        public string CORP_FAX2 { get; set; }

        [StringLength(4)]
        public string CORP_FAX3 { get; set; }
    }
}
