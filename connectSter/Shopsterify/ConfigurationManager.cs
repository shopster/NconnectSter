using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyConfig;

namespace Shopsterify.Shopsterify
{
	public class ConfigurationManager
	{
		private static ConfigurationManager instance = new ConfigurationManager();
		private string configFileName;
		private EasyConfig.ConfigFile confFile;

		private ConfigurationManager()
		{
			configFileName = "C:\\ThirdPartyAPI\\Shopify\\MikesSandbox\\connectSter\\connectSter\\connectSter.conf";
			confFile = new ConfigFile(configFileName);
			if (!VerifyConfigFile())
			{
				throw new Exception("Config file: {0} fails verfication.");
			}


		}

		public static ConfigurationManager Instance()
		{
			return instance;
		}

		public SettingsGroup getSettings(string groupName)
		{
			if (confFile.SettingGroups.Keys.Contains(groupName))
			{
				return confFile.SettingGroups[groupName];
			}
			else
			{
				throw new Exception(String.Format("ConfigurationManager cannot find group {0}. Ensure you add your new settings to VerifyConfigFile().", groupName));
			}
		}


		/// <summary>
		/// Verifies Config file groups, keys and value types. Add any new groups/settings to their own function. Doesn't evaluate the validity of the settings.
		/// </summary>
		/// <returns>True if all groups,settings and types are correct. False otherwise.</returns>
		private bool VerifyConfigFile()
		{
			return (VerifyShopifyCommunicatorConfigs() && VerifyShopsterifyDatabaseConfigs());
		}

		#region Group Verifiers
		private bool VerifyMyApiContextConfigs()
		{
			if (!confFile.SettingGroups.Keys.Contains("MyApiContext")
		 || (
				!confFile.SettingGroups["MyApiContext"].Settings.Keys.Contains("ConsumerKey") ||
				!confFile.SettingGroups["MyApiContext"].Settings.Keys.Contains("ConsumerSecret") ||
				!confFile.SettingGroups["MyApiContext"].Settings.Keys.Contains("EndpointConfigurationName") 
			))
			{
				return false;
			}
			else //Verify types, throws an exception if no good.
			{
				confFile.SettingGroups["MyApiContext"].Settings["ConsumerKey"].GetValueAsString();
				confFile.SettingGroups["MyApiContext"].Settings["ConsumerSecret"].GetValueAsString();
				confFile.SettingGroups["MyApiContext"].Settings["EndpointConfigurationName"].GetValueAsString();
			}
			


			return true;
		}
		private bool VerifyShopifyCommunicatorConfigs()
		{
			//Simple check if the ShopifyCommunicator configurations are present.
			if (!confFile.SettingGroups.Keys.Contains("ShopifyCommunicator")
			 || (
					!confFile.SettingGroups["ShopifyCommunicator"].Settings.Keys.Contains("domain") ||
					!confFile.SettingGroups["ShopifyCommunicator"].Settings.Keys.Contains("protocol") ||
					!confFile.SettingGroups["ShopifyCommunicator"].Settings.Keys.Contains("appAuth_APIKey") ||
					!confFile.SettingGroups["ShopifyCommunicator"].Settings.Keys.Contains("appAuth_APISharedSecret")))
			{
				return false;
			}
			else //Verify types, throws an exception if no good.
			{
				confFile.SettingGroups["ShopifyCommunicator"].Settings["domain"].GetValueAsString();
				confFile.SettingGroups["ShopifyCommunicator"].Settings["protocol"].GetValueAsString();
				confFile.SettingGroups["ShopifyCommunicator"].Settings["appAuth_APIKey"].GetValueAsString();
				confFile.SettingGroups["ShopifyCommunicator"].Settings["appAuth_APISharedSecret"].GetValueAsString();
			}




			return true;

		}
		private bool VerifyShopsterifyDatabaseConfigs()
		{

			if (!confFile.SettingGroups.Keys.Contains("ShopsterifyDatabase")
			 || (
					!confFile.SettingGroups["ShopsterifyDatabase"].Settings.Keys.Contains("databaseIP") ||
					!confFile.SettingGroups["ShopsterifyDatabase"].Settings.Keys.Contains("databasePort") ||
					!confFile.SettingGroups["ShopsterifyDatabase"].Settings.Keys.Contains("databaseName") ||
					!confFile.SettingGroups["ShopsterifyDatabase"].Settings.Keys.Contains("databaseUser") ||
					!confFile.SettingGroups["ShopsterifyDatabase"].Settings.Keys.Contains("databasePw") 
				))
			{
				return false;
			}
			else //Verify types, throws an exception if no good.
			{
				confFile.SettingGroups["ShopsterifyDatabase"].Settings["databaseIP"].GetValueAsString();
				confFile.SettingGroups["ShopsterifyDatabase"].Settings["databasePort"].GetValueAsString();
				confFile.SettingGroups["ShopsterifyDatabase"].Settings["databaseName"].GetValueAsString();
				confFile.SettingGroups["ShopsterifyDatabase"].Settings["databaseUser"].GetValueAsString();
				confFile.SettingGroups["ShopsterifyDatabase"].Settings["databasePw"].GetValueAsString();
			}
			return true;
		}
		#endregion

	}
}
