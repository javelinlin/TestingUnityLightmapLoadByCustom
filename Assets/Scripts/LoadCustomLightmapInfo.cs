// jave.lin : 加载自定义的 lightmap 信息

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LoadCustomLightmapInfo : MonoBehaviour
{
    // jave.lin : 不要使用天空和，因为这个是在运行前决定的，
    // 中途设置除了 camera.clearFlag 有小，其他 renderer 的 reflection 都会有问题，
    // 建议使用 reflection probe 来替代
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
                // jave.lin : 刷新映射 idx
                if (lightMapIndexDict == null)
                {
                    lightMapIndexDict = new Dictionary<int, int>();
                }
                else
                {
                    lightMapIndexDict.Clear();
                }

                // jave.lin : 统计总共使用到多少个 texture 2d array的元素
                // 计算 light map texture 属于哪个索引，相同的索引都会放在同一个 lightmapData 中
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
                // jave.lin : 根据统计出来的数量作为 lightmapData 的数组大小
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
                    // 设置 renderer 的 lightmap 属性
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
