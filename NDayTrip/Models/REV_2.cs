namespace NDayTrip.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class REV_2
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
        public int REV_CU_SEQ { get; set; }

        [StringLength(4)]
        public string CU_YY { get; set; }

        public int? CU_SEQ { get; set; }

        [StringLength(20)]
        public string CU_NAME { get; set; }

        [StringLength(20)]
        public string CU_NAME_FIRST { get; set; }

        [StringLength(20)]
        public string CU_NAME_LAST { get; set; }

        [StringLength(1)]
        public string CU_SEX { get; set; }

        [StringLength(1)]
        public string CU_GB { get; set; }

        [StringLength(20)]
        public string CU_TEL { get; set; }

        [StringLength(100)]
        public string REV_CONTENT { get; set; }
    }
}
