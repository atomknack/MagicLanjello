using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoubleEngine;
using DoubleEngine.Atom;

namespace MagicLanJello
{
    public abstract class AbstractGridMeshGenerator : MonoBehaviour
    {
        public abstract MeshFragmentVec3DWithMaterials GetMeshFragmentVec3D();
        public abstract MeshFragmentVec3DWithMaterials GetDecimatedMeshFragmentVec3D();
        public abstract IThreeDimensionalGrid GetIGridReference();
        public virtual void LoadGrid(string path) =>
            GridLoaders.LoadGrid(GetIGridReference(), path);
        public virtual void SaveGrid(string path) =>
            GridLoaders.SaveGrid(GetIGridReference(), path);
        public abstract void Init(IThreeDimensionalGrid grid);
        public abstract void Put(int x, int y, int z, ThreeDimensionalCell cell);
        public virtual void UpdateAfterLoad() =>
            UpdateMesh();
        public abstract void UpdateMesh();
    }
}
