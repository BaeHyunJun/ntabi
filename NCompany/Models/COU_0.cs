namespace NCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class COU_0
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string COU_AREA_CODE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int COU_SEQ { get; set; }

        [Required]
        [StringLength(100)]
        public string COU_TITLE { get; set; }

        [Required]
        [StringLength(20)]
        public string COU_LATITUDE { get; set; }

        [Required]
        [StringLength(20)]
        public string COU_LONGITUDE { get; set; }

        [Required]
        [StringLength(50)]
        public string COU_ADDRESS { get; set; }

        [Column(TypeName = "text")]
        public string COU_CONT { get; set; }

        [Column(TypeName = "text")]
        public string COU_attachment { get; set; }
    }
}
