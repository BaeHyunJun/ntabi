namespace NDayTrip.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BNK_0
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BNK_SEQ { get; set; }

        [StringLength(3)]
        public string BNK_CODE { get; set; }

        [StringLength(50)]
        public string BNK_NUMBER { get; set; }
    }
}
