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
    class ExchangeRate : IActionHandler
    {
        public List<string> CallCommandList
        {
            get { return new List<string>() { "курс", "exrate" }; }
        }

        public string CommandDescription
        {
            get { return "курсы валют по НацБанку. Для справки добавьте \"помощь\" или \"help\""; }
        }


        public ExRatesSoapClient rates = new ExRatesSoapClient();
        public List<int> requiredCurrencies = new List<int> { 978, 840, 643 };//EUR, USD, RUB //826=GBP, 
        public CultureInfo ruCulture = new CultureInfo("ru-RU", false);

        private DateTime _lastUpdated_exRatesDaily;
        private DataSet _exRatesDaily;
        public DataSet ExRatesDaily { get { return getExRatesDaily(); } }

        private DateTime _lastUpdated_currenciesRef;
        private DataSet _currenciesRef;
        public DataSet CurrenciesRef { get { return getCurrenciesRef(); } }

        private DateTime _lastUpdated_refRateOnDate;
        private DataSet _refRateOnDate;
        public DataSet RefRateOnDate { get { return getRefRateOnDate(); } }

        private DataSet getRefRateOnDate()
        {
            if ((DateTime.Today != _lastUpdated_refRateOnDate) || (!_refRateOnDate.IsInitialized))
            {
                _refRateOnDate = rates.RefRateOnDate(DateTime.Today);
                if (_refRateOnDate.IsInitialized)
                    _lastUpdated_refRateOnDate = DateTime.Today;
            }
            return _refRateOnDate;
        }

        private DataSet getCurrenciesRef()
        {
            if ((DateTime.Today != _lastUpdated_currenciesRef) || (!_currenciesRef.IsInitialized))
            {
                _currenciesRef = rates.CurrenciesRef(0);
                if (_currenciesRef.IsInitialized)
                    _lastUpdated_currenciesRef = DateTime.Today;
            }
            return _currenciesRef;
        }

        private DataSet getExRatesDaily()
        {
            if ((DateTime.Today != _lastUpdated_exRatesDaily) || (!_exRatesDaily.IsInitialized))
            {
                _exRatesDaily = rates.ExRatesDaily(DateTime.Today);
                if (_exRatesDaily.IsInitialized)
                    _lastUpdated_exRatesDaily = DateTime.Today;
            }
            return _exRatesDaily;
        }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            string[] arg = args.Split(' ');
            string result = "";

            switch (arg[0])
            {
                case "помощь":
                    result = getHelp(false);
                    break;
                case "help":
                    result = getHelp(true);
                    break;
                case "коды":
                    result = getCurrencies(false);
                    break;
                case "codes":
                    result = getCurrencies(true);
                    break;
                case "все":
                case "all":
                    result = getExRates(true);
                    break;
                case "ставкареф":
                case "refrate":
                    result = getRefRate();
                    break;
                default:
                    result = getConvertedCurrencies(arg);
                    break;
            }

            sendMessageFunc(result);
        }

        //сконвертировать из одной валюты в другую
        private string getConvertedCurrencies(string[] arg)
        {
            if (arg.Length < 2) return getExRates(false);
            decimal amountToConvert = 0, convertedAmount=0, srcExRate=0, dstExRate=0;
            int srcCurrScale=0, dstCurrScale=0;
            String srcExCurr = arg[1].ToUpper(), dstExCurr="BYR", unknownCurrencyCode="";
            if (arg.Length>2) dstExCurr=arg[2].ToUpper();

            if (!decimal.TryParse(arg[0], out amountToConvert)) return getExRates(false);;

            if (ExRatesDaily.IsInitialized)
            {
                if (srcExCurr == "BYR")
                {
                    srcExRate = 1;
                    srcCurrScale = 1;
                }
                if (dstExCurr == "BYR")
                {
                    dstExRate = 1;
                    dstCurrScale = 1;
                }

                string currShortName;
                foreach (DataRow row in ExRatesDaily.Tables[0].Rows)
                {
                    currShortName = row.ItemArray[4].ToString().ToUpper();//currency shortname
                    if (srcExCurr == currShortName)
                    {
                        srcExRate = decimal.Parse(row.ItemArray[2].ToString());
                        srcCurrScale = int.Parse(row.ItemArray[1].ToString());
                    }
                    if (dstExCurr == currShortName)
                    {
                        dstExRate = decimal.Parse(row.ItemArray[2].ToString());
                        dstCurrScale = int.Parse(row.ItemArray[1].ToString());
                    }
                }

                if ((srcExRate > 0) && (srcCurrScale > 0))
                {
                    convertedAmount = amountToConvert * srcExRate / srcCurrScale;
                    if ((dstExRate > 0) && (dstCurrScale > 0))
                    {
                        convertedAmount = convertedAmount / dstExRate * dstCurrScale;
                    }
                    else {
                        unknownCurrencyCode += dstExCurr + " - неизвестный код валюты. \n";
                        dstExCurr = "BYR";
                    }
                }
                else
                {
                    unknownCurrencyCode += srcExCurr + " - неизвестный код валюты. \n";
                    return unknownCurrencyCode + getExRates(false);
                }
                ruCulture.NumberFormat.NumberGroupSeparator = " ";
                return string.Format("{0}{1} {2} = {3} {4}", unknownCurrencyCode, amountToConvert.ToString("N", ruCulture.NumberFormat), srcExCurr, convertedAmount.ToString("N", ruCulture.NumberFormat), dstExCurr);
            }
            else {
                return getExRates(false);
            }
        }

        //курсы валют
        private string getExRates(bool getAll)
        {
            StringBuilder result = new StringBuilder();

            if (ExRatesDaily.IsInitialized)
            {
                DateTime ExRateDate = DateTime.Parse(ExRatesDaily.ExtendedProperties["onDate"].ToString(), CultureInfo.InvariantCulture);
                result.Append("Курс валют НБРБ на ");
                result.AppendLine(ExRateDate.Date.ToString("dd MMMM yyyy", ruCulture.DateTimeFormat));

                foreach (DataRow row in ExRatesDaily.Tables[0].Rows)
                {
                    if (!getAll && !requiredCurrencies.Contains((int.Parse(row.ItemArray[3].ToString()))))
                    {
                        continue;
                    }
                    result.Append(row.ItemArray[0]); //currency full name
                    result.Append(" (");
                    result.Append(row.ItemArray[4]); //currency shortname
                    result.Append(") = ");
                    decimal currVal = decimal.Parse(row.ItemArray[2].ToString());
                    ruCulture.NumberFormat.NumberGroupSeparator = " ";
                    result.AppendLine(currVal.ToString("N", ruCulture.NumberFormat)); //BRB value
                }
            }
            return result.ToString();
        }

        //список кодов валют
        private string getCurrencies(bool eng)
        {
            StringBuilder result = new StringBuilder();
            if (CurrenciesRef.IsInitialized & ExRatesDaily.IsInitialized)
            {
                //т.к. полный справочник валют весьма большой, 
                // а ежедневные курсы обмена не содержат все валюты, то
                // выбираем только то что необходимо
                foreach (DataRow row1 in ExRatesDaily.Tables[0].Rows)
                {
                    foreach (DataRow row in CurrenciesRef.Tables[0].Rows)
                    {
                        if (row1.ItemArray[3].ToString() == row.ItemArray[4].ToString())//currencycodes
                        {
                            result.Append(row1.ItemArray[4]); //currency abreviation
                            result.Append(" - ");
                            if (eng)
                            {
                                result.Append(row.ItemArray[7]); //currency fullname
                            }
                            else
                            {
                                result.Append(row.ItemArray[6]); //currency fullname
                            }
                            result.AppendLine();
                            break;
                        }
                    }
                }
            }
            else
            {
                result.AppendLine("Ошибка выборки данных");
            }
            return result.ToString();
        }

        private string getHelp(bool eng)
        {
            StringBuilder result = new StringBuilder();
            if (eng)
            {
                result.AppendLine("Additional arguments:");
                result.AppendLine("all - all today's exchange rates");
                result.AppendLine("codes - list of currency codes");
                result.AppendLine("refrate - refinancing rate");
                result.AppendLine("<amount> <currency code> - convert given amount from given currency to belorussian roubles");
                result.AppendLine("<amount> <src currency code> <dst currency code> - convert given amount from src currency to dst currency");
            }
            else
            {
                result.AppendLine("Дополнительные аргументы:");
                result.AppendLine("все - все курсы валют на сегодня");
                result.AppendLine("коды - справочник кодов валют");
                result.AppendLine("ставкареф - ставка рефинансирования");
                result.AppendLine("<сумма> <код валюты> - сконвертировать указанную сумму из указанной валюты в белорусские рубли");
                result.AppendLine("<сумма> <исходный код валюты> <конечный код валюты> - сконвертировать указанную сумму из исходной валюты в конечную валюту");
            }
            return result.ToString();
        }

        //ставка рефинансирования
        private string getRefRate()
        {
            string result;
            if (RefRateOnDate.IsInitialized)
            {
                DateTime RefRateDate = DateTime.Parse(RefRateOnDate.Tables[0].Rows[0].ItemArray[0].ToString(), CultureInfo.InvariantCulture);
                string refRateValue = RefRateOnDate.Tables[0].Rows[0].ItemArray[1].ToString();
                result = string.Format("Ставка рефинансирования составляет {0}%. Установлена {1}", refRateValue, RefRateDate.Date.ToString("dd MMMM yyyy", ruCulture.DateTimeFormat));
            }
            else
            {
                result = "Ошибка выборки данных";
            }
            return result;
        }
    }
}
