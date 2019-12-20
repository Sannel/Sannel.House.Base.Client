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
using System.Text;

namespace Sannel.House.Base.Client
{
	public interface IResults : IResponse
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IResults"/> is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		bool Success { get; set; }

		/// <summary>
		/// Gets the errors.
		/// </summary>
		/// <value>
		/// The errors.
		/// </value>
		Dictionary<string, string[]> Errors { get; }

		/// <summary>
		/// Gets or sets the trace identifier.
		/// </summary>
		/// <value>
		/// The trace identifier.
		/// </value>
		string TraceId { get; set; }

		/// <summary>
		/// Gets or sets the exception captured by any method
		/// </summary>
		/// <value>
		/// The exception.
		/// </value>
		Exception Exception { get; set; }
	}

	public interface IResults<T> : IResults, IResponse<T>
	{
	}
}
