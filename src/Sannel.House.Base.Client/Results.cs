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
using Sannel.House.Base.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sannel.House.Base.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">The class for the Data property</typeparam>
	public class Results<T> : ResponseModel<T>, IResponse, IResponse<T>, IResults
	{
		/// <summary>
		/// Creates a Results object from json.
		/// </summary>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">json</exception>
		public static Results<T> CreateFromJson(string json)
		{
			try
			{
				var r = JsonSerializer.Deserialize<Results<T>>(json ?? throw new ArgumentNullException(nameof(json)));
				r.Success = true;
				return r;
			}
			catch(JsonException jse)
			{
				var r = new Results<T>
				{
					Success = false,
					Exception = jse
				};
				return r;

			}
		}

		/// <summary>
		/// Creates a Results object from json asynchronous.
		/// </summary>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		public static Task<Results<T>> CreateFromJsonAsync(string json)
			=> Task.Run(() => CreateFromJson(json));

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Results{T}"/> is a success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		public bool Success { get; set; }

		/// <summary>
		/// Gets the errors.
		/// </summary>
		/// <value>
		/// The errors.
		/// </value>
		[JsonPropertyName("errors")]
		public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();


		/// <summary>
		/// Gets or sets the trace identifier.
		/// </summary>
		/// <value>
		/// The trace identifier.
		/// </value>
		[JsonPropertyName("traceId")]
		public string TraceId { get; set; }

		/// <summary>
		/// Gets or sets the exception captured by any method
		/// </summary>
		/// <value>
		/// The exception.
		/// </value>
		public Exception Exception { get; set; }

	}
}

