// jave.lin : 自定义的 lightmap 数据

using System;
using UnityEngine;

[Serializable]
public class CustomLightmapData
{
    public Texture2D lightmapColor;
    public Texture2D lightmapDir;
    public Texture2D shadowMask;

    public int GetTex2DHashCode()
    {
        return 
            lightmapColor.GetHashCode() +
            lightmapDir.GetHashCode() +
            shadowMask.GetHashCode();
    }
}
