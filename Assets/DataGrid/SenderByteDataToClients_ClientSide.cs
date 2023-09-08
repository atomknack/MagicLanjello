using UnityEngine;
using Mirror;
using System;

public partial class SenderByteDataToClients
{
    protected interface ClientSide
    {
        public void TargetSendedDataToClient(NetworkConnectionToClient target, ArraySegment<byte> transfer, int currentClientCountShouldBe);
        public void TargetChangeDataVersion(NetworkConnectionToClient target, short dataVersion);
        public void ReadyToChangeDataVersion();
    }

    protected class ClientSideIsClient : ClientSide
    {
        SenderByteDataToClients _outer;
        short notHostClientDataVersionToChange = 0;

        public void TargetSendedDataToClient(NetworkConnectionToClient target, ArraySegment<byte> transfer, int currentClientCountShouldBe)
        {
            //Debug.Log($"RpcSendedDataToClient called id null: {target == null}, arraysegment null: {transfer == null}");
            ////Debug.Log($"RpcSendedDataToClient called from {target}");
            ////Debug.Log($"RpcSendedDataToClient called from id {target.connectionId}, with {transfer}");
            //Debug.Log($"RpcSendedDataToClient {_outer._dataCount} and new transfer is: {transfer.Count}, checker: {currentClientCountShouldBe}");
            //Debug.Log($"{transfer[0]} {transfer.Count} {transfer.Array.Length}");

            if (currentClientCountShouldBe != _outer._dataCount)
                throw new System.Exception($"client have {_outer._dataCount}, and server expecting it to have {currentClientCountShouldBe}, there is tear somewhere");
            int start = _outer._dataCount;
            for (int i = 0; i < transfer.Count; ++i)
            {
                int dataIndex = start + i;
                _outer._data[dataIndex] = transfer[i];
            }
            _outer._dataCount = start + transfer.Count;

            _outer.ClientDataRecievedEvent();
            _outer.CmdClientRecievedTotal(_outer._dataCount);
        }

        public void TargetChangeDataVersion(NetworkConnectionToClient target, short dataVersion)
        {
            notHostClientDataVersionToChange = dataVersion;
            _outer._notHostClient_RequiestToVersionChange.Publish();
        }

        public void ReadyToChangeDataVersion()
        {
            Debug.Log($"Client ready to Change version from {_outer._dataVersion} to {notHostClientDataVersionToChange}");
            if (_outer._dataVersion == notHostClientDataVersionToChange)
                throw new System.InvalidOperationException($"No need to change version {notHostClientDataVersionToChange} on client side, maybe you dont need to call this method, or need to ask to version update from server");

            _outer._dataVersion = notHostClientDataVersionToChange;
            _outer._dataCount = 0;
            _outer.ClientDataUpdater.Reset();
            
            _outer.CmdClientChangedDataVersion(_outer._dataVersion);
        }

        public ClientSideIsClient(SenderByteDataToClients outer)
        {
            _outer = outer;
            _outer.Reset();
        }
    }

    protected class ClientSideIsHost : ClientSide
    {
        public ClientSideIsHost(SenderByteDataToClients senderByteDataToClients)
        {
        }

        public void ReadyToChangeDataVersion()
        {
            throw new NotImplementedException();
        }

        public void TargetChangeDataVersion(NetworkConnectionToClient target, short dataVersion)
        {
            throw new NotImplementedException();
        }

        public void TargetSendedDataToClient(NetworkConnectionToClient target, ArraySegment<byte> transfer, int currentClientCountShouldBe)
        {
            throw new NotImplementedException();
        }
    }
}
