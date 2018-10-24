/******************************  Comment  *****************************
	File Name			: AllatUtil.cs
	File Description	: Allat Script API Utility Function(Class)
	Version				:
	[ Notice ]
	 이 파일은 NewAllatPay를 사용하기 위한 Utility Function을 구현한
	Source Code입니다. 이 파일에 내용을 임의로 수정하실 경우 기술지원을
	받으실 수 없음을 알려드립니다. 이 파일 내용에 문제가 있을 경우,
	아래 연락처로 문의 주시기 바랍니다.

	TEL					: 02-3783-9990
	EMAIL				: allatpay@allat.co.kr
	Homepage			: www.allatpay.com
*************** Copyright Allat Corp. All Right Reserved  ***********/
using System;
using System.Net;
using System.Text;
using System.Collections;

public class AllatUtil
{
	public AllatUtil(){}
    private const string UTIL_LANG         = "ASPX";
    private const string UTIL_VER          = "1.0.7.1";
    private const string APPROVAL_URI      = "/servlet/AllatPay/pay/approval.jsp";
    private const string SANCTION_URI      = "/servlet/AllatPay/pay/sanction.jsp";
    private const string CANCEL_URI        = "/servlet/AllatPay/pay/cancel.jsp";
    private const string CASHREG_URI       = "/servlet/AllatPay/pay/cash_registry.jsp";
    private const string CASHAPP_URI       = "/servlet/AllatPay/pay/cash_approval.jsp";
    private const string CASHCAN_URI       = "/servlet/AllatPay/pay/cash_cancel.jsp";
    private const string ESCROWCHECK_URI   = "/servlet/AllatPay/pay/escrow_check.jsp";
	private const string ESCROWCONFIRM_URI   = "/servlet/AllatPay/pay/escrow_confirm.jsp";
    private const string ESCROWRETURN_URI  = "/servlet/AllatPay/pay/escrow_return.jsp";
    private const string CERTREG_URI       = "/servlet/AllatPay/pay/fix.jsp";
    private const string CERTCANCEL_URI    = "/servlet/AllatPay/pay/fix_cancel.jsp";
	private const string CARDPOINTDC_URI   = "/servlet/AllatPay/pay/cardpointdc.jsp";
	private const string CARDLIST_URI      = "/servlet/AllatPay/nonactivex/nonre/nonre_cardlist.jsp";

    private const string C2C_APPROVAL_URI      = "/servlet/AllatPay/pay/c2c_approval.jsp";
    private const string C2C_CANCEL_URI        = "/servlet/AllatPay/pay/c2c_cancel.jsp";
    private const string C2C_SELLERREG_URI     = "/servlet/AllatPay/pay/seller_registry.jsp";
    private const string C2C_PRODUCTREG_URI    = "/servlet/AllatPay/pay/product_registry.jsp";
    private const string C2C_BUYERCHG_URI      = "/servlet/AllatPay/pay/buyer_change.jsp";
    private const string C2C_ESCROWCHK_URI     = "/servlet/AllatPay/pay/c2c_escrow_check.jsp";
    private const string C2C_ESCROWCONFIRM_URI = "/servlet/AllatPay/pay/c2c_escrow_confirm.jsp";
    private const string C2C_ESREJECTCHECK_URI = "/servlet/AllatPay/pay/c2c_reject_check.jsp";
	private const string C2C_EXPRESSREG_URI    = "/servlet/AllatPay/pay/c2c_express_reg.jsp";

    private const string ALLAT_HOST        = "tx.allatpay.com";

    public Hashtable ApprovalReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, APPROVAL_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, APPROVAL_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable SanctionReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, SANCTION_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, SANCTION_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable CancelReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CANCEL_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CANCEL_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable CashregReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CASHREG_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CASHREG_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable CashappReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CASHAPP_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CASHAPP_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable CashcanReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CASHCAN_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CASHCAN_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable EscrowchkReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, ESCROWCHECK_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, ESCROWCHECK_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

	public Hashtable EscrowConfirmReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, ESCROWCONFIRM_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, ESCROWCONFIRM_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable EscrowretReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, ESCROWRETURN_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, ESCROWRETURN_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable CertregReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CERTREG_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CERTREG_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable CertcanReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CERTCANCEL_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CERTCANCEL_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable CardpointdcReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CARDPOINTDC_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CARDPOINTDC_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

	public Hashtable CardListReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, CARDLIST_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, CARDLIST_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

