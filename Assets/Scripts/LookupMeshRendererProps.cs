// jave.lin : 打印l mesh renderer lightmap 的相关属性

using UnityEngine;

public class LookupMeshRendererProps : MonoBehaviour
{
    private void Start()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        Debug.Log($"go.name:{gameObject.name}, lightIDX:{meshRenderer.lightmapIndex}, lightScaleOffset:{meshRenderer.lightmapScaleOffset}, scaleInLightmap:{meshRenderer.scaleInLightmap}, stitchLightmapSeams:{meshRenderer.stitchLightmapSeams}");
    }
}
