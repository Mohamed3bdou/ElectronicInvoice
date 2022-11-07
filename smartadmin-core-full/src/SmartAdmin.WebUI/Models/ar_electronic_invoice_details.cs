using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{
    public class ar_electronic_invoice_details
    {
        public int n_DataAreaID { get; set; }
        public int n_document_no { get; set; }
        [Key]
        public int nAutoNumber { get; set; }
        public int nLineNo { get; set; }
        public string s_item_id { get; set; }
        public string s_bar_code { get; set; }
        public Nullable<int> n_store_id { get; set; }
        public Nullable<decimal> n_qty { get; set; }
        public Nullable<int> n_unit_id { get; set; }
        public Nullable<decimal> n_unit_price { get; set; }
        public Nullable<decimal> n_item_value { get; set; }
        public Nullable<decimal> nInvDiscountP { get; set; }
        public Nullable<decimal> nInvDiscountV { get; set; }
        public Nullable<decimal> n_item_expenses { get; set; }
        public Nullable<decimal> n_item_net_value { get; set; }
        public Nullable<decimal> nItemDiscountP { get; set; }
        public Nullable<decimal> nItemDiscountV { get; set; }
        public string s_cost_center_id { get; set; }
        public string s_cost_center_id2 { get; set; }
        public string d_date_transaction { get; set; }
        public string sBatchNO { get; set; }
        public string dExpireDate { get; set; }
        public Nullable<int> n_trans_source_doc_no { get; set; }
        public string s_notes { get; set; }
        public Nullable<decimal> n_Bonus { get; set; }
        public Nullable<decimal> n_qty_main_unit { get; set; }
        public Nullable<decimal> n_credit_discount { get; set; }
        public Nullable<decimal> n_item_cost { get; set; }
        public Nullable<int> n_curr_replicated_id { get; set; }
        public string d_UserUpdateDate { get; set; }
        public string d_UserAddDate { get; set; }
        public Nullable<int> n_Service_Order_No { get; set; }
        public string s_Service_Item_Name { get; set; }
        public string s_Service_Desc { get; set; }
        public Nullable<int> n_DataAreaID1 { get; set; }
        public string d_curr_sys_date { get; set; }
        public Nullable<decimal> n_wproofValue { get; set; }
        public Nullable<decimal> n_splastValue { get; set; }
        public Nullable<decimal> n_waterReductionValue { get; set; }
        public Nullable<decimal> n_slikaValue { get; set; }
        public Nullable<decimal> n_ICEPrice { get; set; }
        public Nullable<decimal> n_Pump_Fee { get; set; }
        public Nullable<decimal> n_item_sales_Tax { get; set; }
        public Nullable<decimal> n_item_discount_tax { get; set; }
        public Nullable<decimal> n_item_additional_tax { get; set; }
        public Nullable<int> n_cement_size { get; set; }
        public Nullable<decimal> n_HealthMinistry_Discount_Ratio { get; set; }
        public Nullable<decimal> n_HealthMinistry_Discount_Value { get; set; }
        public Nullable<decimal> n_Bonus_Discount_Ratio { get; set; }
        public Nullable<decimal> n_Bonus_Discount_Count { get; set; }
        public Nullable<decimal> n_over_Discount_Ratio { get; set; }
        public Nullable<decimal> n_over_Discount_Value { get; set; }
        public Nullable<decimal> n_over_Discount2_Value { get; set; }
        public Nullable<decimal> n_over_Discount2_Ratio { get; set; }
        public Nullable<decimal> total_Qty { get; set; }
        public Nullable<decimal> n_item_net_value_WithoutTax { get; set; }
        public Nullable<int> n_taxes_type { get; set; }
        public Nullable<bool> b_is_bonus { get; set; }
        public Nullable<int> n_marbel_count { get; set; }
        public Nullable<decimal> n_marbel_Length { get; set; }
        public Nullable<decimal> n_marbel_Width { get; set; }
        public Nullable<decimal> n_marbel_Amount { get; set; }
        public Nullable<decimal> n_marbel_Lost { get; set; }
        public Nullable<decimal> n_marbel_net_Amount { get; set; }
        public Nullable<decimal> n_Length { get; set; }
        public Nullable<decimal> n_Width { get; set; }
        public Nullable<decimal> n_count { get; set; }
        public Nullable<decimal> n_advance_payment { get; set; }
        public Nullable<bool> b_is_cobon { get; set; }
        public Nullable<decimal> n_patient_load { get; set; }
        public Nullable<int> n_current_year { get; set; }
        public Nullable<int> n_current_company { get; set; }
        public Nullable<int> n_current_branch { get; set; }
        public Nullable<int> n_color_id { get; set; }
        public string s_task_desc { get; set; }
        public string s_size { get; set; }
        public Nullable<decimal> n_salsman_price { get; set; }
        public Nullable<decimal> n_salesman_unit_price { get; set; }
        public Nullable<decimal> n_selective_tax_value { get; set; }
        public Nullable<int> n_TypeCode { get; set; }
        public Nullable<int> n_finance_transaction_no { get; set; }
        public string s_asset_no { get; set; }
        public string s_receiptNo { get; set; }
        public Nullable<decimal> n_commission_value { get; set; }
        public string s_finance_transaction_no { get; set; }
        public Nullable<decimal> n_last_pur_price { get; set; }
        public Nullable<decimal> n_height { get; set; }
        public Nullable<int> n_paper_type { get; set; }
        public string s_layers { get; set; }
        public string s_parent_item_id { get; set; }
        public Nullable<int> n_item_sort_no { get; set; }
        public Nullable<decimal> n_monthly_price { get; set; }
        public Nullable<int> n_storing_type { get; set; }
        public Nullable<decimal> n_capacity { get; set; }
        public Nullable<decimal> n_pallet_no { get; set; }
        public string d_from_date { get; set; }
        public string d_to_date { get; set; }
        public Nullable<decimal> n_month_count { get; set; }
        public Nullable<decimal> n_meter_quantity { get; set; }
        public Nullable<int> n_salesman_id { get; set; }
        public Nullable<bool> b_has_mainitem_ocasion { get; set; }
        public string s_related_item_ocasion { get; set; }
        public string s_mainitem_id { get; set; }
        public string s_container_no { get; set; }
        public Nullable<decimal> n_item_weight { get; set; }
        public string s_medical_notes { get; set; }
        public string s_medical_notes_eng { get; set; }
        public string S_item_doc { get; set; }
        public Nullable<int> n_agent_invoice { get; set; }
        public Nullable<bool> b_item_printed { get; set; }
        public Nullable<bool> b_external_asset { get; set; }
        public Nullable<decimal> n_item_Guarantee_detained { get; set; }
        public Nullable<bool> b_is_comment_item { get; set; }
        public Nullable<decimal> n_scr_count { get; set; }
    }
}
