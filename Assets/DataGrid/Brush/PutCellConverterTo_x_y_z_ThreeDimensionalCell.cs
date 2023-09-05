using DoubleEngine.Atom;
using MagicLanjello.CellPlaceholder;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PutCellConverterTo_x_y_z_ThreeDimensionalCell : MonoBehaviour
{
    [SerializeField]
    UnityEvent<int, int, int, ThreeDimensionalCell> _converted;

    public void Convert(Vector3Int pos, CellPlaceholderStruct cellPlaceholder) =>
        _converted.Invoke(pos.x, pos.y, pos.z,
            ThreeDimensionalCell.Create(
                cellPlaceholder.cellMesh,
                Grid6SidesCached.FromRotationAndScale(ScaleInversionPerpendicularRotation3.FromInt(cellPlaceholder.orientation)).OrientationIndex(),
                cellPlaceholder.material));
}
