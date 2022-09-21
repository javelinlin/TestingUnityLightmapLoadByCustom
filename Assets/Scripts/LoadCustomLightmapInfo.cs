// jave.lin : �����Զ���� lightmap ��Ϣ

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LoadCustomLightmapInfo : MonoBehaviour
{
    // jave.lin : ��Ҫʹ����պͣ���Ϊ�����������ǰ�����ģ�
    // ��;���ó��� camera.clearFlag ��С������ renderer �� reflection ���������⣬
    // ����ʹ�� reflection probe �����
    //public Material skyboxMat;
    public List<LightmapDataAndRendererBinder> lightmapDatas;

    public bool refreshLightmap = false;
    public bool updated = true;

    private Dictionary<int, int> lightMapIndexDict;

    private void Start()
    {
        refreshLightmap = false;
    }

    private void Update()
    {
        if (!updated)
            return;

        if (!refreshLightmap)
        {
            refreshLightmap = true;

            LightmapData[] lightmaps = null;

            if (lightmapDatas != null)
            {
                // jave.lin : ˢ��ӳ�� idx
                if (lightMapIndexDict == null)
                {
                    lightMapIndexDict = new Dictionary<int, int>();
                }
                else
                {
                    lightMapIndexDict.Clear();
                }

                // jave.lin : ͳ���ܹ�ʹ�õ����ٸ� texture 2d array��Ԫ��
                // ���� light map texture �����ĸ���������ͬ�������������ͬһ�� lightmapData ��
                var lightmapDataCount = 0;
                for (int i = 0; i < lightmapDatas.Count; i++)
                {
                    var lightmapDataBinder = lightmapDatas[i];
                    if (lightmapDataBinder.lightmapData.lightmapColor == null)
                    {
                        continue;
                    }
                    var key = lightmapDataBinder.lightmapData.GetTex2DHashCode();
                    if (!lightMapIndexDict.TryGetValue(key, out int lightmapDataIDX))
                    {
                        lightmapDataIDX = lightmapDataCount++;
                        lightMapIndexDict[key] = lightmapDataIDX;
                    }
                    lightmapDataBinder.lightmapDataIndex = lightmapDataIDX;
                }
                // jave.lin : ����ͳ�Ƴ�����������Ϊ lightmapData �������С
                lightmaps = new LightmapData[lightmapDataCount];
                for (int i = 0; i < lightmapDatas.Count; i++)
                {
                    var lightmapDataBinder = lightmapDatas[i];
                    var lightmapIDX = lightmapDataBinder.lightmapDataIndex;
                    if (lightmapIDX < -1 || lightmapIDX > lightmapDataCount - 1)
                    {
                        continue;
                    }
                    var lightmapData = lightmaps[lightmapIDX];
                    if (lightmapData == null)
                    {
                        lightmapData = new LightmapData();
                        lightmapData.lightmapColor = lightmapDataBinder.lightmapData.lightmapColor;
                        lightmapData.lightmapDir = lightmapDataBinder.lightmapData.lightmapDir;
                        lightmapData.shadowMask = lightmapDataBinder.lightmapData.shadowMask;
                        lightmaps[lightmapIDX] = lightmapData;
                    }
                    // ���� renderer �� lightmap ����
                    var meshRenderer = lightmapDataBinder.meshRenderer;
                    meshRenderer.lightmapIndex = lightmapIDX;
                    meshRenderer.lightmapScaleOffset = lightmapDataBinder.lightmapScaleOffset;
                    //meshRenderer.scaleInLightmap = lightmapDataBinder.scaleInLightmap;
                    //meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbesAndSkybox;
                }
            }

            LightmapSettings.lightmaps = lightmaps;
            //RenderSettings.skybox = skyboxMat;
            //RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
        }
    }
}
