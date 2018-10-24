namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CHAT_Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CHAT_Room_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string CHAT_Title { get; set; }

        [Required]
        [StringLength(50)]
        public string CHAT_Name { get; set; }

        [StringLength(100)]
        public string CHAT_Password { get; set; }

        [StringLength(1)]
        public string CHAT_isNew { get; set; }
    }
}
