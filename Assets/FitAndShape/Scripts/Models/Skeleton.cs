using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : MonoBehaviour
{
    [Header("Mesh")]
    [SerializeField]
    protected SkinnedMeshRenderer _meshRenderer;

    [SerializeField] protected Material _overrideMaterial;
    
    [Header("MetaRig")]
    [SerializeField]
    protected GameObject _metaRig;

    private static readonly int Color1 = Shader.PropertyToID("_Color"); // Standard Shaderの色のプロパティ値
    
    // Start is called before the first frame update
    void Start()
    {
        // Override Each Material
        var mats = _meshRenderer.materials;
        for (int i = 0; i < _meshRenderer.materials.Length; i++)
        {
            mats[i] = _overrideMaterial;
        }
        _meshRenderer.materials = mats;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// アタッチされているMaterialのColorに対してAlpha値を書き換えます
    /// </summary>
    public void SetAlpha(float alpha)
    {
        Color newColor = _overrideMaterial.color;
        newColor.a = alpha;
        _overrideMaterial.SetColor(Color1, newColor);
    }
}
