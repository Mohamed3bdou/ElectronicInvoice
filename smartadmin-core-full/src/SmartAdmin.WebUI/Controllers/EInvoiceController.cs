//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SmartAdmin.WebUI.Data;
//using SmartAdmin.WebUI.Models;
//using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore.SqlServer;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Data.SqlClient;
//using System.Data;
//using System.Text.Json;
//using SmartAdmin.WebUI.App_Helpers;
//using FastReport.Web;
//using FastReport.Data;


//namespace SmartAdmin.WebUI.Controllers
//{
//    public class InvoiceController : BaseController
//    {
//        public InvoiceController(ApplicationDbContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment) : base(context, config, webHostEnvironment)
//        {
//        }

//        public ActionResult CarsInvoice()
//        {
//            return View();
//        }
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Vuw_customer_reservations>>> loadDT()
//        {
//            var data = _context.Vuw_invoice_draft.OrderByDescending(x => x.n_document_no);
//            var users = await data.AsNoTracking().ToListAsync();
//            return Json(new { data = users, recordsTotal = users.Count, recordsFiltered = users.Count });
//        }
//        [HttpGet]
//        public async Task<IActionResult> editInvoice(int id)
//        {
//            var data = await _context.ar_sales_invoice_draft.FirstOrDefaultAsync(x => x.n_document_no == id);
//            ViewBag.n_customer_id = new SelectList(_context.ar_customers.ToList(), "n_customer_id", "s_customer_name", data.n_customer_id);
//            ViewBag.n_customer_tax_type = new SelectList(_context.SYS_Combo_Options.Where(c => c.ComboID == 178).ToList(), "Code", "Name_arabic", data.n_customer_tax_type);
//            return View("editInvoice", data);
//        }
//        public JsonResult LoadItems(int docNo)
//        {
//            return Json(_context.vuw_cars_invoices.Where(x => x.n_document_no == docNo).Select(x => new
//            {
//                n_document_no = x.n_document_no,
//                nLineNo = x.nLineNo,
//                s_item_id = x.s_item_id,
//                n_qty = x.n_qty,
//                n_unit_price = x.n_unit_price,
//                n_unit_id = x.n_unit_id,
//                n_item_value = x.n_item_value,
//                n_item_sales_Tax = x.n_item_sales_Tax,
//                n_item_net_value = x.n_item_net_value,
//                n_item_net_value_WithoutTax = x.n_item_net_value_WithoutTax,
//                nItemDiscountP = x.nItemDiscountP,
//                nItemDiscountV = x.nItemDiscountV,
//                car_model = x.car_model,
//                car_manufacture = x.car_manufacture,
//                s_item_name = x.s_item_name,
//                n_model_no = x.n_model_no,
//                s_logo_url = x.s_logo_url,
//                n_item_expenses=x.n_item_expenses,
//                n_VAT = _config.GetValue<int>("SmartSettings:taxP")
//            }));
//        }

//        [HttpPost]
//        public IActionResult editInvoice(ar_sales_invoice_draft model)
//        {
//            try
//            {
//                double total = Convert.ToDouble(model.n_net_value);

//                var res = _context.ar_sales_invoice_draft.Where(x => x.n_document_no == model.n_document_no).FirstOrDefault();
//                res.n_customer_id = model.n_customer_id;
//                res.d_document_date = model.d_document_date;
//                res.s_description_arabic = model.s_description_arabic;
//                res.s_direct_customer_name = model.s_direct_customer_name;
//                res.s_customer_tel = model.s_customer_tel;
//                res.s_identity = model.s_identity;
//                res.s_revenue_descrip = ToArabicLetter(total);
//                res.n_no_of_items = model.ar_sales_invoice_details_draftLst.Count();
//                res.n_total_qty = model.ar_sales_invoice_details_draftLst.Count();
//                res.n_total_value = model.n_total_value;
//                res.n_total_discount = model.n_total_discount;
//                res.n_sales_Tax = model.n_sales_Tax;
//                res.n_net_value = model.n_net_value;
//                res.n_net_value_after_expenses_tax = model.n_net_value;
//                res.n_extra_expenses = model.n_extra_expenses;
//                res.n_customer_tax_type = model.n_customer_tax_type;

//                SetUpdatedBy(res);
//                _context.ar_sales_invoice_draft.Update(res);

//                //Remove Details
//                var detailsLst = _context.ar_sales_invoice_details_draft.Where(s => s.n_document_no == model.n_document_no).ToList();
//                _context.ar_sales_invoice_details_draft.RemoveRange(detailsLst);

//                //Details Save
//                if (model.ar_sales_invoice_details_draftLst != null)
//                {
//                    int i = 0;
//                    foreach (var detail in model.ar_sales_invoice_details_draftLst)
//                    {
//                        //Add in Reservation table details
//                        i = i + 1;
//                        var ar_sales_invoice_details_draft = new ar_sales_invoice_details_draft();
//                        detail.n_document_no = model.n_document_no;
//                        detail.nLineNo = i;
//                        detail.n_qty = 1;
//                        detail.n_unit_id = 1;
//                        var storeD = _context.sc_current_item_balance.Where(o => o.s_item_id == detail.s_item_id && o.n_current_qty_balance == 1).FirstOrDefault();
//                        if (storeD != null)
//                            detail.n_store_id = storeD.n_store_id;
//                        SetCreatedBy(detail);
//                        SetPropertiesByName(detail, ar_sales_invoice_details_draft);
//                        _context.ar_sales_invoice_details_draft.Add(ar_sales_invoice_details_draft);
//                    }
//                }

//                _context.SaveChanges();
//                return Json(new Logger() { status = Status.Success, UserMessage = "تم الحفظ بنجاح", TempId = 0 });
//            }
//            catch (Exception ex)
//            {
//                return Json(new Logger() { status = Status.Failed, UserMessage = "Error", ErrorMessage = ex.Message, TempId = 0 });
//            }
//        }

//        public IActionResult printInvoice(int docNo)
//        {
//            try
//            {
//                string Condition = "n_document_no = ";

//                WebReport web = new WebReport();
//                var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\invoice.frx";

//                web.Report.Load(path);


//                var mssqlDataConnection = new MsSqlDataConnection();
//                mssqlDataConnection.ConnectionString = _config.GetConnectionString("DefaultConnection");
//                web.Report.Dictionary.Connections[0].ConnectionString = mssqlDataConnection.ConnectionString;
//                TableDataSource data = web.Report.GetDataSource("vuw_cars_invoices") as TableDataSource;



//                data.SelectCommand = "SELECT * from vuw_cars_invoices where " + Condition + docNo;
//                web.Report.Prepare();
//                return View(web);
//            }
//            catch (Exception ex)
//            {
//                return Json(new Logger() { status = Status.Failed, UserMessage = ex.Message + "-" + _config.GetConnectionString("DefaultConnection"), TempId = 0 });
//            }
//        }

//        public IActionResult deleteInvoice(int id)
//        {
//            try
//            {
//                //Delete Master
//                var res = _context.ar_sales_invoice_draft.Where(x => x.n_document_no == id).FirstOrDefault();
//                SetDeletedBy(res);
//                _context.ar_sales_invoice_draft.Update(res);

//                _context.SaveChanges();

//                return Json(new Logger() { status = Status.Success, UserMessage = "تم الحذف بنجاح", TempId = 0 });

//            }
//            catch (Exception ex)
//            {
//                return Json(new Logger() { status = Status.Failed, UserMessage = "Error", ErrorMessage = ex.Message, TempId = 0 });
//            }
//        }


//    }
//}
