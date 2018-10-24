namespace NDayTrip.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HTL_1
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int HTL_SEQ { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int HTL_PRC_SEQ { get; set; }

        [StringLength(3)]
        public string HTL_Currency_CODE { get; set; }

        public int? HTL_PRICE { get; set; }

        public int? HTL_PROFIT { get; set; }

        [StringLength(50)]
        public string HTL_ROOM { get; set; }

        [StringLength(50)]
        public string HTL_EXP { get; set; }

        public int? HTL_PRICE2 { get; set; }

        public int? HTL_PRICE3 { get; set; }

        public int? HTL_PRICE4 { get; set; }
    }
}
