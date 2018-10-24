namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DETAIL")]
    public partial class DETAIL
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(4)]
        public string DET_TYPE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string DET_SEQ { get; set; }

        [StringLength(20)]
        public string DET_NAME { get; set; }
    }
}
