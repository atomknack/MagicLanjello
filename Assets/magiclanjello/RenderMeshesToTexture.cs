using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicLanJello
{
    internal class RenderMeshesToTexture : MonoBehaviour
    {
        private RenderTexture _rendererTexture;
        private Queue<(Texture2D texture, Mesh mesh)> _rendererQueue;
        private MeshFilter _meshFilter;

        private Texture2D _currentTexture;
        private int _idleCounter;

        internal Texture2D AddMeshToRenderQueue(Mesh mesh)
        {
            EnsureTextureInitialized();
            EnsureQueueInitialized();
            var newTexture = new Texture2D(_rendererTexture.width, _rendererTexture.height, TextureFormat.ARGB32, false);
            //Debug.Log(newTexture.GetInstanceID());
            //Debug.Log(_rendererQueue.Count);
            _rendererQueue.Enqueue((newTexture, mesh));
            return newTexture;
        }

        private void EnsureTextureInitialized()
        {
            if (_rendererTexture != null)
                return;
            _rendererTexture = Resources.Load("SingleMeshRendererTexture") as RenderTexture;
        }
        private void EnsureQueueInitialized()
        {
            if (_rendererQueue != null)
                return;
            _rendererQueue = new Queue<(Texture2D texture, Mesh mesh)>();
        }
        private void Awake()
        {
            _currentTexture = null;
            DontDestroyOnLoad(this);
            _meshFilter = GetComponentInChildren<MeshFilter>();
            EnsureTextureInitialized();
            EnsureQueueInitialized();
        }
        private void OnEnable()
        {
            _idleCounter = 0;
        }

        private void Update()
        {
            if (_currentTexture != null)
            {
                Graphics.CopyTexture(_rendererTexture, _currentTexture);
                _currentTexture = null;
            }
            if (_rendererQueue.TryDequeue(out var tuple))
            {
                (var texture, var mesh) = tuple;
                _currentTexture = texture;
                _meshFilter.sharedMesh = mesh;
                return;
            }
            ++_idleCounter;
            if (_idleCounter > 1000)
                gameObject.SetActive(false);
        }
    }
}