using UnityEngine;
using UnityEngine.Events;
using Mirror;

public partial class SenderByteDataToClients : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent<System.ArraySegment<byte>> _onClientDataRecieved;

    private byte[] _data = new byte[100_000_000];
    private int _dataCount = 0;

    ServerSide _innerServer;
    ClientSide _innerClient;

    protected void ClientDataRecievedEvent()
    {
        //Debug.Log(_dataCount);
        _onClientDataRecieved?.Invoke(new System.ArraySegment<byte>(_data, 0, _dataCount));
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

        FillDataArray();
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
