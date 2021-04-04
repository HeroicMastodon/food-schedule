using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace foodSchedule.Model.State
{
    public class State {
        private bool isLoggedIn;
        public bool IsLoggedIn {get {return isLoggedIn; } set { isLoggedIn = value; NotifySubscribers();}}
        private List<StateSubscriber> Subscribers = new List<StateSubscriber>();
        private bool isNotifying = false;

        public State() {
            IsLoggedIn = false;
        }

        public async void Subscribe(StateSubscriber subscriber)
        {
            await WaitWhileNotifying();
            Subscribers.Add(subscriber);
        }

        private async Task WaitWhileNotifying()
        {
            while (isNotifying)
            {
                await Task.Delay(25);
            }
        }

        public async  void UnSubscribe(StateSubscriber subscriber) {
            await WaitWhileNotifying();
            Subscribers.Remove(subscriber);
        }

        private async void NotifySubscribers() {
            await WaitWhileNotifying();
            isNotifying = true;
            Subscribers.ForEach(subscriber => subscriber.NotifyStateChanged());
            isNotifying = false;
        }
    }

    public interface StateSubscriber {
        void NotifyStateChanged();
    }
}