namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CHAT_Message
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CHAT_Message_ID { get; set; }

        [Column("CHAT_Message", TypeName = "text")]
        [Required]
        public string CHAT_Message1 { get; set; }

        [Required]
        [StringLength(10)]
        public string CHAT_Message_Date { get; set; }

        [Required]
        [StringLength(8)]
        public string CHAT_Message_Time { get; set; }

        [Required]
        [StringLength(4)]
        public string CHAT_User_YY { get; set; }

        public int CHAT_User_Seq { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CHAT_Room_ID { get; set; }

        [StringLength(1)]
        public string CHAT_isNew { get; set; }
    }
}
