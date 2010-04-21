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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shopsterify.Shopify.Interfaces
{
	interface IShopifyMetafield
	{

		 DateTime CreatedAt { get; set; }
		 string Description { get; set; }
		 int Id { get; set; }
		 string Key { get; set; }
		 string Namespace { get; set; }
		 DateTime UpdatedAt { get; set; }
		 string Value { get; set; }
		 string ValueType { get; set; } //can be int or string

	}
}
