using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Material[] _materials;
    private MeshRenderer _meshRenderer;
    private Material _curMaterial;
    private const int DEFAULT_MATERIAL_INDEX = 0;
    private const int ALPHA_MATERIAL_INDEX = 1;
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _curMaterial = _materials[DEFAULT_MATERIAL_INDEX];
        _meshRenderer.material = _curMaterial;
    }
    public void SetAlpha(bool isAlpha)
    {
        _curMaterial = _materials[isAlpha ? ALPHA_MATERIAL_INDEX : DEFAULT_MATERIAL_INDEX];
        _meshRenderer.material = _curMaterial;
    }
}
