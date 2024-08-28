using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

public class ObstacleController : MonoBehaviour
{
    private Dictionary<Obstacle, bool> _obstacleCoverageMap = new Dictionary<Obstacle, bool>();
    
    private Camera _mainCamera;
    private List<Pawn> _pawns;
    private RaycastHit[] hits = new RaycastHit[10];
    
    private List<Obstacle> preCachedObstacles = new List<Obstacle>();
    private void Awake()
    {
        PawnManager.Instance.OnSpawnComplete += OnPawnSpawn;
        
        var lateUpdateStream = this.LateUpdateAsObservable();
        lateUpdateStream.Subscribe(_ => OnLateUpdate());
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
    public void SetPreCachedObstacles(Obstacle obstacles)
    {
        preCachedObstacles.Add(obstacles);
    }
    public void RemovePreCachedObstacles(Obstacle obstacles)
    {
        preCachedObstacles.Remove(obstacles);
    }
    private void OnLateUpdate()
    {
        if (_pawns == null)
            return;
        foreach (var pawn in _pawns.Where(pawn => !ReferenceEquals(pawn, null)))
        {
            var origin = _mainCamera.transform.position;
            var distance = pawn.transform.position - _mainCamera.transform.position;
            List<Obstacle> obs = RaycastTool.RaycastNonAlloc<Obstacle>(origin,distance,hits);
            foreach (var obstacle in obs.Where(x => !ReferenceEquals(x, null)))
            {
                SetObstacleCovered(obstacle, true);
            }
        }
        foreach(var obstacle in preCachedObstacles)
        {
            SetObstacleCovered(obstacle, true);
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
        if(preCachedObstacles.Contains(obstacle))
        {
            _obstacleCoverageMap[obstacle] = true;
            return;
        }
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
