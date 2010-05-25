using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using Shopster.API.Service.SDK.Core.Soap;

namespace Shopsterify.Database
{
	//Ugly for now
	class ShopsterifyDatabase
	{
const string databaseIP = "127.0.0.1";
const string databasePort = "3306";
const string databaseName = "shopsterify";
const string databaseUser = "root";
const string databasePw = "123.123";
const int FLAG_BIG_PACKETS = 8;
const int FLAG_NO_PROMPT = 16;
OdbcConnection dbConn;

		public ShopsterifyDatabase()
		{

			//Connect to the DB
			string conString = "DRIVER={MySQL ODBC 5.1 Driver};"
			+ "Server=" + databaseIP + ";"
			+ "PORT=" + databasePort + ";"
			+ "DATABASE=" + databaseName + ";"
			+ "UID=" + databaseUser + ";"
			+ "PWD=" + databasePw + ";"
			+ "OPTION=" + (FLAG_BIG_PACKETS + FLAG_NO_PROMPT).ToString() + ";";

			dbConn = new OdbcConnection(conString);

			try
			{
				if (dbConn.State == ConnectionState.Closed)
				{
					dbConn.Open();
					Console.WriteLine("ODBC Connection opened");

				}
				else
				{
					Console.WriteLine("ODBC Connection is already open");
					return;
				}
			}
			catch (OdbcException dbEx)
			{
				Console.WriteLine(dbEx.Message);
			}

		}

		#region Database calls

		/// <summary>
		/// Get all shopsterProduct Ids in Shopsterify
		/// </summary>
		/// <returns>List<int> of all the ids.</int></returns>
		public List<int> getShopsterProductIds()
		{
			return getShopsterProductIdsByUser(null);
		}

		/// <summary>
		/// Get all shopster product ids in Shopsterify, filtered by userId Parameter.
		/// </summary>
		/// <param name="userId">Filter the select statement by this id.</param>
		/// <returns>List<int> of all the ids found for given user.</int></returns>
		public List<int> getShopsterProductIdsByUser(int? userId)
		{
			List<int> returnList = new List<int>();
			OdbcCommand sqlProductIds;
			if (userId == null)
			{
				sqlProductIds = new OdbcCommand("SELECT DISTINCT(shopsterid) FROM shopsterproduct;", dbConn);
			}
			else
			{
				sqlProductIds = new OdbcCommand("SELECT DISTINCT(shopsterid) FROM shopsterproduct WHERE ShopifyUser='"+userId.ToString()+"';", dbConn);
			}
			
			OdbcDataReader sqlProductIdsReader = sqlProductIds.ExecuteReader();
			while (sqlProductIdsReader.Read())
			{
				//	Console.WriteLine("Shopsterify ProductId: " + sqlProductIdsReader[0]);
				returnList.Add(Convert.ToInt32(sqlProductIdsReader[0]));
			}

			return returnList;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="productIds"></param>
		/// <returns>count of rows affected. </returns>
		public int DeleteShopsterProductIds(List<int> productIds)
		{
			if(productIds.Count==0) //get rid of the empty case immediately.
			{ return 0; }

			StringBuilder productCsvBuilder = new StringBuilder(productIds.Count * 9);
			foreach (int productId in productIds)
			{
				productCsvBuilder.Append(", " + productId.ToString());

			}
			
			string productCSV = productCsvBuilder.ToString().TrimStart(',');

			Console.WriteLine("DeleteShopsterProductIds()::Executing: DELETE LOW_PRIORITY FROM shopsterproduct where ShopsterId in (" + productCSV + ")");
			OdbcCommand sqlProductIds = new OdbcCommand("DELETE FROM shopsterproduct where ShopsterId in (" + productCSV + ")", dbConn);
			int numDeleted =  sqlProductIds.ExecuteNonQuery() ;
			Console.WriteLine("DeleteShopsterProductIds()::Info: Deleted " +numDeleted+ " rows");

			return numDeleted;
		}

		/// <summary>
		/// For now takes a List will be refactored to take InventoryItemListType
		/// </summary>
		/// <param name="items"></param>
		/// <param name="shopsterUser"></param>
		/// <returns></returns>
		public int InsertShopsterProducts(List<int> items, int shopsterUser)
		{
			if (items.Count == 0)
			{
				return 0;
			}


			StringBuilder valuesBuilder = new StringBuilder();
			
			foreach (int id in items)
			{
				//Refactor to insert the product date as returned in the items list.
				valuesBuilder.Append("(" + shopsterUser.ToString() + ",now()," + Convert.ToInt32(id)+"),");
			}
			string valuesCsv = valuesBuilder.ToString().TrimEnd(',');


		 string insertQuery = "INSERT INTO shopsterproduct(shopsteruser,versiondate,shopsterid) VALUES " + valuesCsv + ";";
			
			Console.WriteLine(insertQuery);

			OdbcCommand sqlInsertProducts = new OdbcCommand(insertQuery, dbConn);
			int numInserted;
			try
			{
				numInserted = sqlInsertProducts.ExecuteNonQuery();
			}
			catch (OdbcException dbEx)
			{
				//Could try and parse the exception, remove the duplicate key and reinsert. 
				//We'll just make it fail silently, and catch these on the next update. 
				Console.WriteLine("InsertShopsterProducts()::Error: Database exception while attempting insert. " + dbEx.Message);
				numInserted = 0;
			}
			Console.WriteLine("InsertShopsterProducts()::Info: Inserted " + numInserted + " rows");
			return numInserted;

		}

		/// <summary>
		/// Use an apicontext (the authtoken and secret) to find the UserID in the db.
		/// </summary>
		/// <param name="apiContext"></param>
		/// <returns></returns>
		public int GetShopsterUserId(MyApiContext apiContext)
		{
			OdbcCommand sqlUserId = new OdbcCommand("SELECT id FROM shopsteruser WHERE AuthToken = '" 
				+ apiContext.AccessToken + "' and AuthSecret = '" + apiContext.AccessTokenSecret + "' LIMIT 1;", dbConn);

			int returnValue =Convert.ToInt32(sqlUserId.ExecuteScalar());

			Console.WriteLine("GetShopsterUserId::Info: Query= SELECT id FROM shopster user WHERE AuthToken = '"
				+ apiContext.AccessToken + "' and AuthSecret = '" + apiContext.AccessTokenSecret + "' LIMIT 1;");
			Console.WriteLine("GetShopsterUserId::Info: Query returned " + returnValue);
			return returnValue;
		}

		#endregion

	}

}
