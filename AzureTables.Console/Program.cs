using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureApp
{

    class Customers : TableEntity
    {
        public Customers()
        {
        }

        public Customers(string firstName, string lastName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public string Email { get; set; }
    }


    class Program
    {
      
        static void Main(string[] args)
        {

            //Customers customer1 = new Customers("Ivan", "Stepanov");
            //customer1.Email = "ivanov@gmail.com";

            //Customers customer2 = new Customers("Petro", "Stepanov");
            //customer2.Email = "petrov@gmail.com";



            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnections"));

            CloudTableClient client = storageAccount.CreateCloudTableClient();

            CloudTable table = client.GetTableReference("tereshchenko");

            table.CreateIfNotExists();

            TableBatchOperation batch = new TableBatchOperation();

            //batch.Insert(customer1);
            //batch.Insert(customer2);

            //table.ExecuteBatch(batch);


            //SELECT

            TableQuery<Customers> query = new TableQuery<Customers>().Where(
                //query builder
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Melnik")

                );

			


			TableQuery<Customers> queryOR = new TableQuery<Customers>().Where(
				CombineFilterOR(TableQuery.GenerateFilterCondition("PartionKey", QueryComparisons.Equal, "Melnik"), TableQuery.GenerateFilterCondition("PartionKey", QueryComparisons.Equal, "Stepanov"))
				);

			var xxx = table.ExecuteQuery(queryOR);
			foreach (Customers customer in table.ExecuteQuery(queryOR))
			{
				Console.WriteLine(customer.PartitionKey);
				Console.WriteLine(customer.RowKey);
				Console.WriteLine(customer.Email);
				Console.WriteLine("----------------------------");
			}


			//get 1 Entity
			TableOperation getOneOperation =  TableOperation.Retrieve<Customers>("Melnik", "Ivan");
			TableResult result = table.Execute(getOneOperation);

			if (result!= null)
			{
				Customers cr = (Customers)(result).Result;
				Console.WriteLine("---------Retrieve-------------");
				Console.WriteLine($"{cr.PartitionKey} {cr.RowKey} {cr.Email} {cr.Timestamp}");

				//UPDATE

				cr.Email = "UPDATED EMAIL";
				TableOperation updateOper = TableOperation.InsertOrReplace(cr);
				table.Execute(updateOper);

				TableOperation daleteOper = TableOperation.Delete(cr);
				table.Execute(daleteOper);

			}

			foreach (Customers customer in table.ExecuteQuery(query))
			{
				Console.WriteLine(customer.PartitionKey);
				Console.WriteLine(customer.RowKey);
				Console.WriteLine(customer.Email);
				Console.WriteLine("----------------------------");
			}

			Console.ReadLine();
		}




		public static string CombineFilterOR(string condition1, string condition2)
        {
            string where = TableQuery.CombineFilters(condition1, TableOperators.Or, condition2);
            return where;		             
        }

        public static string CombineFilterAND(string condition1, string condition2)
        {
            string where = TableQuery.CombineFilters(condition1, TableOperators.And, condition2);
            return where;
        }
    }
}
