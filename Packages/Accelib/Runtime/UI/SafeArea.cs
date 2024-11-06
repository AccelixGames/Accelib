using System;
using Accelib.Logging;
using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace Accelib.UI
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel. 
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    [Obsolete("이제 사용되지 않습니다.", true)]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform _panel;
        private Rect _lastSafeArea = new(0, 0, 0, 0);
        private Vector2Int _lastScreenSize = new(0, 0);
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;
        [SerializeField] private bool conformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField] private bool conformY = false;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        [SerializeField] private bool logging = false;  // Conform to screen safe area on Y-axis (default true, disable to ignore)

        private void Awake ()
        {
            _panel = GetComponent<RectTransform> ();

            if (_panel == null)
            {
                Deb.LogError ("Cannot apply safe area - no RectTransform found on " + name);
                Destroy (gameObject);
            }

            //Refresh();
        }

        private void OnEnable() => Refresh();

        private void Update() => Refresh();

        private void Refresh ()
        {
            var safeArea = GetSafeArea ();

            if (safeArea != _lastSafeArea
                || Screen.width != _lastScreenSize.x
                || Screen.height != _lastScreenSize.y
                || Screen.orientation != _lastOrientation)
            {
                // Fix for having auto-rotate off and manually forcing a screen orientation.
                // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
                _lastScreenSize.x = Screen.width;
                _lastScreenSize.y = Screen.height;
                _lastOrientation = Screen.orientation;

                ApplySafeArea (safeArea);
            }
        }

        private static Rect GetSafeArea () => Screen.safeArea;

        private void ApplySafeArea (Rect r)
        {
            _lastSafeArea = r;

            // Ignore x-axis?
            if (!conformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            // Ignore y-axis?
            if (!conformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            // Check for invalid screen startup state on some Samsung devices (see below)
            if (Screen.width > 0 && Screen.height > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                var anchorMin = r.position;
                var anchorMax = r.position + r.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
                // See https://forum.unity.com/threads/569236/page-2#post-6199352
                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    _panel.anchorMin = anchorMin;
                    _panel.anchorMax = anchorMax;
                }
            }

            if (logging)
            {
                Debug.LogFormat ("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
            }
        }
    }
}
