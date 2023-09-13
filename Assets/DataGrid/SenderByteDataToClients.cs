using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;
using UKnack.Attributes;
using UKnack.Events;

public partial class SenderByteDataToClients : NetworkBehaviour
{
    private byte[] _data = new byte[100_000_000];
    private int _dataCount = 0;
    private ArraySegment<byte> DataSegment => new System.ArraySegment<byte>(_data, 0, _dataCount);

    private short _dataVersion = 0;

    [SerializeField]
    [ValidReference]
    private SOPublisher _notHostClient_RequiestToVersionChange;
    [SerializeField]
    [ValidReference]
    private SOEvent _notHosClient_AnswerReadyToVersionChange;

    [SerializeField]
    [ValidReference(
        typeof(IDataUpdater<ArraySegment<byte>>), nameof(IDataUpdater<ArraySegment<byte>>.Cast), typeof(IDataUpdater<ArraySegment<byte>>))]
    private UnityEngine.Object _clientDataUpdater;

    private IDataUpdater<ArraySegment<byte>> ClientDataUpdater => (IDataUpdater<ArraySegment<byte>>)_clientDataUpdater;


    [SerializeField]
    private UnityEvent<System.ArraySegment<byte>> _logClientDataRecieved;

    ServerSide _innerServer;
    ClientSide _innerClient;

    public void AddToData(ArraySegment<byte> additional)
    {
        if (TryIsClientButNotServer)
            throw new System.InvalidOperationException("This method could be called only on server");

        for (int i = 0; i < additional.Count; i++)
        {
            _data[_dataCount + i] = additional[i];
        }
        _dataCount = _dataCount + additional.Count;

        if (TryIsServer)
            _innerServer.UpdateClients();
        else
            ClientDataUpdater.UpdateData(DataSegment);
    }

    //called on server
    public void UpDataVersion()
    {
        if (TryIsClientButNotServer)
            throw new System.InvalidOperationException("This method could be called only on server");
        _dataVersion = unchecked((short)(_dataVersion + 1));
        _dataCount = 0;
        if (TryIsServer)
            _innerServer.UpdateClients();
        else
            ClientDataUpdater.ResetData();
    }

    protected void ClientDataRecievedEvent()
    {
        _logClientDataRecieved?.Invoke(DataSegment);
        ClientDataUpdater.UpdateData(DataSegment);
    }


    

    [Command(requiresAuthority = false)]
    protected void CmdClientChangedDataVersion(short dataVersion, NetworkConnectionToClient sender = null) =>
        _innerServer.Command_ClientChangedDataVersion(dataVersion, sender);

    [TargetRpc]
    protected void TargetChangeDataVersion(NetworkConnectionToClient target, short dataVersion)
    {
        if (isServer)
        {
            //_onHostClientDataVersionGonnaChange.Invoke();
            CmdClientChangedDataVersion(_dataVersion);
            return;
        }
        _innerClient.TargetChangeDataVersion(target, dataVersion);
    }


    [Command(requiresAuthority = false)]
    protected void CmdClientRecievedTotal(int clientDataCount, NetworkConnectionToClient sender = null) =>
        _innerServer.Command_ClientRecievedTotal(clientDataCount, sender);

    [TargetRpc]
    protected void TargetSendedDataToClient(NetworkConnectionToClient target, System.ArraySegment<byte> transfer, int currentClientCountShouldBe) =>
        _innerClient.TargetSendedDataToClient(target, transfer, currentClientCountShouldBe);



    public override void OnStartServer()
    {
        //if (_dataVersion < 0)
        //    _dataVersion = 0;
        _innerServer = new ServerSide(this);


        //_serverRunning = true;

        //FillDataArray();
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            //StartHostClient();
            _innerClient = new ClientSideIsHost(this);
        }
        else
        {
            _innerClient = new ClientSideIsClient(this);
            _notHosClient_AnswerReadyToVersionChange.Subscribe(_innerClient.ReadyToChangeDataVersion);
        }
        CmdClientRecievedTotal(_dataCount);
    }

    public override void OnStopClient()
    {
        if (isServer)
        {
            //StopHostClient();
        }
        else
        {
            _notHosClient_AnswerReadyToVersionChange.UnsubscribeNullSafe(_innerClient.ReadyToChangeDataVersion);
        }
    }

    public override void OnStopServer()
    {
    }

    private void ResetData()
    {
        _dataVersion = 0;
        _dataCount = 0;
    }

    private bool TryIsServer
    {
        get
        {
            try
            {
                return isServer;
            }
            catch
            {
                return false;
            }
        }
    }
    private bool TryIsClientButNotServer
    {
        get
        {
            try
            {
                return isServer == false && isClient == true;
            }
            catch
            {
                return false;
            }
        }
    }

}

    /*
    private void FillDataArray()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = (byte)i;
        }
        _dataCount = _data.Length;
    }

    public void OutsideCalledAddToData()
    {
        if (isServer == false)
            throw new System.Exception("This method can only be called from server");

        int numberToAdd = Random.Range(100, 2000);
        int newLength = _dataCount + numberToAdd;
        for (int i = _dataCount; i < _dataCount + numberToAdd; i++)
        {
            _data[i] = (byte)i;
        }
        _dataCount = newLength;


        _innerServer.UpdateClients();
    }

    private void CheckStandardDataAddition()
    {
        for (int i = 0; i < _dataCount; ++i)
        {
            if (_data[i] != (byte)i)
            {
                string message = $"at {i} of {_dataCount}, {_data[i]} not equal to {(byte)i}";
                Debug.LogError("logged Error:" + message);
                throw new System.Exception(message);
            }
        }
        Debug.Log($"Checked {_dataCount}");
    }
    */