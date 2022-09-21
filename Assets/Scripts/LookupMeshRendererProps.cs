// jave.lin : ��ӡl mesh renderer lightmap ���������

using UnityEngine;

public class LookupMeshRendererProps : MonoBehaviour
{
    private void Start()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        Debug.Log($"go.name:{gameObject.name}, lightIDX:{meshRenderer.lightmapIndex}, lightScaleOffset:{meshRenderer.lightmapScaleOffset}, scaleInLightmap:{meshRenderer.scaleInLightmap}, stitchLightmapSeams:{meshRenderer.stitchLightmapSeams}");
    }
}
