namespace NDayTrip_PC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CHAT_User_Room
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CHAT_Room_ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string CHAT_User_YY { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CHAT_User_Seq { get; set; }
    }
}
