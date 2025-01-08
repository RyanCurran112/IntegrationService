using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BehrensGroup_MiddlewareIQ.Tools
{
    class MiscFunctions
    {
        public static string TrainingBehrensAccountCode { get; set; }
        public static string BehrensAccountCode { get; set; }

        public static string GetAccountCode(string AccountCode)
        {
            string AccountCodesPath = @"\\APP01\SalesOrders\Resources\CPTAccountCodes2.csv";
            string CTAccountCode;

            StreamReader addressFile = new StreamReader(AccountCodesPath);

            string line;
            int result;

            try
            {
                if (!int.TryParse(AccountCode, out result))
                {
                    while ((line = addressFile.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        CTAccountCode = values[0];
                        if (CTAccountCode == AccountCode)
                        {
                            //TrainingBehrensAccountCode = values[0];
                            BehrensAccountCode = values[1];
                            break;
                        }
                        else
                        {
                            BehrensAccountCode = "";
                        }
                    }
                }
                else
                {
                    BehrensAccountCode = AccountCode;
                }
                return BehrensAccountCode;
            }
            catch
            {
                return "";
            }
        }

        public static bool GetDOHCode(string DOHCode)
        {
            string AccountCodesPath = @"\\APP01\SalesOrders\Resources\SCDeliveryContacts.csv";
            string SCDeliveryContact;

            StreamReader addressFile = new StreamReader(AccountCodesPath);

            string line;
            int result;

            try
            {
                if (!int.TryParse(DOHCode, out result))
                {
                    while ((line = addressFile.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        SCDeliveryContact = values[0];
                        if (SCDeliveryContact == DOHCode)
                        {
                            result = 1;
                            break;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                }
                else
                {
                    result = 0;
                }

                if (result == 1)
                {
                    return true;
                }
                else
                {
                    StreamWriter addressfile = new StreamWriter(AccountCodesPath);
                    addressfile.WriteLine(DOHCode);

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetPromotionCode(string PromoCode, string Division)
        {
            string CurrentDateTime = DateTime.Now.ToString();
            string PromoCodesPath = @"\\APP01\SalesOrders\Resources\PromoCodes.csv";
            string NewPromoCodesPath = @"\\APP01\SalesOrders\DipAndDoze\PromotionCodes\PromoCode - " + CurrentDateTime + ".csv";
            string PCPromoCode;
            string PCDivision;

            StreamReader promoFile = new StreamReader(PromoCodesPath);

            string line;
            string PromotionCode = "";

            try
            {
                while ((line = promoFile.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    PCDivision = values[0];
                    PCPromoCode = values[1];

                    if (Division == PCDivision && PromoCode == PCPromoCode)
                    {
                        PromotionCode = values[2];
                        break;
                    }
                    else
                    {
                        PromotionCode = "";
                    }
                }

                if (PromotionCode == "")
                {
                    StreamWriter file = new StreamWriter(NewPromoCodesPath, true);
                    file.WriteLine(PromoCode);
                    file.Close();

                    StreamWriter newFile = new StreamWriter(PromoCodesPath, true);
                    newFile.WriteLine(Division + "," + PromoCode + "," + PromoCode);
                    newFile.Close();

                    PromotionCode = PromoCode;
                }
                return PromotionCode;
            }
            catch
            {
                return "";
            }
        }
    }

    static class Extensions
    {
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }

    public static class StringHelper
    {
        public static string ToTitleCase(this string str)
        {
            var tokens = str.Split(new[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token == token.ToUpper()
                    ? token
                    : token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return string.Join(" ", tokens);
        }
    }
}
