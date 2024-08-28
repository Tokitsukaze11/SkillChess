using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RaycastTool
{
    /// <summary>
    /// RaycastNonAlloc을 이용하여 Raycast를 실행하고, 해당하는 컴포넌트를 리스트로 반환합니다.
    /// </summary>
    /// <param name="origin">Raycast의 시작점 </param>
    /// <param name="distance">Raycast의 방향과 거리</param>
    /// <param name="hits">Raycast 결과를 저장할 배열</param>
    /// <typeparam name="T">Raycast 결과로 반환할 컴포넌트 타입</typeparam>
    /// <returns>Raycast 결과로 반환된 컴포넌트 리스트</returns>
    public static List<T> RaycastNonAlloc<T>(Vector3 origin, Vector3 distance, RaycastHit[] hits) where T : Component
    {
        List<T> components = new List<T>();
        Physics.RaycastNonAlloc(origin, distance, hits);
        foreach (var hit in hits.Where(x => !ReferenceEquals(x.collider, null)))
        {
            if (hit.collider.TryGetComponent<T>(out var component))
                components.Add(component);
        }
        return components;
    }
    /// <summary>
    /// 지정된 Layer에서 Raycast를 실행하고, 해당하는 컴포넌트를 반환합니다.
    /// </summary>
    /// <param name="ray">Raycast 정보</param>
    /// <param name="layerMask">Raycast를 실행할 LayerMask</param>
    /// <typeparam name="T">Raycast 결과로 반환할 컴포넌트 타입</typeparam>
    /// <returns>Raycast 결과로 반환된 컴포넌트</returns>
    public static T RaycastOnLayer<T>(Ray ray, LayerMask layerMask) where T : Component
    {
        Physics.Raycast(ray,out var hit, Mathf.Infinity, layerMask);
        if (ReferenceEquals(hit.collider, null))
            return null;
        return hit.collider.TryGetComponent<T>(out var component) ? component : null;
    }
}
