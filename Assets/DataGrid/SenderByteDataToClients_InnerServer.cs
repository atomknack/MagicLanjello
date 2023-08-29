using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;

public partial class SenderByteDataToClients
{
    private class InnerServer
    {
        SenderByteDataToClients _outer;

        public void Command_ClientRecievedTotal(uint clientDataCount, NetworkConnectionToClient sender)
        {
            Debug.Log($"MyCommandLOG: CmdClientRecievedTotal called");
            Debug.Log($"MyCommandLOG: CmdClientRecievedTotal called by {sender}");
            Debug.Log($"MyCommandLOG: CmdClientRecievedTotal sender id {sender.connectionId} with data count {clientDataCount}");

            if (sender == null)
                throw new System.NullReferenceException($"{nameof(sender)} should be generated by Mirror and never be null");

            uint wasConfirmedCount = _outer._clientsRecievedDataCount.GetValueOrDefault(sender.connectionId, 0u);
            if (wasConfirmedCount != clientDataCount)
            {
                if (wasConfirmedCount != 0 || clientDataCount != 0)
                    throw new System.Exception($"Client before had already {wasConfirmedCount} but new number is {clientDataCount}");
            }
            if (clientDataCount > _outer._dataCount)
                throw new System.Exception($"Client can never have more data {clientDataCount} than server {_outer._dataCount}");

            _outer._clientsRecievedDataCount[sender.connectionId] = clientDataCount;

            if (clientDataCount < _outer._dataCount)
                CheckIfNeedToSendMore(sender, clientDataCount);
        }

        private void CheckIfNeedToSendMore(NetworkConnectionToClient sender, long lastCount)
        {
            var expecting = _outer._clientsRecievedDataCount[sender.connectionId] != lastCount;
            if (expecting)
                throw new System.Exception($"client confirmed total {lastCount}, but server expecting it to have {expecting}");
            if (lastCount < _outer._dataCount)
                SendDataToClient(sender);
        }

        protected void SendDataToClient(NetworkConnectionToClient target)
        {
            Debug.Log($"SendDataToClient called from {target.connectionId}, serverData {_outer._dataCount}, recieved {_outer._clientsRecievedDataCount[target.connectionId]}, maxSegmentSize {_outer._maxArraySegmentSize}");

            uint targetDataSended = _outer._clientsRecievedDataCount[target.connectionId];
            if (targetDataSended > _outer._dataCount)
                throw new System.Exception("this should never happen");
            if (targetDataSended == _outer._dataCount)
                return;

            int needToSend = Math.Min((int)_outer._dataCount - (int)targetDataSended, _outer._maxArraySegmentSize);
            if (needToSend <= 0)
                throw new System.Exception($"this should never happen {needToSend}, check {_outer._maxArraySegmentSize}");

            Debug.Log($"will now call RpcSendedDataToClient {target.connectionId}, sended {(int)targetDataSended}, {needToSend}, {targetDataSended}");
            _outer.TargetSendedDataToClient(target, new ArraySegment<byte>(_outer._data, (int)targetDataSended, needToSend), targetDataSended);
            _outer._clientsRecievedDataCount[target.connectionId] = targetDataSended + (uint)needToSend;
        }

        public InnerServer(SenderByteDataToClients outer)
        {
            _outer = outer;
        }
    }
}
