namespace NDayTrip_PC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class COU_2
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string COU_DATE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int COU_SEQ { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string COU_TITLE { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int COU_SUB_NUM { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EVE_SEQ { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(100)]
        public string EVE_REG_DATE { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(20)]
        public string EVE_NAME { get; set; }

        public int? EVE_CNT { get; set; }

        [StringLength(3)]
        public string EVE_OFFICE_CODE { get; set; }

        [StringLength(20)]
        public string EVE_TEL { get; set; }
    }
}
