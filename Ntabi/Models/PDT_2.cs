namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PDT_2
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string PDT_TYPE_CODE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PDT_IST_EMP_NO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(2)]
        public string PDT_YY { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PDT_SEQ { get; set; }

        [StringLength(100)]
        public string PDT_IMG { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH0 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH1 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH2 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH3 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH4 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH5 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH6 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH7 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH8 { get; set; }

        [Column(TypeName = "text")]
        public string PDT_SCH9 { get; set; }
    }
}
