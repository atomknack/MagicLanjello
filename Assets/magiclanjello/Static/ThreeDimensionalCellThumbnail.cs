#if USES_DOUBLEENGINE
using DoubleEngine.Atom;
using DoubleEngine.UHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicLanJello
{
    public static class ThreeDimensionalCellThumbnail
    {
        private static Dictionary<short,Texture2D> s_thumbnails = new Dictionary<short,Texture2D>();

        public static Texture2D GridCellRenderedTexture(short cellId)
        {
            if(s_thumbnails.ContainsKey(cellId))
                return s_thumbnails[cellId];
            Texture2D texture = MagicLanJello.RenderMeshesToTextureStatic.RenderMeshSometime(UThreeDimensionalCellMeshes.GetUnityMesh(cellId));
            s_thumbnails[cellId] = texture;
            return texture;
        }
    }
}
#endif