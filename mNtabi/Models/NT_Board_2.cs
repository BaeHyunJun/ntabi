namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NT_Board_2
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CORP_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long post_ID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string post_type { get; set; }

        [StringLength(4)]
        public string CU_YY { get; set; }

        public int? CU_SEQ { get; set; }

        [StringLength(2)]
        public string PDT_TYPE_CODE { get; set; }

        public int? PDT_IST_EMP_NO { get; set; }

        [StringLength(2)]
        public string PDT_YY { get; set; }

        public int? PDT_SEQ { get; set; }

        [Column(TypeName = "ntext")]
        public string post_title { get; set; }

        [Column(TypeName = "ntext")]
        public string post_content { get; set; }

        [Column(TypeName = "ntext")]
        public string post_mContent { get; set; }

        [Column(TypeName = "ntext")]
        public string post_category { get; set; }

        [StringLength(1)]
        public string post_status { get; set; }

        [StringLength(8)]
        public string ist_date { get; set; }

        [StringLength(8)]
        public string udt_date { get; set; }

        public long? post_parent { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(50)]
        public string password { get; set; }

        [StringLength(1)]
        public string DEL_FG { get; set; }

        [StringLength(8)]
        public string ist_time { get; set; }

        [StringLength(8)]
        public string udt_time { get; set; }

        [StringLength(20)]
        public string post_options { get; set; }

        [StringLength(200)]
        public string post_thumb { get; set; }

        [StringLength(3)]
        public string DptCode { get; set; }

        [StringLength(3)]
        public string RtnCode { get; set; }

        [StringLength(3)]
        public string TripNation { get; set; }

        [StringLength(3)]
        public string TripArea { get; set; }

        public int? AdultCount { get; set; }

        public int? ChildCount { get; set; }

        public int? BabyCount { get; set; }

        [StringLength(10)]
        public string DptDate { get; set; }

        public int? TripPeriod { get; set; }

        [StringLength(50)]
        public string Keyword { get; set; }

        [StringLength(50)]
        public string ForTrip { get; set; }

        [StringLength(3)]
        public string TransferCode { get; set; }

        public int? TripBudget { get; set; }

        [StringLength(1)]
        public string ReqQuotation { get; set; }

        [StringLength(20)]
        public string TempCol01 { get; set; }

        [StringLength(20)]
        public string TempCol02 { get; set; }
    }
}
