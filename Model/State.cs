using System.Collections.Generic;

namespace foodSchedule.Model.State
{
    public class State {
        private bool isLoggedIn;
        public bool IsLoggedIn {get {return isLoggedIn; } set { isLoggedIn = value; NotifySubscribers();}}
        private List<StateSubscriber> Subscribers = new List<StateSubscriber>();

        public State() {
            IsLoggedIn = false;
        }

        public void Subscribe(StateSubscriber subscriber) {
            Subscribers.Add(subscriber);
        }

        public void UnSubscribe(StateSubscriber subscriber) {
            Subscribers.Remove(subscriber);
        }

        private void NotifySubscribers() {
            Subscribers.ForEach(subscriber => subscriber.NotifyStateChanged());
        }
    }

    public interface StateSubscriber {
        void NotifyStateChanged();
    }
}