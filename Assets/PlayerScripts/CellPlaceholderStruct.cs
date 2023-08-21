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

        public CellPlaceholderStruct(int orientation, short cellMesh, byte material)
        {
            ValidateParameters(orientation, cellMesh, material);
            this.orientation = orientation;
            this.cellMesh = cellMesh;
            this.material = material;
        }

        public static bool ValidateParameters(int orientation, short cellMesh, byte material)
        {
            if (material < 0 || material >= DEMaterials.Count)
                throw new Exception($"Material id {material} should be less than {DEMaterials.Count}");
            //if (DoubleEngine.UHelpers.UThreeDimensionalCellMeshes) helper does not have count
            if (cellMesh < 0 || cellMesh >= ThreeDimensionalCellMeshes.GetCount())
                throw new Exception($"Mesh id {cellMesh} should be less than {ThreeDimensionalCellMeshes.GetCount()}");
            //try{
            var orient = ScaleInversionPerpendicularRotation3.FromInt(orientation);
            //}catch (Exception ex) {throw ex}
            return true;
        }

        public bool Equals(CellPlaceholderStruct other) =>
            orientation == other.orientation &&
            cellMesh == other.cellMesh &&
            material == other.material;
    }
}


