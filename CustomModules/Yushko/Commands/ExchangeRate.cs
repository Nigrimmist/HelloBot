using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloBotCommunication;
using Yushko.ExRates;

namespace Yushko.Commands
{
    class ExchangeRate:IActionHandler
    {
        public List<string> CallCommandList
        {
             get { return new List<string>() { "курс" }; }
        }

        public string CommandDescription
        {
            get { return "курсы валют по НацБанку"; }
        }


        public static ExRatesSoapClient rates = new ExRatesSoapClient();
        public static List<int> requiredCurrencies = new List<int> { 978, 840, 826, 643 };//EUR, USD, GBP, RUB

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            DataSet ds = new DataSet();
            StringBuilder output = new StringBuilder();

            CultureInfo ruCulture = new CultureInfo( "ru-RU", false );

            ds = rates.ExRatesDaily(DateTime.Today);
            DateTime ExRateDate = DateTime.Parse(ds.ExtendedProperties["onDate"].ToString(),CultureInfo.InvariantCulture);
            output.Append("Курс валют НБРБ на ");
            output.AppendLine(ExRateDate.Date.ToString("dd MMMM yyyy",ruCulture.DateTimeFormat));

            foreach (DataRow row in ds.Tables[0].Rows){
                if (requiredCurrencies.Contains((int.Parse(row.ItemArray[3].ToString())))){
                    output.Append(row.ItemArray[0]); //currency full name
                    output.Append(" (");
                    output.Append(row.ItemArray[4]); //currency shortname
                    output.Append(") = ");
                    decimal currVal =decimal.Parse(row.ItemArray[2].ToString());
                    ruCulture.NumberFormat.NumberGroupSeparator = " ";
                    output.AppendLine(currVal.ToString("N",ruCulture.NumberFormat)); //BRB value
                }
            }

            sendMessageFunc(output.ToString());
        }

    }
}
