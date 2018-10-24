namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PDT_0
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
        public short PDT_SEQ { get; set; }

        public int PDT_EMP_NO { get; set; }

        [Required]
        [StringLength(2)]
        public string PDT_NATION_CODE { get; set; }

        [Required]
        [StringLength(3)]
        public string PDT_AREA_CODE { get; set; }

        [Required]
        [StringLength(100)]
        public string PDT_TITLE { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string PDT_CONTENT { get; set; }

        [Required]
        [StringLength(3)]
        public string PDT_SCITY_CODE { get; set; }

        [Required]
        [StringLength(2)]
        public string PDT_PROC_CODE { get; set; }

        public short PDT_ORDER_NO { get; set; }

        [Required]
        [StringLength(4)]
        public string PDT_DAYS_CODE { get; set; }

        [StringLength(4)]
        public string PDT_OPTIONS { get; set; }

        [Required]
        [StringLength(10)]
        public string PDT_IST_DATE { get; set; }

        [Required]
        [StringLength(10)]
        public string PDT_UDT_DATE { get; set; }

        public int PDT_UDT_EMP_NO { get; set; }

        [StringLength(1)]
        public string PDT_Option1 { get; set; }

        [StringLength(1)]
        public string PDT_Option2 { get; set; }

        [StringLength(1)]
        public string PDT_Option3 { get; set; }

        [StringLength(1)]
        public string PDT_Option4 { get; set; }

        [StringLength(1)]
        public string PDT_Option5 { get; set; }

        [StringLength(1)]
        public string PDT_Option6 { get; set; }

        [StringLength(100)]
        public string PDT_TOUR_CITY { get; set; }

        public int? PDT_MIN_COUNT { get; set; }

        [StringLength(1)]
        public string PDT_COMBINE_HTL { get; set; }

        [StringLength(1)]
        public string PDT_HTL_FLAG { get; set; }
    }
}
