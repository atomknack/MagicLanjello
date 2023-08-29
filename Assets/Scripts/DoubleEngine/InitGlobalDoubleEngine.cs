using UnityEngine;
using DoubleEngine;

public class InitGlobalDoubleEngine : MonoBehaviour
{
    void Awake()
    {
        DoubleEngine.__GlobalStatic.Init(Application.dataPath, Debug.Log);
    }
}