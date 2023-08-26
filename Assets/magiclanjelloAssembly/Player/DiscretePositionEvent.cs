using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MagicLanjello.Player
{
    public class DiscretePositionEvent : NetworkBehaviour
    {
        [SerializeField]
        UnityEvent<Vector3Int> _onClientWhenPositionChanged;

        [SerializeField]
        UnityEvent<Vector3> _onPlayerOnlyWhenPositionChanged;

        [SyncVar(hook = nameof(OnClientPosChanged))]
        private Vector3Int _syncVarPosition = Vector3Int.zero;

        [SerializeField]
        private Vector3Int _discretePosition = Vector3Int.zero;

        private Vector3Int PositionToDiscrete(Vector3 pos) =>
            new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));

        public override void OnStartServer()
        {
            _syncVarPosition = _discretePosition = PositionToDiscrete(transform.position);
        }

        private void Update()
        {
            if (isServer ==false)
                return;

            var _newDiscrete = PositionToDiscrete(transform.position);
            if (_newDiscrete == _discretePosition)
                return;

            //Debug.Log($"Position changed from {_discretePosition} to {_newDiscrete}, parent: {transform.parent.name}");
            _syncVarPosition = _discretePosition = _newDiscrete;
        }

        private void OnClientPosChanged(Vector3Int oldPos, Vector3Int newPos) 
        {
            Debug.Log($"OnClientPosChanged. old: {oldPos}, new: {newPos}");
            if (oldPos == newPos)
            {
                Debug.Log($"This should not happen {oldPos} equal to {newPos}");
                return;
            }

            _onClientWhenPositionChanged?.Invoke(newPos);
            if (isLocalPlayer)
                _onPlayerOnlyWhenPositionChanged?.Invoke(newPos);
        }
    }

}