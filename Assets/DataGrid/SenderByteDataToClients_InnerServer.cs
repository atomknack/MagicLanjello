using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;

public partial class SenderByteDataToClients
{
    private class InnerServer
    {
        SenderByteDataToClients _outer;

        private int _maxArraySegmentSize = 100;

        protected Dictionary<int, uint> _clientsRecievedDataCount = new Dictionary<int, uint>();

        public void OnServerWhenClientConnect(NetworkConnectionToClient conn)
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

        public void OnServerWhenClientDisconnect(NetworkConnectionToClient conn)
        {
            _clientsRecievedDataCount.Remove(conn.connectionId);
        }

        public void Command_ClientRecievedTotal(uint clientDataCount, NetworkConnectionToClient sender)
        {
            Debug.Log($"MyCommandLOG: CmdClientRecievedTotal called");
            Debug.Log($"MyCommandLOG: CmdClientRecievedTotal called by {sender}");
            Debug.Log($"MyCommandLOG: CmdClientRecievedTotal sender id {sender.connectionId} with data count {clientDataCount}");

            if (sender == null)
                throw new System.NullReferenceException($"{nameof(sender)} should be generated by Mirror and never be null");

            uint wasConfirmedCount = _clientsRecievedDataCount.GetValueOrDefault(sender.connectionId, 0u);
            if (wasConfirmedCount != clientDataCount)
            {
                if (wasConfirmedCount != 0 || clientDataCount != 0)
                    throw new System.Exception($"Client before had already {wasConfirmedCount} but new number is {clientDataCount}");
            }
            if (clientDataCount > _outer._dataCount)
                throw new System.Exception($"Client can never have more data {clientDataCount} than server {_outer._dataCount}");

            _clientsRecievedDataCount[sender.connectionId] = clientDataCount;

            if (clientDataCount < _outer._dataCount)
                CheckIfNeedToSendMore(sender, clientDataCount);
        }

        private void CheckIfNeedToSendMore(NetworkConnectionToClient sender, long lastCount)
        {
            var expecting = _clientsRecievedDataCount[sender.connectionId] != lastCount;
            if (expecting)
                throw new System.Exception($"client confirmed total {lastCount}, but server expecting it to have {expecting}");
            if (lastCount < _outer._dataCount)
                SendDataToClient(sender);
        }

        protected void SendDataToClient(NetworkConnectionToClient target)
        {
            Debug.Log($"SendDataToClient called from {target.connectionId}, serverData {_outer._dataCount}, recieved {_clientsRecievedDataCount[target.connectionId]}, maxSegmentSize {_maxArraySegmentSize}");

            uint targetDataSended = _clientsRecievedDataCount[target.connectionId];
            if (targetDataSended > _outer._dataCount)
                throw new System.Exception("this should never happen");
            if (targetDataSended == _outer._dataCount)
                return;

            int needToSend = Math.Min((int)_outer._dataCount - (int)targetDataSended, _maxArraySegmentSize);
            if (needToSend <= 0)
                throw new System.Exception($"this should never happen {needToSend}, check {_maxArraySegmentSize}");

            Debug.Log($"will now call RpcSendedDataToClient {target.connectionId}, sended {(int)targetDataSended}, {needToSend}, {targetDataSended}");
            _outer.TargetSendedDataToClient(target, new ArraySegment<byte>(_outer._data, (int)targetDataSended, needToSend), targetDataSended);
            _clientsRecievedDataCount[target.connectionId] = targetDataSended + (uint)needToSend;
        }

        public InnerServer(SenderByteDataToClients outer)
        {
            _outer = outer;
            _maxArraySegmentSize = GetMaxArraySegmentSize();
        }

        private void ZeroClient(NetworkConnectionToClient conn)
        {
            if (IsConnectionFromHost(conn))
                throw new System.Exception("This method should not be ever called from host");

            if (_clientsRecievedDataCount.ContainsKey(conn.connectionId))
                throw new System.Exception($"there are already {conn.connectionId}");

            _clientsRecievedDataCount[conn.connectionId] = 0;
        }

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

        private int GetMaxArraySegmentSize() =>
            Math.Max(100, NetworkManager.singleton.transport.GetMaxPacketSize() - 64);
    }
}