	public Hashtable C2CApprovalReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_APPROVAL_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_APPROVAL_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CCancelReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_CANCEL_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_CANCEL_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CSellerregReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_SELLERREG_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_SELLERREG_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CProductregReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_PRODUCTREG_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_PRODUCTREG_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CBuyerchgReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_BUYERCHG_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_BUYERCHG_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CEscrowchkReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_ESCROWCHK_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_ESCROWCHK_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CEscrowconfirmReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_ESCROWCONFIRM_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_ESCROWCONFIRM_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CEsrejectcheckReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_ESREJECTCHECK_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_ESREJECTCHECK_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

    public Hashtable C2CExpressregReq(string strReq, string sslFlag)
    {
        Hashtable retHt = null;
        bool isEnc = false;
        if (sslFlag.Equals("SSL"))
        {
            retHt = SendRepo(strReq, ALLAT_HOST, C2C_EXPRESSREG_URI, 443);
        }
        else
        {
            isEnc = CheckEnc(strReq);
            if (isEnc)
            {
                retHt = SendRepo(strReq, ALLAT_HOST, C2C_EXPRESSREG_URI, 80);
            }
            else
            {
                retHt = GetValue("reply_cd=0230\nreply_msg=암호화 오류\n");
            }
        }
        return retHt;
    }

	private Hashtable SendRepo(string srpReq, string srpHost, string srpUri, int srpPort)
    {
        Hashtable retHt = null;
        string retTxt = "";
        retTxt = SendReq(srpReq, srpHost, srpUri, srpPort);
        retHt = GetValue(retTxt);
        return retHt;
    }

    private string SendReq(string sendMsg, string atHost, string atUri, int atPort)
    {
        string sendStr = "";
        string resultStr = "";
        string callURL = "";

        DateTime cDate = DateTime.Now;
        sendStr = "&allat_apply_ymdhms=" + cDate.ToString("yyyyMMddHHmmss")
                + "&allat_opt_lang=" + UTIL_LANG
                + "&allat_opt_ver=" + UTIL_VER;
        if (atPort == 80)
        {
            callURL = "http://" + atHost + atUri;
        }
        else
        {
            callURL = "https://" + atHost + atUri;
        }

        byte[] szByteSend = Encoding.ASCII.GetBytes(sendMsg + sendStr);
        WebClient svrRet = new WebClient();
        svrRet.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        try
        {
            resultStr = Encoding.Default.GetString(svrRet.UploadData(callURL, "POST", szByteSend));
        }
        catch (WebException)
        {
            return "reply_cd=0223\nreply_msg=Exception:페이지를 찾을수 없거나 연결이 불가능한 상태" + callURL + "\n";
        }
        return resultStr;
    }


    public Hashtable GetValue(string srcTxt)
    {
        int i = 0;
        Hashtable retHt = new Hashtable();
        if (srcTxt == null || srcTxt.Length == 0)
        {
            retHt.Add("reply_cd=0223", "reply_msg=Souce Txt Error");
            return retHt;
        }
        string[] outerStr = srcTxt.Split('\n');
        for (i = 0; i < outerStr.Length; i++)
        {
            if (outerStr[i].Trim().Length == 0) continue;
            string[] innerStr = outerStr[i].Split('=');
            try
            {
                retHt.Add(innerStr[0], innerStr[1]);
            }
            catch (IndexOutOfRangeException)
            {
                retHt.Add("reply_cd=0223", "reply_msg=IndexOutOfRangeException:GetValue()");
                return retHt;
            }
        }
        return retHt;
    }


    private bool CheckEnc(string srcStr)
    {
        int ckIdx;
        ckIdx = srcStr.IndexOf("allat_enc_data=");

        if (ckIdx == -1)
        {
            return false;
        }
        else
        {
            ckIdx += "allat_enc_data=".Length + 5;
        }
        if ((srcStr.Substring(ckIdx, ckIdx + 1)).Equals("1"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

	public string SetValue(Hashtable srcHt)
    {
        string formData = "";
        bool bFirst = false;
        if (srcHt == null)
        {
            formData = null;
            return formData;
        }
        ICollection keys = srcHt.Keys;
        foreach (object obj in keys)
        {
            if (bFirst)
            {
                formData += (string)obj + "" + (string)srcHt[obj] + "";
            }
            else
            {
                formData += "00000010" + (string)obj + "" + (string)srcHt[obj] + "";
                bFirst = true;
            }
        }

        return formData;

    }

}
