using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RaycastTool
{
    // 1. 레이케스트를 쏠거고 하나만 반환할 때
    public static T Raycast<T>(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        if (Physics.Raycast(origin, direction, out hit, distance, layerMask))
        {
            hit.collider.TryGetComponent<T>(out var component);
            return component;
        }
        return default;
    }
}
