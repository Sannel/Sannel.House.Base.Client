/* Copyright 2019 Sannel Software, L.L.C.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
      http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Base.Client
{
	public class AuthenticationTokenArgs
	{
		/// <summary>
		/// Used to set the authentication token to use for request.
		/// </summary>
		/// <value>
		/// The token.
		/// </value>
		public string Token { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not to Cache the token in the class or call the event each time.
		/// </summary>
		/// <value>
		///   <c>true</c> if [cache token]; otherwise, <c>false</c>.
		/// </value>
		public bool CacheToken { get; set; }

	}
}
