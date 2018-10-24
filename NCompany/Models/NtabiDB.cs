namespace NCompany.Models
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
        }
    }
}
