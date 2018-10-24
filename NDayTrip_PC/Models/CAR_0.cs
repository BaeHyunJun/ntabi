namespace NDayTrip_PC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CAR_0
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CAR_SEQ { get; set; }

        [Required]
        [StringLength(50)]
        public string CAR_REG_NUM { get; set; }

        [StringLength(50)]
        public string CAR_NAME { get; set; }

        [StringLength(3)]
        public string CAR_TYPE_CODE { get; set; }

        [StringLength(3)]
        public string CAR_AREA_CODE { get; set; }

        [Column(TypeName = "text")]
        public string CAR_NOTE { get; set; }

        [StringLength(50)]
        public string CAR_RENT_CORP { get; set; }

        [StringLength(100)]
        public string CAR_FEATURE { get; set; }
    }
}
