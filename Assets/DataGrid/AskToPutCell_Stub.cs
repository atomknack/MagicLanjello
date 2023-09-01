using Mirror;
using System.Collections;
using System.Collections.Generic;
using UKnack.Events;
using UnityEngine;
using UnityEngine.Events;

public class AskToPutCell_Stub : NetworkBehaviour
{
    [SerializeField]
    private SOEvent _clientAskToPutCell;

    [SerializeField]
    private UnityEvent _onServerWhenClientAskedCorrectly_Stub;

    private void ClientAsk()
    {
        Debug.Log($"OnClient trying to ask to put cell");
        CommandClientAskToPutCell();
    }


    [Command(requiresAuthority = false)]
    private void CommandClientAskToPutCell(NetworkConnectionToClient sender = null)
    {
        Debug.Log($"Server recieved request to by Client");
        _onServerWhenClientAskedCorrectly_Stub?.Invoke();
    }

    public override void OnStartClient()
    {
        _clientAskToPutCell.Subscribe(ClientAsk);
    }

    public override void OnStopClient()
    {
        _clientAskToPutCell.UnsubscribeNullSafe(ClientAsk);
    }

}
