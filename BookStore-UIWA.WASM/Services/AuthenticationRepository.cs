using Blazored.LocalStorage;
using BookStore_UIWA_WASM.Contracts;
using BookStore_UIWA_WASM.Models;
using BookStore_UIWA_WASM.Providers;
using BookStore_UIWA_WASM.Static;

using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UIWA_WASM.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        // private readonly IHttpClientFactory _client;
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationRepository(HttpClient client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.LoginEndpoint, user);
            //var request = new HttpRequestMessage(HttpMethod.Post
            //   , Endpoints.LoginEndpoint);
            //request.Content = new StringContent(JsonConvert.SerializeObject(user)
            //    , Encoding.UTF8, "application/json");

            //var client = _client.CreateClient();
            //HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);

            //Store Token
            await _localStorage.SetItemAsync("authToken", token.Token);

            //Change auth state of app
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider)
                .LoggedIn();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token.Token);

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider)
                .LoggedOut();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.LoginEndpoint, user);
            //var request = new HttpRequestMessage(HttpMethod.Post
            //    , Endpoints.RegisterEndpoint);
            //request.Content = new StringContent(JsonConvert.SerializeObject(user)
            //    , Encoding.UTF8, "application/json");

            //var client = _client.CreateClient();
            //HttpResponseMessage response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
