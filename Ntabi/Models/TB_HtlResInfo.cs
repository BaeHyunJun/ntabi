namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_HtlResInfo
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
        [StringLength(10)]
        public string HTL_SEQ { get; set; }

        [Required]
        [StringLength(2)]
        public string HTL_NATION_CODE { get; set; }

        [Required]
        [StringLength(3)]
        public string HTL_AREA_CODE { get; set; }

        [StringLength(2)]
        public string HTL_TYPE { get; set; }

        [StringLength(10)]
        public string HTL_ChkIn { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Stay_Seq { get; set; }

        public int? HTL_StayDays { get; set; }

        [StringLength(4)]
        public string CU_YY { get; set; }

        public int? CU_SEQ { get; set; }

        [StringLength(20)]
        public string CU_NAME { get; set; }

        [StringLength(20)]
        public string CU_NAME_FIRST { get; set; }

        [StringLength(20)]
        public string CU_NAME_LAST { get; set; }

        [Required]
        [StringLength(20)]
        public string CU_HP { get; set; }

        [StringLength(50)]
        public string CU_EMAIL { get; set; }

        public int? ADULT_CNT { get; set; }

        public int? CHILD_CNT { get; set; }

        public int? BABY_CNT { get; set; }

        [StringLength(50)]
        public string HTL_RoomType { get; set; }

        [StringLength(500)]
        public string Htl_ResComment { get; set; }
    }
}
