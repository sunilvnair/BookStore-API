﻿using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        // private readonly IHttpClientFactory _client;
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;

        //public BaseRepository(IHttpClientFactory client ,
        public BaseRepository(HttpClient client,
            ILocalStorageService localStorage)
        {
            _client = client;
            _localStorage = localStorage;
    }
        public async  Task<bool> Create(string url, T obj)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await _client.PostAsJsonAsync <T>(url, obj);

            //var request = new HttpRequestMessage(HttpMethod.Post , url);
            //if (obj == null)
            //    return false;
            //request.Content = new StringContent(JsonConvert.SerializeObject(obj),
            //           Encoding.UTF8, "application/json");

            //var client = _client.CreateClient();
            //client.DefaultRequestHeaders.Authorization =
            //   new AuthenticationHeaderValue("bearer", await GetBearerToken());
            //HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return true;
            return false;


        }

        public async  Task<bool> Delete(string url, int id)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await _client.DeleteAsync(url + id);

   //         var request = new HttpRequestMessage(HttpMethod.Delete, url +id );
   //         if (id <1)
   //             return false;
          
   //         var client = _client.CreateClient();
   //         client.DefaultRequestHeaders.Authorization =
   //new AuthenticationHeaderValue("bearer", await GetBearerToken());

   //         HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            return false;
        }

        public async Task<T> Get(string url, int id)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var response = await _client.GetFromJsonAsync<T>(url + id);
   //         var request = new HttpRequestMessage(HttpMethod.Get, url +id);
   //         if (id < 1)
   //             return null;

   //         var client = _client.CreateClient();
   //         client.DefaultRequestHeaders.Authorization =
   //new AuthenticationHeaderValue("bearer", await GetBearerToken());
   //         HttpResponseMessage response = await client.SendAsync(request);

   //         if (response.StatusCode == System.Net.HttpStatusCode.OK)
   //         {
   //             var content = await response.Content.ReadAsStringAsync();
   //             return JsonConvert.DeserializeObject<T>(content);
   //         }
            return response;
          
        }

        public async  Task<IList<T>> Get(string url)
        {

            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var response = await _client.GetFromJsonAsync<IList<T>>(url );

   //         _client.DefaultRequestHeaders.Authorization =
   //             new AuthenticationHeaderValue("bearer", await GetBearerToken());

   //         var request = new HttpRequestMessage(HttpMethod.Get, url);
       

   //         var client = _client.CreateClient();
   //         client.DefaultRequestHeaders.Authorization =
   //new AuthenticationHeaderValue("bearer", await GetBearerToken());
   //         HttpResponseMessage response = await client.SendAsync(request);

   //         if (response.StatusCode == System.Net.HttpStatusCode.OK)
   //         {
   //             var content = await response.Content.ReadAsStringAsync();
   //             return JsonConvert.DeserializeObject<IList<T>>(content);
   //         }
            return response;
        }

        public async Task<bool> Update(string url, T obj,int id)
        {
            if (obj == null)
                return false;

            _client.DefaultRequestHeaders.Authorization =
new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var response = await _client.PutAsJsonAsync<T>(url + id,obj);


   //         var request = new HttpRequestMessage(HttpMethod.Put, url+id);
   //         if (obj == null)
   //             return false;
   //         request.Content = new StringContent(JsonConvert.SerializeObject(obj),
   //             Encoding.UTF8,"application/json");

   //         var client = _client.CreateClient();
   //         client.DefaultRequestHeaders.Authorization =
   //new AuthenticationHeaderValue("bearer", await GetBearerToken());
   //         HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            return false;
        }
        private async Task <string> GetBearerToken ()
        {

            return await _localStorage.GetItemAsync<string>("authToken");


        }
}
}
