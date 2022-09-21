// jave.lin : lightmap ���� �� renderer �İ���

using System;
using UnityEngine;

[Serializable]
public class LightmapDataAndRendererBinder
{
    [Header("lightmap ����")]
    public CustomLightmapData lightmapData;
    
    [Header("��Ⱦ��")]
    public MeshRenderer meshRenderer;

    [Header("lightmap Scale, Offset")]
    public int lightmapDataIndex = -1;
    public Vector4 lightmapScaleOffset;
    public float scaleInLightmap = 1.0f;
}
