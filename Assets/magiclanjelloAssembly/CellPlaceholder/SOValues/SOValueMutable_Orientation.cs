using DoubleEngine.Atom;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Preconcrete.Values;
using UnityEngine;


namespace MagicLanjello.CellPlaceholder.SOValues
{
    [CreateAssetMenu(fileName = "SOValueMutable_Orientation", menuName = "MagicLanjello/SOValueMutable_Orientation", order = 110)]
    public sealed class SOValueMutable_Orientation : SOValueMutableWithSubscribingAction<int>
    {
        [SerializeField][ValidReference] private SOEvent rotateX90;
        [SerializeField][ValidReference] private SOEvent rotateY90;
        [SerializeField][ValidReference] private SOEvent rotateZ90;
        [SerializeField][ValidReference] private SOEvent invertX;
        [SerializeField][ValidReference] private SOEvent invertY;
        [SerializeField][ValidReference] private SOEvent invertZ;

        [NonSerialized]
        private bool _subscribedToDependency = false;

        [NonSerialized]
        private Grid6SidesCached _orientation = Grid6SidesCached.Default;

        private void SetOrientationWithInvokeSubscribers(Grid6SidesCached value)
        {
            _orientation = value;
            InvokeSubscribers(this, OrientationAsInt);
        }

        private int OrientationAsInt => _orientation._orientation.index;

        public override int RawValue 
        { 
            get => OrientationAsInt;
            // we don't need Invoke subscribers here because SetValue do this afterwards if it setting value
            // if user setting RawValue directly he probably does NOT want to InvokeSubscribers here
            protected set 
            {
                ScaleInversionPerpendicularRotation3 temp = ScaleInversionPerpendicularRotation3.FromInt(value);
                _orientation = Grid6SidesCached.FromRotationAndScale(temp);
            }
        }

        void PlaceholderOrientationChanged_RotateX90() => SetOrientationWithInvokeSubscribers(_orientation.RotateX90());
        void PlaceholderOrientationChanged_RotateY90() => SetOrientationWithInvokeSubscribers(_orientation.RotateY90());
        void PlaceholderOrientationChanged_RotateZ90() => SetOrientationWithInvokeSubscribers(_orientation.RotateZ90());
        void PlaceholderOrientationChanged_InvertZ() => SetOrientationWithInvokeSubscribers(_orientation.InvertZ());
        void PlaceholderOrientationChanged_InvertX() => SetOrientationWithInvokeSubscribers(_orientation.InvertX());
        void PlaceholderOrientationChanged_InvertY() => SetOrientationWithInvokeSubscribers(_orientation.InvertY());


        protected override void AfterUnsubscribing()
        {
            if (_subscribedToDependency == false)
                return;
            if (SubscribersCount() > 0)
                return;

            rotateX90.UnsubscribeNullSafe(PlaceholderOrientationChanged_RotateX90);
            rotateY90.UnsubscribeNullSafe(PlaceholderOrientationChanged_RotateY90);
            rotateZ90.UnsubscribeNullSafe(PlaceholderOrientationChanged_RotateZ90);
            invertX.UnsubscribeNullSafe(PlaceholderOrientationChanged_InvertX);
            invertY.UnsubscribeNullSafe(PlaceholderOrientationChanged_InvertY);
            invertZ.UnsubscribeNullSafe(PlaceholderOrientationChanged_InvertZ);

            _subscribedToDependency = false;
        }

        protected override void BeforeSubscribing()
        {
            if (_subscribedToDependency)
                return;

            NullChecks();
            rotateX90.Subscribe(PlaceholderOrientationChanged_RotateX90);
            rotateY90.Subscribe(PlaceholderOrientationChanged_RotateY90);
            rotateZ90.Subscribe(PlaceholderOrientationChanged_RotateZ90);
            invertX.Subscribe(PlaceholderOrientationChanged_InvertX);
            invertY.Subscribe(PlaceholderOrientationChanged_InvertY);
            invertZ.Subscribe(PlaceholderOrientationChanged_InvertZ);

            _subscribedToDependency = true;
        }

        private void NullChecks()
        {
            if (rotateX90 == null) throw new ArgumentNullException(nameof(rotateX90));
            if (rotateY90 == null) throw new ArgumentNullException(nameof(rotateY90));
            if (rotateZ90 == null) throw new ArgumentNullException(nameof(rotateZ90));
            if (invertX == null) throw new ArgumentNullException(nameof(invertX));
            if (invertY == null) throw new ArgumentNullException(nameof(invertY));
            if (invertZ == null) throw new ArgumentNullException(nameof(invertZ));
        }
    }
}