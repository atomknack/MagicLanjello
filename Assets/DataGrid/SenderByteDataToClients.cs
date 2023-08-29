using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Mirror;
using UnityEngine;

public partial class SenderByteDataToClients : NetworkBehaviour
{
    private byte[] _data = new byte[100_000_000];
    private uint _dataCount = 0;
    private int _maxArraySegmentSize = 100;

    protected Dictionary<int, uint> _clientsRecievedDataCount = new Dictionary<int, uint>();

    InnerServer _innerServer;
    InnerClient _innerClient;

    [Command(requiresAuthority = false)]
    protected void CmdClientRecievedTotal(uint clientDataCount, NetworkConnectionToClient sender = null) =>
        _innerServer.Command_ClientRecievedTotal(clientDataCount, sender);

    [TargetRpc]
    protected void TargetSendedDataToClient(NetworkConnectionToClient target, ArraySegment<byte> transfer, uint currentClientCountShouldBe) =>
        _innerClient.TargetSendedDataToClient(target, transfer, currentClientCountShouldBe);

    protected void OnServerWhenClientConnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"Client {conn.connectionId} connected to server");
        if (IsConnectionFromHost(conn))
        {
            Debug.Log($"No need to send data to itself for {conn.connectionId}");
            return;
        }
        ZeroClient(conn);
        //SendDataToClient(conn);
    }

    private void ZeroClient(NetworkConnectionToClient conn)
    {
        if (IsConnectionFromHost(conn))
            throw new System.Exception("This method should not be ever called from host");

        if (_clientsRecievedDataCount.ContainsKey(conn.connectionId))
            throw new System.Exception($"there are already {conn.connectionId}");

        _clientsRecievedDataCount[conn.connectionId] = 0;
    }

    protected void OnServerWhenClientDisconnect(NetworkConnectionToClient conn)
    {
        _clientsRecievedDataCount.Remove(conn.connectionId);
    }

    public override void OnStartServer()
    {
        _innerServer = new InnerServer(this);
        NetworkManagerCallbacks.OnServerWhenClientConnect += OnServerWhenClientConnect;
        NetworkManagerCallbacks.OnServerWhenClientDisconnect += OnServerWhenClientDisconnect;

        _maxArraySegmentSize = GetMaxArraySegmentSize();
        //_serverRunning = true;

        FillDataArray();
    }

    public override void OnStartClient()
    {
        _innerClient = new InnerClient(this);
        if (isServer == false)
            _maxArraySegmentSize = GetMaxArraySegmentSize();
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
        //if (_serverRunning == false)
        //    return;

        NetworkManagerCallbacks.OnServerWhenClientConnect -= OnServerWhenClientConnect;
        NetworkManagerCallbacks.OnServerWhenClientDisconnect -= OnServerWhenClientDisconnect;

        //_serverRunning = false;
    }
    private int GetMaxArraySegmentSize() =>
        Math.Max(100, NetworkManager.singleton.transport.GetMaxPacketSize() - 64);

    private bool IsConnectionFromHost(NetworkConnectionToClient conn)
    {
        if (conn == null)
            throw new System.ArgumentNullException($"are you trying that null connection is local? Why?");

        LocalConnectionToClient local = NetworkServer.localConnection;
        if (local != null)
        {
            if (conn == local)
            {
                Debug.Log($"Just so you know connection: {conn.connectionId} is local: {local.connectionId}");
                return true;
            }
        }
        return false;
    }

    private void FillDataArray()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = (byte)i;
        }
        _dataCount = (uint)_data.Length;
    }

}
