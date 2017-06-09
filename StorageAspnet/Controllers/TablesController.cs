using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using StorageAspnet.Models;

namespace StorageAspnet.Controllers
{
    public class TablesController : Controller
    {
        // GET: Tables закоментил раздел создание таблиц с сайта
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateTable()  //создаем таблицу
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("<bora>_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            ViewBag.Success = table.CreateIfNotExists();
            ViewBag.TableName = table.Name;
            return View();
        }

        public ActionResult AddEntity()     //создаем сущность
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("<bora>_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            CustomerEntity customer1 = new CustomerEntity("Harp", "Walter");
            customer1.Email = "Walter@contoso.com";
            TableOperation insertOperation = TableOperation.Insert(customer1);
            TableResult result = table.Execute(insertOperation);
            ViewBag.TableName = table.Name;
            ViewBag.Result = result.HttpStatusCode;
            return View();
        }

        public ActionResult AddEntities()   //добавляем сущность
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("<bora>_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");

            CustomerEntity customer1 = new CustomerEntity("Smith", "Jeff");
            customer1.Email = "Jeff@contoso.com";
            CustomerEntity customer2 = new CustomerEntity("Smith", "Ben");
            customer2.Email = "Ben@contoso.com";

            TableBatchOperation batchOperation = new TableBatchOperation();

            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);

            IList<TableResult> results = table.ExecuteBatch(batchOperation);

            return View(results);
        }


        public ActionResult GetSingle() //получение одной сущности
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("<bora>_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");
            TableResult result = table.Execute(retrieveOperation);

            return View(result);
        }

        public ActionResult GetPartition()  //получение всех сущностей
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("<bora>_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

            List<CustomerEntity> customers = new List<CustomerEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<CustomerEntity> resultSegment = table.ExecuteQuerySegmented(query, token);
                token = resultSegment.ContinuationToken;

                foreach (CustomerEntity customer in resultSegment.Results)
                {
                    customers.Add(customer);
                }
            } while (token != null);

            return View(customers);

        }

        public ActionResult DeleteEntity()  //удаление сущности
        {
           
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("<bora>_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("TestTable");
            TableOperation deleteOperation = TableOperation.Delete(new CustomerEntity("Smith", "Ben") { ETag = "*" });
            TableResult result = table.Execute(deleteOperation);

            return View(result);

        }

    }
}