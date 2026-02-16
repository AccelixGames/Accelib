using UnityEngine;

namespace Accelib.Module.Localization.Helper.Formatter
{
    /// <summary>
    /// 문자열 배열 기반의 포맷터. string.Format() 보간에 사용한다.
    /// </summary>
    public class LocalizedFormatter_String : MonoBehaviour, ILocalizedFormatter
    {
        [SerializeField] private string[] args;

        public object[] GetArgs() => args;

        public void SetArgs(params object[] a)
        {
            if (a == null)
            {
                args = null;
                return;
            }
            if(args == null || args.Length != a.Length)
                args = new string[a.Length];

            for (var i = 0; i < a.Length; i++)
                args[i] = a[i].ToString();
        }
    }
}
