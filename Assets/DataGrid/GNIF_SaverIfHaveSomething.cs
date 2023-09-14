using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UnityEngine;

[RequireComponent(typeof(GNIF_SaverLoader))]
public class GNIF_SaverIfHaveSomething : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private SenderByteDataToClients _dataSender;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private SOEvent _saveGNIFevent;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private SOValue<bool> _sOValueOffline;

    private GNIF_SaverLoader _saverLoader;

    Lazy<string> _where = new Lazy<string>(() => GetPath() + "/AutosavedGNIFs/");


    private void SaveGNIFWhenOnline()
    {
        bool offline = _sOValueOffline.GetValue();
        //Debug.Log($"SaveGNIFWhenOnline {_sOValueOffline.GetValue()}");
        if ( offline == false)
            SaveGNIF();
    }

    private void OfflineStatusChanged(bool offline)
    {
        if (offline)
            SaveGNIF();
    }

    public void SaveGNIF()
    {
        if (_dataSender.AsBytesReadOnlySpan.Length == 0)
            return;

        if (_saverLoader == null)
            _saverLoader = GetComponent<GNIF_SaverLoader>();

        string dir = _where.Value.ToString();
        if (Directory.Exists(dir) == false)
            Directory.CreateDirectory(dir);

        string path = dir + DateTime.Now.ToString("yyMMddHHmmss") + ".gnif";
        _saverLoader.SaveFile(path);
    }

    /*
    private static string DeleteDuplicate(string oldPath, string path)
    {
        if (File.Exists(oldPath) == false)
            return path;


        if (IdenticalGNIFs(oldPath, path))
        {
            File.Delete(path);
            return oldPath;
        }

        return path;
    }

    private static bool IdenticalGNIFs(string oldPath, string path)
    {
            var oldFile = new FileInfo(oldPath);
            var newFile = new FileInfo(path);

            if (oldFile.Length != newFile.Length)
            {
                return false;
            }
            using (FileStream fs1 = oldFile.OpenRead())
            using (FileStream fs2 = newFile.OpenRead())
            {
                for (int i = 0; i < oldFile.Length; i++)
                {
                    if (fs1.ReadByte() != fs2.ReadByte())
                    {
                        return false;
                    }
                }
            }

            return true;
    }*/

    private static string GetPath()
    {
        string path;
#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR_WIN
        path = Path.GetDirectoryName(Application.dataPath);
#else
        path = Application.persistentDataPath;
#endif
        return path;
    }

    //public void PreviousGNIFChanged(string filename) =>
    //    _previousGNIF = filename;

    private void OnEnable()
    {
        NullChecks();
        _saveGNIFevent.Subscribe(SaveGNIFWhenOnline);
        _sOValueOffline.Subscribe(OfflineStatusChanged);
    }

    private void OnDisable()
    {
        NullChecks();
        _saveGNIFevent.UnsubscribeNullSafe(SaveGNIFWhenOnline);
        _sOValueOffline.UnsubscribeNullSafe(OfflineStatusChanged);
    }

    private void NullChecks()
    {
        if (_saveGNIFevent == null)
            throw new System.ArgumentNullException(nameof(_saveGNIFevent));
    }
}
