namespace Accelib.AccelixWeb.Module.Advertisement.Model
{
    public static class AdsCode
    {
        public const string Failed = "failed"; // Load/Show 호출 실패
        
        public const string Loaded = "loaded"; // 광고 로드 성공
        public const string Show = "show"; // 광고 컨텐츠 보여졌음
        public const string Impression = "impression"; // 광고 노출
        public const string FailedToShow = "failedToShow"; // 광고 컨텐츠 보여주기 실패
        public const string Clicked = "clicked"; // 광고 클릭
        public const string UserEarnedReward = "userEarnedReward"; // 사용자가 광고 시청을 완료했음
        public const string Dismissed = "dismissed"; // 광고 닫힘 
        public const string Requested = "requested"; // Show 호출 성공
    }
}