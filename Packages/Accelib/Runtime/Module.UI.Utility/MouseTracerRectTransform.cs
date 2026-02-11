using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Accelib.Module.UI.Utility
{
    public class MouseTracerRectTransform : MonoBehaviour
    {
        [Title("Mouse Tracer")]
        [SerializeField] private bool screenSafe = false;      // 화면 중앙 기준으로 좌/우 상/하 피벗 변경 + 오프셋 반전
        [SerializeField] private bool keepInCanvas = false;     // 캔버스 Rect 안에 완전히 들어오도록 클램프
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform pivot;
        [SerializeField] private Vector2 offset = Vector2.zero;

        [Title("Values")]
        [ShowInInspector, ReadOnly] private Vector2 _screenHalf;

        private Mouse _mouse;
        private RectTransform _canvasRect;
        private Camera _canvasCamera;
        private RectTransform _pivotParentRect;

        public RectTransform Pivot => pivot;

        private void OnEnable()
        {
            // (단위 기능) 마우스 디바이스 캐시
            _mouse = Mouse.current;

            // (단위 기능) canvas 자동 연결
            if (canvas == null && pivot != null)
                canvas = pivot.GetComponentInParent<Canvas>()?.rootCanvas;

            // (단위 기능) 캔버스 RectTransform, 카메라 캐시
            CacheCanvasRefs();

            // (단위 기능) pivot 부모 RectTransform 캐시(anchoredPosition 기준)
            _pivotParentRect = pivot != null ? pivot.parent as RectTransform : null;
        }

        private void LateUpdate()
        {
            // (단위 기능) 필수 참조/상태 가드
            if (!enabled) return;
            if (pivot == null) return;

            // (단위 기능) 마우스 런타임 보정(재연결)
            if (_mouse == null) _mouse = Mouse.current;
            if (_mouse == null) return;

            // (단위 기능) canvas/rect/camera 런타임 보정(프리팹/리로드 대응)
            if (canvas == null)
                canvas = pivot.GetComponentInParent<Canvas>()?.rootCanvas;
            CacheCanvasRefs();
            if (_pivotParentRect == null)
                _pivotParentRect = pivot.parent as RectTransform;

            if (_canvasRect == null || _pivotParentRect == null) return;

            // (단위 기능) 마우스 스크린 좌표 획득
            var mousePos = _mouse.position.value;

            // (단위 기능) 화면 절반 좌표 갱신(사분면 판단용)
            _screenHalf = new Vector2(Screen.width, Screen.height) * 0.5f;

            // (단위 기능) 최종 적용 오프셋 준비
            var finalOffset = offset;

            // (단위 기능) screenSafe: 화면 중앙 기준으로 피봇 전환 + 오프셋 반전(말풍선 방향 전환)
            if (screenSafe)
            {
                var pivotPos = pivot.pivot;

                var isRight = mousePos.x >= _screenHalf.x;
                pivotPos.x = isRight ? 1f : 0f;
                finalOffset.x = offset.x * (isRight ? -1f : 1f);

                var isUp = mousePos.y >= _screenHalf.y;
                pivotPos.y = isUp ? 1f : 0f;
                finalOffset.y = offset.y * (isUp ? 1f : -1f);

                pivot.pivot = pivotPos;
            }

            // (단위 기능) 스크린 -> 캔버스 로컬 좌표 변환(캔버스 Rect 기준)
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect, mousePos, _canvasCamera, out var canvasLocalMouse))
                return;

            // (단위 기능) offset을 "pivot 부모 로컬"에서 "캔버스 로컬"로 변환(계층/스케일 차이 대응)
            var offsetCanvasLocal = (Vector2)_canvasRect.InverseTransformVector(
                _pivotParentRect.TransformVector(finalOffset)
            );

            // (단위 기능) 캔버스 로컬에서의 목표 위치(마우스 + 오프셋)
            var desiredCanvasLocal = canvasLocalMouse + offsetCanvasLocal;

            // (단위 기능) keepInCanvas: pivot Rect가 캔버스 Rect 밖으로 나가지 않게 목표점을 클램프
            if (keepInCanvas)
            {
                // (단위 기능) pivot Rect 크기를 "캔버스 로컬" 단위로 환산
                var sizeCanvasLocal = GetRectSizeInLocalSpaceOf(_canvasRect, pivot);

                // (단위 기능) pivot 기준으로 좌/우/하/상 여유 계산(피봇 위치 반영)
                var left = pivot.pivot.x * sizeCanvasLocal.x;
                var right = (1f - pivot.pivot.x) * sizeCanvasLocal.x;
                var bottom = pivot.pivot.y * sizeCanvasLocal.y;
                var top = (1f - pivot.pivot.y) * sizeCanvasLocal.y;

                // (단위 기능) 캔버스 Rect 내부에 Rect 전체가 들어오도록 클램프
                var r = _canvasRect.rect;
                desiredCanvasLocal.x = Mathf.Clamp(desiredCanvasLocal.x, r.xMin + left, r.xMax - right);
                desiredCanvasLocal.y = Mathf.Clamp(desiredCanvasLocal.y, r.yMin + bottom, r.yMax - top);
            }

            // (단위 기능) 캔버스 로컬 -> 월드 변환
            var world = _canvasRect.TransformPoint(desiredCanvasLocal);

            // (단위 기능) 월드 -> 스크린 변환(다음 단계에서 부모 로컬로 변환하기 위함)
            var screenFromWorld = RectTransformUtility.WorldToScreenPoint(_canvasCamera, world);

            // (단위 기능) 스크린 -> pivot 부모 로컬 변환 후 anchoredPosition 적용
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _pivotParentRect, screenFromWorld, _canvasCamera, out var parentLocal))
            {
                pivot.anchoredPosition = parentLocal;
            }
        }

        // (단위 기능) 캔버스/카메라/RectTransform 캐시 및 보정
        private void CacheCanvasRefs()
        {
            if (canvas == null) return;

            if (_canvasRect == null || _canvasRect.gameObject != canvas.gameObject)
                _canvasRect = canvas.GetComponent<RectTransform>();

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                _canvasCamera = null;
            else
                _canvasCamera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }

        // (단위 기능) target RectTransform의 rect.size를 reference 로컬 단위로 환산(스케일 차이 반영)
        private static Vector2 GetRectSizeInLocalSpaceOf(RectTransform reference, RectTransform target)
        {
            // (단위 기능) target 로컬 크기 -> 월드 벡터로 변환
            var worldSize = target.TransformVector(target.rect.size);

            // (단위 기능) 월드 벡터 -> reference 로컬 벡터로 역변환
            var localSize = reference.InverseTransformVector(worldSize);

            // (단위 기능) 방향/스케일에 따른 부호 제거
            return new Vector2(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y));
        }

#if UNITY_EDITOR
        private void Reset()
        {
            // (단위 기능) 에디터에서 canvas 자동 연결
            if (canvas == null)
                canvas = GetComponentInParent<Canvas>()?.rootCanvas;
        }

        private void OnValidate() => OnEnable();
#endif
    }
}
