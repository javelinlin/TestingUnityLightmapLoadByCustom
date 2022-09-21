// jave.lin : �Զ���� lightmap ����

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
