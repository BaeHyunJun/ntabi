namespace NDayTrip.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class COU_1
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int COU_SEQ { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string COU_DATE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(3)]
        public string COU_AREA_CODE { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string COU_TITLE { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int COU_SUB_NUM { get; set; }

        public int? COU_CAR_SEQ { get; set; }

        public int? COU_EMP_NO { get; set; }

        [StringLength(50)]
        public string COU_DRIVER { get; set; }

        [Column(TypeName = "text")]
        public string COU_NOTE { get; set; }

        public int? COU_CNT { get; set; }

        public int? COU_DRIVER_NUM { get; set; }

        [StringLength(3)]
        public string CAR_TYPE_CODE { get; set; }
    }
}
