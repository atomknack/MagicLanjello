using DoubleEngine.Atom;
using DoubleEngine.Network;
using DoubleEngine.UHelpers;
using System;

namespace MagicLanjello.CellPlaceholder
{
    [System.Serializable]
    public readonly struct CellPlaceholderStruct : IEquatable<CellPlaceholderStruct>
    {
        //[System.NonSerialized]
        //private static readonly CellPlaceholderStruct _empty = (CellPlaceholderStruct)NetGridCell.Empty;
        public static CellPlaceholderStruct DefaultPlaceholder => (CellPlaceholderStruct)NetGridCell.Empty; //_empty;
        //UnityException: Load is not allowed to be called from a ScriptableObject constructor (or instance field initializer), call it in OnEnable instead. Called from ScriptableObject 'SOValueCellPlaceholder_Material'.
        //new CellPlaceholderStruct (0 ,Grid6SidesCached.Default._orientation.index, UMaterials.Default );

        public readonly short cellMesh;
        public readonly int orientation;
        public readonly byte material;

        public static explicit operator CellPlaceholderStruct(NetGridCell netCell) =>
            new CellPlaceholderStruct(netCell.cellMesh, netCell.orientation, netCell.material);

        public CellPlaceholderStruct(short cellMesh, int orientation, byte material)
        {
            //NetGridCell.ValidateParameters(cellMesh, orientation, material);
            this.orientation = orientation;
            this.cellMesh = cellMesh;
            this.material = material;
        }

        public bool Equals(CellPlaceholderStruct other) =>
            orientation == other.orientation &&
            cellMesh == other.cellMesh &&
            material == other.material;
    }
}


