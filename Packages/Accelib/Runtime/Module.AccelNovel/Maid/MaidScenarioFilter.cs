using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{

    public enum EMaidName
    {
        Common = 0,
        Omong = 1,
        Minyung = 2,
        Bukuki = 3
    }
    
    /// <summary> 음식 오픈 플래그 </summary>
    [Flags]
    public enum EFoodFlags
    {
        None = 0,
        Omelette = 1 << 0,
        Parfait = 1 << 1,
        Soda = 1 << 2,
    }
    
    /// <summary> 일상 시나리오 오픈 플래그 </summary>
    [Flags]
    public enum EDailyFlags
    {
        None = 0,
        HasGift = 1 << 0,
        Meet = 1 << 1,
        Promise = 1 << 2,
    }
    
    
    /// <summary> 일상 시나리오 오픈 수치 </summary>
    [System.Serializable]
    public struct SDailyFigure
    {
        public int Bond;
        public int Relationship;
        public int Level;
    }

    [System.Serializable]
    /// <summary> 현재 비교용 필터 </summary>
    public struct SCurrentFilter
    {
        public EMaidName MaidName;
        public EFoodFlags FoodFlags;
        public EDailyFlags CurrentFlags;
        
        public SDailyFigure CurrentFigure;
        
        
        
        
        
    }
    
    /// <summary> 일상 시나리오 필터 </summary>
    [System.Serializable]
    public struct SDailyFilter
    {
        public EMaidName MaidName;
        
        public SDailyFigure Figure;
    
        public EDailyFlags RequiredFlags;
        public EDailyFlags ForbiddenFlags;
        
        public EFoodFlags FoodFlags;
    }

    // [Flags]
    // public enum ELikeabilityFlags
    // {
    //     None = 0,
    //     
    // }
    
    /// <summary> 호감 시나리오 오픈 수치 </summary>
    [System.Serializable]
    public struct SFavorFigure
    {
        public int Relationship;
        public int Level;
    }

    /// <summary> 호감 시나리오 필터 </summary>
    [System.Serializable]
    public struct SFavorFilter
    {
        public EMaidName MaidName;
        
        public SFavorFigure Figure;
    }
    
    [System.Serializable]
    public struct SBaseFigure
    {
        public int Level;
    }

    [System.Serializable]
    public struct SBaseFilter
    {
        public EMaidName MaidName;
        
        public SBaseFigure Figure;
    }
    
    
    public static class MaidScenarioFilter
    {
        
        private static readonly List<SO_MaidScenarioBase> BaseBuffer = new (capacity: 128);
        private static readonly List<SO_MaidDailyScenario> DailyBuffer = new (capacity: 128);
        private static readonly List<SO_MaidFavorScenario> FavorBuffer = new (capacity: 128);

        /// <summary> 일상 대화 필터링 </summary>
        /// todo 봤던 대화들 어떻게 처리할지 필요함
        public static List<SO_MaidDailyScenario> DailyFilter(IReadOnlyList<SO_MaidDailyScenario> source, in SCurrentFilter f)
        {
            DailyBuffer.Clear();
            var count = source.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = source[i];
                var filter = scenario.dFilter;
                
                // 대상 누군지
                if(filter.MaidName != f.MaidName) continue;
                
                // Daily 오픈 플래그 조건
                var requiredFlags = filter.RequiredFlags;
                var forbiddenFlags = filter.ForbiddenFlags;
                
                if((requiredFlags & f.CurrentFlags) != requiredFlags) continue;
                if((forbiddenFlags & f.CurrentFlags) != 0) continue;
                
                
                // 음식 오픈 조건
                if((filter.FoodFlags & f.FoodFlags) != filter.FoodFlags) continue;
                
                // 수치 비교
                var figure = filter.Figure;
                
                if(figure.Bond < f.CurrentFigure.Bond) continue;
                if(figure.Relationship < f.CurrentFigure.Relationship) continue;
                if(figure.Level < f.CurrentFigure.Level) continue;
                
                DailyBuffer.Add(scenario);
            }
            
            return DailyBuffer;
        }

        /// <summary> 호감도 대화 필터링 </summary>
        /// todo 봤던 대화들 어떻게 처리할지 필요함
        public static List<SO_MaidFavorScenario> FavorFilter(IReadOnlyList<SO_MaidFavorScenario> source, in SCurrentFilter f)
        {
            FavorBuffer.Clear();
            var count = source.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = source[i];
                var filter = scenario.FFilter;
                
                // 대상 누군지
                if(filter.MaidName != f.MaidName) continue;
                
                // 수치 비교
                var figure = filter.Figure;
                
                if(figure.Relationship < f.CurrentFigure.Relationship) continue;
                if(figure.Level < f.CurrentFigure.Level) continue;

                FavorBuffer.Add(scenario);
            }
            
            return FavorBuffer;
        }
        
        /// <summary> 호감도 대화 필터링 </summary>
        /// todo 봤던 대화들 어떻게 처리할지 필요함
        public static List<SO_MaidScenarioBase> StartConversationFilter(IReadOnlyList<SO_MaidScenarioBase> source, in SCurrentFilter f)
        {
            BaseBuffer.Clear();
            var count = source.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = source[i];
                var filter = scenario.BFilter;
                
                // 대상 누군지
                if(filter.MaidName != f.MaidName) continue;
                
                // 수치 비교
                var figure = filter.Figure;
                
                if(figure.Level < f.CurrentFigure.Level) continue;

                BaseBuffer.Add(scenario);
            }
            
            return BaseBuffer;
        }
        
    }
}
