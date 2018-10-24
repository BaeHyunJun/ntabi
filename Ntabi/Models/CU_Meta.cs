namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CU_Meta
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int meta_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string CU_YY { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CU_SEQ { get; set; }

        [StringLength(50)]
        public string meta_key { get; set; }

        [StringLength(500)]
        public string meta_value { get; set; }
    }
}
