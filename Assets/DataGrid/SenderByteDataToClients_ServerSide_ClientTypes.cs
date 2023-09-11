using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public partial class SenderByteDataToClients
{
    private partial class ServerSide
    {


        public abstract class ClientType 
        {
            protected int _recievedConfirmedDataCount;
            protected int _sendedDataCount;
            protected int _clientDataVersion;
            protected bool _updatingDataVersion;

            protected SenderByteDataToClients _outer;
            protected NetworkConnectionToClient _connection;

            public void AttemptToSendDataToClient()
            {
                if (TryDataVersionUpdate())
                    return;

                CheckForIncorrectState();

                if (_recievedConfirmedDataCount == _outer._dataCount)
                    return; // no need to send

                if (_sendedDataCount > _recievedConfirmedDataCount)
                    return; //already sent something, waiting for confirmation

                ActuallySendDataToClient();
            }

            protected abstract void ActuallySendDataToClient();

            public void ClientRecievedTotalBytes(int clientDataCount)
            {
                if (TryDataVersionUpdate())
                    return;

                CheckForIncorrectState();

                if (clientDataCount > _outer._dataCount)
                {
                    Debug.Log(StateAsString());
                    throw new System.Exception($"Client says it recieved total {clientDataCount} but server have only {_outer._dataCount}");
                }

                if (clientDataCount < _recievedConfirmedDataCount)
                {
                    Debug.Log(StateAsString());
                    throw new System.Exception($"Client before had already {_recievedConfirmedDataCount} but new number is smaller: {clientDataCount}");
                }


                //if (_recievedConfirmedDataCount == clientDataCount)
                //    return;

                UpdateRecievedBytes(clientDataCount);
            }

            public void ClientChangedDataVersion(short clientDataVersion)
            {
                UpdateClientDataVersionInfo(clientDataVersion);

                AttemptToSendDataToClient();
            }

            protected virtual void UpdateClientDataVersionInfo(short clientDataVersion)
            {
                InitByteCounters(clientDataVersion);
            }
            protected ClientType(SenderByteDataToClients outer, NetworkConnectionToClient connection)
            {
                _outer = outer;
                _connection = connection;

                InitByteCounters(0);
            }

            protected abstract void UpdateRecievedBytes(int clientDataCount);

            protected bool TryDataVersionUpdate()
            {
                if (_updatingDataVersion)
                    return true;

                if (_clientDataVersion != _outer._dataVersion)
                {
                    _updatingDataVersion = true;
                    _outer.TargetChangeDataVersion(_connection, _outer._dataVersion);
                    return true;
                }
                return false;
            }
            private void InitByteCounters(int dataVersion)
            {
                _recievedConfirmedDataCount = 0;
                _sendedDataCount = 0;
                _clientDataVersion = dataVersion;
                _updatingDataVersion = false;
            }

            private void CheckForIncorrectState()
            {
                try
                {
                    if (_recievedConfirmedDataCount > _sendedDataCount)
                        throw new System.Exception(
                            $"{nameof(_recievedConfirmedDataCount)}({_recievedConfirmedDataCount})>{nameof(_sendedDataCount)}({_sendedDataCount})");
                    if (_recievedConfirmedDataCount > _outer._dataCount)
                        throw new System.Exception(
                            $"{nameof(_recievedConfirmedDataCount)}({_recievedConfirmedDataCount})>{nameof(_outer._dataCount)}({_outer._dataCount})");
                    if (_sendedDataCount > _outer._dataCount)
                        throw new System.Exception(
                            $"{nameof(_sendedDataCount)}({_sendedDataCount})>{nameof(_outer._dataCount)}({_outer._dataCount})");

                }
                catch (Exception ex)
                {
                    Debug.LogError("This should never happen");
                    Debug.Log(StateAsString());
                    Debug.LogException(ex);
                }
            }

            private string StateAsString() => $"State - client id: {_connection.identity}, outer count {_outer._dataCount}, outer data version {_outer._dataVersion}";
        }

        private sealed class ClientIsClient : ClientType
        {
            public ClientIsClient(SenderByteDataToClients outer, NetworkConnectionToClient connection) : base(outer, connection)
            {}

            protected override void UpdateRecievedBytes(int clientDataCount)
            {
                if (_sendedDataCount != clientDataCount)
                {
                     throw new System.Exception($"Client should confirm {_sendedDataCount} but confirms: {clientDataCount}");
                }

                _recievedConfirmedDataCount = clientDataCount;

                CheckIfNeedToSendMore();

                void CheckIfNeedToSendMore()
                {
                    if (_recievedConfirmedDataCount < _outer._dataCount)
                        AttemptToSendDataToClient();
                }
            }

            protected override void ActuallySendDataToClient()
            {

                int maxSegmentSize = _outer._innerServer._maxArraySegmentSize;
                //Debug.Log($"SendDataToClient called from {_connection.connectionId}, serverData {_outer._dataCount}, recieved {_recievedDataCount}, maxSegmentSize {maxSegmentSize}");

                int needToSend = Math.Min(_outer._dataCount - _recievedConfirmedDataCount, maxSegmentSize);
                if (needToSend <= 0)
                    throw new System.Exception($"this should never happen {needToSend}, check {maxSegmentSize}");

                //Debug.Log($"will now call RpcSendedDataToClient {_connection.connectionId}, sended {_recievedConfirmedDataCount}, {needToSend}, {_recievedConfirmedDataCount}");
                _outer.TargetSendedDataToClient(_connection, new ArraySegment<byte>(_outer._data, _recievedConfirmedDataCount, needToSend), _recievedConfirmedDataCount);
                _sendedDataCount = _recievedConfirmedDataCount + needToSend;
            }


        }

        private sealed class ClientIsHostItself : ClientType
        {
            //private int _recievedBytesCount = 0; //probably can replace to _recievedConfirmedDataCount

            protected override void UpdateClientDataVersionInfo(short clientDataVersion)
            {
                base.UpdateClientDataVersionInfo(clientDataVersion);
                _outer.ClientDataUpdater.ResetData();
                _outer.ClientDataRecievedEvent();
            }

            protected override void ActuallySendDataToClient()
            {
                _recievedConfirmedDataCount =_sendedDataCount = _outer._dataCount;

                _outer.ClientDataRecievedEvent();
            }


            protected override void UpdateRecievedBytes(int clientDataCount) 
            {
                return;
                //throw new System.NotImplementedException();

                if (_clientDataVersion != _outer._dataVersion)
                {
                    _outer.TargetChangeDataVersion(_connection, _outer._dataVersion);
                    return;
                }

                if (clientDataCount != _outer._dataCount)
                    throw new System.Exception($"On ClientIsHostItself {clientDataCount} should always be same as {_outer._dataCount}");


                _recievedConfirmedDataCount = clientDataCount;
                AttemptToSendDataToClient();
            }

            public ClientIsHostItself(SenderByteDataToClients outer, NetworkConnectionToClient connection) : base(outer, connection)
            {
            }
        }


    }
}
