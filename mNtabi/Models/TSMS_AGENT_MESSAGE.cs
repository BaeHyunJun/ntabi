namespace mNtabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TSMS_AGENT_MESSAGE
    {
        [Key]
        public long MESSAGE_SEQNO { get; set; }

        public long SERVICE_SEQNO { get; set; }

        [StringLength(4000)]
        public string SEND_MESSAGE { get; set; }

        [StringLength(200)]
        public string SUBJECT { get; set; }

        [StringLength(4000)]
        public string BACKUP_MESSAGE { get; set; }

        [Required]
        [StringLength(4)]
        public string BACKUP_PROCESS_CODE { get; set; }

        [Required]
        [StringLength(1)]
        public string SEND_FLAG { get; set; }

        public DateTime? SEND_DATE { get; set; }

        [Required]
        [StringLength(3)]
        public string MESSAGE_TYPE { get; set; }

        [StringLength(3)]
        public string CONTENTS_TYPE { get; set; }

        [StringLength(13)]
        public string CALLBACK_NO { get; set; }

        [Required]
        [StringLength(20)]
        public string RECEIVE_MOBILE_NO { get; set; }

        [StringLength(300)]
        public string RECEIVE_DEVICE_ID { get; set; }

        [StringLength(3)]
        public string RECEIVE_PHONE_TYPE { get; set; }

        [Required]
        [StringLength(3)]
        public string JOB_TYPE { get; set; }

        public DateTime? SEND_RESERVE_DATE { get; set; }

        [StringLength(40)]
        public string VIEW_TYPE { get; set; }

        [StringLength(40)]
        public string BATCH_FILE_NAME { get; set; }

        [StringLength(25)]
        public string LATITUDE { get; set; }

        [StringLength(25)]
        public string LONGITUDE { get; set; }

        [StringLength(3)]
        public string COLOR_TYPE { get; set; }

        public int? TEMPLATE_NO { get; set; }

        [StringLength(30)]
        public string MODEL_NAME { get; set; }

        [StringLength(100)]
        public string ETC1 { get; set; }

        [StringLength(100)]
        public string ETC2 { get; set; }

        [StringLength(100)]
        public string ETC3 { get; set; }

        [StringLength(100)]
        public string ETC4 { get; set; }

        [StringLength(100)]
        public string ETC5 { get; set; }

        [StringLength(100)]
        public string ETC6 { get; set; }

        [StringLength(100)]
        public string ETC7 { get; set; }

        [StringLength(100)]
        public string ETC8 { get; set; }

        [StringLength(100)]
        public string ETC9 { get; set; }

        [StringLength(100)]
        public string ETC10 { get; set; }

        [StringLength(100)]
        public string APP_ETC1 { get; set; }

        [StringLength(100)]
        public string APP_ETC2 { get; set; }

        [StringLength(100)]
        public string APP_ETC3 { get; set; }

        [StringLength(100)]
        public string APP_ETC4 { get; set; }

        [StringLength(100)]
        public string APP_ETC5 { get; set; }

        [StringLength(8)]
        public string VALID_START_DATE { get; set; }

        [StringLength(8)]
        public string VALID_END_DATE { get; set; }

        [StringLength(150)]
        public string IMG1_PATH { get; set; }

        [StringLength(150)]
        public string IMG2_PATH { get; set; }

        [StringLength(150)]
        public string IMG3_PATH { get; set; }

        [StringLength(20)]
        public string BARCODE_TYPE { get; set; }

        [StringLength(4000)]
        public string BARCODE_VALUE { get; set; }

        [StringLength(100)]
        public string IMG1_SERVER_PATH { get; set; }

        [StringLength(100)]
        public string IMG2_SERVER_PATH { get; set; }

        [StringLength(100)]
        public string IMG3_SERVER_PATH { get; set; }

        [Required]
        [StringLength(1)]
        public string IMG_SEND_FLAG { get; set; }

        public DateTime? IMG_SEND_DATE { get; set; }

        public DateTime REGISTER_DATE { get; set; }

        [Required]
        [StringLength(20)]
        public string REGISTER_BY { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        [StringLength(20)]
        public string UPDATE_BY { get; set; }

        [StringLength(100)]
        public string RESERVE1 { get; set; }

        [StringLength(100)]
        public string RESERVE2 { get; set; }

        [StringLength(100)]
        public string RESERVE3 { get; set; }

        [StringLength(100)]
        public string RESERVE4 { get; set; }

        [StringLength(100)]
        public string RESERVE5 { get; set; }

        [StringLength(100)]
        public string RESERVE6 { get; set; }

        [StringLength(100)]
        public string RESERVE7 { get; set; }

        [StringLength(2000)]
        public string POPUP_MEDIA_URL { get; set; }

        [StringLength(2000)]
        public string INFOR_MEDIA_URL { get; set; }

        [StringLength(2000)]
        public string MEDIA_CLICK_URL { get; set; }
    }
}
