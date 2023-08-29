using UnityEngine;
using Mirror;

public partial class SenderByteDataToClients
{
    private class InnerClient
    {
        SenderByteDataToClients _outer;

        public void ClientRecievedTotal(uint clientDataCount, NetworkConnectionToClient sender)
        {
            throw new System.NotImplementedException();
        }

        public InnerClient(SenderByteDataToClients outer)
        {
            _outer = outer;
        }
    }
}
