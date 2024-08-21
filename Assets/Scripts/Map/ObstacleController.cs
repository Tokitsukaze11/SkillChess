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
        foreach (var pawn in _pawns.Where(pawn => pawn != null))
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(_mainCamera.transform.position, pawn.transform.position - _mainCamera.transform.position);
            //Debug.DrawRay(_mainCamera.transform.position, pawn.transform.position - _mainCamera.transform.position, Color.red);
            foreach (var hit in hits)
            {
                var obstacle = hit.collider.GetComponent<Obstacle>();
                if (obstacle == null)
                    continue;
                SetObstacleCovered(obstacle, true);
            }
        }
        SetObstaclesUncovered();
    }
    public void SetObstacle(List<Obstacle> obstacles)
    {
        _obstacleCoverageMap.Clear();
        foreach(var obstacle in obstacles)
        {
            if(obstacle == null)
                continue;
            _obstacleCoverageMap.TryAdd(obstacle, false);
        }
    }
    private void SetObstacleCovered(Obstacle obstacle, bool isCovered)
    {
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
