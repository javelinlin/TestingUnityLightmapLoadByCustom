// jave.lin : 记录 lightmap 信息
// 针对单个 lightmap 的处理方式

using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class RecordLightmapInfo : MonoBehaviour
{
    public List<RendererLightmapInfo> lightmapInfos;
    private void Start()
    {
        CreateLightmapDatas();
    }
    public void CreateLightmapDatas()
    {
        List<LightmapData> lightmapDatas = new List<LightmapData>();

        var maxIDX = -1;
        // jave.lin : 填充空元素
        for (int i = 0; i < lightmapInfos.Count; i++)
        {
            var info = lightmapInfos[i];
            if (info.index > maxIDX)
            {
                maxIDX = info.index;
            }
        }
        for (int i = 0; i < maxIDX + 1; i++)
        {
            lightmapDatas.Add(null);
        }

        // jave.lin : 构建元素
        for (int i = 0; i < lightmapInfos.Count; i++)
        {
            var info = lightmapInfos[i];
            var lightmapData = lightmapDatas[info.index];
            if (lightmapData == null)
            {
                lightmapData = new LightmapData
                {
                    lightmapColor = info.color,
                    lightmapDir = info.dir,
                    shadowMask = info.shadowMask,
                };
                lightmapDatas[info.index] = lightmapData;
            }
            // jave.lin : 下面两个属性应该不用设置也是有效的
            // 因为这些属性在 *.prefab 文件中已经保存了
            info.renderer.lightmapIndex = info.index;
            info.renderer.lightmapScaleOffset = info.scaleOffset;
        }

        // jave.lin : 设置 lightmaps
        LightmapSettings.lightmaps = lightmapDatas.ToArray();
    }
}

[Serializable]
public class RendererLightmapInfo
{
    public MeshRenderer renderer;
    public int index;
    public Vector4 scaleOffset;
    public Texture2D color;
    public Texture2D dir;
    public Texture2D shadowMask;
}

#if UNITY_EDITOR

// jave.lin : 再
[CustomEditor(typeof(RecordLightmapInfo))]
public class RecordLightmapInfoEditor : Editor
{
    private static List<MeshRenderer> meshRendererListHelper = new List<MeshRenderer>();
    private RecordLightmapInfo info;
    private void Awake()
    {
        info = (RecordLightmapInfo)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("RecordLightmapInfo"))
        {
            //Debug.Log($"RecordLightmapInfo");
            DoRecordLightmapInfo(info);
        }
        if (GUILayout.Button("CreateLightmapDatas"))
        {
            //Debug.Log($"CreateLightmapDatas");
            info.CreateLightmapDatas();
        }
    }

    private void DoRecordLightmapInfo(RecordLightmapInfo info)
    {
        if (info.lightmapInfos == null)
        {
            info.lightmapInfos = new List<RendererLightmapInfo>();
        }
        else
        {
            info.lightmapInfos.Clear();
        }
        if (meshRendererListHelper == null)
        {
            meshRendererListHelper = new List<MeshRenderer>();
        }
        else
        {
            meshRendererListHelper.Clear();
        }

        info.gameObject.GetComponentsInChildren<MeshRenderer>(false, meshRendererListHelper);

        var activeScene = SceneManager.GetActiveScene();
        var curActiveScenePath = activeScene.path;

        // jave.lin : 打印 path 结果，例如：curActiveScenePath:Assets/Scenes/RecordLightmapInfo2Prefab.unity
        Debug.Log($"curActiveScenePath:{curActiveScenePath}");
        var sceneName = activeScene.name;
        // jave.lin : bakeInfoPath 是我们的烘焙好之后的 lightmap 纹理信息的目录，例如：curActiveScenePath:Assets/Scenes/RecordLightmapInfo2Prefab
        var bakeInfoPath = curActiveScenePath.Substring(0, curActiveScenePath.Length - 6);
        Debug.Log($"activeScene, name {sceneName}, path:{curActiveScenePath}, bakeInfoPath:{bakeInfoPath}");

        for (int i = 0; i < meshRendererListHelper.Count; i++)
        {
            var meshRenderer = meshRendererListHelper[i];
            var goFlags = GameObjectUtility.GetStaticEditorFlags(meshRenderer.gameObject);
            if (goFlags.HasFlag(StaticEditorFlags.ContributeGI))
            {
                //Debug.Log($"mesh renderer is ContributeGI, lightmapIDX:{meshRenderer.lightmapIndex}, scale offset : {meshRenderer.lightmapScaleOffset}");
                Debug.Log($"{meshRenderer.gameObject.name}, mesh renderer is ContributeGI");
            }
            else
            {
                Debug.Log($"{meshRenderer.gameObject.name}, mesh renderer isn't ContributeGI");
            }
            // jave.lin : 如果没有 ContributeGI 的，lightmapIndex == -1, lightmapScaleOffset = (1, 1, 0, 0)
            Debug.Log($"lightmapIDX:{meshRenderer.lightmapIndex}, scale offset : {meshRenderer.lightmapScaleOffset}");

            if (meshRenderer.lightmapIndex != -1)
            {
                var lightmapInfo = new RendererLightmapInfo
                {
                    renderer = meshRenderer,
                    index = meshRenderer.lightmapIndex,
                    scaleOffset = meshRenderer.lightmapScaleOffset,
                    color = AssetDatabase.LoadAssetAtPath<Texture2D>($"{bakeInfoPath}/Lightmap-{meshRenderer.lightmapIndex}_comp_light.exr"),
                    dir = AssetDatabase.LoadAssetAtPath<Texture2D>($"{bakeInfoPath}/Lightmap-{meshRenderer.lightmapIndex}_comp_dir.png"),
                    shadowMask = AssetDatabase.LoadAssetAtPath<Texture2D>($"{bakeInfoPath}/Lightmap-{meshRenderer.lightmapIndex}_comp_shadowmask.png"),
                };
                info.lightmapInfos.Add(lightmapInfo);
            }
        }

        // jave.lin : 最后是替换/更新 prefab
        var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(info.gameObject);
        Debug.Log($"prefabPath:{prefabPath}");
        PrefabUtility.SaveAsPrefabAssetAndConnect(info.gameObject, prefabPath, InteractionMode.AutomatedAction);
    }
}

#endif