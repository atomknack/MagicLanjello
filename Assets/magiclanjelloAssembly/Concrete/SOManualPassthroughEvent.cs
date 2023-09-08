using UnityEngine;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using System;
using UnityEngine.Events;

namespace MagicLanjello.Concrete
{
    [CreateAssetMenu(fileName = "SOManualPassthroughEvent", menuName = "MagicLanjello/SOManualPassthroughEvent")]
    public class SOManualPassthroughEvent : SOEvent
    {
        [SerializeField]
        private SOEvent _output;

        [SerializeField]
        private bool _isPassthrough = false;

        private bool _havePendingEventToPass = false;

        public bool HavePendingEvent => _havePendingEventToPass;
        public bool IsPassthrough => _isPassthrough;
        public bool TryPassEvent()
        {
            if (HavePendingEvent)
            {
                base.InternalInvoke();
                if (_output != null )
                    _output.InternalInvoke();
                _havePendingEventToPass = false;
                return true;
            }
            return false;
        }
        internal override void InternalInvoke()
        {
            _havePendingEventToPass = true;
            AttemptPassthrough();
        }

        public void SetPassthroughMode(bool passthrough)
        {
            _isPassthrough = passthrough;
            AttemptPassthrough();
        }

        private void AttemptPassthrough()
        {
            if (IsPassthrough)
                TryPassEvent();
        }

    }
}