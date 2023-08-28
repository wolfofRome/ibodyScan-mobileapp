using FitAndShape;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PostureAdviceAsset))]
public sealed class PostureAdviceAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("更新"))
        {
            var asset = target as PostureAdviceAsset;
            asset.Create();

            EditorUtility.SetDirty(target);
        }
    }
}