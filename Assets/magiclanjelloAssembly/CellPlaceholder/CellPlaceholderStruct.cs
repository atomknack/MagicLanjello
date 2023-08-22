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
            if (!DEMaterials.IsValidMaterial(material))
                throw new Exception($"Material id {material} should be less than {DEMaterials.Count}");
            //if (DoubleEngine.UHelpers.UThreeDimensionalCellMeshes) helper does not have count
            if (!ThreeDimensionalCellMeshes.IsValidMeshId(cellMesh))
                throw new Exception($"Mesh id {cellMesh} should be less than {ThreeDimensionalCellMeshes.GetCount()}");
            if (!ScaleInversionPerpendicularRotation3.IsValid(orientation))
                throw new Exception($"orientation {orientation} is not valid");
        }

        public bool Equals(CellPlaceholderStruct other) =>
            orientation == other.orientation &&
            cellMesh == other.cellMesh &&
            material == other.material;
    }
}


