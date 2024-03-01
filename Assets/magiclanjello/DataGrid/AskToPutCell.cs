using MagicLanjello.CellPlaceholder;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;
using UnityEngine.Events;

public class AskToPutCell : NetworkBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOPublisher<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient> _onServerWhenClientAskedCorrectly;

    private Vector3Int _positionOnServer = Vector3Int.zero;
    private CellPlaceholderStruct _cellPlaceholderOnServer = CellPlaceholderStruct.DefaultPlaceholder;

    public void ClientAskToPutCell()
    {
        if (isClient == false)
            throw new System.Exception("This operation can be only called from client");
        //Debug.Log($"OnClient trying to ask to put cell");
        CommandClientAskToPutCell();
    }

    public void OnServerPositionChanged(Vector3Int position)
    {
        if (isServer == false) 
            throw new System.Exception("this operation can only be called from Server");
        _positionOnServer = position;
    }

    public void OnServerCellPlaceholderChanged(CellPlaceholderStruct placeholder)
    {
        if (isServer == false)
            throw new System.Exception("this operation can only be called from Server");
        _cellPlaceholderOnServer = placeholder;
    }


    [Command]
    private void CommandClientAskToPutCell(NetworkConnectionToClient sender = null)
    {
        Debug.Log($"Server recieved request to by Client");
        _onServerWhenClientAskedCorrectly.Publish(_positionOnServer, _cellPlaceholderOnServer, sender);
    }

    public override void OnStartClient()
    {
        if(_onServerWhenClientAskedCorrectly == null)
            throw new System.ArgumentNullException(nameof(_onServerWhenClientAskedCorrectly));
    }

    public override void OnStopClient()
    {
        //_clientAskToPutCell.UnsubscribeNullSafe(ClientAsk);
    }

}
