using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;

public partial class SenderByteDataToClients
{
    private partial class ServerSide
    {
        public abstract class ClientType
        {
            protected SenderByteDataToClients _outer;
            protected NetworkConnectionToClient _connection;
            protected ClientType(SenderByteDataToClients outer, NetworkConnectionToClient connection)
            {
                _outer = outer;
                _connection = connection;
            }

            public abstract void SendDataToClient();
            public abstract void ClientRecievedTotalBytes(int clientDataCount);

        }

        private sealed class ClientIsClient : ClientType
        {
            private int _recievedConfirmedDataCount;
            private int _sendedDataCount;


            public override void ClientRecievedTotalBytes(int clientDataCount)
            {
                if (_sendedDataCount != clientDataCount)
                {
                     throw new System.Exception($"Client should confirm {_sendedDataCount} but confirms: {clientDataCount}");
                }
                if (_recievedConfirmedDataCount >= clientDataCount)
                {
                    if (_recievedConfirmedDataCount != 0 && clientDataCount != 0)
                        throw new System.Exception($"Client before had already {_recievedConfirmedDataCount} but new number is smaller: {clientDataCount}");
                }
                if (clientDataCount > _outer._dataCount)
                    throw new System.Exception($"Client can never have more data {clientDataCount} than server {_outer._dataCount}");

                _recievedConfirmedDataCount = clientDataCount;

                CheckIfNeedToSendMore();

                void CheckIfNeedToSendMore()
                {
                    if (_recievedConfirmedDataCount < _outer._dataCount)
                        SendDataToClient();
                }
            }

            public override void SendDataToClient()
            {
                int maxSegmentSize = _outer._innerServer._maxArraySegmentSize;
                //Debug.Log($"SendDataToClient called from {_connection.connectionId}, serverData {_outer._dataCount}, recieved {_recievedDataCount}, maxSegmentSize {maxSegmentSize}");

                if (_recievedConfirmedDataCount > _outer._dataCount)
                    throw new System.Exception("this should never happen");

                if (_recievedConfirmedDataCount == _outer._dataCount)
                    return;

                if (_sendedDataCount > _recievedConfirmedDataCount)
                    return; // we wait for confirmation of recieving previous packet

                // from here we sure that we have some data to send

                int needToSend = Math.Min(_outer._dataCount - _recievedConfirmedDataCount, maxSegmentSize);
                if (needToSend <= 0)
                    throw new System.Exception($"this should never happen {needToSend}, check {maxSegmentSize}");

                //Debug.Log($"will now call RpcSendedDataToClient {_connection.connectionId}, sended {_recievedConfirmedDataCount}, {needToSend}, {_recievedConfirmedDataCount}");
                _outer.TargetSendedDataToClient(_connection, new ArraySegment<byte>(_outer._data, _recievedConfirmedDataCount, needToSend), _recievedConfirmedDataCount);
                _sendedDataCount = _recievedConfirmedDataCount + needToSend;
            }

            public ClientIsClient(SenderByteDataToClients outer, NetworkConnectionToClient connection) : base(outer, connection)
            {
                _recievedConfirmedDataCount = 0;
            }
        }

        private sealed class ClientIsHostItself : ClientType
        {


            public override void SendDataToClient() =>
                _outer.ClientDataRecievedEvent();

            public override void ClientRecievedTotalBytes(int recievedBytesCount) 
            {
            }

            public ClientIsHostItself(SenderByteDataToClients outer, NetworkConnectionToClient connection) : base(outer, connection)
            {
            }
        }


    }
}
