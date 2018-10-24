namespace NDayTrip_PC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GRO_0
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GRO_NO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string GRO_CODE { get; set; }

        [Required]
        [StringLength(40)]
        public string GRO_NAME { get; set; }

        [StringLength(4)]
        public string GRO_PARENT { get; set; }

        public int? GRO_OR { get; set; }

        public int? GRO_CNT { get; set; }
    }
}
