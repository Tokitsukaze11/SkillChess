using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    HashSet<MapSquare> _obstacleSet = new HashSet<MapSquare>();
    List<MapSquare> _obstacleSquaresList = new List<MapSquare>();
    private void Start()
    {
        PawnManager.Instance.OnTurnChange += SetObstacle;
    }
    private void SetObstacle()
    {
        if(_obstacleSquaresList.Count < 1)
        {
            _obstacleSquaresList.Clear();
            SquareCalculator.MapSquareDic.Values.Where(x => x.Obstacle != null).ToList().ForEach(x => _obstacleSquaresList.Add(x));
        }
        var pawnSquares = SquareCalculator.MapSquareDic.Values.Where(x => x.CurPawn != null).ToList();
        var newObstacleSet = new HashSet<MapSquare>();
        foreach (var pawnSquare in pawnSquares)
        {
            int mapIndex = SquareCalculator.CurrentIndex(pawnSquare);
            var targetList = new List<MapSquare>();
            SquareCalculator.CheckTargetSquares(1, mapIndex, targetList);
            var diagonalTargets = new List<MapSquare>();
            SquareCalculator.CheckDiagonalTargetSquares(1, mapIndex, diagonalTargets);
            var finalList = targetList.Concat(diagonalTargets).ToList().Where(x => x.Obstacle != null).ToList();
            if (finalList.Count <= 0)
                continue;
            foreach (var map in finalList)
                newObstacleSet.Add(map);
        }
        /*foreach(var obstacle in _obstacleSquaresList)
        {
            int mapIndex = SquareCalculator.CurrentIndex(obstacle);
            var targetList = new List<MapSquare>();
            SquareCalculator.CheckTargetSquares(1, mapIndex, targetList);
            var diagonalTargets = new List<MapSquare>();
            SquareCalculator.CheckDiagonalTargetSquares(1, mapIndex, diagonalTargets);
            var finalList = targetList.Concat(diagonalTargets).ToList().Where(x => x.Obstacle != null).ToList();
            if (finalList.Count <= 0)
                continue;
            foreach(var map in finalList)
                newObstacleSet.Add(map);
        }*/
        foreach(var map in _obstacleSet)
        {
            if(!newObstacleSet.Contains(map))
                map.Obstacle.SetAlpha(false);
        }
        _obstacleSet = newObstacleSet;
        foreach(var map in _obstacleSet)
            map.Obstacle.SetAlpha(true);
    }
}
