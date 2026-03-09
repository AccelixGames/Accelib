#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace Accelib.DebugServer
{
    /// <summary>
    /// 디버그 서버 상태를 화면 우측에 IMGUI로 표시한다.
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
            _bgTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.85f));
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
                alignment = TextAnchor.MiddleLeft
            };

            _valueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) },
                alignment = TextAnchor.MiddleLeft
            };

            _cmdStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.55f, 0.55f, 0.55f) },
                alignment = TextAnchor.MiddleLeft
            };
        }

        private void DrawOverlay(DebugServerCore core)
        {
            const float width = 280f;
            const float lineHeight = 20f;
            const float headerHeight = 24f;
            const float margin = 8f;
            const int contentLines = 3;

            // 스케일 적용
            var prevMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1f));

            // 스케일된 좌표계에서의 화면 크기
            var screenW = Screen.width / scale;
            var screenH = Screen.height / scale;

            var totalHeight = headerHeight + lineHeight * contentLines + margin * 2;
            var x = screenW - width - margin;
            var y = screenH - totalHeight - margin;

            // 투명도 적용
            var prevColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);

            // 배경
            GUI.Box(new Rect(x, y, width, totalHeight), GUIContent.none, _boxStyle);

            var cx = x + 10f;
            var cy = y + margin;
            var cw = width - 20f;

            // 헤더
            GUI.Label(new Rect(cx, cy, cw, headerHeight), "Debug Server", _headerStyle);
            cy += headerHeight;

            // 상태 + 엔드포인트 (한 줄)
            var statusColor = core.IsRunning
                ? new Color(0.4f, 1f, 0.4f, alpha)
                : new Color(1f, 0.4f, 0.4f, alpha);
            var statusText = core.IsRunning
                ? $"Running ({core.RegisteredEndpointCount} Endpoint)"
                : "Stopped";
            GUI.color = statusColor;
            GUI.Label(new Rect(cx, cy, cw, lineHeight), statusText, _valueStyle);
            GUI.color = new Color(1f, 1f, 1f, alpha);
            cy += lineHeight;

            // 포트
            GUI.Label(new Rect(cx, cy, cw, lineHeight), $"Port: {core.Port}", _valueStyle);
            cy += lineHeight;

            // curl 명령어
            GUI.Label(new Rect(cx, cy, cw, lineHeight), $"curl localhost:{core.Port}/api/help", _cmdStyle);

            // 복원
            GUI.color = prevColor;
            GUI.matrix = prevMatrix;
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
