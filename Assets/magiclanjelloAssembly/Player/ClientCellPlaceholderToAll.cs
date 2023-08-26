using MagicLanjello.CellPlaceholder;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MagicLanjello.Player
{
    public class ClientCellPlaceholderToAll : NetworkBehaviour
    {
        [SerializeField]
        private UnityEvent<CellPlaceholderStruct> _varHookOnAllClients;

        [SyncVar(hook = nameof(HookOnAllClients))]
        private CellPlaceholderStruct _cellPlaceholder;

        [SerializeField]
        private UnityEvent<CellPlaceholderStruct> _onServerWhenChanged;

        public void OnClientAskToChangeOrientation(CellPlaceholderStruct cell) =>
            CommandClientAskToChangeOrientation(cell);

        [Command]
        private void CommandClientAskToChangeOrientation(CellPlaceholderStruct cell)
        {
            _cellPlaceholder = cell;
            _onServerWhenChanged?.Invoke(cell);
        }


        private void HookOnAllClients(CellPlaceholderStruct oldCell, CellPlaceholderStruct newCell)
        {
            _varHookOnAllClients?.Invoke(newCell);
        }
    }
}
