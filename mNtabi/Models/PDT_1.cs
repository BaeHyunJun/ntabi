namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PDT_1
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string PDT_TYPE_CODE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PDT_IST_EMP_NO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(2)]
        public string PDT_YY { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PDT_SEQ { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TRF_SEQ { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PRC_SEQ { get; set; }

        [Required]
        [StringLength(2)]
        public string PDT_STATE_CODE { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(10)]
        public string PDT_DATE { get; set; }
    }
}
