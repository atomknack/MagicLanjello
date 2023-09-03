using System;
using UnityEngine;
using UnityEngine.Events;
using MagicLanjello.CellPlaceholder;
using DoubleEngine.UHelpers;
using DoubleEngine;
using System.Runtime.InteropServices;

public class DataBrush : MonoBehaviour
{
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
        MoveByVec3I         = 0b_0000_1_111, 
        SetMeshFromShort    = 0b_0001_0_111,
        SetOrientationInt   = 0b_0001_1_111,
        SetMaterialByte     = 0b_0010_0_111,
    }
#pragma warning restore format

    public class ErrorInUnityEvent : System.Exception
    {
        public ErrorInUnityEvent(string message, Exception innerException) : base(message, innerException)
        {}
    }

    [SerializeField]
    private UnityEvent<ArraySegment<byte>> _toBytes;

    [SerializeField]
    private UnityEvent<Vector3Int, CellPlaceholderStruct> _readOneFromBytesSucess;

    private Vector3Int _position;
    private CellPlaceholderStruct _placeholder;

    private byte[] _buffer = new byte[10000];


    public void ToBytes(Vector3Int position, CellPlaceholderStruct placeholder)
    {
        var buffer = _buffer.AsSpan(_buffer.Length);
        if (position == _position && placeholder.Equals(_placeholder))
            return;
        int totalBytesWritten = 0;
        if (_position != position)
        {
            totalBytesWritten += WriteToBufferAndAdvance<byte>(ref buffer, AsByte(WholeByteCommand.MoveByVec3I));
            totalBytesWritten += WriteToBufferAndAdvance<Vec3I>(ref buffer, (position - _position).ToVec3I());
        }
        if (_placeholder.cellMesh != placeholder.cellMesh)
        {
            totalBytesWritten += WriteToBufferAndAdvance<byte>(ref buffer, AsByte(WholeByteCommand.SetMeshFromShort));
            totalBytesWritten += WriteToBufferAndAdvance<short>(ref buffer, placeholder.cellMesh);
        }
        if (_placeholder.orientation != placeholder.orientation)
        {
            totalBytesWritten += WriteToBufferAndAdvance<byte>(ref buffer, AsByte(WholeByteCommand.SetOrientationInt));
            totalBytesWritten += WriteToBufferAndAdvance<int>(ref buffer, placeholder.orientation);
        }
        if (_placeholder.material != placeholder.material)
        {
            totalBytesWritten += WriteToBufferAndAdvance<byte>(ref buffer, AsByte(WholeByteCommand.SetMaterialByte));
            totalBytesWritten += WriteToBufferAndAdvance<byte>(ref buffer, placeholder.material);
        }
        
        totalBytesWritten += WriteToBufferAndAdvance<byte>(ref buffer, Brush3BitCommandAsByte(Brush3BitCommand.PutCell));
        _toBytes.Invoke(new ArraySegment<byte>(_buffer, 0, totalBytesWritten));


        static byte AsByte(WholeByteCommand command) => (byte)command;
        static byte Brush3BitCommandAsByte(Brush3BitCommand command) => (byte)command;

        static int WriteToBufferAndAdvance<T>(ref Span<byte> buffer, T value) where T : struct
        {
            MemoryMarshal.Write(buffer, ref value);
            var bytesWritten = Marshal.SizeOf(typeof(Vec3I));
            buffer = buffer.Slice(bytesWritten);
            return bytesWritten;
        }
    }



    public bool TryReadOneFromBytes(ReadOnlySpan<byte> bytes, ref int index)
    {
        if (index >= bytes.Length)
            return false;

        try
        {
            int tempIndex = index;
            var tempPosition = _position;
            var tempPlaceholder = _placeholder;

            Brush3BitCommand command;
            while ((command = Brush3BitCommand_Read(bytes[tempIndex])) != Brush3BitCommand.PutCell)
            {
                if (command != Brush3BitCommand.WholeByteCommand)
                    throw new System.NotImplementedException("Currently we can do only whole byte commands");
                WholeByteCommand wholeByte = (WholeByteCommand)bytes[tempIndex];

                tempIndex++;

                tempIndex += ReadWholeByteCommand(wholeByte, bytes.Slice(tempIndex), ref _position, ref _placeholder);
            }
            try
            {
                _readOneFromBytesSucess.Invoke(tempPosition, tempPlaceholder);
            }
            catch (Exception ex)
            {
                throw new ErrorInUnityEvent($"Cannot invoke _readOneFromBytesSucess with ({tempPosition}, {tempPlaceholder}), previous values: ({_position}, {_placeholder}). ", ex);
            }

            _position = tempPosition;
            _placeholder = tempPlaceholder;
            index = tempIndex;
            return true;
        }
        catch (Exception ex) 
        {
            var typeOfException = ex.GetType();
            if (typeOfException == typeof(NotImplementedException))
                Debug.LogException(ex);
            if (typeOfException == typeof(ErrorInUnityEvent))
                Debug.LogException(ex);
            return false;
        }
    }

    private int ReadWholeByteCommand(WholeByteCommand wholeByte, ReadOnlySpan<byte> slice, ref Vector3Int position, ref CellPlaceholderStruct placeholder)
    {
        switch (wholeByte)
        {
            case WholeByteCommand.MoveByVec3I:
                position += MemoryMarshal.Read<Vec3I>(slice).ToVector3Int();
                return Marshal.SizeOf<Vec3I>();
            case WholeByteCommand.SetMeshFromShort:
                placeholder = new CellPlaceholderStruct(MemoryMarshal.Read<short>(slice), placeholder.orientation, placeholder.material);
                return sizeof(short);
            case WholeByteCommand.SetOrientationInt:
                placeholder = new CellPlaceholderStruct(placeholder.cellMesh, MemoryMarshal.Read<int> (slice), placeholder.material);
                return sizeof(int);
            case WholeByteCommand.SetMaterialByte:
                placeholder = new CellPlaceholderStruct(placeholder.cellMesh, placeholder.orientation, MemoryMarshal.Read<byte>(slice));
                return sizeof(byte);
        }
        throw new NotImplementedException();
    }

    public void Reset()
    {
        _position = Vector3Int.zero;
        _placeholder = CellPlaceholderStruct.DefaultPlaceholder;
    }

    private void Awake()
    {
        Reset();
    }


    private static Brush3BitCommand Brush3BitCommand_Read(byte b) => (Brush3BitCommand)(b & 0b_0000_0_111);
    private static int Brush3BitCommand_SignBit(byte b) => ((b & 0b_0000_1_000) >> 3) * (-1);
    private int Brush3BitCommand_high4Bits(byte b) => b >> 4;

}
