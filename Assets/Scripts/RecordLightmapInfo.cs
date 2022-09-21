// jave.lin : ��¼ lightmap ��Ϣ
// ��Ե��� lightmap �Ĵ���ʽ

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
        // jave.lin : ����Ԫ��
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

        // jave.lin : ����Ԫ��
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
            // jave.lin : ������������Ӧ�ò�������Ҳ����Ч��
            // ��Ϊ��Щ������ *.prefab �ļ����Ѿ�������
            info.renderer.lightmapIndex = info.index;
            info.renderer.lightmapScaleOffset = info.scaleOffset;
        }

        // jave.lin : ���� lightmaps
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

// jave.lin : ��
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

        // jave.lin : ��ӡ path ��������磺curActiveScenePath:Assets/Scenes/RecordLightmapInfo2Prefab.unity
        Debug.Log($"curActiveScenePath:{curActiveScenePath}");
        var sceneName = activeScene.name;
        // jave.lin : bakeInfoPath �����ǵĺ決��֮��� lightmap ������Ϣ��Ŀ¼�����磺curActiveScenePath:Assets/Scenes/RecordLightmapInfo2Prefab
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
            // jave.lin : ���û�� ContributeGI �ģ�lightmapIndex == -1, lightmapScaleOffset = (1, 1, 0, 0)
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

        // jave.lin : ������滻/���� prefab
        var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(info.gameObject);
        Debug.Log($"prefabPath:{prefabPath}");
        PrefabUtility.SaveAsPrefabAssetAndConnect(info.gameObject, prefabPath, InteractionMode.AutomatedAction);
    }
}

#endif