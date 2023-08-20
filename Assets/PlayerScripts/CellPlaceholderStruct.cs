using DoubleEngine.Atom;
using System;

namespace UKnack.MagicLanjello
{
    [System.Serializable]
    public struct CellPlaceholderStruct : IEquatable<CellPlaceholderStruct>
    {
        public static CellPlaceholderStruct DefaultPlaceholder =>
            new CellPlaceholderStruct { orientation = 0, cellMesh = 1, material = DEMaterials.DefaultMaterial.id };

    public int orientation;
        public short cellMesh;
        public byte material;

        public bool Equals(CellPlaceholderStruct other) =>
            orientation == other.orientation &&
            cellMesh == other.cellMesh &&
            material == other.material;
    }
}


