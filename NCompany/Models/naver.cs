using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCompany.Models
{
    [Serializable]
    public class naver
    {
        /* 필수 입력 */

        [Required]
        public int index { get; set; }

        [Required]
        [StringLength(50)]
        public string id { get; set; }

        [Required]
        [StringLength(100)]
        public string title { get; set; }

        [Required]
        public int price_pc { get; set; }

        [Required]
        [StringLength(255)]
        public string link { get; set; }

        [Required]
        [StringLength(255)]
        public string image_link { get; set; }

        [Required]
        [StringLength(50)]
        public string category_name1 { get; set; }

        [Required]
        [StringLength(100)]
        public string shipping { get; set; }

        [Required]
        [StringLength(10)]
        public string classs { get; set; }

        [Required]
        [StringLength(19)]
        public string update_time { get; set; }

        /* 필수 입력 */

        /* 해당 상품 필수 입력 */

        [StringLength(10)]
        public string condition { get; set; }

        [StringLength(1)]
        public string import_flag { get; set; }

        [StringLength(1)]
        public string paraller_import { get; set; }

        [StringLength(1)]
        public string order_made { get; set; }

        [StringLength(10)]
        public string product_flag { get; set; }

        [StringLength(1)]
        public string adult { get; set; }

        [StringLength(1)]
        public string partner_coupon_downlad { get; set; }

        [StringLength(1)]
        public string installation_costs { get; set; }

        [StringLength(10)]
        public string minimum_purchase_quantity { get; set; }

        [StringLength(1)]
        public string delivery_grade { get; set; }

        [StringLength(100)]
        public string delivery_detail { get; set; }

        /* 해당 상품 필수 입력 */

        /* 권장 */

        public int price_mobile { get; set; }

        public int normal_price { get; set; }

        [StringLength(255)]
        public string mobile_link { get; set; }

        [StringLength(255)]
        public string add_image_link { get; set; }

        [StringLength(50)]
        public string category_name2 { get; set; }

        [StringLength(50)]
        public string category_name3 { get; set; }

        [StringLength(50)]
        public string category_name4 { get; set; }

        [StringLength(8)]
        public string naver_category { get; set; }

        [StringLength(50)]
        public string naver_product_id { get; set; }

        [StringLength(10)]
        public string goods_type { get; set; }

        [StringLength(13)]
        public string barcode { get; set; }

        [StringLength(100)]
        public string manufacture_define_number { get; set; }

        [StringLength(60)]
        public string model_number { get; set; }

        [StringLength(60)]
        public string brand { get; set; }

        [StringLength(60)]
        public string maker { get; set; }

        [StringLength(30)]
        public string origin { get; set; }

        [StringLength(100)]
        public string card_event { get; set; }

        [StringLength(100)]
        public string event_words { get; set; }

        [StringLength(100)]
        public string coupon { get; set; }

        [StringLength(100)]
        public string interest_free_event { get; set; }

        [StringLength(50)]
        public string point { get; set; }

        [StringLength(100)]
        public string search_tag { get; set; }

        [StringLength(50)]
        public string group_id { get; set; }

        [StringLength(500)]
        public string vendor_id { get; set; }

        [StringLength(500)]
        public string coordi_id { get; set; }

        public int review_count { get; set; }

        [StringLength(500)]
        public string attribute { get; set; }

        [StringLength(1000)]
        public string option_detail { get; set; }

        [StringLength(50)]
        public string seller_id { get; set; }

        [StringLength(10)]
        public string age_group { get; set; }

        [StringLength(10)]
        public string gender { get; set; }

        /* 권장 */
    }
}