using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTables.Console
{

	public class Customers : TableEntity
	{
		public Customers(string firstNamae, string lastName)
		{
			this.PartitionKey = lastName;
			this.RowKey = firstNamae;
		}
		public Customers()
		{

		}
		public string Email { get; set; }		
	}

	class Program
	{
		static void Main(string[] args)
		{
			Customers customer1 = new Customers("Ivan", "Melnyk") { Email = "Ivan@ukr.net" };
			Customers customer2 = new Customers("Petro", "Melnyk") { Email = "Petro@ukr.net" };

			// сторадж=> клієнт => тейбл
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse("StorageConnectionString");
			CloudTableClient client = storageAccount.CreateCloudTableClient();
			CloudTable table = client.GetTableReference("tmpLesson2");

			table.CreateIfNotExists();

			 
			TableBatchOperation batch = new TableBatchOperation();
			batch.Insert(customer1);
			batch.Insert(customer2);
			table.ExecuteBatch(batch);

			// Select 
			//queryBuilder

			TableQuery<Customers> query = new TableQuery<Customers>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Melnyk"));

			foreach (Customers customer in table.ExecuteQuery(query))
			{
				System.Console.WriteLine(customer.PartitionKey);
				System.Console.WriteLine(customer.RowKey);
				System.Console.WriteLine(customer.Email);
				System.Console.WriteLine(customer.Timestamp);
				System.Console.WriteLine("================");			
			}
			


		}
	}
}
