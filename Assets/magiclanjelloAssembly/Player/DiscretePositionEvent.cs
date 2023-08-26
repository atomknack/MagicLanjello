using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UKnack.Attributes;
using UnityEngine.Serialization;

namespace MagicLanjello.Player
{
    public class DiscretePositionEvent : NetworkBehaviour
    {
        [SerializeField]
        [ValidReference]
        private Transform _observedTarget;

        [SerializeField]
        UnityEvent<Vector3Int> _onClientsWhenChanged;

        [SerializeField]
        UnityEvent<Vector3> _onPlayerWhenChanged;

        [SerializeField]
        UnityEvent<Vector3Int> _onServerWhenChanged;

        [SyncVar(hook = nameof(OnClientPosChanged))]
        private Vector3Int _syncVarPosition = Vector3Int.zero;

        [SerializeField]
        private Vector3Int _discretePosition = Vector3Int.zero;

        private Vector3Int PositionToDiscrete(Vector3 pos) =>
            new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));

        public override void OnStartServer()
        {
            if (_observedTarget == null)
                throw new System.ArgumentNullException(nameof(_observedTarget));

            _syncVarPosition = _discretePosition = PositionToDiscrete(_observedTarget.position);
        }

        private void Update()
        {
            if (isServer ==false)
                return;

            var _newDiscrete = PositionToDiscrete(_observedTarget.position);
            if (_newDiscrete == _discretePosition)
                return;

            //Debug.Log($"Position changed from {_discretePosition} to {_newDiscrete}, parent: {transform.parent.name}");
            _syncVarPosition = _discretePosition = _newDiscrete;
            _onServerWhenChanged?.Invoke(_discretePosition);
        }

        private void OnClientPosChanged(Vector3Int oldPos, Vector3Int newPos) 
        {
            Debug.Log($"OnClientPosChanged. old: {oldPos}, new: {newPos}");
            if (oldPos == newPos)
            {
                Debug.Log($"This should not happen {oldPos} equal to {newPos}");
                return;
            }

            _onClientsWhenChanged?.Invoke(newPos);
            if (isLocalPlayer)
                _onPlayerWhenChanged?.Invoke(newPos);
        }
    }

}