namespace NDayTrip_PC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EMP_0
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EMP_NO { get; set; }

        [Required]
        [StringLength(20)]
        public string EMP_NAME { get; set; }

        [StringLength(20)]
        public string EMP_ID { get; set; }

        [StringLength(20)]
        public string EMP_PWD { get; set; }

        [Required]
        [StringLength(50)]
        public string EMP_EMAIL { get; set; }

        [Required]
        [StringLength(4)]
        public string EMP_TEL1 { get; set; }

        [Required]
        [StringLength(4)]
        public string EMP_TEL2 { get; set; }

        [Required]
        [StringLength(4)]
        public string EMP_TEL3 { get; set; }

        [Required]
        [StringLength(4)]
        public string GRO_CODE { get; set; }

        [Required]
        [StringLength(4)]
        public string POS_CODE { get; set; }

        [StringLength(8)]
        public string EMP_BIRTH { get; set; }

        [Column(TypeName = "date")]
        public DateTime EMP_JOIN { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EMP_LEAVE { get; set; }

        [StringLength(100)]
        public string EMP_INTRO { get; set; }

        [StringLength(4)]
        public string PER_CODE { get; set; }

        [StringLength(2)]
        public string CORP_CODE { get; set; }

        [StringLength(4)]
        public string EMP_PHONE1 { get; set; }

        [StringLength(4)]
        public string EMP_PHONE2 { get; set; }

        [StringLength(4)]
        public string EMP_PHONE3 { get; set; }
    }
}
