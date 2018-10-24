namespace NDayTrip_PC.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataBase : DbContext
    {
        public DataBase()
            : base("name=DataBase")
        {
        }

        public virtual DbSet<ACC_0> ACC_0 { get; set; }
        public virtual DbSet<CAR_0> CAR_0 { get; set; }
        public virtual DbSet<CHAT_Message> CHAT_Message { get; set; }
        public virtual DbSet<CHAT_Room> CHAT_Room { get; set; }
        public virtual DbSet<COU_0> COU_0 { get; set; }
        public virtual DbSet<COU_1> COU_1 { get; set; }
        public virtual DbSet<COU_2> COU_2 { get; set; }
        public virtual DbSet<DETAIL> DETAIL { get; set; }
        public virtual DbSet<EMP_0> EMP_0 { get; set; }
        public virtual DbSet<EVE_0> EVE_0 { get; set; }
        public virtual DbSet<GRO_0> GRO_0 { get; set; }
        public virtual DbSet<HTL_0> HTL_0 { get; set; }
        public virtual DbSet<HTL_1> HTL_1 { get; set; }
        public virtual DbSet<HTL_2> HTL_2 { get; set; }
        public virtual DbSet<NT_Board_2> NT_Board_2 { get; set; }
        public virtual DbSet<PDT_0> PDT_0 { get; set; }
        public virtual DbSet<PDT_1> PDT_1 { get; set; }
        public virtual DbSet<PDT_2> PDT_2 { get; set; }
        public virtual DbSet<PDT_4> PDT_4 { get; set; }
        public virtual DbSet<PDT_5> PDT_5 { get; set; }
        public virtual DbSet<PER_0> PER_0 { get; set; }
        public virtual DbSet<POS_0> POS_0 { get; set; }
        public virtual DbSet<PRC_0> PRC_0 { get; set; }
        public virtual DbSet<REV_0> REV_0 { get; set; }
        public virtual DbSet<REV_1> REV_1 { get; set; }
        public virtual DbSet<REV_2> REV_2 { get; set; }
        public virtual DbSet<REV_3> REV_3 { get; set; }
        public virtual DbSet<TRF_0> TRF_0 { get; set; }
        public virtual DbSet<TRF_1> TRF_1 { get; set; }
        public virtual DbSet<VOU_0> VOU_0 { get; set; }
        public virtual DbSet<ADM_0> ADM_0 { get; set; }
        public virtual DbSet<BANK> BANK { get; set; }
        public virtual DbSet<BNK_0> BNK_0 { get; set; }
        public virtual DbSet<CHAT_User_Room> CHAT_User_Room { get; set; }
        public virtual DbSet<PDT_3> PDT_3 { get; set; }
        public virtual DbSet<TB_ComCode> TB_ComCode { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ACC_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<ACC_0>()
                .Property(e => e.ACC_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<ACC_0>()
                .Property(e => e.ACC_BANK_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ACC_0>()
                .Property(e => e.ACC_NUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<ACC_0>()
                .Property(e => e.ACC_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<ACC_0>()
                .Property(e => e.ACC_IST_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<ACC_0>()
                .Property(e => e.ACC_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CAR_REG_NUM)
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CAR_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CAR_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CAR_AREA_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CAR_NOTE)
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CAR_RENT_CORP)
                .IsUnicode(false);

            modelBuilder.Entity<CAR_0>()
                .Property(e => e.CAR_FEATURE)
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_Message>()
                .Property(e => e.CHAT_Message1)
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_Message>()
                .Property(e => e.CHAT_Message_Date)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_Message>()
                .Property(e => e.CHAT_Message_Time)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_Message>()
                .Property(e => e.CHAT_User_YY)
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_Room>()
                .Property(e => e.CHAT_Title)
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_Room>()
                .Property(e => e.CHAT_Name)
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_Room>()
                .Property(e => e.CHAT_Password)
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.COU_AREA_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.COU_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.COU_LATITUDE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.COU_LONGITUDE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.COU_ADDRESS)
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.COU_CONT)
                .IsUnicode(false);

            modelBuilder.Entity<COU_0>()
                .Property(e => e.COU_attachment)
                .IsUnicode(false);

            modelBuilder.Entity<COU_1>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_1>()
                .Property(e => e.COU_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_1>()
                .Property(e => e.COU_AREA_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<COU_1>()
                .Property(e => e.COU_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_1>()
                .Property(e => e.COU_DRIVER)
                .IsUnicode(false);

            modelBuilder.Entity<COU_1>()
                .Property(e => e.COU_NOTE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_1>()
                .Property(e => e.CAR_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<COU_2>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_2>()
                .Property(e => e.COU_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_2>()
                .Property(e => e.COU_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_2>()
                .Property(e => e.EVE_REG_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<COU_2>()
                .Property(e => e.EVE_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<COU_2>()
                .Property(e => e.EVE_OFFICE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<COU_2>()
                .Property(e => e.EVE_TEL)
                .IsUnicode(false);

            modelBuilder.Entity<DETAIL>()
                .Property(e => e.DET_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<DETAIL>()
                .Property(e => e.DET_SEQ)
                .IsUnicode(false);

            modelBuilder.Entity<DETAIL>()
                .Property(e => e.DET_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_ID)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_PWD)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_TEL1)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_TEL2)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_TEL3)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.GRO_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.POS_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_BIRTH)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_INTRO)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.PER_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.CORP_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_PHONE1)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_PHONE2)
                .IsUnicode(false);

            modelBuilder.Entity<EMP_0>()
                .Property(e => e.EMP_PHONE3)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_REG_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_TEL)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_OFFICE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_ETC)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_AREA_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.OFFICE_SEQ)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_SET)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_CHKMAIL)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_IST_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<EVE_0>()
                .Property(e => e.EVE_UDT_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<GRO_0>()
                .Property(e => e.GRO_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<GRO_0>()
                .Property(e => e.GRO_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<GRO_0>()
                .Property(e => e.GRO_PARENT)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_NATION_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_AREA_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_PROC_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_IST_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_UDT_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_CHK_PRICE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_CHK_DAYS)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_CHK_CONTENT)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_0>()
                .Property(e => e.HTL_TYPE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_1>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_1>()
                .Property(e => e.HTL_Currency_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<HTL_1>()
                .Property(e => e.HTL_ROOM)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_1>()
                .Property(e => e.HTL_EXP)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_2>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_2>()
                .Property(e => e.HTL_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<HTL_2>()
                .Property(e => e.HTL_STATE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.CU_YY)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.PDT_YY)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.post_status)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.ist_date)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.udt_date)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.DEL_FG)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.ist_time)
                .IsFixedLength();

            modelBuilder.Entity<NT_Board_2>()
                .Property(e => e.udt_time)
                .IsFixedLength();

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_NATION_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_AREA_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_SCITY_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_PROC_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_DAYS_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_OPTIONS)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_IST_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_UDT_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_Option1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_Option2)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_Option3)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_Option4)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_Option5)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_Option6)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_TOUR_CITY)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_0>()
                .Property(e => e.PDT_COMBINE_HTL)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_1>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_1>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_1>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_1>()
                .Property(e => e.PDT_STATE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_1>()
                .Property(e => e.PDT_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_IMG)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH0)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH1)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH2)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH3)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH4)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH5)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH6)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH7)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH8)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_2>()
                .Property(e => e.PDT_SCH9)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_4>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_4>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_4>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_4>()
                .Property(e => e.IST_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_4>()
                .Property(e => e.UDT_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_5>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_5>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_5>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_5>()
                .Property(e => e.PDT_IMG_URL)
                .IsUnicode(false);

            modelBuilder.Entity<PER_0>()
                .Property(e => e.PER_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PER_0>()
                .Property(e => e.PER_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<POS_0>()
                .Property(e => e.POS_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<POS_0>()
                .Property(e => e.POS_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<PRC_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PRC_0>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PRC_0>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PRC_0>()
                .Property(e => e.PRC_Currency_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PRC_0>()
                .Property(e => e.PRC_EXP)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.REV_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.REV_STATE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.REV_STARTDAY)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.IST_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.UDT_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.SEL_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.REV_CHK_PRICE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.PDT_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.PDT_DAYS_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.CU_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.CU_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.REV_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<REV_0>()
                .Property(e => e.CHK_VOU)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.REV_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.REV_PRC_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.REV_PRC_GB)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.PRC_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<REV_1>()
                .Property(e => e.PRC_CURRENCY_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.REV_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CU_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CU_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CU_NAME_FIRST)
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CU_NAME_LAST)
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CU_SEX)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CU_GB)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.CU_TEL)
                .IsUnicode(false);

            modelBuilder.Entity<REV_2>()
                .Property(e => e.REV_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.REV_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.REV_ACC_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.REV_ACC_REG_DATE)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.REV_ACC_GB_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.REV_ACC_DET_SEQ)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.REV_ACC_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.REV_ACC_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<REV_3>()
                .Property(e => e.ACC_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_TYPE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_CO_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_STIME)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_SAREA)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_ETIME)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_EAREA)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_0>()
                .Property(e => e.TRF_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_TITLE)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_TYPE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_CO_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_STIME)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_SAREA)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_ETIME)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_EAREA)
                .IsUnicode(false);

            modelBuilder.Entity<TRF_1>()
                .Property(e => e.TRF_CONTENT)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.REV_DAY)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_CONT1)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_CONT2)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_CONT3)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_CONT4)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_LATITUDE)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_LONGITUDE)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_PHOTO1)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_PHOTO2)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_PHOTO3)
                .IsUnicode(false);

            modelBuilder.Entity<VOU_0>()
                .Property(e => e.VOU_CONT5)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_TEL1)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_TEL2)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_TEL3)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_ADDRESS)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_REG_NO)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_CEO_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_BUSI_CDTS)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_BUSI_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_FAX1)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_FAX2)
                .IsUnicode(false);

            modelBuilder.Entity<ADM_0>()
                .Property(e => e.CORP_FAX3)
                .IsUnicode(false);

            modelBuilder.Entity<BANK>()
                .Property(e => e.BANK_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<BANK>()
                .Property(e => e.BANK_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<BNK_0>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<BNK_0>()
                .Property(e => e.BNK_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<BNK_0>()
                .Property(e => e.BNK_NUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<CHAT_User_Room>()
                .Property(e => e.CHAT_User_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_3>()
                .Property(e => e.CORP_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<PDT_3>()
                .Property(e => e.PDT_TYPE_CODE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_3>()
                .Property(e => e.PDT_YY)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PDT_3>()
                .Property(e => e.SCH_CONT)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ComCode>()
                .Property(e => e.UseFlag)
                .IsFixedLength();

            modelBuilder.Entity<TB_ComCode>()
                .Property(e => e.AlterCode)
                .IsFixedLength();

            modelBuilder.Entity<TB_ComCode>()
                .Property(e => e.Limit)
                .IsFixedLength();
        }
    }
}
