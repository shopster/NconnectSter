//	Copyright 2010 Shopster E-Commerce Inc.
//
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.using System;

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using Connectster.EasyConfig;
using Connectster.Shopify;
using Connectster.Shopster;
using Shopster.API.Service.SDK.Core.Soap;
using Shopster.API.Service.SDK.Core;
using log4net;

namespace Connectster.Server.Database
{
	//Ugly for now
	class ShopsterifyDatabase
	{

		//loaded from the config file. 
		private string databaseIP;
		private string databasePort;
		private string databaseName;
		private string databaseUser;
		private string databasePw;
		private int FLAG_BIG_PACKETS = 8;
		private int FLAG_NO_PROMPT = 16;

		OdbcConnection dbConn;
		static ILog logger = log4net.LogManager.GetLogger("ShopsterifyDatabase");
		static ConfigurationManager configManager = ConfigurationManager.Instance();
		public ShopsterifyDatabase()
		{

			SettingsGroup config = configManager.getSettings("ShopsterifyDatabase");
			databaseIP = config.Settings["databaseIP"].GetValueAsString();
			databasePort = config.Settings["databasePort"].GetValueAsString(); 
			databaseName = config.Settings["databaseName"].GetValueAsString(); 
			databaseUser = config.Settings["databaseUser"].GetValueAsString(); 
			databasePw = config.Settings["databasePw"].GetValueAsString(); 

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
					
#if DEBUG
					logger.DebugFormat("ODBC Connection opened: Server is({0}):{1}), Using({2}), User({3})",databaseIP, databasePort, databaseName,databaseUser);
#endif

				}
				else
				{
					return;
				}
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("Failed to open ODBC Connection: Message is {0}",dbEx.Message);
			}
		}

		

		#region ShopsterifyProduct Procedures
		public bool DeleteShopsterifyProductMapping(int shopsterId, int shopifyId)
		{
			if (!(shopsterId > 0 && shopifyId > 0))
			{
				throw new ArgumentException("shopsterId and shopifyId must be greater than 0");
			}
			string query = String.Format("CALL DeleteShopsterifyProduct({0},{1});", shopsterId, shopifyId);

			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{
				OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
				if (sqlResult.Read())
				{
					if (Convert.ToInt32(sqlResult[0]) == 1)
					{ //As long as the map is deleted, then the bond is broken. We can clean up later if something failed. 
						return true;
					}
				}
			}
			catch (OdbcException dbEx)
			{
				
				logger.ErrorFormat("DeleteShopsterifyProductMapping()::Error: Database exception while attempting insert. " + dbEx.Message);
				logger.ErrorFormat("DeleteShopsterifyProductMapping()::Error: Query w/ Error was: {0}", query);
			}

			return false;
		}

		public bool InsertProductForUser(ShopsterifyUser user, int shopsterItemId, DateTime shopsterVersionDate, int shopifyItemId, DateTime shopifyVersionDate)
		{
			DateTime insertTime = DateTime.Now.ToUniversalTime();
			string query = String.Format(" CALL InsertShopsterifyProduct({0},'{1:yyyy-MM-dd HH:mm:ss}',{2},{3},'{4}','{5:yyyy-MM-dd HH:mm:ss}',{6}); ",
				user.ShopsterUser, insertTime, shopsterItemId, shopifyItemId, user.ShopifyUser, insertTime, true);
			
			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{	
				OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
				if(sqlResult.Read())
				{
					if (Convert.ToInt32(sqlResult[0]) == 1 && Convert.ToInt32(sqlResult[1]) == 1 && Convert.ToInt32(sqlResult[2]) == 1)
					{
						return true;
					}
				}
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("InsertProductForUser()::Error: Database exception while attempting insert. " + dbEx.Message);
				logger.ErrorFormat("InsertProductForUser()::Error: Query w/ Error was: {0}", query);
			}

			return false;
		}



		public bool InsertProductForUser(ShopsterifyUser user, InventoryItemType shopsterProduct, ShopifyProduct shopifyProduct )
		{
			//TODO: have this take a bool to decide if shopster is master, for now true
			//Todo: have actual dates from Shopster API, when available.
			return InsertProductForUser( user, Convert.ToInt32(shopsterProduct.ItemId), (DateTime.Now.ToUniversalTime()), (int) shopifyProduct.Id, ((DateTime) shopifyProduct.Variants[0].UpdatedAt).ToUniversalTime());
		}

		public bool CreateProductMapping(int shopsterProductId, int shopifyProductId)
		{

			//TODO: refactor to allow deciding if shopster is master.
			string query = String.Format(" Insert into shopsterifyproductmap(ShopsterProductId, ShopifyProductId, ShopsterIsMaster) Values ({0},{1},{2})",
				shopsterProductId, shopifyProductId, true);
			
			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{
				int numRows = sqlCommand.ExecuteNonQuery();
				if (numRows == 0)
				{
					return false;
				}
				else if (numRows == 1)
				{
					return true;
				}
				else
				{
					logger.ErrorFormat("Insert of new mapping failed. ShopsterId({0}), ShopifyId({1})", shopsterProductId, shopifyProductId);
					return false;
				}
				
			}
			catch (OdbcException dbEx)
			{
				
				logger.ErrorFormat("InsertProductForUser(): Database exception while attempting insert. " + dbEx.Message);
			}

			return false;
		}

		/// <summary>
		/// Retrieves all ShopsterifyProducts for a user. 
		/// </summary>
		/// <param name="shopsterUser"></param>
		/// <param name="shopifyUser"></param>
		/// <returns></returns>
		/// 
		public List<ShopsterifyProduct> SelectProductForUser(int shopsterUser, string shopifyUser)
		{
			if (shopsterUser <= 0 || shopifyUser==string.Empty || shopifyUser.Length>255)
			{
				throw new ArgumentException("shopsterUser and shopifyUser cannot be 0 or negative");
			}
			List<ShopsterifyProduct> returnList = new List<ShopsterifyProduct>();

			string query = String.Format("CALL SelectProductForUser({0},'{1}');", shopsterUser, shopifyUser);
			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{
				OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
				while (sqlResult.Read())
				{
					returnList.Add(new ShopsterifyProduct(Convert.ToInt32(sqlResult[1]), Convert.ToDateTime(sqlResult[0]),
						Convert.ToInt32(sqlResult[2]), Convert.ToDateTime(sqlResult[3])));
				}
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("SelectProductForUser()::Error: Database exception while attempting insert. " + dbEx.Message);
				returnList = null;
			}
			catch (FormatException fmtEx)
			{
				logger.ErrorFormat("SelectProductForUser(): FormatException: {0} . Check DBSchema and how we're casting the data.", fmtEx.Message);
				returnList = null;
			}

			return returnList;
		}

		public List<ShopsterifyProduct> SelectProductForUser(ShopsterifyUser user)
		{
			return SelectProductForUser(user.ShopsterUser, user.ShopifyUser);
		}

		#endregion

		#region ShopsterifyUser calls
		public ShopifyStoreAuth SelectShopifyUserDetails(ShopsterifyUser user)
		{
			string query = String.Format(
				@" SELECT su.AuthToken,su.SubDomain
				FROM ShopifyUser su JOIN ShopsterifyUserMap smap on smap.ShopifySubdomain = su.subdomain
				WHERE smap.ShopsterUserId  = {0} and smap.ShopifySubdomain = '{1}';", user.ShopsterUser, user.ShopifyUser);

			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{
				OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
				while (sqlResult.Read())
				{
					return new ShopifyStoreAuth((string)sqlResult[0], (string)sqlResult[1]);
				}
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("SelectShopifyUser(): Database exception while attempting select shopifyuser. " + dbEx.Message);
				return null;
			}

			return null;

		}

		public ShopsterUser SelectShopsterUserDetails(ShopsterifyUser user)
		{
			string query = String.Format(
			@"SELECT su.id, su.AuthToken, su.AuthSecret, su.ShopsterAccountType
			FROM ShopsterUser su JOIN ShopsterifyUserMap smap on smap.ShopsterUserId = su.id
			WHERE smap.ShopsterUserId  = {0} and smap.ShopifySubdomain = '{1}';", user.ShopsterUser, user.ShopifyUser);


			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{
				OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
				while (sqlResult.Read())
				{
					return new ShopsterUser(Convert.ToInt32(sqlResult[0]), (string)sqlResult[1], (string)sqlResult[2], Convert.ToInt32(sqlResult[3]));
				}
				return null; // no results
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("SelectShopsterUserDetails(): Database exception while attempting select user by apicontext. " + dbEx.Message);
				return null;
			}
			catch (FormatException fmtEx)
			{
				logger.ErrorFormat("SelectShopsterUserDetails(): FormatException: {0} . Check DBSchema and how we're casting the data.", fmtEx.Message);
				return null;
			}
			
			
		}

		public List<ShopsterifyUser> SelectAllUsers()
		{
			List<ShopsterifyUser> returnList = new List<ShopsterifyUser>();
			string query = "SELECT sm.ShopsterUserId, sm.ShopifySubdomain, sm.sleepUntil FROM ShopsterifyUserMap AS sm "
			+ "JOIN ShopifyUser AS shopifyU on sm.ShopifySubdomain = shopifyU.subdomain "
			+ "WHERE shopifyU.dateDisabled is null;";

			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{

				OdbcDataReader sqlResult=sqlCommand.ExecuteReader();
				while(sqlResult.Read())
				{
					try //For the Convert.ToInt32 , also so we can continue reading and get as many users as possible.
					{
						DateTime time = sqlResult[2] == DBNull.Value ? DateTime.MinValue.ToUniversalTime() : Convert.ToDateTime(sqlResult[2]).ToUniversalTime();
						ShopsterifyUser tempUser = new ShopsterifyUser(Convert.ToInt32(sqlResult[0]),(string) sqlResult[1], time);
						returnList.Add(tempUser);
					}
					catch (FormatException e) 
					{
						logger.DebugFormat("SelectAllUsers(): FormatException for returned userId({0}) or possibly sleepUntil DateTime. Exception Message is {1}", (string)sqlResult[0], e.Message);
						continue; //Just abandon this user and move on.
					}
					catch (OverflowException e)
					{
						logger.DebugFormat("SelectAllUsers(): OverflowException for returned userId({0}) or possibly sleepUntil DateTime. Exception Message is {1}", (string)sqlResult[0], e.Message);
						continue;
					}
				
				}
				return returnList;
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("SelectAllUsers()::Error: Database exception while attempting select user by apicontext. " + dbEx.Message);
				return null;
			}
		}


		public int UpdateShopsterifyUsers(List<ShopsterifyUser> userList, DateTime newTime)
		{
            string query = string.Empty;
			int returnValue = 0;
            foreach(ShopsterifyUser user in userList)
            {
				query = String.Format(@" UPDATE shopsterifyusermap SET sleepUntil = '{0}' 
						WHERE  ShopsterUserId={1} AND ShopifySubDomain='{2}' LIMIT 1;",
						newTime, user.ShopsterUser, user.ShopifyUser);
				OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
				returnValue += sqlCommand.ExecuteNonQuery();
			}

			return returnValue;
		}

		public ShopsterifyUser SelectShopsterifyUser(ApiContext apiContext, ShopifyStoreAuth shopifyAuth)
		{
			if (apiContext == null || shopifyAuth == null)
			{
				logger.ErrorFormat("SelectShopsterifyUser(): Was given null apiContext or ShopifyAuth");
				throw new ArgumentException("Neither arguments can be null");
			}

			string query = String.Format(
				@" SELECT shopsterU.id as ShopsterUserId, shopifyU.subdomain as ShopifyUserId, smap.sleepUntil
				FROM shopifyUser shopifyU JOIN shopsterifyusermap smap ON shopifyU.subdomain = smap.ShopifySubdomain
				JOIN shopsteruser shopsterU ON shopsterU.id = smap.ShopsterUserId
				WHERE  shopsterU.AuthToken = '{0}' AND
				shopsterU.AuthSecret = '{1}' AND
				shopifyU.subDomain = '{2}' AND
				shopifyU.AuthToken = '{3}' LIMIT 1;",
											apiContext.AccessToken,
											apiContext.AccessTokenSecret,
											shopifyAuth.StoreSubDomain,
											shopifyAuth.StoreAuthToken);

			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{


				OdbcDataReader sqlResult = sqlCommand.ExecuteReader();


				while (sqlResult.Read()) //We return the first read.
				{
					try //Try to do Convert.ToInt32
					{
						return new ShopsterifyUser(Convert.ToInt32(sqlResult[0]), (string)sqlResult[1]);
					}
					catch (FormatException e)
					{
						logger.DebugFormat("SelectShopsterifyUser(): FormatException for returned userId({0}). Exception Message is {1}", (string)sqlResult[0], e.Message);
						continue; //Just abandon this user and move on.
					}
					catch (OverflowException e)
					{
						logger.DebugFormat("SelectShopsterifyUser(): OverflowException for returned userId({0}). Exception Message is {1}", (string)sqlResult[0], e.Message);
						continue;
					}
				}

				//User not found.
				return null;
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("SelectShopsterifyUser(): Database exception while attempting to get shopsterifyUser. " + dbEx.Message);
				return null;
			}

		}
	
		// todo refactor this to use the timestamp in the shopsterItem
		public bool UpdateShopsterProductTimeStamp(InventoryItemType shopsterItem, DateTime timestamp)
		{

			if (shopsterItem == null || timestamp== null)
			{
				throw new ArgumentException("Neither arguments can be null");
			}

			string query = String.Format(
				@" UPDATE ShopsterProduct SET versiondate = '{0:yyyy-MM-dd HH:mm:ss}' where ShopsterId = {1} LIMIT 1;",
											timestamp, shopsterItem.ItemId);

			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{

				if (sqlCommand.ExecuteNonQuery() == 1)
				{
					return true;
				}
				else
				{
					logger.ErrorFormat("UpdateShopsterProductTimeStamp(): Attempted to update timestamp but product({0}) wasnt found", shopsterItem.ItemId);
					return false;
				}

			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("UpdateShopsterProductTimeStamp(): Database exception while attempting to update shopsterProduct({0}) with timestamp ({1}). " + dbEx.Message, shopsterItem.ItemId , timestamp);
				return false;
			}

			
		}
		#endregion


		internal bool UpdateShopifyProductTimeStamp(ShopifyProduct item, DateTime timestamp)
		{

			if (item == null || timestamp == null)
			{
				throw new ArgumentException("Neither arguments can be null");
			}

			string query = String.Format(
				@" UPDATE ShopifyProduct SET versiondate = '{0:yyyy-MM-dd HH:mm:ss}' where id = {1} LIMIT 1;",
											timestamp, item.Id);

			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{

				if (sqlCommand.ExecuteNonQuery() == 1)
				{
					return true;
				}
				else
				{
					logger.ErrorFormat("UpdateShopifyProductTimeStamp(): Couldnt update timestamp on ShopifyItem({0}).", item.Id);
					return false;
				}

			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("UpdateShopsterProductTimeStamp(): Database exception while attempting to update shopsterProduct({0}) with timestamp ({1}). " + dbEx.Message, item.Id, timestamp);
				return false;
			}

		}

		/// <summary>
		/// Create and order mapping, also updates on duplicate keys. 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="shopifyOrder"></param>
		/// <param name="shopsterOrderId"></param>
		/// <returns></returns>
		internal bool CreateOrderMapping(ShopsterifyUser user, ShopifyOrder shopifyOrder, int shopsterOrderId)
		{
			if (user == null || shopifyOrder == null || shopsterOrderId < 0)
			{
				throw new ArgumentException("User and shopifyOrder cannot be null. shopsterOrderId must be > 0 ");
			}

		
			string query = String.Format(
			@"INSERT
			INTO shopsterifyorder (ShopsterOrderId, ShopifyOrderId, ShopsterUserId, ShopifyAccountDomain , ShopsterVersion, ShopifyVersion)
			VALUES  ({0},{1},{2},'{3}', '{4:yyyy-MM-dd HH:mm:ss}', '{5:yyyy-MM-dd HH:mm:ss}')
			ON DUPLICATE KEY UPDATE shopsterVersion = '{4:yyyy-MM-dd HH:mm:ss}' , shopifyVersion = '{5:yyyy-MM-dd HH:mm:ss}'
			;",
				 shopsterOrderId, shopifyOrder.Id, user.ShopsterUser, user.ShopifyUser, shopifyOrder.UpdatedAt, DateTime.UtcNow);

			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{
				int result;
				if ( (result = sqlCommand.ExecuteNonQuery())==1 || result==2) //Inserted (1) or updated(2)
				{
					return true;
				}
				
				else
				{
#if DEBUG
					logger.DebugFormat("CreateOrderMapping(): Failed to CreateOrderMapping Query was {0}", query);
#endif
					return false;
				}

			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("CreateOrderMapping(): Database exception while attempting to insert/update shopsterifyOrder. ShopifyOrder is ({0}) with timestamp ({1}). " + dbEx.Message, shopifyOrder.Id, shopifyOrder.UpdatedAt);
				return false;
			}


		}

		/// <summary>
		/// Gets a dictionary of ordermappings for this shopsterify user from the database.
		/// </summary>
		/// <param name="user"></param>
		/// <returns>A dictionary <int ShopifyOrderId, int ShopsterOrderId> meaning ShopifyOrderId was created in shopster's system as ShopsterOrderId</returns>
		internal Dictionary<int, int> SelectOrderMappingsForUser(ShopsterifyUser user)
		{

			Dictionary<int, int> returnMap = null;

			string query = String.Format(
			@"SELECT ShopifyOrderId, ShopsterOrderId
			FROM shopsterifyOrder
			WHERE shopsterUserId = {0} and ShopifyAccountDomain = '{1}'
			;",
				 user.ShopsterUser, user.ShopifyUser);


			OdbcCommand sqlCommand = new OdbcCommand(query, dbConn);
			try
			{
				OdbcDataReader sqlResult = sqlCommand.ExecuteReader();
				
				returnMap = new Dictionary<int, int>();
				while (sqlResult.Read())
				{
					try
					{
						returnMap.Add(Convert.ToInt32(sqlResult[0]), Convert.ToInt32(sqlResult[1]));
					}
					catch (FormatException fmtEx)
					{
						logger.ErrorFormat("SelectOrderMappingsForUser():FormatException User is ({0},{1}), Message: {2}", user.ShopsterUser, user.ShopifyUser, fmtEx.Message);
						continue;
					}
					catch (OverflowException e)
					{
						logger.ErrorFormat("SelectOrderMappingsForUser():OverflowException User is ({0},{1}), Message: {2}", user.ShopsterUser, user.ShopifyUser, e.Message);
						continue;
					}
				}
				return returnMap;
			}
			catch (OdbcException dbEx)
			{
				logger.ErrorFormat("SelectOrderMappingsForUser()::Error: Database exception while attempting to select shopsterifyOrder mappings for user(Shopster({0}), Shopify({1})). " + dbEx.Message, user.ShopsterUser, user.ShopifyUser);
			}

			//Failure if we reach here.
			return null;
		}

		/// <summary>
		/// Calls UpdateShopsterProductTimeStamp for each item in given list.
		/// </summary>
		/// <param name="shopsterList"></param>
		/// <param name="dateTime"></param>
	
		internal void UpdateShopsterProductTimeStamps(List<InventoryItemType> shopsterList, DateTime dateTime)
		{
			foreach (InventoryItemType item in shopsterList)
			{
				UpdateShopsterProductTimeStamp(item, dateTime);
			}
		}
	}
		
}
