using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SmartAdmin.WebUI.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SmartAdmin.WebUI.Models;
using System.Text;
using FastReport.Web;
using FastReport.Data;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartAdmin.WebUI.Controllers
{
    public class BaseController : Controller
    {
        public IConfiguration _config;
        public ConfigDbContext _context;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public ApplicationDbContext _contextFactory;

        public BaseController(ConfigDbContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            ////Check if session is expired or not
            if (actionName != "Login" && UserID() == 0 && actionName != "SessionExpired")
            {
                context.Result = new RedirectResult(@Url.Action("SessionExpired", "Base"));
            }
            else if (actionName != "Login" && UserID() != 0 && actionName != "SessionExpired")
            {
                string DefaultConnection = "Data Source="+ HttpContext.Session.GetString("serverName") + ";Initial Catalog="+ HttpContext.Session.GetString("databaseName") + ";User Id="+ HttpContext.Session.GetString("dbUser") + "; Password="+ HttpContext.Session.GetString("dbPass") + ";Trusted_Connection=False;MultipleActiveResultSets=true;";
                _contextFactory = new ApplicationDbContext(DefaultConnection);
            }
        }

        public void setSessions(CompanyInfo viewModel)
        {
            try
            {
                HttpContext.Session.SetInt32("userId", Convert.ToInt32(viewModel.n_id));
                HttpContext.Session.SetString("userName",viewModel.s_user_name);
                HttpContext.Session.SetInt32("compId", Convert.ToInt32(viewModel.n_comp_id));
                HttpContext.Session.SetInt32("currentYear", _config.GetValue<int>("SmartSettings:Year"));
                HttpContext.Session.SetInt32("branchId", _config.GetValue<int>("SmartSettings:branch"));
                HttpContext.Session.SetString("databaseName", viewModel.s_db_name);
                HttpContext.Session.SetString("dbUser", viewModel.s_db_user);
                HttpContext.Session.SetString("dbPass", viewModel.s_db_password);
                HttpContext.Session.SetString("serverName", viewModel.s_server);

                string compName = _context.Company.Where(x => x.n_company_id == viewModel.n_comp_id).Select(x => x.s_company_name).FirstOrDefault();
                HttpContext.Session.SetString("compName", compName);

                var CompId = HttpContext.Session.GetInt32("compId").ToString().Length > 1 ? "0" + HttpContext.Session.GetInt32("compId") : "00" + HttpContext.Session.GetInt32("compId");
                var dataAreaID = Convert.ToInt32(HttpContext.Session.GetInt32("currentYear") + CompId + "0" + _config.GetValue<int>("SmartSettings:branch"));
                HttpContext.Session.SetInt32("dataAreaId", dataAreaID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public void SetPropertiesByName(object source, object destination, bool simpleType = true)
        {
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                var type = srcProp.PropertyType;
                bool complex = !IsSimpleType(type);
                if (simpleType && !IsSimpleType(type))
                {
                    continue;
                }
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }

        }

        public static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                new Type[] {
            typeof(Enum),
            typeof(String),
            typeof(Decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }

        public int DataAreaID()
        {
            return Convert.ToInt32(HttpContext.Session.GetInt32("dataAreaId"));
        }
        public int currentYear()
        {
            return Convert.ToInt32(HttpContext.Session.GetInt32("currentYear"));
        }
        public int compId()
        {
            return Convert.ToInt32(HttpContext.Session.GetInt32("compId"));
        }
        public int BranchId()
        {
            return Convert.ToInt32(HttpContext.Session.GetInt32("branchId"));
        }
        public string compName()
        {
            return HttpContext.Session.GetString("compName");
        }
        public int UserID()
        {
            return Convert.ToInt32(HttpContext.Session.GetInt32("userId"));
        }

        public static string GetCurrentDateTime()
        {
            string format = "yyyy/MM/dd HH:mm:ss";
            try
            {
                System.Globalization.CultureInfo en = new System.Globalization.CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = en;
                return DateTime.Now.ToString(format);

            }
            catch (Exception)
            {

                return string.Empty;
            }
        }
        public static string GetCurrentDate(string format = "yyyy/MM/dd")
        {
            try
            {
                System.Globalization.CultureInfo en = new System.Globalization.CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = en;
                return DateTime.Now.ToString(format);

            }
            catch (Exception)
            {

                return string.Empty;
            }
        }

        public void SetCreatedBy(object model)
        {
            try
            {
                Type type = model.GetType();

                PropertyInfo prop = type.GetProperty("d_UserAddDate");
                if (prop != null)
                    prop.SetValue(model, GetCurrentDateTime(), null);

                // set Date
                prop = type.GetProperty("n_UserAdd");
                if (prop != null)
                    prop.SetValue(model, UserID(), null);

                prop = type.GetProperty("n_DataAreaId");
                if (prop != null)
                    prop.SetValue(model, DataAreaID(), null);

                prop = type.GetProperty("n_DataAreaID");
                if (prop != null)
                    prop.SetValue(model, DataAreaID(), null);

                prop = type.GetProperty("n_current_year");
                if (prop != null)
                    prop.SetValue(model, currentYear(), null);

                prop = type.GetProperty("n_current_company");
                if (prop != null)
                    prop.SetValue(model, compId(), null);

                prop = type.GetProperty("n_current_branch");
                if (prop != null)
                    prop.SetValue(model, BranchId(), null);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void SetUpdatedBy(object model)
        {
            try
            {
                Type type = model.GetType();
                PropertyInfo prop = type.GetProperty("d_UserUpdateDate");
                if (prop != null)
                    prop.SetValue(model, GetCurrentDateTime(), null);

                // set Date
                prop = type.GetProperty("n_UserUpdate");
                if (prop != null)
                    prop.SetValue(model, UserID(), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SetDeletedBy(object model)
        {
            try
            {
                Type type = model.GetType();

                PropertyInfo prop = type.GetProperty("b_Deleted");
                if (prop != null)
                    prop.SetValue(model, true, null);

                prop = type.GetProperty("b_deleted");
                if (prop != null)
                    prop.SetValue(model, true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }




        [HttpGet]
        public IActionResult SessionExpired()
        {
            return View();
        }

        //
        public string ToArabicLetter(double givenNumber)
        {
            string ToArabicLetterRet = default;
            string FinalOutput, NumberCurrency, FractionsCurrency;
            string Tafkeet = "";  //" فقط لا غير";

            var WholeNumber = Strings.Split(givenNumber.ToString(), ".");

            NumberCurrency = NumberAsCurrency(Convert.ToDouble(WholeNumber[0]));
            FinalOutput = NumberCurrency;

            if (WholeNumber.Length >= 2)
            {
                if (WholeNumber[1].Length.Equals(1))
                {
                    WholeNumber[1] = WholeNumber[1] + "0";
                }
                else if (WholeNumber[1].Length > 2)
                {
                    WholeNumber[1] = WholeNumber[1].Substring(0, 2);
                }

                FractionsCurrency = FractionAsCurrency(Convert.ToDouble(WholeNumber[1]));
                FinalOutput = FinalOutput + " و" + FractionsCurrency;
            }

            if (!string.IsNullOrEmpty(FinalOutput) & !string.IsNullOrEmpty(FinalOutput))
            {
                FinalOutput = FinalOutput + Tafkeet;
            }

            ToArabicLetterRet = FinalOutput;
            return ToArabicLetterRet;
        }

        public string SFormatNumber(double X)
        {
            string SFormatNumberRet = default;

            string Letter1 = default, Letter2 = default, Letter3 = default, Letter4 = default, Letter5 = default, Letter6 = default;
            string c = Strings.Format(Math.Floor(X), "000000000000");
            double C1 = Conversion.Val(Strings.Mid(c, 12, 1));
            switch (C1)
            {
                case var @case when @case == 1d:
                    {
                        Letter1 = "واحد";
                        break;
                    }
                case var case1 when case1 == 2d:
                    {
                        Letter1 = "اثنان";
                        break;
                    }
                case var case2 when case2 == 3d:
                    {
                        Letter1 = "ثلاثة";
                        break;
                    }
                case var case3 when case3 == 4d:
                    {
                        Letter1 = "اربعة";
                        break;
                    }
                case var case4 when case4 == 5d:
                    {
                        Letter1 = "خمسة";
                        break;
                    }
                case var case5 when case5 == 6d:
                    {
                        Letter1 = "ستة";
                        break;
                    }
                case var case6 when case6 == 7d:
                    {
                        Letter1 = "سبعة";
                        break;
                    }
                case var case7 when case7 == 8d:
                    {
                        Letter1 = "ثمانية";
                        break;
                    }
                case var case8 when case8 == 9d:
                    {
                        Letter1 = "تسعة";
                        break;
                    }
            }


            double C2 = Conversion.Val(Strings.Mid(c, 11, 1));
            switch (C2)
            {
                case var case9 when case9 == 1d:
                    {
                        Letter2 = "عشر";
                        break;
                    }
                case var case10 when case10 == 2d:
                    {
                        Letter2 = "عشرون";
                        break;
                    }
                case var case11 when case11 == 3d:
                    {
                        Letter2 = "ثلاثون";
                        break;
                    }
                case var case12 when case12 == 4d:
                    {
                        Letter2 = "اربعون";
                        break;
                    }
                case var case13 when case13 == 5d:
                    {
                        Letter2 = "خمسون";
                        break;
                    }
                case var case14 when case14 == 6d:
                    {
                        Letter2 = "ستون";
                        break;
                    }
                case var case15 when case15 == 7d:
                    {
                        Letter2 = "سبعون";
                        break;
                    }
                case var case16 when case16 == 8d:
                    {
                        Letter2 = "ثمانون";
                        break;
                    }
                case var case17 when case17 == 9d:
                    {
                        Letter2 = "تسعون";
                        break;
                    }
            }


            if (!string.IsNullOrEmpty(Letter1) & C2 > 1d)
                Letter2 = Letter1 + " و" + Letter2;
            if (string.IsNullOrEmpty(Letter2) | Letter2 is null)
            {
                Letter2 = Letter1;
            }
            if (C1 == 0d & C2 == 1d)
                Letter2 = Letter2 + "ة";
            if (C1 == 1d & C2 == 1d)
                Letter2 = "احدى عشر";
            if (C1 == 2d & C2 == 1d)
                Letter2 = "اثنى عشر";
            if (C1 > 2d & C2 == 1d)
                Letter2 = Letter1 + " " + Letter2;
            double C3 = Conversion.Val(Strings.Mid(c, 10, 1));
            switch (C3)
            {
                case var case18 when case18 == 1d:
                    {
                        Letter3 = "مائة";
                        break;
                    }
                case var case19 when case19 == 2d:
                    {
                        Letter3 = "مائتان";
                        break;
                    }
                case var case20 when case20 > 2d:
                    {
                        Letter3 = Strings.Left(SFormatNumber(C3), Strings.Len(SFormatNumber(C3)) - 1) + "مائة";
                        break;
                    }
            }
            if (!string.IsNullOrEmpty(Letter3) & !string.IsNullOrEmpty(Letter2))
                Letter3 = Letter3 + " و" + Letter2;
            if (string.IsNullOrEmpty(Letter3))
                Letter3 = Letter2;


            double C4 = Conversion.Val(Strings.Mid(c, 7, 3));
            switch (C4)
            {
                case var case21 when case21 == 1d:
                    {
                        Letter4 = "الف";
                        break;
                    }
                case var case22 when case22 == 2d:
                    {
                        Letter4 = "الفان";
                        break;
                    }
                case var case23 when 3d <= case23 && case23 <= 10d:
                    {
                        Letter4 = SFormatNumber(C4) + " آلاف";
                        break;
                    }
                case var case24 when case24 > 10d:
                    {
                        Letter4 = SFormatNumber(C4) + " الف";
                        break;
                    }
            }
            if (!string.IsNullOrEmpty(Letter4) & !string.IsNullOrEmpty(Letter3))
                Letter4 = Letter4 + " و" + Letter3;
            if (string.IsNullOrEmpty(Letter4))
                Letter4 = Letter3;
            double C5 = Conversion.Val(Strings.Mid(c, 4, 3));
            switch (C5)
            {
                case var case25 when case25 == 1d:
                    {
                        Letter5 = "مليون";
                        break;
                    }
                case var case26 when case26 == 2d:
                    {
                        Letter5 = "مليونان";
                        break;
                    }
                case var case27 when 3d <= case27 && case27 <= 10d:
                    {
                        Letter5 = SFormatNumber(C5) + " ملايين";
                        break;
                    }
                case var case28 when case28 > 10d:
                    {
                        Letter5 = SFormatNumber(C5) + " مليون";
                        break;
                    }
            }
            if (!string.IsNullOrEmpty(Letter5) & !string.IsNullOrEmpty(Letter4))
                Letter5 = Letter5 + " و" + Letter4;
            if (string.IsNullOrEmpty(Letter5))
                Letter5 = Letter4;


            double C6 = Conversion.Val(Strings.Mid(c, 1, 3));
            switch (C6)
            {
                case var case29 when case29 == 1d:
                    {
                        Letter6 = "مليار";
                        break;
                    }
                case var case30 when case30 == 2d:
                    {
                        Letter6 = "ملياران";
                        break;
                    }
                case var case31 when case31 > 2d:
                    {
                        Letter6 = SFormatNumber(C6) + " مليار";
                        break;
                    }
            }
            if (!string.IsNullOrEmpty(Letter6) & !string.IsNullOrEmpty(Letter5))
                Letter6 = Letter6 + " و" + Letter5;
            if (string.IsNullOrEmpty(Letter6))
                Letter6 = Letter5;

            SFormatNumberRet = Letter6;
            return SFormatNumberRet;

        }

        public string NumberAsCurrency(double givenNumber)
        {
            string NumberAsCurrencyRet = default;
            string Number, Currency;

            Number = SFormatNumber(givenNumber);

            if (!string.IsNullOrEmpty(Number) & !string.IsNullOrEmpty(Number) & givenNumber <= 2d)
            {
                if (Number.StartsWith("واحد"))
                {
                    Number = Number.Substring(4);
                }
                else if (Number.StartsWith("اثنان"))
                {
                    Number = Number.Substring(5);
                }
            }

            switch (givenNumber)
            {
                case var @case when @case == default(double):
                    {
                        Currency = "";
                        break;
                    }
                case var case1 when case1 == 2d:
                    {
                        Currency = "ريالان سعودي";
                        break;
                    }
                case var case2 when 3d <= case2 && case2 <= 10d:
                    {
                        Currency = "ريال سعودى";
                        break;
                    }

                default:
                    {
                        Currency = "ريال سعودى";
                        break;
                    }
            }

            NumberAsCurrencyRet = Number + " " + Currency;
            return NumberAsCurrencyRet;

        }

        public string FractionAsCurrency(double givenNumber)
        {
            string FractionAsCurrencyRet = default;
            string Fractions, Currency;

            Fractions = SFormatNumber(givenNumber);

            if (!string.IsNullOrEmpty(Fractions) & !string.IsNullOrEmpty(Fractions) & givenNumber <= 2d)
            {
                if (Fractions.StartsWith("واحد"))
                {
                    Fractions = Fractions.Substring(4);
                }
                else if (Fractions.StartsWith("اثنان"))
                {
                    Fractions = Fractions.Substring(5);
                }
            }

            switch (givenNumber)
            {
                case var @case when @case == default(double):
                    {
                        Currency = "";
                        break;
                    }
                case var case1 when case1 == 2d:
                    {
                        Currency = "هللتان";
                        break;
                    }
                case var case2 when 3d <= case2 && case2 <= 10d:
                    {
                        Currency = "هللة";
                        break;
                    }

                default:
                    {
                        Currency = "هللة";
                        break;
                    }
            }

            FractionAsCurrencyRet = Fractions + " " + Currency;
            return FractionAsCurrencyRet;

        }

        //public string ConvertNumbersToArabicAlphabet(string Number)
        //{
        //    return ConvertNumberToAlpha(Number);
        //}
        //private string ConvertNumberToAlpha(string Number)
        //{
        //    if (Number.Contains('.'))
        //    {
        //        if (Number.Split('.')[0].ToCharArray().Length > 6)
        //        {
        //            return "No Number";
        //        }
        //        else
        //        {
        //            switch (Number.Split('.')[0].ToCharArray().Length)
        //            {
        //                case 1: return convertOneDigits(Number.ToString()) + " ريال سعودى " + " و " + convertTwoDigits(Number.Split('.')[1]) + " هلله ";
        //                case 2: return convertTwoDigits(Number.ToString()) + " ريال سعودى " + " و " + convertTwoDigits(Number.Split('.')[1]) + " هلله ";
        //                case 3: return convertThreeDigits(Number.ToString()) + " ريال سعودى " + " و " + convertTwoDigits(Number.Split('.')[1]) + " هلله ";
        //                case 4: return convertFourDigits(Number.ToString()) + " ريال سعودى " + " و " + convertTwoDigits(Number.Split('.')[1]) + " هلله ";
        //                case 5: return convertFiveDigits(Number.ToString()) + " ريال سعودى " + " و " + convertTwoDigits(Number.Split('.')[1]) + " هلله ";
        //                case 6: return convertSixDigits(Number.ToString()) + " ريال سعودى " + " و " + convertTwoDigits(Number.Split('.')[1]) + " هلله ";
        //                default: return "";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (Number.ToCharArray().Length > 6)
        //        {
        //            return "No Number";
        //        }
        //        else
        //        {
        //            switch (Number.ToCharArray().Length)
        //            {
        //                case 1: return convertOneDigits(Number.ToString()) + " ريال سعودى ";
        //                case 2: return convertTwoDigits(Number.ToString()) + " ريال سعودى ";
        //                case 3: return convertThreeDigits(Number.ToString()) + " ريال سعودى ";
        //                case 4: return convertFourDigits(Number.ToString()) + " ريال سعودى ";
        //                case 5: return convertFiveDigits(Number.ToString()) + " ريال سعودى ";
        //                case 6: return convertSixDigits(Number.ToString()) + " ريال سعودى ";
        //                default: return "";
        //            }

        //        }
        //    }
        //}
        //private string convertTwoDigits(string TwoDigits)
        //{
        //    string returnAlpha = "00";
        //    if (TwoDigits.ToCharArray()[0] == '0' && TwoDigits.ToCharArray()[1] != '0')
        //    {
        //        return convertOneDigits(TwoDigits.ToCharArray()[1].ToString());
        //    }
        //    else
        //    {
        //        switch (int.Parse(TwoDigits.ToCharArray()[0].ToString()))
        //        {
        //            case 1:
        //                {
        //                    if (int.Parse(TwoDigits.ToCharArray()[1].ToString()) == 1)
        //                    {
        //                        return "إحدى عشر";
        //                    }
        //                    else if (int.Parse(TwoDigits.ToCharArray()[1].ToString()) == 2)
        //                    {
        //                        return "إثنى عشر";
        //                    }
        //                    else
        //                    {
        //                        returnAlpha = "عشر";
        //                        return convertOneDigits(TwoDigits.ToCharArray()[1].ToString()) + " " + returnAlpha;
        //                    }
        //                }
        //            case 2: returnAlpha = "عشرون"; break;
        //            case 3: returnAlpha = "ثلاثون"; break;
        //            case 4: returnAlpha = "أريعون"; break;
        //            case 5: returnAlpha = "خمسون"; break;
        //            case 6: returnAlpha = "ستون"; break;
        //            case 7: returnAlpha = "سبعون"; break;
        //            case 8: returnAlpha = "ثمانون"; break;
        //            case 9: returnAlpha = "تسعون"; break;
        //            default: returnAlpha = ""; break;
        //        }
        //    }
        //    if (convertOneDigits(TwoDigits.ToCharArray()[1].ToString()).Length == 0)
        //    { return returnAlpha; }
        //    else
        //    {
        //        return convertOneDigits(TwoDigits.ToCharArray()[1].ToString()) + " و " + returnAlpha;
        //    }
        //}
        //private string convertOneDigits(string OneDigits)
        //{
        //    switch (int.Parse(OneDigits))
        //    {
        //        case 1: return "واحد";
        //        case 2: return "إثنان";
        //        case 3: return "ثلاثه";
        //        case 4: return "أربعه";
        //        case 5: return "خمسه";
        //        case 6: return "سته";
        //        case 7: return "سبعه";
        //        case 8: return "ثمانيه";
        //        case 9: return "تسعه";
        //        default: return "";
        //    }
        //}
        //private string convertThreeDigits(string ThreeDigits)
        //{
        //    switch (int.Parse(ThreeDigits.ToCharArray()[0].ToString()))
        //    {
        //        case 1:
        //            {
        //                if (int.Parse(ThreeDigits.ToCharArray()[1].ToString()) == 0)
        //                {
        //                    if (int.Parse(ThreeDigits.ToCharArray()[2].ToString()) == 0)
        //                    {
        //                        return "مائه";
        //                    }
        //                    return "مائه" + " و " + convertOneDigits(ThreeDigits.ToCharArray()[2].ToString());
        //                }
        //                else
        //                {
        //                    return "مائه" + " و " + convertTwoDigits(ThreeDigits.Substring(1, 2));
        //                }
        //            }
        //        case 2:
        //            {
        //                if (int.Parse(ThreeDigits.ToCharArray()[1].ToString()) == 0)
        //                {
        //                    if (int.Parse(ThreeDigits.ToCharArray()[2].ToString()) == 0)
        //                    {
        //                        return "مائتين";
        //                    }
        //                    return "مائتين" + " و " + convertOneDigits(ThreeDigits.ToCharArray()[2].ToString());
        //                }
        //                else
        //                {
        //                    return "مائتين" + " و " + convertTwoDigits(ThreeDigits.Substring(1, 2));
        //                }
        //            }
        //        case 3:
        //            {
        //                if (int.Parse(ThreeDigits.ToCharArray()[1].ToString()) == 0)
        //                {
        //                    if (int.Parse(ThreeDigits.ToCharArray()[2].ToString()) == 0)
        //                    {
        //                        return convertOneDigits(ThreeDigits.ToCharArray()[0].ToString()).Split('ه')[0] + "مائه";
        //                    }
        //                    return convertOneDigits(ThreeDigits.ToCharArray()[0].ToString()).Split('ه')[0] + "مائه" + " و " + convertOneDigits(ThreeDigits.ToCharArray()[2].ToString());
        //                }
        //                else
        //                {
        //                    return convertOneDigits(ThreeDigits.ToCharArray()[0].ToString()).Split('ه')[0] + "مائه" + " و " + convertTwoDigits(ThreeDigits.Substring(1, 2));
        //                }
        //            }
        //        case 4:
        //            {
        //                goto case 3;
        //            }
        //        case 5:
        //            {
        //                goto case 3;
        //            }
        //        case 6:
        //            {
        //                goto case 3;
        //            }
        //        case 7:
        //            {
        //                goto case 3;
        //            }
        //        case 8:
        //            {
        //                goto case 3;
        //            }
        //        case 9:
        //            {
        //                goto case 3;
        //            }
        //        case 0:
        //            {
        //                if (ThreeDigits.ToCharArray()[1] == '0')
        //                {
        //                    if (ThreeDigits.ToCharArray()[2] == '0')
        //                    {
        //                        return "";
        //                    }
        //                    else
        //                    {
        //                        return convertOneDigits(ThreeDigits.ToCharArray()[2].ToString());
        //                    }
        //                }
        //                else
        //                {
        //                    return convertTwoDigits(ThreeDigits.Substring(1, 2));
        //                }
        //            }
        //        default: return "";
        //    }
        //}
        //private string convertFourDigits(string FourDigits)
        //{
        //    switch (int.Parse(FourDigits.ToCharArray()[0].ToString()))
        //    {
        //        case 1:
        //            {
        //                if (int.Parse(FourDigits.ToCharArray()[1].ToString()) == 0)
        //                {
        //                    if (int.Parse(FourDigits.ToCharArray()[2].ToString()) == 0)
        //                    {
        //                        if (int.Parse(FourDigits.ToCharArray()[3].ToString()) == 0)
        //                            return "ألف";
        //                        else
        //                        {
        //                            return "ألف" + " و " + convertOneDigits(FourDigits.ToCharArray()[3].ToString());
        //                        }
        //                    }
        //                    return "ألف" + " و " + convertTwoDigits(FourDigits.Substring(2, 2));
        //                }
        //                else
        //                {
        //                    return "ألف" + " و " + convertThreeDigits(FourDigits.Substring(1, 3));
        //                }
        //            }
        //        case 2:
        //            {
        //                if (int.Parse(FourDigits.ToCharArray()[1].ToString()) == 0)
        //                {
        //                    if (int.Parse(FourDigits.ToCharArray()[2].ToString()) == 0)
        //                    {
        //                        if (int.Parse(FourDigits.ToCharArray()[3].ToString()) == 0)
        //                            return "ألفين";
        //                        else
        //                        {
        //                            return "ألفين" + " و " + convertOneDigits(FourDigits.ToCharArray()[3].ToString());
        //                        }
        //                    }
        //                    return "ألفين" + " و " + convertTwoDigits(FourDigits.Substring(2, 2));
        //                }
        //                else
        //                {
        //                    return "ألفين" + " و " + convertThreeDigits(FourDigits.Substring(1, 3));
        //                }
        //            }
        //        case 3:
        //            {
        //                if (int.Parse(FourDigits.ToCharArray()[1].ToString()) == 0)
        //                {
        //                    if (int.Parse(FourDigits.ToCharArray()[2].ToString()) == 0)
        //                    {
        //                        if (int.Parse(FourDigits.ToCharArray()[3].ToString()) == 0)
        //                            return convertOneDigits(FourDigits.ToCharArray()[0].ToString()) + " ألاف";
        //                        else
        //                        {
        //                            return convertOneDigits(FourDigits.ToCharArray()[0].ToString()) + " ألاف" + " و " + convertOneDigits(FourDigits.ToCharArray()[3].ToString());
        //                        }
        //                    }
        //                    return convertOneDigits(FourDigits.ToCharArray()[0].ToString()) + " ألاف" + " و " + convertTwoDigits(FourDigits.Substring(2, 2));
        //                }
        //                else
        //                {
        //                    return convertOneDigits(FourDigits.ToCharArray()[0].ToString()) + " ألاف" + " و " + convertThreeDigits(FourDigits.Substring(1, 3));
        //                }
        //            }
        //        case 4:
        //            {
        //                goto case 3;
        //            }
        //        case 5:
        //            {
        //                goto case 3;
        //            }
        //        case 6:
        //            {
        //                goto case 3;
        //            }
        //        case 7:
        //            {
        //                goto case 3;
        //            }
        //        case 8:
        //            {
        //                goto case 3;
        //            }
        //        case 9:
        //            {
        //                goto case 3;
        //            }
        //        default: return "";
        //    }
        //}
        //private string convertFiveDigits(string FiveDigits)
        //{
        //    if (convertThreeDigits(FiveDigits.Substring(2, 3)).Length == 0)
        //    {
        //        return convertTwoDigits(FiveDigits.Substring(0, 2)) + " ألف ";
        //    }
        //    else
        //    {
        //        return convertTwoDigits(FiveDigits.Substring(0, 2)) + " ألفا " + " و " + convertThreeDigits(FiveDigits.Substring(2, 3));
        //    }
        //}
        //private string convertSixDigits(string SixDigits)
        //{
        //    if (convertThreeDigits(SixDigits.Substring(2, 3)).Length == 0)
        //    {
        //        return convertThreeDigits(SixDigits.Substring(0, 3)) + " ألف ";
        //    }
        //    else
        //    {
        //        return convertThreeDigits(SixDigits.Substring(0, 3)) + " ألفا " + " و " + convertThreeDigits(SixDigits.Substring(3, 3));
        //    }
        //}



    }
}
