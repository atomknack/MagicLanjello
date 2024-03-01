using System.IO;
using UnityEngine;
using VectorCore;
using DoubleEngine;
using DoubleEngine.Atom;
using UKnack.Attributes;
using UKnack.Events;
using System;

namespace MagicLanJello
{
    public class FileOperationsSaverExporter : MonoBehaviour
    {
        [SerializeField]
        [ValidReference]
        [DisableEditingInPlaymode]
        private AbstractGridMeshGenerator gridMeshGenerator;

        [SerializeField]
        [MarkNullAsColor(0.5f, 0.5f, 0, "if you need to do action, then subscribe to event")]
        [DisableEditingInPlaymode]
        private SOEvent<string> _loadGrid;
        
        [SerializeField]
        [MarkNullAsColor(0.5f, 0.5f, 0, "if you need to do action, then subscribe to event")]
        [DisableEditingInPlaymode]
        private SOEvent<string> _loadGridAdditive;
        
        [SerializeField]
        [MarkNullAsColor(0.5f, 0.5f, 0, "if you need to do action, then subscribe to event")]
        [DisableEditingInPlaymode]
        private SOEvent<string> _saveGrid;
        
        [SerializeField]
        [MarkNullAsColor(0.5f, 0.5f, 0, "if you need to do action, then subscribe to event")]
        [DisableEditingInPlaymode]
        private SOEvent<string> _exportUnDecimatedMesh3D;
        
        [SerializeField]
        [MarkNullAsColor(0.5f, 0.5f, 0, "if you need to do action, then subscribe to event")]
        [DisableEditingInPlaymode]
        private SOEvent<string> _exportDecimatedMesh3D;

        [SerializeField]
        [MarkNullAsColor(0.5f, 0.5f, 0, "if you need to do action, then subscribe to event")]
        [DisableEditingInPlaymode]
        private SOEvent<string> _exportUnDecimatedObj;

        [SerializeField]
        [MarkNullAsColor(0.5f, 0.5f, 0, "if you need to do action, then subscribe to event")]
        [DisableEditingInPlaymode]
        private SOEvent<string> _exportDecimatedObj;

        private void OnEnable()
        {
            if (gridMeshGenerator == null)
            {
                Debug.Log("No gridMeshGenerator component");
                throw new MissingComponentException(nameof(gridMeshGenerator));
            }

            SubscribeIfNotNull(_loadGrid, LoadGrid);
            SubscribeIfNotNull(_loadGridAdditive, LoadGridBrushAdditive);
            SubscribeIfNotNull(_saveGrid, SaveGrid);
            SubscribeIfNotNull(_exportUnDecimatedMesh3D, ExportUnDecimated);
            SubscribeIfNotNull(_exportDecimatedMesh3D, ExportDecimated);
            SubscribeIfNotNull(_exportUnDecimatedObj, ExportUnDecimatedObj);
            SubscribeIfNotNull(_exportDecimatedObj, ExportDecimatedObj);

            static void SubscribeIfNotNull(SOEvent<string> ev, Action<string> action)
            {
                if (ev == null)
                    return;
                ev.Subscribe(action);
            }
        }

        private void OnDisable()
        {
            _loadGrid.UnsubscribeNullSafe(LoadGrid);
            _loadGridAdditive.UnsubscribeNullSafe(LoadGridBrushAdditive);
            _saveGrid.UnsubscribeNullSafe(SaveGrid);
            _exportUnDecimatedMesh3D.UnsubscribeNullSafe(ExportUnDecimated);
            _exportDecimatedMesh3D.UnsubscribeNullSafe(ExportDecimated);
            _exportUnDecimatedObj.UnsubscribeNullSafe(ExportUnDecimatedObj);
            _exportDecimatedObj.UnsubscribeNullSafe(ExportDecimatedObj);
        }

        void SaveGrid(string path)
        {
            Try.Action(x => gridMeshGenerator.SaveGrid(x), path, "save to bg file");
        }
        void LoadGrid(string path)
        {
            Try.Action(x =>
            {
                gridMeshGenerator.LoadGrid(x);
                gridMeshGenerator.UpdateAfterLoad();
            }, path, "load from bg file");
        }

        void LoadGridBrushAdditive(string path)
        {
            Try.Action(x =>
            {
                var brush = LoadNewBrush(path);
                foreach (var item in brush.GetAllMeaningfullCells())
                    gridMeshGenerator.Put(item.pos.x, item.pos.y, item.pos.z, item.cell);
                gridMeshGenerator.UpdateAfterLoad();
            }, path, "load additive from file");
        }

        private IThreeDimensionalGrid LoadNewBrush(string path)
        {
            IThreeDimensionalGrid newGrid = ThreeDimensionalGrid.Create(65, 65, 65);
            var offsetter = ThreeDimensionalGridOffsetter.Create(newGrid);
            offsetter.SetOffset(new Vec3I(32, 32, 32));
            GridLoaders.LoadGrid(offsetter, path);
            return offsetter;
        }

        void ExportUnDecimated(string path)
        {
            Try.Action(ExportModel, path, $"export UnDecimatedMesh3D file");
            void ExportModel(string path)
            {
                JsonHelpers.SaveToJsonFile(gridMeshGenerator.GetMeshFragmentVec3D(), path);
            }
        }

        void ExportDecimated(string path)
        {
            Debug.Log($"Export path chosen: {path}");
            Try.Action(ExportModel, path, $"export Decimated file");
            void ExportModel(string path)
            {
                JsonHelpers.SaveToJsonFile(gridMeshGenerator.GetDecimatedMeshFragmentVec3D(), path);
            }
        }

        void ExportUnDecimatedObj(string path)
        {
            Try.Action(ExportModel, path, "export UnDecimated to obj file");
            void ExportModel(string path)
            {
                File.WriteAllText(path, gridMeshGenerator.GetMeshFragmentVec3D().SerializeAsOBJFormatString());//JsonHelpers.SaveToJsonFile(deshelled, path);
            }
        }
        void ExportDecimatedObj(string path)
        {
            Try.Action(ExportModel, path, "export to obj file");
            void ExportModel(string path)
            {
                File.WriteAllText(path, gridMeshGenerator.GetDecimatedMeshFragmentVec3D().SerializeAsOBJFormatString());//JsonHelpers.SaveToJsonFile(deshelled, path);
            }
        }
    }
}