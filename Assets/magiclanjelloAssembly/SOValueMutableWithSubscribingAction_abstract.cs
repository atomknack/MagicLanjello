using System;
using UKnack.Events;
using UKnack.Values;
using UnityEngine.Events;

namespace UKnack.Preconcrete.Values
{
    public abstract class SOValueMutableWithSubscribingAction<T> : SOValueMutable<T>
    {
        protected abstract void BeforeSubscribing();
        protected abstract void AfterUnsubscribing();

        public override void Subscribe(Action<T> subscriber)
        {
            BeforeSubscribing();
            base.Subscribe(subscriber);
        }

        public override void Subscribe(UnityEvent<T> subscriber)
        {
            BeforeSubscribing();
            base.Subscribe(subscriber);
        }
        public override void Subscribe(ISubscriberToEvent<T> subscriber)
        {
            BeforeSubscribing();
            base.Subscribe(subscriber);
        }

        internal override void Unsubscribe(Action<T> subscriber)
        {
            base.Unsubscribe(subscriber);
            AfterUnsubscribing();
        }
        internal override void Unsubscribe(UnityEvent<T> subscriber)
        {
            base.Unsubscribe(subscriber);
            AfterUnsubscribing();
        }
        internal override void Unsubscribe(ISubscriberToEvent<T> subscriber)
        {
            base.Unsubscribe(subscriber);
            AfterUnsubscribing();
        }
    }
}

