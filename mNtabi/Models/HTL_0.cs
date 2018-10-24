namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HTL_0
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int HTL_SEQ { get; set; }

        [Required]
        [StringLength(2)]
        public string HTL_NATION_CODE { get; set; }

        [Required]
        [StringLength(3)]
        public string HTL_AREA_CODE { get; set; }

        [Required]
        [StringLength(100)]
        public string HTL_NAME { get; set; }

        [Required]
        [StringLength(2)]
        public string HTL_PROC_CODE { get; set; }

        [Required]
        [StringLength(10)]
        public string HTL_IST_DATE { get; set; }

        public int HTL_IST_EMP_NO { get; set; }

        [Required]
        [StringLength(10)]
        public string HTL_UDT_DATE { get; set; }

        public int HTL_UDT_EMP_NO { get; set; }

        [StringLength(1)]
        public string HTL_CHK_PRICE { get; set; }

        [StringLength(1)]
        public string HTL_CHK_DAYS { get; set; }

        [StringLength(1)]
        public string HTL_CHK_CONTENT { get; set; }

        [StringLength(2)]
        public string HTL_TYPE { get; set; }
    }
}
