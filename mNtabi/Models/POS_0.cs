namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class POS_0
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int POS_NO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string POS_CODE { get; set; }

        [Required]
        [StringLength(20)]
        public string POS_NAME { get; set; }

        public int? POS_OR { get; set; }
    }
}
