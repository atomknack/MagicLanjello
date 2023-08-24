using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MagicLanjello.Player
{
    public class DiscretePositionEvent : NetworkBehaviour
    {
        [SerializeField]
        UnityEvent<Vector3Int> _onClientWhenPositionChanged;

        [SyncVar(hook = nameof(OnClientPosChanged))]
        private Vector3Int _syncVarPosition = Vector3Int.zero;

        [SerializeField]
        private Vector3Int _discretePosition = Vector3Int.zero;

        private Vector3Int PositionToDiscrete(Vector3 pos) =>
            new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);

        private void Update()
        {
            if (isServer ==false)
                return;

            var _newDiscrete = PositionToDiscrete(transform.position);
            if (_newDiscrete == _discretePosition)
                return;

            _syncVarPosition = _discretePosition = _newDiscrete;
        }

        private void OnClientPosChanged(Vector3Int oldPos, Vector3Int newPos) 
        {
            if (oldPos == newPos)
            {
                Debug.Log($"This should not happen {oldPos} equal to {newPos}");
                return;
            }

            _onClientWhenPositionChanged?.Invoke(newPos);
        }
    }

}