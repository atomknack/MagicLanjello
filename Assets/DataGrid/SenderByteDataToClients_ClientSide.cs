using UnityEngine;
using Mirror;
using System;

public partial class SenderByteDataToClients
{
    private class ClientSide
    {
        SenderByteDataToClients _outer;

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

        public ClientSide(SenderByteDataToClients outer)
        {
            _outer = outer;
        }
    }
}
