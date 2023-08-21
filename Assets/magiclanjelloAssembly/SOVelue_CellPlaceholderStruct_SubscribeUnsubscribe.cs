using System;
using UKnack.Events;
using UKnack.MagicLanjello;
using UKnack.Values;
using UnityEngine.Events;

namespace UKnack.Concrete.Values
{
    public sealed partial class SOValueMutable_CellPlaceholderStruct
    {
        // there should be 3 soValues
        // [SerializeField][PlaymodeDisabled] private SOValue<bool> _dependUpon;
        // 
        // 

        // [NonSerialized]  bool _subscribedTo_dependUpon = false;
        // getter of RawValue

        private void ValueOfOrientationChanged(int newOrientation)
        {
            InvokeSubscribers(this, RawValue);
        }

        private void TrySubscribeToProvider()
        {
            throw new System.NotImplementedException();
            //if (_subscribedTo_dependUpon == true)
            //    return;
            //if (_dependUpon == null)
            //    return;
            //_dependUpon.Subscribe(DependUponValueChanged);
            ////_subscribedTo_dependUpon = true;
        }


        private void TryUnSubscribeFromProvider()
        {
            throw new System.NotImplementedException();
            //if (_subscribedTo_dependUpon == false)
            //    return;
            //if (_dependUpon == null)
            //    return;
            //if (SubscribersCount() == 0)
            //{
            //    _dependUpon.UnsubscribeNullSafe(DependUponValueChanged);
            //    _subscribedTo_dependUpon = false;
            //}
        }

        public override void Subscribe(Action<CellPlaceholderStruct> subscriber)
        {
            TrySubscribeToProvider();
            base.Subscribe(subscriber);
        }
        public override void Subscribe(UnityEvent<CellPlaceholderStruct> subscriber)
        {
            TrySubscribeToProvider();
            base.Subscribe(subscriber);
        }
        public override void Subscribe(ISubscriberToEvent<CellPlaceholderStruct> subscriber)
        {
            TrySubscribeToProvider();
            base.Subscribe(subscriber);
        }

        internal override void Unsubscribe(Action<CellPlaceholderStruct> subscriber)
        {
            base.Unsubscribe(subscriber);
            TryUnSubscribeFromProvider();
        }
        internal override void Unsubscribe(UnityEvent<CellPlaceholderStruct> subscriber)
        {
            base.Unsubscribe(subscriber);
            TryUnSubscribeFromProvider();
        }
        internal override void Unsubscribe(ISubscriberToEvent<CellPlaceholderStruct> subscriber)
        {
            base.Unsubscribe(subscriber);
            TryUnSubscribeFromProvider();
        }

    }
}
