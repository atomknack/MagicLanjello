using DoubleEngine.Atom;
using DoubleEngine.UHelpers;
using System;

namespace MagicLanjello.CellPlaceholder
{
    [System.Serializable]
    public readonly struct CellPlaceholderStruct : IEquatable<CellPlaceholderStruct>
    {
        public static CellPlaceholderStruct DefaultPlaceholder =>
            //UnityException: Load is not allowed to be called from a ScriptableObject constructor (or instance field initializer), call it in OnEnable instead. Called from ScriptableObject 'SOValueCellPlaceholder_Material'.
            //new CellPlaceholderStruct (Grid6SidesCached.Default._orientation.index, 1, UMaterials.Default );
            new CellPlaceholderStruct(0, 1, 0);



        public readonly int orientation;
        public readonly short cellMesh;
        public readonly byte material;

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


