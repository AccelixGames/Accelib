using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Utility
{
    public static class GridUtility
    {
        public static void GetGridTransform(Transform parent, Vector2Int capacity, float unitSize, float unitSpace
        , Action<Vector3, Vector3, Quaternion> onGrid)
        {
            // 사이즈 캐싱
            var size = new Vector3(1f, 1f, 1f) * unitSize;
            var offsetY = size.y * 0.5f;
            var center = parent.position;
            var rot = parent.rotation; // 부모 회전 반영

            // 전체 크기 계산 (x,z 방향)
            var totalWidth = capacity.x * size.x + (capacity.x - 1) * unitSpace;
            var totalDepth = capacity.y * size.z + (capacity.y - 1) * unitSpace;

            // 왼쪽/앞쪽 기준 offset (아직 로컬 좌표)
            var startOffset = new Vector3(-totalWidth * 0.5f + size.x * 0.5f, 0f, -totalDepth * 0.5f + size.z * 0.5f);

            for (var x = 0; x < capacity.x; x++)
            for (var y = 0; y < capacity.y; y++)
            {
                var localOffset = new Vector3(x * (size.x + unitSpace), offsetY, y * (size.z + unitSpace));

                // 부모 회전에 맞게 offset 회전 적용
                var worldPos = center + rot * (startOffset + localOffset);
                onGrid?.Invoke(worldPos, size, rot);
            }
        }
        
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