using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorCore;
using DoubleEngine;
using DoubleEngine.Atom;
using System.Threading;
using System.Linq;
using DoubleEngine.Atom.Multithreading;
using UKnack.Attributes;
using DoubleEngine.UHelpers;
using UKnack;
using UKnack.Values;

namespace MagicLanJello
{
    [RequireComponent(typeof(PooledPrefabWithModifiableMesh))]
    public class GridChunkedGenerator : AbstractGridMeshGenerator
    {
        public Transform gridChunksPlaceholder;

        private SimpleBoolCondition _runThreads;
        private Dictionary<Vec3I, GameObject> _chunks;
        private IThreeDimensionalGrid _simpleGrid;
        private ThreeDimensionalChunker _chunkerOverlord;
        private ThreeDimensionalChunker.Worker _thisThreadWorker;
        [SerializeField][Range(10, 100)][DisableEditingInPlaymode] private int xSize = 40;
        [SerializeField][Range(10, 100)][DisableEditingInPlaymode] private int ySize = 40;
        [SerializeField][Range(10, 100)][DisableEditingInPlaymode] private int zSize = 40;
        private const int chunkSize = 8;
        private Vec3I _offset;

        private PooledPrefabWithModifiableMesh _pooledPrefab;

        [SerializeField]
        [ValidReference(typeof(SOValueMutable<long>), typeof(IValueMutable<long>))]
        private SOValueMutable<long> _counter;

        private void CountCells()
        {
            _counter.SetValue(IThreeDimensionalGrid.CountNonEmpty(_simpleGrid));
        }

        public override void LoadGrid(string path)
        {
            ClearChunks();
            base.LoadGrid(path);
        }

        public void Clear()
        {
            ClearChunks();
            _simpleGrid.Clear();
        }

        private void ClearChunks()
        {
            foreach (var chunk in _chunks.Values)
                if (_pooledPrefab.TryReturnToPool(chunk) == false)
                    throw new Exception("all chunks should be returnable");
            _chunks.Clear();
        }

        public override MeshFragmentVec3DWithMaterials GetMeshFragmentVec3D()
        {
            var mesh = _chunkerOverlord.BuildFullMeshNotDecimated();
            var fragment = MeshFragmentVec3DWithMaterials.Create(mesh);
            mesh.Dispose();
            return fragment;
            /*
            if (_coloredMesh == null)
                UpdateMesh();
            return _coloredMesh;
            */
        }
        public override MeshFragmentVec3DWithMaterials GetDecimatedMeshFragmentVec3D()
        {
            var mesh = _chunkerOverlord.BuildFullMeshDecimated();
            var fragment = MeshFragmentVec3DWithMaterials.Create(mesh);
            mesh.Dispose();
            return fragment;
            /*
            if (_coloredDecimated == null)
                UpdateMesh();
            return _coloredDecimated;
            */
        }
        public override IThreeDimensionalGrid GetIGridReference()
        {
            var offsetter = ThreeDimensionalGridOffsetter.Create(_simpleGrid);
            offsetter.SetOffset(_offset);
            return offsetter;
        }


        public override void Put(int x, int y, int z, ThreeDimensionalCell cell)
        {
            if (InsideGrid(new Vector3Int(x,y,z)) == false)
            {
                Debug.Log($"Grid Chunked Generator - Put {x},{y},{z}, {cell} is outside grid with size: ({xSize}, {ySize}, {zSize}), and offset ({_offset})");
                return;
            }

            _chunkerOverlord.UpdateCell(x, y, z, cell);
            //_simpleGrid.UpdateCell(x, y, z, cell);
            //UpdateMesh();
        }
        public override void UpdateAfterLoad()
        {
            //base.UpdateAfterLoad();
            _chunkerOverlord.PauseProcessingInput();
            for (int xi = 0; xi < xSize; xi += chunkSize)
                for (int yi = 0; yi < ySize; yi += chunkSize)
                    for (int zi = 0; zi < zSize; zi += chunkSize)
                        if (ChunkHaveMeaningfullCells(xi, yi, zi))
                        {
                            var chunk = _thisThreadWorker.DoTheActualWork(new Vec3I(xi, yi, zi));
                            UpdateFromChunkAndDisposeIt(chunk);
                        }

            _chunkerOverlord.UnPauseProcessingInput();
            CountCells();
        }

        public override void UpdateMesh()
        { }

        private void Update()
        {
            UpdateChunks();
        }

        private void UpdateChunks()
        {
            if (_chunkerOverlord.ResultsReady(out int resutsCount, out int processedInputCount))
            {
                //var iGrid = (IThreeDimensionalGrid)_chunkerOverlord;
                //Debug.Log($"{iGrid.GetAllMeaningfullCells().Count()} meaningfullCells");
                Span<BuildedChunk> resultChunks = new BuildedChunk[resutsCount];
                Span<SpaceCell> processedCells = new SpaceCell[processedInputCount];
                _chunkerOverlord.RetrieveResults(resultChunks, processedCells);
                foreach (var chunk in resultChunks)
                {
                    UpdateFromChunkAndDisposeIt(chunk);
                }
                //StaticBatchingUtility.Combine(gameObject);
                CountCells();
            }
        }

