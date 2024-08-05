using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshController : Singleton<NavMeshController>
{
    public NavMeshSurface _navMeshSurface;
    private void Awake()
    {
        _navMeshSurface ??= GetComponent<NavMeshSurface>();
    }
    public void BakeNavMesh()
    {
        if(_navMeshSurface.navMeshData == null)
            _navMeshSurface.BuildNavMesh();
        else
            UpdateNavMesh();
    }
    private void UpdateNavMesh()
    {
        _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
    }
}
