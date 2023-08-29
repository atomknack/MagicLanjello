using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SenderByteDataToClients : NetworkBehaviour
{
    private byte[] _data = new byte[1000];
    private uint _dataCount = 0;
    private int _maxArraySegmentSize = 10;

    protected Dictionary<int, uint> _clientsRecievedDataCount = new Dictionary<int, uint>();

    InnerClient _innerClient;

    [Command(requiresAuthority = false)]
    protected void CmdClientRecievedTotal(uint clientDataCount, NetworkConnectionToClient sender = null) =>
        _innerClient.ClientRecievedTotal(clientDataCount, sender);


}
