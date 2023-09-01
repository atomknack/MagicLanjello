using UnityEngine;
using UnityEngine.Events;
using Mirror;

public partial class SenderByteDataToClients : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent<System.ArraySegment<byte>> _onClientDataRecieved;

    private byte[] _data = new byte[100_000_000];
    private int _dataCount = 0;

    private short _dataVersion = 0;

    [SerializeField]
    private UnityEvent<System.ArraySegment<byte>> _onNotHostClientBeforeDataVersionChange;

    ServerSide _innerServer;
    ClientSide _innerClient;

    //called on server
    public void OutsideServerUpDataVersion()
    {
        if (isServer == false)
            throw new System.InvalidOperationException("This method could be called only on server");
        _dataVersion = unchecked((short)(_dataVersion + 1));
        _innerServer.UpdateClients();
    }

    //when server updated version and client was notified, client won't get any new data from server until it confirm new data version
    public void OutsideClientReadyToChangeDataVersion()
    {
        if (isClient == false)
            throw new System.InvalidOperationException("This method could be called only on client");
        _innerClient.ReadyToChangeDataVersion();
    }

    public void OutsideCalledAddToData()
    {
        if (isServer == false)
            throw new System.Exception("This method can only be called from server");

        int numberToAdd = Random.Range(100, 2000);
        int newLength = _dataCount + numberToAdd;
        for (int i = _dataCount; i < _dataCount+numberToAdd; i++)
        {
            _data[i] = (byte)i;
        }
        _dataCount = newLength;


        _innerServer.UpdateClients();
    } 

    protected void ClientDataRecievedEvent()
    {
        //Debug.Log(_dataCount);
        _onClientDataRecieved?.Invoke(new System.ArraySegment<byte>(_data, 0, _dataCount));
    }
    protected void ClientThatNotHostDataChangeEvent()
    {
        //Debug.Log(_dataCount);
        _onNotHostClientBeforeDataVersionChange?.Invoke(new System.ArraySegment<byte>(_data, 0, _dataCount));
    }

    [Command(requiresAuthority = false)]
    protected void CmdClientChangedDataVersion(short dataVersion, NetworkConnectionToClient sender = null) =>
        _innerServer.Command_ClientChangedDataVersion(dataVersion, sender);

    [TargetRpc]
    protected void TargetChangeDataVersion(NetworkConnectionToClient target, short dataVersion)
    {
        if (ServerSide.IsConnectionFromHost(target))
        {
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
        _innerServer = new ServerSide(this);

        //_serverRunning = true;

        //FillDataArray();
    }

    public override void OnStartClient()
    {
        _innerClient = new ClientSide(this);

        if (isServer == false)
        {
            _dataCount = 0;
        }

        CmdClientRecievedTotal(_dataCount);
    }

    public override void OnStopClient()
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

    public override void OnStopServer()
    {
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
