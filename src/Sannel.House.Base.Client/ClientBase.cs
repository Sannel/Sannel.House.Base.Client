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
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sannel.House.Base.Client
{
	public abstract class ClientBase
	{
		protected readonly IHttpClientFactory factory=null;
		protected readonly ILogger logger;
		protected readonly HttpClient client = null;
		protected readonly string baseUri = "/";

		/// <summary>
		/// Initializes a new instance of the <see cref="DevicesClient" /> class.
		/// </summary>
		/// <param name="factory">The HttpClientFactory.</param>
		/// <param name="baseUri">The base path i.e. http://host/v1/ or http://host/</param>
		/// <param name="logger">The logger.</param>
		/// <exception cref="ArgumentNullException">factory
		/// or
		/// logger</exception>
		protected ClientBase(IHttpClientFactory factory, string baseUri, ILogger logger)
		{
			if(!Uri.IsWellFormedUriString(baseUri ?? throw new ArgumentNullException(nameof(baseUri))
				, UriKind.Absolute))
			{
				throw new ArgumentException("Invalid Uri format", nameof(baseUri));
			}
			this.baseUri = baseUri;
			this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientBase" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="baseUri">The base path i.e. http://host/v1/ or http://host/</param>
		/// <param name="logger">The logger.</param>
		/// <exception cref="ArgumentNullException">client
		/// or
		/// logger</exception>
		protected ClientBase(HttpClient client, string baseUri, ILogger logger)
		{
			if(!Uri.IsWellFormedUriString(baseUri ?? throw new ArgumentNullException(nameof(baseUri))
				, UriKind.Absolute))
			{
				throw new ArgumentException("Invalid Uri format", nameof(baseUri));
			}
			this.baseUri = baseUri;
			this.client = client ?? throw new ArgumentNullException(nameof(client));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Gets or sets the bearer authentication token.
		/// </summary>
		/// <value>
		/// The authentication token.
		/// </value>
		public virtual string AuthToken
		{
			get;
			set;
		}

		/// <summary>
		/// Deserializes if supported code asynchronous.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		protected virtual async Task<T> DeserializeIfSupportedCodeAsync<T>(HttpResponseMessage message)
			where T : IResults, new()
		{
			if(message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			switch (message.StatusCode)
			{
				case System.Net.HttpStatusCode.OK:
				case System.Net.HttpStatusCode.NotFound:
				case System.Net.HttpStatusCode.BadRequest:
					using (var data = await message.Content.ReadAsStreamAsync())
					{
						var obj = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(data);
						obj.Success = message.StatusCode == System.Net.HttpStatusCode.OK;
						return obj;
					}

				default:
					var err = new T
					{
						Success = false,
						Status = (int)message.StatusCode,
					};
					if (message.Content != null && message.Content.Headers.ContentLength > 0)
					{
						err.Title = await message.Content.ReadAsStringAsync();
					}
					return err;
			};
		}

		/// <summary>
		/// Adds the authorization header.
		/// </summary>
		/// <param name="message">The message.</param>
		protected virtual void AddAuthorizationHeader(HttpRequestMessage message)
			=> (message ?? throw new ArgumentNullException(nameof(message))).Headers.Authorization
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthToken);


		/// <summary>
		/// Gets the HttpClient from the factory.
		/// </summary>
		/// <returns></returns>
		protected abstract HttpClient GetClient();

		/// <summary>
		/// Prepares the path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">path</exception>
		protected virtual Uri PreparePath(string path)
		{
			if(path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			var builder = new UriBuilder(baseUri);
			if (path.StartsWith("/", StringComparison.InvariantCulture))
			{
				builder.Path = path;
			}
			else
			{
				builder.Path += path;
			}

			return builder.Uri;
		}

		/// <summary>
		/// Does a get call to the provided url
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		protected virtual async Task<T> GetAsync<T>(string url)
			where T : IResults, new()
		{
			var client = GetClient();
			try
			{
				using (var message = new HttpRequestMessage(HttpMethod.Get, PreparePath(url)))
				{
					AddAuthorizationHeader(message);
					if (logger.IsEnabled(LogLevel.Debug))
					{
						logger.LogDebug("RequestUri: {0}", message.RequestUri);
						logger.LogDebug("AuthHeader: {0}", message.Headers.Authorization);
					}
					var response = await client.SendAsync(message);
					return await DeserializeIfSupportedCodeAsync<T>(response);
				}
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
			{
				return new T()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

		/// <summary>
		/// Posts the object asynchronous.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		protected virtual async Task<T> PostAsync<T>(string url, object obj)
			where T : IResults, new()
		{
			var client = GetClient();
			try
			{
				using (var message = new HttpRequestMessage(HttpMethod.Post, PreparePath(url)))
				{
					AddAuthorizationHeader(message);
					if (logger.IsEnabled(LogLevel.Debug))
					{
						logger.LogDebug("RequestUri: {0}", message.RequestUri);
						logger.LogDebug("AuthHeader: {0}", message.Headers.Authorization);
					}

					message.Content = new StringContent(
							await Task.Run(() => JsonSerializer.Serialize(obj)),
							System.Text.Encoding.UTF8,
							"application/json");
					var response = await client.SendAsync(message);
					return await DeserializeIfSupportedCodeAsync<T>(response);
				}
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
			{
				return new T()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

		/// <summary>
		/// Puts the object asynchronous.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		protected virtual async Task<T> PutAsync<T>(string url, object obj)
			where T : IResults, new()
		{
			var client = GetClient();
			try
			{
				using (var message = new HttpRequestMessage(HttpMethod.Put, PreparePath(url)))
				{
					AddAuthorizationHeader(message);
					if (logger.IsEnabled(LogLevel.Debug))
					{
						logger.LogDebug("RequestUri: {0}", message.RequestUri);
						logger.LogDebug("AuthHeader: {0}", message.Headers.Authorization);
					}
					message.Content = new StringContent(
							await Task.Run(() => JsonSerializer.Serialize(obj)),
							System.Text.Encoding.UTF8,
							"application/json");
					var response = await client.SendAsync(message);
					return await DeserializeIfSupportedCodeAsync<T>(response);
				}
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
			{
				return new T()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

		/// <summary>
		/// Deletes the  asynchronous.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		protected virtual async Task<T> DeleteAsync<T>(string url)
			where T : IResults, new()
		{
			var client = GetClient();
			try
			{
				using (var message = new HttpRequestMessage(HttpMethod.Delete, PreparePath(url)))
				{
					AddAuthorizationHeader(message);
					if (logger.IsEnabled(LogLevel.Debug))
					{
						logger.LogDebug("RequestUri: {0}", message.RequestUri);
						logger.LogDebug("AuthHeader: {0}", message.Headers.Authorization);
					}
					var response = await client.SendAsync(message);
					return await DeserializeIfSupportedCodeAsync<T>(response);
				}
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
			{
				return new T()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}
	}
}
