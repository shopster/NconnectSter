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
//	limitations under the License.
using System;

namespace Connectster.Shopify.Interfaces
{
	interface IShopifyCollection
	{
		 string BodyHtml { get; set; }		
		 string Handle { get; set; }		
		 int Id { get; set; }		
		 DateTime PublishedAt { get; set; }		
		 string SortOrder { get; set; }		
		 string TemplateSuffix { get; set; }		
		 string Title { get; set; }		
		 DateTime UpdatedAt { get; set; }		
		 string Body { get; set; }
	}
}
