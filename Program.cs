using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using foodSchedule.Net;
using Blazored.LocalStorage;

namespace foodSchedule {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var uri = builder.HostEnvironment.IsDevelopment() ?
                new Uri("http://localhost:5000/api/food/") :
                new Uri(builder.HostEnvironment.BaseAddress + "api/food/");

            builder.Services
                .AddScoped(sp => new HttpClient { BaseAddress = uri })
                .AddBlazoredLocalStorage()
                .AddScoped<ServerFacade, ServerFacade>();

            await builder.Build().RunAsync();
        }
    }
}
