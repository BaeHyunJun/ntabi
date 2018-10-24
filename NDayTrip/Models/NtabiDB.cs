namespace NDayTrip.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class NtabiDB : DbContext
    {
        public NtabiDB()
            : base("name=NtabiDB")
        {
        }

        public virtual DbSet<CU001> CU001 { get; set; }
        public virtual DbSet<TSMS_AGENT_MESSAGE> TSMS_AGENT_MESSAGE { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CU001>()
                .Property(e => e.ASHOP_SITE_CD)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_JMNO)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_NM_KOR)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_NM_ENG_FIRST)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_NM_ENG_LAST)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_ID)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_PASS)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.REGI_FG)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.ETC_TEL_NO)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.BIRTHDAY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.BIRTH_CHK)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.MARRYDAY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.MARRIED_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.SPOUSE_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.ZIP_CD)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_HOME_ADDR_F)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_HOME_ADDR)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.JOB_CD)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CO_ZIP_CD)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_CO_ADDR_F)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_CO_ADDR)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_CO_NM)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_JOB_NM)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_DEPT_NM)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_GRADE_NM)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_SCH_NM)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_SCH_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_SUBJECT)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_GUB)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_AIRCARD_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_DM_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.EMAIL_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.PP_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.PP_NO)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.PP_TERMINATION_DAY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.COMP_DESC)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_URL)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_CARD_NO)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.TOUR_EXPE_CD)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.TOUR_PLAN_CD)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.REMARK)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.INS_DT)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.INS_EMP_NO)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.UPD_DT)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.UPD_EMP_NO)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.DEL_FG)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CO_FAX1)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CO_FAX2)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CO_FAX3)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.HANDPHONE1)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.HANDPHONE2)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.HANDPHONE3)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.HOME_TEL1)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.HOME_TEL2)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.HOME_TEL3)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CO_TEL1)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CO_TEL2)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CO_TEL3)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.AGEN_CD)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_PCARD_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.MEMBERCODE)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.ONOFF)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.MEM_NO)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.MEM_ID)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.SMS_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.INFO_OPEN_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.NICK_NM)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_DETAIL_CD)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.SITE_CD)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_YY_M)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.REL_CD)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CU_GB)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.AUTH_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.ACCEPT_GBN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.CHA_EMP_NO)
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.EMAIL_CON_YN)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.REG_WEB_CD)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.BUSI_TYPE1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.BUSI_TYPE2)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.BUSI_TYPE3)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CU001>()
                .Property(e => e.SEX)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.SEND_MESSAGE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.SUBJECT)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.BACKUP_MESSAGE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.BACKUP_PROCESS_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.SEND_FLAG)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.MESSAGE_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.CONTENTS_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.CALLBACK_NO)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RECEIVE_MOBILE_NO)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RECEIVE_DEVICE_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RECEIVE_PHONE_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.JOB_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.VIEW_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.BATCH_FILE_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.LATITUDE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.LONGITUDE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.COLOR_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.MODEL_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC1)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC2)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC3)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC4)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC5)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC6)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC7)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC8)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC9)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.ETC10)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.APP_ETC1)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.APP_ETC2)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.APP_ETC3)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.APP_ETC4)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.APP_ETC5)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.VALID_START_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.VALID_END_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.IMG1_PATH)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.IMG2_PATH)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.IMG3_PATH)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.BARCODE_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.BARCODE_VALUE)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.IMG1_SERVER_PATH)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.IMG2_SERVER_PATH)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.IMG3_SERVER_PATH)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.IMG_SEND_FLAG)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.REGISTER_BY)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.UPDATE_BY)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RESERVE1)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RESERVE2)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RESERVE3)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RESERVE4)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RESERVE5)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RESERVE6)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.RESERVE7)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.POPUP_MEDIA_URL)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.INFOR_MEDIA_URL)
                .IsUnicode(false);

            modelBuilder.Entity<TSMS_AGENT_MESSAGE>()
                .Property(e => e.MEDIA_CLICK_URL)
                .IsUnicode(false);
        }
    }
}
