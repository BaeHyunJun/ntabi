namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TRF_0
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

        [Required]
        [StringLength(50)]
        public string TRF_TITLE { get; set; }

        [Required]
        [StringLength(3)]
        public string TRF_TYPE { get; set; }

        [StringLength(4)]
        public string TRF_CO_CODE { get; set; }

        [Required]
        [StringLength(5)]
        public string TRF_STIME { get; set; }

        [Required]
        [StringLength(50)]
        public string TRF_SAREA { get; set; }

        [Required]
        [StringLength(5)]
        public string TRF_ETIME { get; set; }

        [Required]
        [StringLength(50)]
        public string TRF_EAREA { get; set; }

        [StringLength(100)]
        public string TRF_CONTENT { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TRF_SUB_SEQ { get; set; }
    }
}
