using MagicLanjello.CellPlaceholder;
using MagicLanjello.CellPlaceholder.SOValues;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public class DataBrush : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<ArraySegment<byte>> _toBytes;

    [SerializeField]
    private UnityEvent<Vector3Int, CellPlaceholderStruct> _fromBytesSucess;

    private Vector3Int _position;
    private CellPlaceholderStruct _placeholderStruct;

    private byte[] _buffer = new byte[10000];


    public void ToBytes(Vector3Int position, CellPlaceholderStruct placeholderStruct)
    {

    }

    public bool TryGetFromBytes(byte[] bytes, ref int index)
    {
        try
        {
            int tempIndex = index;
            var tempPosition = _position;
            var tempPlaceholder = _placeholderStruct;

            Brush3BitCommand command;
            do
            {
                command = Brush3BitCommand_Read(bytes[tempIndex]);
                throw new System.NotImplementedException();
            }
            while (command != Brush3BitCommand.PutCell);
        }
        catch
        {
            return false;
        }
    }

    public void Reset()
    {
        _position = Vector3Int.zero;
        _placeholderStruct = CellPlaceholderStruct.DefaultPlaceholder;
    }

    private void Awake()
    {
        Reset();
    }


    private static Brush3BitCommand Brush3BitCommand_Read(byte b) => (Brush3BitCommand)(b & 0b_0000_0_111);
    private static int Brush3BitCommand_SignBit(byte b) => (b & 0b_0000_1_000) >> 3;
    private int Brush3BitCommand_high4Bits(byte b) => b >> 4;

#pragma warning disable format
    enum Brush3BitCommand : int {
        PutCell             = 0b_0000_0_000, //0
        ChangeMesh          = 0b_0000_0_001, //1
        MoveX               = 0b_0000_0_010, //2
        MoveY               = 0b_0000_0_011, //3
        MoveZ               = 0b_0000_0_100, //4
        ChangeMaterial      = 0b_0000_0_101, //5
        ChangeOrientation   = 0b_0000_0_110, //6
        WholeByteCommand    = 0b_0000_0_111, //7
}   enum WholeByteCommand : int { // lowest 3 bits are 111 for all commands here
        MoveVector3Int      = 0b_0000_1_111, 
        SetMeshFromShort    = 0b_0001_0_111,
        SetMaterialByte     = 0b_0001_1_111,
        SetOrientationInt   = 0b_0010_0_111,
    }
#pragma warning restore format

}
