using UnityEngine;
using UKnack.Attributes;
using UKnack.Events;
using Mirror;
using MagicLanjello.CellPlaceholder;

namespace MagicLanjello.Concrete
{

    [CreateAssetMenu(fileName = "PutCellOnServer_Publisher", menuName = "MagicLanjello/PutCellOnServer_Publisher")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class SOEventPublisher_Position_Placeholder_ConnectionToClient : SOPublisher<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient>
    {
        [SerializeField]
        [ValidReference(typeof(IEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient>), 
            nameof(IEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient>.Validate),
            typeof(SOEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient>),
            typeof(SOEvent_Position_Placeholder_ConnectionToClient)
        )]
        private SOEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient> where;

        public override void Publish(Vector3Int position, CellPlaceholderStruct placeholder, NetworkConnectionToClient connection)
        {
            ValidateWhere();
            where.InternalInvoke(position, placeholder, connection);
        }

        internal void ValidateWhere() =>
            IEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient>.Validate(where);

    }

}
