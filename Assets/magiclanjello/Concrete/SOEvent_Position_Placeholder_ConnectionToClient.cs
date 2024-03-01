using MagicLanjello.CellPlaceholder;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UKnack.Events;
using UnityEngine;

namespace MagicLanjello.Concrete
{
    [CreateAssetMenu(fileName = "PutCellOnServer", menuName = "MagicLanjello/PutCellOnServer")]
    public class SOEvent_Position_Placeholder_ConnectionToClient : SOEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient>
    {
    }
}