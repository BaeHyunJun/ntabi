namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CHAT_SaveText
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EMP_NO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CHAT_NO { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string CHAT_TEXT { get; set; }

        [Column(TypeName = "date")]
        public DateTime CHAT_IST_DATE { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CHAT_UDT_DATE { get; set; }

        [Required]
        [StringLength(1)]
        public string CHAT_DEL_FG { get; set; }

        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [StringLength(2)]
        public string PDT_TYPE_CODE { get; set; }

        public int? PDT_IST_EMP_NO { get; set; }

        [StringLength(2)]
        public string PDT_YY { get; set; }

        public short? PDT_SEQ { get; set; }
    }
}
