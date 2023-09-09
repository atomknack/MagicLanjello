using UnityEngine;
using Mirror;
using DoubleEngine;
using DoubleEngine.Atom;
using UKnack.Events;
using MagicLanjello.CellPlaceholder;

public class LoaderOfIGridToSOEvents : MonoBehaviour, IThreeDimensionalGrid
{
    [SerializeField]
    private SOPublisher<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient> _onUpdateCell;

    [SerializeField]
    private SOPublisher _onClear;

    public void LoadGridFile(string gridFile)
    {
        GridLoaders.LoadGrid(this, gridFile);
    }

    void IThreeDimensionalGrid.UpdateCell(int x, int y, int z, ThreeDimensionalCell cell)
    {
        _onUpdateCell.Publish(new Vector3Int(x, y, z), (CellPlaceholderStruct)cell.ToNetGridCell(), null);
    }
    void IThreeDimensionalGrid.Clear() 
    { 
        _onClear.Publish();
    }

    ThreeDimensionalCell IThreeDimensionalGrid.GetCell(int x, int y, int z)
    {
        throw new System.NotImplementedException();
    }

    Vec3I IThreeDimensionalGrid.GetDimensions()
    {
        throw new System.NotImplementedException();
    }


}
