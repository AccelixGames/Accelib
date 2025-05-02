namespace Accelib.Utility
{
    public static class KoreanTextHelper
    {
        /// <summary>
        /// '이' 또는 '가'를 반환합니다.
        /// </summary>
        public static string GetSubjectParticle_I_Ga(string word) => HasFinalConsonant(word) ? "이" : "가";

        /// <summary>
        /// '은' 또는 '는'을 반환합니다.
        /// </summary>
        public static string GetTopicParticle_Eun_Neun(string word) => HasFinalConsonant(word) ? "은" : "는";

        /// <summary>
        /// '을' 또는 '를'을 반환합니다.
        /// </summary>
        public static string GetObjectParticle_Eul_Reul(string word) => HasFinalConsonant(word) ? "을" : "를";
        
        /// <summary>
        /// 커스텀 조사 세트를 받아서 적절한 조사를 반환합니다.
        /// 받침이 있으면 particleWithFinal, 없으면 particleWithoutFinal 리턴
        /// </summary>
        public static string GetParticle(string word, string particleWithFinal, string particleWithoutFinal) => HasFinalConsonant(word) ? particleWithFinal : particleWithoutFinal;

        
        // 마지막 글자가 받침(종성)이 있는지 확인합니다.
        private static bool HasFinalConsonant(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            var lastChar = word[^1];
            if (lastChar < 0xAC00 || lastChar > 0xD7A3)
                return false; // 한글이 아닐 경우 받침 없음으로 처리

            var baseCode = lastChar - 0xAC00;
            var jongseongIndex = baseCode % 28;
            return jongseongIndex != 0;
        }
    }
}