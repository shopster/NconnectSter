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
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Connectster.EasyConfig;
using Connectster.Server;
using log4net;

namespace Connectster
{
	class Program
	{ 


		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;

			ConfigurationManager configs = ConfigurationManager.Instance();
			SettingsGroup settings = configs.getSettings("MyApiContext");
			string ShopsterAuthToken = settings.Settings["ConsumerKey"].GetValueAsString();
			string ShopsterAuthSecret = settings.Settings["ConsumerSecret"].GetValueAsString();
			

			//Logging 
			FileInfo logConfigFile = new FileInfo("../../log4net.config.xml");
			log4net.Config.XmlConfigurator.ConfigureAndWatch(logConfigFile);
			ILog log = LogManager.GetLogger("Shopsterify");


			// Authentication with Shopster API
			MyApiContext apiContext = new MyApiContext();
			apiContext.AccessToken = ShopsterAuthToken;
			apiContext.AccessTokenSecret = ShopsterAuthSecret;

			ShopsterifyController controller = ShopsterifyController.Instance();
			
			int sleepTime = 1024;
			while (true)
			{
				List<ShopsterifyUser> userList = controller.getAllUsers();
				log.InfoFormat("Found {0} ShopsterifyUsers, beginning sync for each user.", userList.Count);

				//Todo: add call to update sleepUntil Timestamps on users (based on how long they should sleep).
				//todo: Also throughout code, we should add points where the user should sleep
				//		such as in shopifycommunicator, if they have used up their call limit (per 10 mins)
				//		then we should sleep that user for 10 mins

				int actions = 0;
				List<ShopsterifySyncJob> myJobs = new List<ShopsterifySyncJob>(userList.Count);
				ManualResetEvent[] resetEvents = new ManualResetEvent[userList.Count];

				for(int i =0; i<userList.Count; i++)
				{
					resetEvents[i] = new ManualResetEvent(false);

					//TODO: refactor code so it will sleep a certain user, but keep others active
					ShopsterifySyncJob job = new ShopsterifySyncJob(controller, userList[i], resetEvents[i]);
					myJobs.Add(job);
					ThreadPool.QueueUserWorkItem(new WaitCallback(job.ThreadStart), null );
				
				}

				if (resetEvents.Count() > 0)
				{
					WaitHandle.WaitAll(resetEvents);
				}

				foreach (ShopsterifySyncJob job in myJobs)
				{

					Console.WriteLine("Job({0},{1}) had {2} actions", job.myUser.ShopsterUser, job.myUser.ShopifyUser, job.actionsPerformed);
					actions += job.actionsPerformed;
				}
				
				if (actions == 0)
				{
				    //Double the amount of time we sleep if there was nothing to do last time.
				    //Maxes out at 4.3 minutes and stays there until something happens.
					if (sleepTime < 262144)
				    {
				        sleepTime *= 2;
				    }
				}
				else
				{
				    sleepTime = 1024; //1 second
				}

				log.InfoFormat("Completed {0} actions going to sleep for {1} ms.", actions, sleepTime);
				Thread.Sleep(sleepTime);
			}
		}
	}

}