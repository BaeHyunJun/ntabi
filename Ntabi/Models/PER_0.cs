namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PER_0
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PER_NO { get; set; }

        [StringLength(4)]
        public string PER_CODE { get; set; }

        [StringLength(20)]
        public string PER_NAME { get; set; }
    }
}
