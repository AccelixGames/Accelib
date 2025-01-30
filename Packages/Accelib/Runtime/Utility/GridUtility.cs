using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Utility
{
    public static class GridUtility
    {
        public static List<Vector2Int> GetCoordNxN(Vector2 center, float cellSize, Vector2Int range)
        {
            // 2. 월드 좌표 → 그리드 좌표 변환
            var gridX = Mathf.FloorToInt(center.x / cellSize);
            var gridY = Mathf.FloorToInt(center.y / cellSize);

            // 3. NxN 크기의 셀 리스트 만들기
            var selectedCells = new List<Vector2Int>(range.x * range.y);

            var halfSize = range / 2; // 중심을 기준으로 좌우 상하 확장
            var xStart = gridX - (range.x % 2 == 0 ? halfSize.x - 1 : halfSize.x);
            var yStart = gridY - (range.y % 2 == 0 ? halfSize.y - 1 : halfSize.y);

            for (var x = 0; x < range.x; x++)
            for (var y = 0; y < range.y; y++)
                selectedCells.Add(new Vector2Int(xStart + x, yStart + y));

            return selectedCells;
        }
    }
}