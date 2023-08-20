using UnityEngine;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UKnack.MagicLanjello;

public class HandleCellChange : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOEvent<CellPlaceholderStruct> _placeholder;


    /*
    [SerializeField]
    [ValidReference]
    private SOValue<int> _orientation;

    [SerializeField]
    [ValidReference] 
    private SOValue<short> _mesh;

    [SerializeField]
    [ValidReference] 
    private SOValue<byte> _material;
    */


}
