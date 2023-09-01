using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;

public partial class SenderByteDataToClients
{
    private partial class ServerSide
    {
        private SenderByteDataToClients _outer;

        private int _maxArraySegmentSize = 100;

        private Dictionary<int, ClientType> _clients = new Dictionary<int, ClientType>();

        public void UpdateClients()
        {
            foreach(var client in _clients.Values)
            {
                client.SendDataToClient();
            }
        }

        public void OnServerWhenClientConnect(NetworkConnectionToClient conn)
        {
            GetOrAddClient(conn);

            //SendDataToClient(conn);
        }

        private ClientType GetOrAddClient(NetworkConnectionToClient conn)
        {
            int id = conn.connectionId;
            if (_clients.ContainsKey(id))
                return _clients[id];
            
            Debug.Log($"Client {id} is newly connected to server");
            
            if (IsConnectionFromHost(conn))
            {
                Debug.Log($"No need to send data to itself for {id}");
                _clients.Add(id, new ClientIsHostItself(_outer, conn));
                return _clients[id];
            }

            _clients.Add(id, new ClientIsClient(_outer, conn));
            return _clients[id];
        }

        public void OnServerWhenClientDisconnect(NetworkConnectionToClient conn)
        {
            _clients.Remove(conn.connectionId);
        }

        public void Command_ClientRecievedTotal(int clientDataCount, NetworkConnectionToClient sender)
        {
            //Debug.Log($"MyCommandLOG: CmdClientRecievedTotal called");
            //Debug.Log($"MyCommandLOG: CmdClientRecievedTotal called by {sender}");
            //Debug.Log($"MyCommandLOG: CmdClientRecievedTotal sender id {sender.connectionId} with data count {clientDataCount}");

            if (sender == null)
                throw new System.NullReferenceException($"{nameof(sender)} should be generated by Mirror and never be null");

            var client = GetOrAddClient(sender);
            client.ClientRecievedTotalBytes(clientDataCount);
        }

        public ServerSide(SenderByteDataToClients outer)
        {
            _outer = outer;
            _maxArraySegmentSize = GetMaxArraySegmentSize();
        }

        private int GetMaxArraySegmentSize() =>
            Math.Max(100, NetworkManager.singleton.transport.GetMaxPacketSize() - 64);

        private bool IsConnectionFromHost(NetworkConnectionToClient conn)
        {
            if (conn == null)
                throw new System.ArgumentNullException($"are you trying to check that null connection is local? Why?");

            LocalConnectionToClient local = NetworkServer.localConnection;
            if (local != null)
            {
                if (conn == local)
                {
                    //Debug.Log($"Just so you know connection: {conn.connectionId} is local: {local.connectionId}");
                    return true;
                }
            }
            return false;
        }
    }
}
