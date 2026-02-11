using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Accelib.Module.UI.Utility
{
    public class MouseTracerRectTransform : MonoBehaviour
    {
        [Header("Mouse Tracer")]
        [SerializeField] private bool enableScreenSafe = false;   // 캔버스 Rect 밖으로 나가지 않게(클램프)
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform pivot;
        [SerializeField] private Vector2 offset = Vector2.zero;

        [Header("Values")]
        [ShowInInspector, ReadOnly] private Vector2 _screenHalf;

        private Mouse _mouse;
        private RectTransform _canvasRect;
        private Camera _canvasCamera;
        private RectTransform _pivotParentRect;

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

            // (단위 기능) 화면 절반 좌표 갱신(인스펙터 디버그용)
            _screenHalf = new Vector2(Screen.width, Screen.height) * 0.5f;

            // (단위 기능) 기본 오프셋 준비
            var finalOffset = offset;

            // (단위 기능) enableScreenSafe: 화면 사분면 기준 pivot/offset 반전(말풍선 같은 UI용)
            if (enableScreenSafe)
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

            // (단위 기능) offset을 "pivot 부모 로컬"에서 "캔버스 로컬"로 변환
            var offsetCanvasLocal = (Vector2)_canvasRect.InverseTransformVector(
                _pivotParentRect.TransformVector(finalOffset)
            );

            // (단위 기능) 캔버스 로컬에서의 목표 위치(마우스 + 오프셋)
            var desiredCanvasLocal = canvasLocalMouse + offsetCanvasLocal;

            // (단위 기능) enableScreenSafe의 진짜 목적: 캔버스 Rect 안으로 위치 클램프(피봇의 실제 크기 고려)
            if (enableScreenSafe)
            {
                // (단위 기능) pivot 크기를 "캔버스 로컬 단위"로 환산
                var sizeCanvasLocal = GetRectSizeInLocalSpaceOf(_canvasRect, pivot);

                // (단위 기능) 피봇을 기준으로 rect가 차지하는 좌/우/하/상 여유(캔버스 로컬)
                var left = pivot.pivot.x * sizeCanvasLocal.x;
                var right = (1f - pivot.pivot.x) * sizeCanvasLocal.x;
                var bottom = pivot.pivot.y * sizeCanvasLocal.y;
                var top = (1f - pivot.pivot.y) * sizeCanvasLocal.y;

                // (단위 기능) 캔버스 Rect 내부에 rect 전체가 들어오도록 목표점을 제한
                var r = _canvasRect.rect;
                desiredCanvasLocal.x = Mathf.Clamp(desiredCanvasLocal.x, r.xMin + left, r.xMax - right);
                desiredCanvasLocal.y = Mathf.Clamp(desiredCanvasLocal.y, r.yMin + bottom, r.yMax - top);
            }

            // (단위 기능) 캔버스 로컬 -> 월드 변환
            var world = _canvasRect.TransformPoint(desiredCanvasLocal);

            // (단위 기능) 월드 -> pivot 부모 로컬(anchoredPosition 기준) 변환을 위해 스크린 포인트로 재변환
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
