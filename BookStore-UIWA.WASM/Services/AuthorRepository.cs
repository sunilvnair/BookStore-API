using Blazored.LocalStorage;
using BookStore_UIWA_WASM.Contracts;
using BookStore_UIWA_WASM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStore_UIWA_WASM.Services
{
    public class AuthorRepository : BaseRepository<Author> , IAuthorRepository
    {
        // private readonly IHttpClientFactory _client;
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        public AuthorRepository(HttpClient client ,
             ILocalStorageService localStorage) :base (client , localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }
    }
}

