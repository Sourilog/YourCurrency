/* a RESTFul service which uses attribute based routing with four functionalities of
 *  Creatinf Db, 
 *  Changing DB data, 
 *  Finding a specific currency
 *  Converting currency to one another
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Currency.Models;						 // method currency in controller class                 

namespace Currency.Controllers
{

    // attribute based routing - override convention for all controller actions
    [RoutePrefix("Currency")]				 
    public class CurrencyController : ApiController
    {

        /*
       * GET /currency/all                     retrieve avaliabl currency data		         RetrieveAllCurrencyInformation()  
       * GET /currency/find/Currencyname       get information to find currency             GetInformationForCurrency(currncyname) 
       * POST /currency/SaveNewCurrency		update currency rate						 PostCurrencyChange(CurrencyInformation item)					    
        */

        //Azure  data storage 
        String dataConnectionString = "DefaultEndpointsProtocol=https;AccountName=currencystorage;AccountKey=s15sEadXpqIp0B64aYPvVeIwcAV/ZBVB7YPcTBYnjHztjIVLTi2FeNWaUkjgILFIypuMu2Rj+UJvUk71OGnvmA==;BlobEndpoint=https://currencystorage.blob.core.windows.net/;TableEndpoint=https://currencystorage.table.core.windows.net/;QueueEndpoint=https://currencystorage.queue.core.windows.net/;FileEndpoint=https://currencystorage.file.core.windows.net/";
        //storing a new data table 
        String tableName = "CurrencyDataBase";
        private static List<CurrencyInformation> currency = new List<CurrencyInformation>() 
                { 
		//currency information list for phase 1  with primary rate considered as dollar
                    new CurrencyInformation { Currencyname = "Dollar", CurrencyRate = 1, RateDate = DateTime.Now, CurrencyWarning = false }, 
                    new CurrencyInformation { Currencyname = "Euro", CurrencyRate = 0.876, RateDate =  DateTime.Now, CurrencyWarning = true  }, 
                    new CurrencyInformation { Currencyname = "Pound", CurrencyRate = 0.692, RateDate =  DateTime.Now, CurrencyWarning = false },
                    new CurrencyInformation { Currencyname = "Yen", CurrencyRate = 107.12, RateDate =  DateTime.Now, CurrencyWarning = true  } 
                };
         
       //GET - returns currency rates 
        [Route("all")]                                                                  // on a controller action                                                              
        [HttpGet]                                                                   
        public IHttpActionResult RetrieveAllCurrencyInformation()
        {
			// List defined to store information which will be queried
            List<CurrencyInformation> readcurrency = new List<CurrencyInformation>();
            try
            {

				//Azure data storage 

                CloudStorageAccount account = CloudStorageAccount.Parse(dataConnectionString);
                CloudTableClient tableClient = account.CreateCloudTableClient();
                
              // bind table -  create the table if it doesn't exist
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
			// first-time table creation
         
               foreach (CurrencyInformation c in currency)
                {
              TableOperation insertOperation = TableOperation.Insert(c);
                table.Execute(insertOperation);
                }
           

				//query written for the table that return currency information (CurrencyInformation)
                TableQuery<CurrencyInformation> query;
				// return all info with the condition of "currency name" to be equivalent to dollar /euro/pound/yen
                 query = new TableQuery<CurrencyInformation>().Where(TableQuery.GenerateFilterCondition("Currencyname", QueryComparisons.Equal, "Dollar"));  // all
                  // query date which rate of dollar has been highest available                                                                                                                                             // DateTime t =
             var q=   table.ExecuteQuery(query).Max(w => w.RateDate);
			  // return date which rate of dollar has been highest available 
                var f = table.ExecuteQuery(query).Where(w => w.RateDate.Equals(q));
                if (f != null)
				// data added to the "readcurrency" list created at start of method ALL
                    readcurrency.AddRange(f);
                query = new TableQuery<CurrencyInformation>().Where(TableQuery.GenerateFilterCondition("Currencyname", QueryComparisons.Equal, "Euro"));  // all
                                                                                                                                                            // DateTime t =
                var q2 = table.ExecuteQuery(query).Max(w => w.RateDate);
                var f2 = table.ExecuteQuery(query).Where(w => w.RateDate.Equals(q2));
                if (f2 != null)
                    readcurrency.AddRange(f2);
                query = new TableQuery<CurrencyInformation>().Where(TableQuery.GenerateFilterCondition("Currencyname", QueryComparisons.Equal, "Pound"));  // all
                                                                                                                                                          // DateTime t =
                var q3 = table.ExecuteQuery(query).Max(w => w.RateDate);
                var f3 = table.ExecuteQuery(query).Where(w => w.RateDate.Equals(q3));
                if(f3!=null)
                readcurrency.AddRange(f3);
                query = new TableQuery<CurrencyInformation>().Where(TableQuery.GenerateFilterCondition("Currencyname", QueryComparisons.Equal, "Yen"));  // all
                                                                                                                                                           // DateTime t =
                var q4 = table.ExecuteQuery(query).Max(w => w.RateDate);
                var f4 = table.ExecuteQuery(query).Where(w => w.RateDate.Equals(q4));
                if (f4!= null)
                    readcurrency.AddRange(f4);
                              
            }
            catch (Exception e)
            {
                return Ok(e);
            } // return readcurrency "list" with data stored in order of currencyname
            return Ok(readcurrency.OrderBy(w => w.Currencyname).ToList());

        }

        [Route("Find/{currncyname:alpha}")] //gets a currency name and returns all data related to that currency name recieved 
        public IHttpActionResult GetInformationForCurrency(String currncyname)
        {
			// gets currencyname 
            List<CurrencyInformation> readcurrency = new List<CurrencyInformation>();
            try
            {



                CloudStorageAccount account = CloudStorageAccount.Parse(dataConnectionString);
                CloudTableClient tableClient = account.CreateCloudTableClient();

                // create the table if it doesn't exist
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();

                TableQuery<CurrencyInformation> query;

                query = new TableQuery<CurrencyInformation>().Where(TableQuery.GenerateFilterCondition("Currencyname", QueryComparisons.Equal, currncyname));  // all

                var f = table.ExecuteQuery(query);
                if (f != null)
                    readcurrency.AddRange(f);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
			  //adds currencyname to list read currency
			  // returns list in order of currency name
            return Ok(readcurrency.OrderBy(w => w.Currencyname).ToList());
        }

//Post 
        [Route("SaveNewCurrency")]      //saves new rate to database                             
        public IHttpActionResult PostCurrencyChange(CurrencyInformation item)
        {
            try { 
           
            CloudStorageAccount account = CloudStorageAccount.Parse(dataConnectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();

           
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();


            // insert  entity
            TableOperation insertOperation = TableOperation.Insert(item);


            table.Execute(insertOperation);
            return Ok("Save Currency Successfully");
        }
            catch (Exception e)
            {
                return Ok(e);
    }
}
//GET Method 
        [Route("Convert")]  //convert a currency to another 
        public IHttpActionResult GetConvert(String currncyname_in,String p_in,String currncyname_to)
        {
            try
            {
                CloudStorageAccount account = CloudStorageAccount.Parse(dataConnectionString);
                CloudTableClient tableClient = account.CreateCloudTableClient();
               
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
                TableQuery<CurrencyInformation> query;

                query = new TableQuery<CurrencyInformation>().Where(TableQuery.GenerateFilterCondition("Currencyname", QueryComparisons.Equal, currncyname_in));  // all
                                                                                                                                                            // DateTime t =
                var q = table.ExecuteQuery(query).Max(w => w.RateDate);
                var res = table.ExecuteQuery(query).Where(w => w.RateDate.Equals(q));
                CurrencyInformation currncy_in_info=new CurrencyInformation();
                foreach (CurrencyInformation c in res)
                    currncy_in_info = c;
                     query = new TableQuery<CurrencyInformation>().Where(TableQuery.GenerateFilterCondition("Currencyname", QueryComparisons.Equal, currncyname_to));  // all
                                                                                                                                                                  // DateTime t =
               var q2 = table.ExecuteQuery(query).Max(w => w.RateDate);
           var res2   = table.ExecuteQuery(query).Where(w => w.RateDate.Equals(q2));
                CurrencyInformation currncy_to_info=new CurrencyInformation();
                foreach (CurrencyInformation c in res2)
                    currncy_to_info = c;
                double todoliar = 1 / currncy_in_info.CurrencyRate *double.Parse( p_in);
               double to = todoliar * currncy_to_info.CurrencyRate;

                return Ok(to.ToString());
            }
            catch (Exception e)
            {
                return Ok(e);
            }
            return Ok(currncyname_in.ToString());
        }
        [Route("currency/warning/{warning:bool}")]                               
       
        public IEnumerable<String> GetCurrencyNameForWarningStatus(bool warning)
        {
           
            var cities = currency.Where(w => w.CurrencyWarning == warning).Select(w => w.Currencyname);
            return cities;                                                      
        }

        
    }
}
