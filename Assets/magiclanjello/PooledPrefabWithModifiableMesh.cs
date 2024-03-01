using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoubleEngine;
using System;
using UnityEditor;

namespace MagicLanJello
{
    [Obsolete("Not tested")]
    public class PooledPrefabWithModifiableMesh : MonoBehaviour
    {
        private int s_createNewPoolCounter = 0;
        //private static int s_poolGetMeshCounter = 0;
        //private static int s_poolLastReturnedCounter = 0;
        private Stack<GameObject> _pool = new Stack<GameObject>(10000);
        private HashSet<int> _pooledIds = new HashSet<int>(10000);
        public GameObject prefab;
        public bool TryReturnToPool(GameObject returned)
        {
            if (returned == null || (!_pooledIds.Contains(returned.GetInstanceID())))//|| prefab != PrefabUtility.GetCorrespondingObjectFromSource(returned))
                return false;
#if DEBUG
            foreach (var pooledInstance in _pool)
                if (pooledInstance == returned)
                    throw new ArgumentException($"trying to return mesh: {nameof(pooledInstance)} that already in pool");
#endif
            returned.SetActive(false);
            returned.GetComponent<MeshFilter>().mesh.Clear();
            _pool.Push(returned);
            return true;
        }
        public GameObject GetFromPool()
        {
            if (_pool.TryPop(out GameObject result))
            {
                if (result == null)
                {
                    Debug.LogWarning("somehow there was null object in pool");
                    Debug.Log(result.name);
                    throw new NullReferenceException(nameof(result));
                }
                return result;
            }
            return CreateNew();
        }
        private GameObject CreateNew()
        {
            var newInstance = Instantiate(prefab);
            newInstance.SetActive(false);
            Mesh newMesh = new Mesh();
            newMesh.name = $"grid pool mesh _{s_createNewPoolCounter}_ _";
            prefab.GetComponent<MeshFilter>().mesh = newMesh;
            _pooledIds.Add(newInstance.GetInstanceID());
            return newInstance;
        }
    }
}