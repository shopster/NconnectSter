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
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Shopster.API.Service.SDK;
using Shopster.API.Service.SDK.Call;
using Shopster.API.Service.SDK.Core;
using Shopster.API.Service.SDK.Core.Exceptions;
using Shopster.API.Service.SDK.Core.Soap;
using Shopsterify.Shopify;
using System.Data.Odbc;
using Shopsterify.Database;
using Shopsterify.Shopsterify;
using Shopsterify.Shopster;
using log4net;


namespace Shopsterify
{
	class Program
	{


		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;

			//Logging 
			FileInfo logConfigFile = new FileInfo("../../log4net.config.xml");
			log4net.Config.XmlConfigurator.ConfigureAndWatch(logConfigFile);
			ILog log = LogManager.GetLogger("Shopsterify");


			//TODO: create code that captures this info and works with insertion into db
			//Shopster's App auth tokens on Shopify
			string ShopsterAuthToken = "qpGmu1KwOkX+WGB0I2Y7zvgo/Yc=";
			string ShopsterAuthSecret = "SDLHOywjXBDXn96NiPc9ELs+HS8=";



			// Authentication with Shopster API
			MyApiContext apiContext = new MyApiContext();
			apiContext.AccessToken = ShopsterAuthToken;
			apiContext.AccessTokenSecret = ShopsterAuthSecret;

			ShopsterifyController controller = ShopsterifyController.Instance();

			//Some Calls to the controller
			

			
			
			int sleepTime = 1024;
			while (true)
			{
				List<ShopsterifyUser> userList = controller.getAllUsers();
				log.InfoFormat("Found {0} ShopsterifyUsers, beginning sync for each user.", userList.Count);

				//Todo: add call to update sleepUntil Timestamps on users (based on how long they should sleep).
				//todo: Also throughout code, we should add points where the user should sleep, such as in shopifycommunicator, if they have used up their call limit (per 10 mins) then we should sleep that user for 10 mins (or a little less)

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
				
				//if (actions == 0)
				//{
				//    //Double the amount of time we sleep if there was nothing to do last time.
				//    //Maxes out at 8.7 minutes and stays there until something happens.
				//    if (sleepTime < 32768)
				//    {
				//        sleepTime *= 2;
				//    }
				//}
				//else
				//{
				//    sleepTime = 1024; //1 second
				//}

				//todo update this logging to work for each user.
				log.InfoFormat("Completed {0} actions going to sleep for {1} ms.", actions, sleepTime);
				Thread.Sleep(sleepTime);

			}
		}
	}

}