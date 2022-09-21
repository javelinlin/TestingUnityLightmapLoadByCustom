// jave.lin : lightmap 数据 与 renderer 的绑定器

using System;
using UnityEngine;

[Serializable]
public class LightmapDataAndRendererBinder
{
    [Header("lightmap 数据")]
    public CustomLightmapData lightmapData;
    
    [Header("渲染器")]
    public MeshRenderer meshRenderer;

    [Header("lightmap Scale, Offset")]
    public int lightmapDataIndex = -1;
    public Vector4 lightmapScaleOffset;
    public float scaleInLightmap = 1.0f;
}
