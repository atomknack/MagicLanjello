using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UnityEngine;

public class HandleCellChange : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOValue<int> _orientation;

    [SerializeField]
    [ValidReference] 
    private SOValue<short> _mesh;

    [SerializeField]
    [ValidReference] 
    private SOValue<byte> _material;



}
