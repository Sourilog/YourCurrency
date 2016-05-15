using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Currency.Models
{
    // Currency info: currency name, currency id, curency rate, currency date
    public class CurrencyInformation: TableEntity
    {
		// create new rowkey
        public CurrencyInformation()
        {
            RowKey = Guid.NewGuid().ToString();
        }
		//get currency name from user 
        public String Currencyname
        {
            get
            {
                return PartitionKey;
            }
            set  
            {
                PartitionKey = value;
            }
        }

        // assing currency name recieved to value
        public String CurrencyID
        {
            get
            {
                return RowKey;
            }
            set
            {
                RowKey = value;
            }
        }
		//currency rate information
        public double CurrencyRate
        {
            get;
            set;
        }
		//currency date and time information
        public DateTime RateDate
        {
            get;
            set;
        }

   
        public bool CurrencyWarning { get; set; }
    }
}