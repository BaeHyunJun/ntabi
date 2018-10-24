namespace NCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HTL_2
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

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string HTL_DATE { get; set; }

        [Required]
        [StringLength(2)]
        public string HTL_STATE_CODE { get; set; }
    }
}
