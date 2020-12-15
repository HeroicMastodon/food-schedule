using System.Linq.Expressions;
using System.Net.Http;
using System.Net;
using foodSchedule.Model.Request;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using foodSchedule.Model;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace foodSchedule.Net {
    public class ServerFacade {
        private static string authToken;

        public HttpClient Client { get; set; }

        public ILocalStorageService LocalStorage {get; set;}

        public ServerFacade(HttpClient client, ILocalStorageService localStorage) {
            Client = client;
            LocalStorage = localStorage;
        }

        public async Task<bool> Login(LoginRequest request)  {
            var res = await Client.PostAsJsonAsync("login", request);
            authToken = res.Headers.GetValues("Authorization").First();
            authToken = authToken.Substring(7);
            await LocalStorage.SetItemAsync("authtoken", authToken);
            Console.WriteLine(authToken);
            return res.IsSuccessStatusCode;
        } 

        public async Task<bool> Register(RegisterRequest request) {
            var res = await Client.PostAsJsonAsync("register", request);
            return res.IsSuccessStatusCode;
        }

        public async Task<GetFoodScheduleResponse> GetFoodSchedule() {
            var request = new HttpRequestMessage(HttpMethod.Get, "");

            if (authToken == null) {
                authToken = await LocalStorage.GetItemAsync<string>("authtoken");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var res = await Client.SendAsync(request);
            var list = await res.Content.ReadFromJsonAsync<GetFoodScheduleResponse>();
            return list;
        }

        public async Task<GetFoodScheduleResponse> UpdateFoodSchedule(List<string> days) {
            var request = new HttpRequestMessage(HttpMethod.Post, "update");
            request.Content = JsonContent.Create(new UpdateScheduleRequest(null, days));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var res = await Client.SendAsync(request);

            // var res = await Client.PostAsJsonAsync("update", new UpdateScheduleRequest(null, days));
            
            if (! res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<GetFoodScheduleResponse>();
        }
    }
}