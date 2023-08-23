using MagicLanjello.CellPlaceholder;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClientCellPlaceholderToAll : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent<CellPlaceholderStruct> _onAllClients;

    public void OnClientAskToChangeOrientation(CellPlaceholderStruct cell) =>
        CommandClientAskToChangeOrientation(cell);

    [Command]
    private void CommandClientAskToChangeOrientation(CellPlaceholderStruct cell) =>
        RpcOnAllClients(cell);

    [ClientRpc]
    private void RpcOnAllClients(CellPlaceholderStruct cell)
    {
        _onAllClients?.Invoke(cell);
    }
}
