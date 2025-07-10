using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class LayerMaskExtenstion
    {
        public static List<int> ToList(this LayerMask mask)
        {
            var result = new List<int>();

            // Unity는 0~31까지 총 32개의 레이어를 지원
            for (var i = 0; i < 32; i++)
                if ((mask.value & (1 << i)) != 0)
                    result.Add(i);

            return result;
        }
    }
}