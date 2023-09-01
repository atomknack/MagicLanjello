using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Mirror;
using UnityEngine;
using UnityEngine.Events;

public partial class SenderByteDataToClients : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent<ArraySegment<byte>> _onClientDataRecieved;

    private byte[] _data = new byte[100_000_000];
    private int _dataCount = 0;

    ServerSide _innerServer;
    ClientSide _innerClient;

    protected void ClientDataRecievedEvent() => 
        _onClientDataRecieved?.Invoke(new ArraySegment<byte>(_data, 0, _dataCount));

    [Command(requiresAuthority = false)]
    protected void CmdClientRecievedTotal(int clientDataCount, NetworkConnectionToClient sender = null) =>
        _innerServer.Command_ClientRecievedTotal(clientDataCount, sender);

    [TargetRpc]
    protected void TargetSendedDataToClient(NetworkConnectionToClient target, ArraySegment<byte> transfer, int currentClientCountShouldBe) =>
        _innerClient.TargetSendedDataToClient(target, transfer, currentClientCountShouldBe);



    public override void OnStartServer()
    {
        _innerServer = new ServerSide(this);
        NetworkManagerCallbacks.OnServerWhenClientConnect += _innerServer.OnServerWhenClientConnect;
        NetworkManagerCallbacks.OnServerWhenClientDisconnect += _innerServer.OnServerWhenClientDisconnect;

        //_serverRunning = true;

        FillDataArray();
    }

    public override void OnStartClient()
    {
        _innerClient = new ClientSide(this);

        if (isServer == false)
        {
            _dataCount = 0;
            CmdClientRecievedTotal(_dataCount);
        }
    }

    public override void OnStopClient()
    {
        for (int i = 0; i < _dataCount; ++i)
        {
            if (_data[i] != (byte)i)
            {
                string message = $"at {i} of {_dataCount}, {_data[i]} not equal to {(byte)i}";
                Debug.LogError("logged Error:" + message);
                throw new Exception(message);
            }
        }
        Debug.Log($"Checked {_dataCount}");
    }

    public override void OnStopServer()
    {
        NetworkManagerCallbacks.OnServerWhenClientConnect -= _innerServer.OnServerWhenClientConnect;
        NetworkManagerCallbacks.OnServerWhenClientDisconnect -= _innerServer.OnServerWhenClientDisconnect;
    }



    private void FillDataArray()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = (byte)i;
        }
        _dataCount = _data.Length;
    }

}
