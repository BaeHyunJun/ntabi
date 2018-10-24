namespace NCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BANK")]
    public partial class BANK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BANK_SEQ { get; set; }

        [StringLength(3)]
        public string BANK_CODE { get; set; }

        [StringLength(50)]
        public string BANK_NAME { get; set; }
    }
}
