namespace Ntabi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CU001
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string ASHOP_SITE_CD { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string CU_YY { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CU_SEQ { get; set; }

        [StringLength(13)]
        public string CU_JMNO { get; set; }

        [StringLength(20)]
        public string CU_NM_KOR { get; set; }

        [StringLength(40)]
        public string CU_NM_ENG_FIRST { get; set; }

        [StringLength(40)]
        public string CU_NM_ENG_LAST { get; set; }

        [StringLength(100)]
        public string CU_ID { get; set; }

        [StringLength(20)]
        public string CU_PASS { get; set; }

        [StringLength(2)]
        public string REGI_FG { get; set; }

        [StringLength(20)]
        public string ETC_TEL_NO { get; set; }

        [StringLength(8)]
        public string BIRTHDAY { get; set; }

        [StringLength(1)]
        public string BIRTH_CHK { get; set; }

        [StringLength(8)]
        public string MARRYDAY { get; set; }

        [StringLength(1)]
        public string MARRIED_YN { get; set; }

        [StringLength(1)]
        public string SPOUSE_YN { get; set; }

        [StringLength(10)]
        public string ZIP_CD { get; set; }

        [StringLength(100)]
        public string CU_HOME_ADDR_F { get; set; }

        [StringLength(100)]
        public string CU_HOME_ADDR { get; set; }

        [StringLength(10)]
        public string JOB_CD { get; set; }

        [StringLength(10)]
        public string CO_ZIP_CD { get; set; }

        [StringLength(100)]
        public string CU_CO_ADDR_F { get; set; }

        [StringLength(100)]
        public string CU_CO_ADDR { get; set; }

        [StringLength(30)]
        public string CU_CO_NM { get; set; }

        [StringLength(30)]
        public string CU_JOB_NM { get; set; }

        [StringLength(30)]
        public string CU_DEPT_NM { get; set; }

        [StringLength(30)]
        public string CU_GRADE_NM { get; set; }

        [StringLength(30)]
        public string CU_SCH_NM { get; set; }

        [StringLength(1)]
        public string CU_SCH_YN { get; set; }

        [StringLength(30)]
        public string CU_SUBJECT { get; set; }

        [StringLength(1)]
        public string CU_GUB { get; set; }

        [StringLength(1)]
        public string CU_AIRCARD_YN { get; set; }

        [StringLength(1)]
        public string CU_DM_YN { get; set; }

        [StringLength(1)]
        public string EMAIL_YN { get; set; }

        [StringLength(1)]
        public string PP_YN { get; set; }

        [StringLength(20)]
        public string PP_NO { get; set; }

        [StringLength(8)]
        public string PP_TERMINATION_DAY { get; set; }

        [StringLength(60)]
        public string COMP_DESC { get; set; }

        [StringLength(50)]
        public string CU_URL { get; set; }

        [StringLength(50)]
        public string EMAIL { get; set; }

        [StringLength(20)]
        public string CU_CARD_NO { get; set; }

        [StringLength(1)]
        public string TOUR_EXPE_CD { get; set; }

        [StringLength(1)]
        public string TOUR_PLAN_CD { get; set; }

        [StringLength(4000)]
        public string REMARK { get; set; }

        [StringLength(8)]
        public string INS_DT { get; set; }

        [StringLength(10)]
        public string INS_EMP_NO { get; set; }

        [StringLength(8)]
        public string UPD_DT { get; set; }

        [StringLength(10)]
        public string UPD_EMP_NO { get; set; }

        [StringLength(1)]
        public string DEL_FG { get; set; }

        [StringLength(4)]
        public string CO_FAX1 { get; set; }

        [StringLength(4)]
        public string CO_FAX2 { get; set; }

        [StringLength(4)]
        public string CO_FAX3 { get; set; }

        [StringLength(4)]
        public string HANDPHONE1 { get; set; }

        [StringLength(4)]
        public string HANDPHONE2 { get; set; }

        [StringLength(4)]
        public string HANDPHONE3 { get; set; }

        [StringLength(4)]
        public string HOME_TEL1 { get; set; }

        [StringLength(4)]
        public string HOME_TEL2 { get; set; }

        [StringLength(4)]
        public string HOME_TEL3 { get; set; }

        [StringLength(4)]
        public string CO_TEL1 { get; set; }

        [StringLength(4)]
        public string CO_TEL2 { get; set; }

        [StringLength(4)]
        public string CO_TEL3 { get; set; }

        [StringLength(10)]
        public string AGEN_CD { get; set; }

        [StringLength(1)]
        public string CU_PCARD_YN { get; set; }

        [StringLength(11)]
        public string MEMBERCODE { get; set; }

        [StringLength(1)]
        public string ONOFF { get; set; }

        [StringLength(10)]
        public string MEM_NO { get; set; }

        [StringLength(100)]
        public string MEM_ID { get; set; }

        [StringLength(1)]
        public string SMS_YN { get; set; }

        public int? IDX { get; set; }

        [StringLength(1)]
        public string INFO_OPEN_YN { get; set; }

        [StringLength(100)]
        public string NICK_NM { get; set; }

        [StringLength(3)]
        public string CU_DETAIL_CD { get; set; }

        [StringLength(10)]
        public string SITE_CD { get; set; }

        [StringLength(4)]
        public string CU_YY_M { get; set; }

        public int? CU_SEQ_M { get; set; }

        [StringLength(2)]
        public string REL_CD { get; set; }

        [StringLength(1)]
        public string CU_GB { get; set; }

        [StringLength(1)]
        public string AUTH_YN { get; set; }

        [StringLength(1)]
        public string ACCEPT_GBN { get; set; }

        [StringLength(10)]
        public string CHA_EMP_NO { get; set; }

        [StringLength(1)]
        public string EMAIL_CON_YN { get; set; }

        [StringLength(2)]
        public string REG_WEB_CD { get; set; }

        [StringLength(3)]
        public string BUSI_TYPE1 { get; set; }

        [StringLength(3)]
        public string BUSI_TYPE2 { get; set; }

        [StringLength(3)]
        public string BUSI_TYPE3 { get; set; }

        [StringLength(1)]
        public string SEX { get; set; }
    }
}
