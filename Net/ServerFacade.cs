using System.Net.Http;
using foodSchedule.Model.Request;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using foodSchedule.Model;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using foodSchedule.Model.State;

namespace foodSchedule.Net {
    public class ServerFacade {
        private static string authToken;

        public HttpClient Client { get; set; }

        public ILocalStorageService LocalStorage {get; set;}
        public State SessionState {get; private set;}

        public ServerFacade(HttpClient client, ILocalStorageService localStorage, State state) {
            Client = client;
            LocalStorage = localStorage;
            SessionState = state;
        }

        public async Task<bool> Login(LoginRequest request)
        {
            var res = await Client.PostAsJsonAsync("login", request);
            await SetLoginInfo(res);
            return SessionState.IsLoggedIn;
        }

        private async Task SetLoginInfo(HttpResponseMessage res)
        {
            authToken = res.Headers.GetValues("Authorization").First();
            authToken = authToken.Substring(7);
            await LocalStorage.SetItemAsync("authtoken", authToken);
            SessionState.IsLoggedIn = res.IsSuccessStatusCode;
        }

        public async Task<bool> Register(RegisterRequest request) {
            var res = await Client.PostAsJsonAsync("register", request);
            await SetLoginInfo(res);
            return SessionState.IsLoggedIn;
        }

        public async Task<bool> Authenticate() {
            var request = new HttpRequestMessage(HttpMethod.Get, "login");
            try {
                await AddAuthTokenToRequest(request);
                var res = await Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                SessionState.IsLoggedIn = res.IsSuccessStatusCode;

            } catch (Exception) {
                SessionState.IsLoggedIn = false;
            }
            return SessionState.IsLoggedIn;
        }

        public async Task<GetFoodScheduleResponse> GetFoodSchedule()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "");

            await AddAuthTokenToRequest(request);

            var res = await Client.SendAsync(request);
            var list = await res.Content.ReadFromJsonAsync<GetFoodScheduleResponse>();

            return list;
        }

        private async Task AddAuthTokenToRequest(HttpRequestMessage request)
        {
            if (authToken == null)
            {
                authToken = await LocalStorage.GetItemAsync<string>("authtoken");
                if (authToken == null)
                {
                    throw new Exception("Missing auth Token");
                }
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        public async Task<GetFoodScheduleResponse> UpdateFoodSchedule(List<string> days) {
            var request = new HttpRequestMessage(HttpMethod.Post, "update");
            request.Content = JsonContent.Create(new UpdateScheduleRequest(null, days));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var res = await Client.SendAsync(request);

            if (! res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<GetFoodScheduleResponse>();
        }

        public async Task<bool> Logout() {
            await LocalStorage.ClearAsync();
            SessionState.IsLoggedIn = false;
            return true;
        }

        public async Task<GetExpensesResponse> RetrieveExpenses() {
            var request = new HttpRequestMessage(HttpMethod.Get, "expenses");

            await AddAuthTokenToRequest(request);

            var res = await Client.SendAsync(request);
            var expenses = await res.Content.ReadFromJsonAsync<GetExpensesResponse>();

            return expenses;
        }

        public async Task<GetExpensesResponse> UpdateExpenses(List<Expense> expenses) {
            var request = new HttpRequestMessage(HttpMethod.Post, "expenses/update");
            request.Content = JsonContent.Create(new UpdateExpensesRequest(null, expenses));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var res = await Client.SendAsync(request);

            if (! res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<GetExpensesResponse>();
        }
    }
}