using MagicLanjello.CellPlaceholder;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UnityEngine;
using UnityEngine.Events;

namespace MagicLanjello.Player
{
    public class InputCellPlaceholderToUnityEvent : MonoBehaviour
    {
        [SerializeField]
        [ValidReference]
        private SOValue<CellPlaceholderStruct> _input;

        [SerializeField]
        private UnityEvent<CellPlaceholderStruct> _onValueChanged;

        private void PlaceholderChanged(CellPlaceholderStruct input) =>
            _onValueChanged?.Invoke(input);

        private void OnEnable()
        {
            if (_input == null)
                throw new System.ArgumentNullException(nameof(_input));
            _input.Subscribe(PlaceholderChanged);
        }

        private void OnDisable()
        {
            _input.UnsubscribeNullSafe(PlaceholderChanged);
        }
    }
}

