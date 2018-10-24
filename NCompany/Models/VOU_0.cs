namespace NCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VOU_0
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(8)]
        public string REV_DAY { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int REV_SEQ { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VOU_SEQ { get; set; }

        [Column(TypeName = "text")]
        public string VOU_CONT1 { get; set; }

        [Column(TypeName = "text")]
        public string VOU_CONT2 { get; set; }

        [Column(TypeName = "text")]
        public string VOU_CONT3 { get; set; }

        [Column(TypeName = "text")]
        public string VOU_CONT4 { get; set; }

        [StringLength(20)]
        public string VOU_LATITUDE { get; set; }

        [StringLength(20)]
        public string VOU_LONGITUDE { get; set; }

        [StringLength(100)]
        public string VOU_PHOTO1 { get; set; }

        [StringLength(100)]
        public string VOU_PHOTO2 { get; set; }

        [StringLength(100)]
        public string VOU_PHOTO3 { get; set; }

        [Column(TypeName = "text")]
        public string VOU_CONT5 { get; set; }
    }
}
