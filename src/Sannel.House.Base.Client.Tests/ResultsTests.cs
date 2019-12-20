/* Copyright 2019 Sannel Software, L.L.C.

   Licensed under the Apache License, Version 2.0 (the ""License"");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

	   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an ""AS IS"" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Sannel.House.Base.Client.Tests
{
	public class ResultsTests
	{
		[Fact]
		public async Task CreateFromJsonExceptionsTest()
		{
			Assert.Throws<ArgumentNullException>("json", () => Results<string>.CreateFromJson(null));
			await Assert.ThrowsAsync<ArgumentNullException>("json", () => Results<string>.CreateFromJsonAsync(null));

			var results = Results<object>.CreateFromJson("{");
			Assert.False(results.Success);
			Assert.IsType<JsonException>(results.Exception);

			results = await Results<object>.CreateFromJsonAsync("{");
			Assert.False(results.Success);
			Assert.IsType<JsonException>(results.Exception);
		}

		[Fact]
		public async Task CreateFromJsonTest()
		{
			Results<object> results;
			results = Results<object>.CreateFromJson(@"
{
	""errors"": {
		""Name"": [
			""The Name field is required."",
			""Name does not contain test.""
		]
	},
	""title"": ""One or more validation errors occurred."",
	""status"": 400,
	""traceId"": ""0HLIURDPH1MOO:00000001""}");

			Assert.True(results.Success);
			Assert.Equal(HttpStatusCode.BadRequest, results.StatusCode);
			Assert.Equal("0HLIURDPH1MOO:00000001", results.TraceId);
			Assert.Single(results.Errors);
			Assert.True(results.Errors.ContainsKey("Name"));
			Assert.Equal("The Name field is required.,Name does not contain test.", string.Join(',',results.Errors["Name"]));

			results = await Results<object>.CreateFromJsonAsync(@"
{
	""errors"": {
		""Name"": [
			""The Name field is required."",
			""Name does not contain test.""
		]
	},
	""title"": ""One or more validation errors occurred."",
	""status"": 400,
	""traceId"": ""0HLIURDPH1MOO:00000001""}");

			Assert.True(results.Success);
			Assert.Equal(HttpStatusCode.BadRequest, results.StatusCode);
			Assert.Equal("0HLIURDPH1MOO:00000001", results.TraceId);
			Assert.Single(results.Errors);
			Assert.True(results.Errors.ContainsKey("Name"));
			Assert.Equal("The Name field is required.,Name does not contain test.", string.Join(',',results.Errors["Name"]));

		}
	}
}
