using UnityEngine;
using Mirror;
using DoubleEngine;
using DoubleEngine.Atom;
using UKnack.Events;
using MagicLanjello.CellPlaceholder;
using System.Collections.Generic;

public class LoaderOfIGridToSOEvents : MonoBehaviour, IThreeDimensionalGridElementsProvider
{
    [SerializeField]
    private SOPublisher<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient> _onUpdateCell;

    [SerializeField]
    private SOPublisher _onClear;

    public void LoadGridFile(string gridFile)
    {
        GridLoaders.LoadGrid(this, gridFile);
    }

    void IThreeDimensionalGridElementsProvider.UpdateCell(int x, int y, int z, ThreeDimensionalCell cell) =>
        _onUpdateCell.Publish(new Vector3Int(x, y, z), (CellPlaceholderStruct)cell.ToNetGridCell(), null);
    void IThreeDimensionalGridElementsProvider.Clear() =>
        _onClear.Publish();

    IEnumerable<(Vec3I pos, ThreeDimensionalCell cell)> IThreeDimensionalGridElementsProvider.GetAllMeaningfullCells()
    {
        throw new System.NotImplementedException();
    }
}
