using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Dictionary<Obstacle, bool> _obstacleCoverageMap = new Dictionary<Obstacle, bool>();
    
    private Camera _mainCamera;
    private List<Pawn> _pawns;
    private RaycastHit[] hits = new RaycastHit[10];
    private void Awake()
    {
        PawnManager.Instance.OnSpawnComplete += OnPawnSpawn;
    }
    private void Start()
    {
        _mainCamera = GameManager.Instance.mainCamera;
    }
    private void OnPawnSpawn()
    {
        var playerPawns = PawnManager.Instance.GetPawns(true);
        var enemyPawns = PawnManager.Instance.GetPawns(false);
        _pawns = playerPawns.Concat(enemyPawns).ToList();
    }
    private void LateUpdate()
    {
        if (_pawns == null)
            return;
        foreach (var pawn in _pawns.Where(pawn => !ReferenceEquals(pawn, null)))
        {
            /*RaycastHit[] hits;
            hits = Physics.RaycastAll(_mainCamera.transform.position, pawn.transform.position - _mainCamera.transform.position);*/
            
            Physics.RaycastNonAlloc(_mainCamera.transform.position, pawn.transform.position - _mainCamera.transform.position, hits);
            
            //Debug.DrawRay(_mainCamera.transform.position, pawn.transform.position - _mainCamera.transform.position, Color.red);
            foreach (var hit in hits.Where(x => !ReferenceEquals(x.collider,null)))
            {
                /*var obstacle = hit.collider.GetComponent<Obstacle>();
                if (obstacle is null)
                    continue;*/
                if(hit.collider.TryGetComponent<Obstacle>(out var obstacle))
                    SetObstacleCovered(obstacle, true);
            }
        }
        SetObstaclesUncovered();
    }
    public void SetObstacle(List<Obstacle> obstacles)
    {
        _obstacleCoverageMap.Clear();
        foreach (var obstacle in obstacles.Where(obstacle => !ReferenceEquals(obstacle, null)))
        {
            _obstacleCoverageMap.TryAdd(obstacle, false);
        }
    }
    private void SetObstacleCovered(Obstacle obstacle, bool isCovered)
    {
        if (_obstacleCoverageMap.ContainsKey(obstacle))
            _obstacleCoverageMap[obstacle] = isCovered;
    }
    private void SetObstaclesUncovered()
    {
        // 가려지지 않은 오브젝트 찾기
        foreach (var pair in _obstacleCoverageMap)
        {
            pair.Key.SetAlpha(pair.Value);
        }
        // 가려짐 상태 업데이트
        _obstacleCoverageMap.ToList().ForEach(pair => _obstacleCoverageMap[pair.Key] = false);
    }
}
