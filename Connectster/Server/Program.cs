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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using log4net;

namespace Connectster.Server
{
	class Program
	{
		static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;

		    var shopsterAuthToken = ConfigurationManager.AppSettings["ShopsterConsumerKey"];
            var shopsterAuthSecret = ConfigurationManager.AppSettings["ShopsterConsumerSecret"];

			//Logging 
			var logConfigFile = new FileInfo("log4net.config.xml");
			log4net.Config.XmlConfigurator.ConfigureAndWatch(logConfigFile);

            ILog log = LogManager.GetLogger("Connectster");

			// Authentication with Shopster API
			var apiContext = new MyApiContext();
			apiContext.AccessToken = shopsterAuthToken;
			apiContext.AccessTokenSecret = shopsterAuthSecret;

			ConnectsterController controller = ConnectsterController.Instance();
			
			int sleepTime = 1024;
			while (true)
			{
				List<ConnectsterUser> userList = controller.GetAllUsers();
				log.InfoFormat("Found {0} ShopsterifyUsers, beginning sync for each user.", userList.Count);

				//Todo: add call to update sleepUntil Timestamps on users (based on how long they should sleep).
				//todo: Also throughout code, we should add points where the user should sleep
				//		such as in shopifycommunicator, if they have used up their call limit (per 10 mins)
				//		then we should sleep that user for 10 mins

				int actions = 0;
				var myJobs = new List<ConnectsterSyncJob>(userList.Count);
				var resetEvents = new ManualResetEvent[userList.Count];

				for(int i =0; i<userList.Count; i++)
				{
					resetEvents[i] = new ManualResetEvent(false);

					//TODO: refactor code so it will sleep a certain user, but keep others active
					var job = new ConnectsterSyncJob(controller, userList[i], resetEvents[i]);
					myJobs.Add(job);
					ThreadPool.QueueUserWorkItem(job.ThreadStart, null );
				
				}

				if (resetEvents.Count() > 0)
				{
					WaitHandle.WaitAll(resetEvents);
				}

				foreach (ConnectsterSyncJob job in myJobs)
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
// ReSharper disable FunctionNeverReturns
		}
// ReSharper restore FunctionNeverReturns
	}

}