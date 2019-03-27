using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TRYExchangeInfoService
{
    /// <summary>
    /// T.C. Central Bank Exchange Info Service
    /// Getting Exchange Info from T.C. Central Bank Service
    /// </summary>
    class getProcess
    {
        /// <summary>
        /// TCMB Base API
        /// </summary>
        private const string ApiBaseUrl = "http://www.tcmb.gov.tr/kurlar/{0}.xml";
        
        /// <summary>
        /// Base API with Date
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// Date from public
        /// </summary>
        public DateTime CurrencyDate { get; set; }

        /// <summary>
        /// Date for API call
        /// </summary>
        public DateTime ActualCurrencyDate { get; set; }

        /// <summary>
        /// XML
        /// </summary>
        private XmlDocument XmlDoc { get; set; }


        /// <summary>
        /// </summary>
        /// <param name="currencyDate">Date from public to Process</param>
        public getProcess(DateTime currencyDate)
        {
            CurrencyDate = currencyDate;
        }

        /// <summary>
        /// Get Call
        /// </summary>
        /// <param name="currency">Exchange Currency</param>
        /// <param name="exchRateType">Exchange Type</param>
        /// <returns></returns>
        public Decimal GetExchRate(string currency, ExchRateType exchRateType)
        {
            bool loadResult=false;
            
            loadResult = LoadExchRate();
            

            // Culture Convert
            System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-US", true);

            // XML Node
            string nodeStr = String.Format("Tarih_Date/Currency[@CurrencyCode='{0}']/{1}", currency.ToUpper(), GetExchRateTypeNodeStr(exchRateType));

            // String to Decimal
            if (loadResult)
                return Decimal.Parse(XmlDoc.SelectSingleNode(nodeStr).InnerXml, culInfo);
            else
                return 0;
        }


        /// <summary>
        /// Create API
        /// </summary>
        private void GenerateApiUrl()
        {
            ApiUrl = String.Format(getProcess.ApiBaseUrl, this.ActualCurrencyDate.ToString("yyyyMM") + "/" + this.ActualCurrencyDate.ToString("ddMMyyyy"));
        }


        /// <summary>
        /// Get Service
        /// </summary>
        public bool LoadExchRate()
        {
            ActualCurrencyDate = CurrencyDate;            

            try
            {
                GenerateApiUrl();

                XmlDoc = new XmlDocument();
                XmlDoc.Load(ApiUrl);
                if (XmlDoc == null)
                {
                    return false;
                }
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
            
        }


        private string GetExchRateTypeNodeStr(ExchRateType exchRateType)
        {
            string ret = "";

            switch (exchRateType)
            {
                case ExchRateType.ForexBuying:
                    ret = "ForexBuying";
                    break;

                case ExchRateType.ForexSelling:
                    ret = "ForexSelling";
                    break;

                case ExchRateType.BanknoteBuying:
                    ret = "BanknoteBuying";
                    break;

                case ExchRateType.BanknoteSelling:
                    ret = "BanknoteSelling";
                    break;
            }

            return ret;
        }
    }


    /// <summary>
    /// Exchange Type
    /// </summary>
    public enum ExchRateType
    {
        ForexBuying,
        ForexSelling,
        BanknoteBuying,
        BanknoteSelling
    }
}
