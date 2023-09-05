using DoubleEngine.Atom;
using MagicLanjello.CellPlaceholder;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PutCellConverterTo_x_y_z_ThreeDimensionalCell : MonoBehaviour
{
    [SerializeField]
    UnityEvent<int, int, int, ThreeDimensionalCell> _converted;

    public void Convert(Vector3Int pos, CellPlaceholderStruct cellPlaceholder)
    {
        var cell = ThreeDimensionalCell.Create(
            cellPlaceholder.cellMesh,
            Grid6SidesCached.FromRotationAndScale(ScaleInversionPerpendicularRotation3.FromInt(cellPlaceholder.orientation)).OrientationIndex(),
            cellPlaceholder.material);

        Debug.Log($"Convert {pos}, {cellPlaceholder} {cell}");

        _converted.Invoke(pos.x, pos.y, pos.z, cell);

    }

}
