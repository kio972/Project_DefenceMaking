using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FogOfWar : MonoBehaviour
{
    private void UpdateFog()
    {
        foreach(TileNode node in NodeManager.Instance.inDirectSightNodes)
            node?.SetFog(FogState.Half);

        foreach (TileNode node in NodeManager.Instance.directSightNodes)
            node?.SetFog(FogState.Opened);
    }

    private void Start()
    {
        ReactiveCollection<TileNode> directSightNodes = NodeManager.Instance.directSightNodes;
        var addStream = directSightNodes.ObserveAdd().Select(_ => true);
        var removeStream = directSightNodes.ObserveRemove().Select(_ => true);

        var addStream2 = NodeManager.Instance.inDirectSightNodes.ObserveAdd().Select(_ => true);
        var removeStream2 = NodeManager.Instance.inDirectSightNodes.ObserveRemove().Select(_ => true);

        var mergedStream = Observable.Merge(addStream, removeStream, addStream2, removeStream2);

        mergedStream.ThrottleFrame(1).Subscribe(_ => UpdateFog());
    }
}
