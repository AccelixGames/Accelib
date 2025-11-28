using UnityEngine;

namespace Accelib.Utility.Layout
{
    [ExecuteAlways]
    public class SimpleGridLayout : MonoBehaviour
    {
        [Header("기본 설정")]
        [SerializeField, Min(1)] private int columns = 4;          // 한 줄에 몇 개?
        [SerializeField] private Vector2 cellSize = new Vector2(1f, 1f);
        [SerializeField] private Vector2 spacing = new Vector2(0.1f, 0.1f);
        [SerializeField] private bool includeInactive = false;     // 비활성 자식도 포함할지

        [Header("정렬 옵션")]
        [SerializeField] private Pivot pivot = Pivot.TopLeft;
        [SerializeField] private bool autoUpdateInEditor = true;   // 에디터에서 자동 업데이트

        private enum Pivot
        {
            TopLeft,
            Center,
            BottomLeft,
        }

        private void OnEnable()
        {
            ApplyLayout();
        }

#if UNITY_EDITOR
        // 에디터에서 값 변경/자식 변경 시 자동 갱신
        private void OnValidate()
        {
            if (!autoUpdateInEditor) return;
            ApplyLayout();
        }

        private void OnTransformChildrenChanged()
        {
            if (!autoUpdateInEditor) return;
            ApplyLayout();
        }
#endif

        [ContextMenu("Apply Layout Manually")]
        public void ApplyLayout()
        {
            if (columns < 1) columns = 1;

            // 자식 리스트 수집
            var children = GetActiveChildren();
            int count = children.Count;
            if (count == 0) return;

            int rows = Mathf.CeilToInt(count / (float)columns);

            float stepX = cellSize.x + spacing.x;
            float stepY = cellSize.y + spacing.y;

            // 전체 그리드 크기
            float gridWidth  = stepX * Mathf.Min(columns, count);
            float gridHeight = stepY * rows;

            // 피벗 기준 원점(0,0) 계산 (부모 로컬 공간)
            Vector2 origin = Vector2.zero;
            switch (pivot)
            {
                case Pivot.TopLeft:
                    origin = Vector2.zero;
                    break;

                case Pivot.Center:
                    origin = new Vector2(-gridWidth * 0.5f + stepX * 0.5f,
                        gridHeight * 0.5f - stepY * 0.5f);
                    break;

                case Pivot.BottomLeft:
                    origin = new Vector2(0f,
                        -gridHeight + stepY);
                    break;
            }

            for (int i = 0; i < count; i++)
            {
                int col = i % columns;
                int row = i / columns;

                float x = origin.x + col * stepX;
                float y = origin.y - row * stepY;

                var child = children[i];
                Vector3 localPos = child.localPosition;
                localPos.x = x;
                localPos.y = y;      // 2D/탑뷰라면 Y축 사용
                // localPos.z = 0;   // 필요시 고정

                child.localPosition = localPos;
            }
        }

        // 활성/비활성 필터링된 자식 Transform 리스트
        private System.Collections.Generic.List<Transform> GetActiveChildren()
        {
            var result = new System.Collections.Generic.List<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (!includeInactive && !child.gameObject.activeSelf)
                    continue;

                result.Add(child);
            }

            return result;
        }
    }
}
