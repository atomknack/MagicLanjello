using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoubleEngine.Atom.Loaders;
using System.Diagnostics;

public class Init_FlatNodes_CellNodes : MonoBehaviour
{
    string flatnodesData;
    string cellMeshesData;

    public class LoaderFromEnc3: IEngineLoader
    {
        private Init_FlatNodes_CellNodes _monobeh;

        string IEngineLoader.LoadFlatNodes() => _monobeh.flatnodesData;
        string IEngineLoader.LoadTDCellMeshes() => _monobeh.cellMeshesData;
        
        public LoaderFromEnc3(Init_FlatNodes_CellNodes monobeh)
        {
            _monobeh = monobeh;
        }
    }

    private LoaderFromEnc3 _loaderFromEnc3;

    private void Awake()
    {
        TextAsset fN = Resources.Load("magicLanJello_flatnodesData.Enc3") as TextAsset;
        flatnodesData = EncodersTB.DecodeAsENC3(fN.bytes);

        TextAsset cMD = Resources.Load("magicLanJello_cellMeshesData.Enc3") as TextAsset;
        cellMeshesData = EncodersTB.DecodeAsENC3(cMD.bytes);

        _loaderFromEnc3 = new LoaderFromEnc3(this);
        EngineLoader.SetLoader(_loaderFromEnc3);
        UnityEngine.Debug.Log(DoubleEngine.Atom.FlatNodes.AllDefaultNodes.Count);
    }


}
