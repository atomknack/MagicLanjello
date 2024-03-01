using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagicLanJello
{
    internal static class RenderMeshesToTextureStatic
    {
        private static RenderMeshesToTexture _rendererScript = null;
        private static GameObject _rendererGameobject;

        internal static Texture2D RenderMeshSometime(Mesh mesh)
        {
            EnsureRendererLoaded();
            var texture = _rendererScript.AddMeshToRenderQueue(mesh);
            _rendererGameobject.SetActive(true);
            return texture;
        }

        private static void EnsureRendererLoaded()
        {
            if (_rendererScript != null)
                return;
            var rendererScript = Object.FindObjectOfType<RenderMeshesToTexture>();
            if (rendererScript == null)
            {
                _rendererGameobject = UnityEngine.GameObject.Instantiate(Resources.Load("SingleMeshToTextureRenderer_root") as GameObject);
                _rendererScript = _rendererGameobject.GetComponent<RenderMeshesToTexture>();
                return;
            }
            _rendererGameobject = rendererScript.gameObject;
            _rendererScript = rendererScript;
        }
    }
}