        private void UpdateFromChunkAndDisposeIt(BuildedChunk chunk)
        {
            (GameObject unityChunkPlaceholder, Mesh unityMesh) = GetUnityMeshForChunk(chunk.globalOffset);
            var mesh = chunk.chunkMesh;
            //Debug.Log(mesh.Vertices.Length);
            mesh.Translate(Vec3D.FromVec3I(chunk.globalOffset));
            mesh.UpdateUnityMesh(unityMesh);
            unityChunkPlaceholder.transform.SetParent(gridChunksPlaceholder);
            chunk.Dispose();
        }

        private (GameObject g, Mesh m) GetUnityMeshForChunk(Vec3I chunk)
        {
            GameObject unityChunkPlaceholder;
            Mesh unityMesh;
            if (_chunks.ContainsKey(chunk))
            {
                unityChunkPlaceholder = _chunks[chunk];
                unityMesh = unityChunkPlaceholder.GetComponent<MeshFilter>().mesh;
                return (unityChunkPlaceholder, unityMesh);
            }
            unityChunkPlaceholder = _pooledPrefab.GetFromPool();
            unityMesh = unityChunkPlaceholder.GetComponent<MeshFilter>().mesh;
            unityChunkPlaceholder.SetActive(true);
            _chunks.Add(chunk, unityChunkPlaceholder);
            return (unityChunkPlaceholder, unityMesh);

        }

        private void Start()
        {
            if (_counter == null)
            {
                throw new NullReferenceException(nameof(_counter));
            }
            if (gridChunksPlaceholder == null)
                gridChunksPlaceholder = transform;
            gameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            _pooledPrefab = GetComponent<PooledPrefabWithModifiableMesh>();
            //_meshFilter = GetComponent<MeshFilter>();
            //_meshFilter.sharedMesh = new Mesh();
            _offset = new Vec3I(xSize / 2, ySize / 2, zSize / 2);
            Init(ThreeDimensionalGrid.Create(xSize, ySize, zSize));
        }
        private void OnDisable()
        {
            _runThreads.SetValue(false);
        }

        private void InitDoubleEngine()
        {
            FlatNodes.GetFlatNode(0, FlatNodeTransform.Default);
            UThreeDimensionalCellMeshes.GetUnityMesh(0);
        }
        public override void Init(IThreeDimensionalGrid grid)
        {
            InitDoubleEngine();
            _runThreads = new SimpleBoolCondition(true);
            _chunks = new Dictionary<Vec3I, GameObject>();

            _simpleGrid = grid;


            _chunkerOverlord = new ThreeDimensionalChunker(chunkSize, _simpleGrid, _offset);
            _thisThreadWorker = (ThreeDimensionalChunker.Worker)_chunkerOverlord.CreateSubordinateWorker();
            ThreeDimensionalChunker.Worker worker = (ThreeDimensionalChunker.Worker)_chunkerOverlord.CreateSubordinateWorker();
            WorkersConditionRunner overlordWith1WorkerRunner = new WorkersConditionRunner(new IWorker[] { _chunkerOverlord, worker }, _runThreads);
            var thread = new Thread(() => overlordWith1WorkerRunner.RunWorkersWhileConditionTrueWithRestIfNoWork());
            thread.Start();
            _chunkerOverlord.UnPauseProcessingInput();

            //offsetter = ThreeDimensionalGridOffsetter.Create(grid);
            //offsetter.SetOffset(_offset);
            //_grid = offsetter;
            //_builder = ThreeDimensionalBuilder.Create(xSize, ySize, zSize);
            //_violatileMesh = MeshVolatileFragmentWithMaterials.Create();
        }

        private bool ChunkHaveMeaningfullCells(int chunkStartX, int chunkStartY, int chunkStartZ)
        {
            int endX = chunkStartX + chunkSize;
            int endY = chunkStartY + chunkSize;
            int endZ = chunkStartZ + chunkSize;
            for (int xi = chunkStartX; xi < endX; ++xi)
                for (int yi = chunkStartY; yi < endY; ++yi)
                    for (int zi = chunkStartZ; zi < endZ; ++zi)
                        if (_simpleGrid.GetCell(xi, yi, zi).HasNotEmptyMesh())
                            return true;
            return false;
        }
        private bool InsideGrid(Vector3Int v)
        {
            var diff = (v + _offset.ToVector3Int());
            if (diff.x < 0 || diff.y < 0 || diff.z < 0)
                return false;
            if (diff.x >= xSize || diff.y >= ySize || diff.z >= zSize)
                return false;
            return true;
        }
    }
}