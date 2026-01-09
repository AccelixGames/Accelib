using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Extensions
{
	public static class ScrollRectExtension
	{
		public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
		{
			var contentSize = scrollView.content.rect.size;
			var viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
			Vector2 contentScale = scrollView.content.localScale;

			contentSize.Scale(contentScale);
			focusPoint.Scale(contentScale);

			var scrollPosition = scrollView.normalizedPosition;
			if (scrollView.horizontal && contentSize.x > viewportSize.x)
				scrollPosition.x =
					Mathf.Clamp01((focusPoint.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x));
			if (scrollView.vertical && contentSize.y > viewportSize.y)
				scrollPosition.y =
					Mathf.Clamp01((focusPoint.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));

			return scrollPosition;
		}

		public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
		{
			if(scrollView == null || item == null) return Vector2.zero;
			
			Vector2 itemCenterPoint =
				scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));

			var contentSizeOffset = scrollView.content.rect.size;
			contentSizeOffset.Scale(scrollView.content.pivot);

			return scrollView.CalculateFocusedScrollPosition(itemCenterPoint + contentSizeOffset);
		}

		public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint) => scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
		public static void FocusOnItem(this ScrollRect scrollView, RectTransform item) => scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);

		public static async UniTask FocusOnItemDelayed(this ScrollRect scrollView, RectTransform item, int frame)
		{
			await UniTask.DelayFrame(frame);
			scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
		}

		private static Tweener DoScrollPosition(this ScrollRect scrollView, Vector2 targetNormalizedPos,
			float duration)
		{
			return scrollView?.DONormalizedPos(targetNormalizedPos, duration)?.SetLink(scrollView.gameObject);
		}

		public static Tweener DoFocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint, float duration)
		{
			if (scrollView == null) return null;
			
			var pos = scrollView.CalculateFocusedScrollPosition(focusPoint);
			return scrollView.DoScrollPosition(pos, duration);
		}

		public static Tweener DoFocusOnItem(this ScrollRect scrollView, RectTransform item, float duration)
		{
			if (scrollView == null) return null;
			
			var pos = scrollView.CalculateFocusedScrollPosition(item);
			return scrollView.DoScrollPosition(pos, duration);
		}

		// public static void BringChildIntoView(this UnityEngine.UI.ScrollRect instance, RectTransform child, bool horizontal, bool vertical)
		// {
		//     Canvas.ForceUpdateCanvases();
		//     var viewportLocalPosition = instance.viewport.localPosition;
		//     var childLocalPosition = child.localPosition;
		//     var result = new Vector2();
		//     if (horizontal)
		//         result.x = 0 - (viewportLocalPosition.x + childLocalPosition.x);
		//     if (vertical)
		//         result.y = 0 - (viewportLocalPosition.y + childLocalPosition.y);
		//     instance.content.localPosition = result;
		//     
		//     if(horizontal) instance.horizontalNormalizedPosition = Mathf.Clamp(instance.horizontalNormalizedPosition, 0f, 1f);
		//     if(vertical) instance.verticalNormalizedPosition = Mathf.Clamp(instance.verticalNormalizedPosition, 0f, 1f);
		//
		// }
		//
		// public static void FitScrollAreaToChildVertically(this UnityEngine.UI.ScrollRect scrollRect, RectTransform selected)
		// {
		//     Canvas.ForceUpdateCanvases();
		//
		//     var transform = scrollRect.transform;
		//     var rect = ((RectTransform)transform).rect;
		//     var itemPositionY = transform.InverseTransformPoint(selected.position).y;
		//     var halfItemHeight = selected.rect.height / 2f;
		//
		//     var upperBound = rect.yMax - 0f; 
		//     var lowerBound = rect.yMin + 0f;
		//
		//     var itemLowerBound = itemPositionY - halfItemHeight;
		//     var itemUpperBound = itemPositionY + halfItemHeight;
		//
		//     if (itemLowerBound < lowerBound)
		//         scrollRect.content.anchoredPosition += new Vector2(0, lowerBound - itemLowerBound);
		//     else if (itemUpperBound > upperBound)
		//         scrollRect.content.anchoredPosition += new Vector2(0, -(itemUpperBound - upperBound));
		// }
	}
}