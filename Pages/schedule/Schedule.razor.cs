using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using foodSchedule.Model.State;
using foodSchedule.Net;
using Microsoft.AspNetCore.Components;

namespace foodSchedule.Pages
{
    public class ScheduleBase : ComponentBase, StateSubscriber, IDisposable
    {
        [Inject]
        public State State { get; set; }
        [Inject]
        public ServerFacade ServerFacade { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        
        public List<string> Days = new List<string> { "", "", "", "", "", "", "" };
        private bool disposedValue;

        protected async override Task OnInitializedAsync()
        {
            if (!State.IsLoggedIn && !await ServerFacade.Authenticate())
            {
                NavigationManager.NavigateTo("");
            }
            await RetrieveFoodSchedule();
            State.Subscribe(this);
        }

        public async void updateList()
        {
            await ServerFacade.UpdateFoodSchedule(Days);
        }

        private async Task RetrieveFoodSchedule()
        {
            try
            {
                var res = await ServerFacade.GetFoodSchedule();
                Days = res.Days;

                if (Days == null || Days.Count < 7)
                {
                    Days = new List<string> { "", "", "", "", "", "", "" };
                }
            }
            catch (Exception)
            {
                NavigationManager.NavigateTo("");
            }
        }

        public void NotifyStateChanged()
        {
            if (! State.IsLoggedIn) {
                NavigationManager.NavigateTo("");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    State.UnSubscribe(this);
                }
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ScheduleBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
