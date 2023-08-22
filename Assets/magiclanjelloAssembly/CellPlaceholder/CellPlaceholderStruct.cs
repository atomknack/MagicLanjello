using DoubleEngine.Atom;
using System;

namespace MagicLanjello.CellPlaceholder
{
    [System.Serializable]
    public struct CellPlaceholderStruct : IEquatable<CellPlaceholderStruct>
    {
        public static CellPlaceholderStruct DefaultPlaceholder =>
            new CellPlaceholderStruct { orientation = 0, cellMesh = 1, material = DEMaterials.DefaultMaterial.id };

        public int orientation;
        public short cellMesh;
        public byte material;

        public CellPlaceholderStruct(int orientation, short cellMesh, byte material)
        {
            ValidateParameters(orientation, cellMesh, material);
            this.orientation = orientation;
            this.cellMesh = cellMesh;
            this.material = material;
        }

        public static void ValidateParameters(int orientation, short cellMesh, byte material)
        {
            if (!ScaleInversionPerpendicularRotation3.IsValid(orientation))
                throw new Exception($"orientation {orientation} is not valid");
            ThreeDimensionalCellMeshes.ValidateMeshId(cellMesh);
            DEMaterials.ValidateMaterial(material);
        }

        public bool Equals(CellPlaceholderStruct other) =>
            orientation == other.orientation &&
            cellMesh == other.cellMesh &&
            material == other.material;
    }
}


