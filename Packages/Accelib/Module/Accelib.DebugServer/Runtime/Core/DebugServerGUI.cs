#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace Accelib.DebugServer
{
    /// <summary>
    /// 디버그 서버 상태를 화면 우측 하단에 IMGUI로 표시한다.
    /// 인스펙터에서 직접 추가하여 사용한다.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class DebugServerGUI : MonoBehaviour
    {
        [SerializeField, Range(0.25f, 2f)]
        private float scale = 0.5f;

        [SerializeField, Range(0f, 1f)]
        private float alpha = 0.7f;

        private GUIStyle _boxStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _valueStyle;
        private GUIStyle _cmdStyle;
        private Texture2D _bgTex;

        private void OnGUI()
        {
            if (!DebugServerCore.TryGetInstance(out var core)) return;
            if (_boxStyle == null) InitStyles();
            DrawOverlay(core);
        }

        private void InitStyles()
        {
            // 배경 텍스처 (투명도는 GUI.color.a로 제어)
            _bgTex = new Texture2D(1, 1);
            _bgTex.SetPixel(0, 0, Color.black);
            _bgTex.Apply();

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = _bgTex },
                padding = new RectOffset(10, 10, 8, 8)
            };

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleRight
            };

            _valueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) },
                alignment = TextAnchor.MiddleRight
            };

            _cmdStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.55f, 0.55f, 0.55f) },
                alignment = TextAnchor.MiddleRight
            };
        }

        private void DrawOverlay(DebugServerCore core)
        {
            const float baseWidth = 280f;
            const float baseLineHeight = 20f;
            const float baseHeaderHeight = 24f;
            const float basePadding = 10f;
            const float margin = 8f;
            const int contentLines = 4;

            // 스케일된 크기 (직접 픽셀 계산 — GUI.matrix 사용하지 않음)
            var w = baseWidth * scale;
            var lh = baseLineHeight * scale;
            var hh = baseHeaderHeight * scale;
            var pad = basePadding * scale;
            var totalH = hh + lh * contentLines + pad * 2;

            // 우측 하단 앵커 (margin은 고정 픽셀)
            var x = Screen.width - w - margin;
            var y = Screen.height - totalH - margin;

            // 폰트 크기 스케일
            _headerStyle.fontSize = Mathf.Max(1, Mathf.RoundToInt(13 * scale));
            _valueStyle.fontSize = Mathf.Max(1, Mathf.RoundToInt(12 * scale));
            _cmdStyle.fontSize = Mathf.Max(1, Mathf.RoundToInt(11 * scale));

            // 투명도
            var prevColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);

            // 배경
            GUI.Box(new Rect(x, y, w, totalH), GUIContent.none, _boxStyle);

            var cx = x + pad;
            var cy = y + pad;
            var cw = w - pad * 2;

            // 헤더
            GUI.Label(new Rect(cx, cy, cw, hh), "Debug Server", _headerStyle);
            cy += hh;

            // 상태
            var statusColor = core.IsRunning
                ? new Color(0.4f, 1f, 0.4f, alpha)
                : new Color(1f, 0.4f, 0.4f, alpha);
            var statusText = core.IsRunning
                ? $"Running ({core.RegisteredEndpointCount} Endpoint)"
                : "Stopped";
            GUI.color = statusColor;
            GUI.Label(new Rect(cx, cy, cw, lh), statusText, _valueStyle);
            GUI.color = new Color(1f, 1f, 1f, alpha);
            cy += lh;

            // 포트
            GUI.Label(new Rect(cx, cy, cw, lh), $"Port: {core.Port}", _valueStyle);
            cy += lh;

            // 게임 버전
            GUI.Label(new Rect(cx, cy, cw, lh), $"v{Application.version}", _valueStyle);
            cy += lh;

            // curl 명령어
            GUI.Label(new Rect(cx, cy, cw, lh), $"curl localhost:{core.Port}/api/help", _cmdStyle);

            // 복원
            GUI.color = prevColor;
        }

        private void OnDestroy()
        {
            if (_bgTex != null)
            {
                DestroyImmediate(_bgTex);
                _bgTex = null;
            }
        }
    }
}
#endif
