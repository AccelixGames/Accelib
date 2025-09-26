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

#region Flags

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
        Meet = 1 << 0,
        Promise = 1 << 1,
    }

    /// <summary> 선물 플래그 </summary>
    [Flags]
    public enum EGiftFlags
    {
        None = 0,
        Gift1 = 1 << 0,
        Gift2 = 1 << 1,
        Gift3 = 1 << 2,
    }

    /// <summary> 장난감 플래그 </summary>
    [Flags]
    public enum EToyFlags
    {
        None = 0,
        Toy1 = 1 << 0,
        Toy2 = 1 << 1,
        Toy3 = 1 << 2,
    }
    
#endregion

#region Figures

    /// <summary> 시작 대화 오픈 수치 비교군 </summary>
    [System.Serializable]
    public struct SBaseFigure
    {
        public int Level;
    }
    
    /// <summary> 일상 시나리오 오픈 수치 </summary>
    [System.Serializable]
    public struct SDailyFigure
    {
        public int Bond;
        public int Relationship;
        public int Level;
    }
    
    /// <summary> 호감 시나리오 오픈 수치 </summary>
    [System.Serializable]
    public struct SFavorFigure
    {
        public int Relationship;
        public int Level;
    }
    
    
#endregion

#region Filters

    /// <summary> 시작 대화 필터 </summary>
    [System.Serializable]
    public struct SStartFilter
    {
        public EMaidName MaidName;
        
        public SBaseFigure Figure;
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

    /// <summary> 호감 시나리오 필터 </summary>
    [System.Serializable]
    public struct SFavorFilter
    {
        public EMaidName MaidName;
        
        public SFavorFigure Figure;
    }

    /// <summary> 선물 시나리오 필터 </summary>
    [System.Serializable]
    public struct SGiftFilter
    {
        public EMaidName MaidName;
        
        public EGiftFlags RequiredGifts;
    }

    /// <summary> 장난감 시나리오 필터 </summary>
    [System.Serializable]
    public struct SToyFilter
    {
        public EMaidName MaidName;
        
        public EToyFlags RequiredToys;
    }
    
    [System.Serializable]
    /// <summary> 현재 상태 비교용 필터 </summary>
    public class CurrentFilter
    {
        public EMaidName MaidName;
        public EFoodFlags FoodFlags;
        public EGiftFlags CurrentGift;
        public EToyFlags CurrentToy;
        public EDailyFlags CurrentFlags;
        
        public SDailyFigure CurrentFigure;


    }
    
#endregion

    
    public static partial class MaidScenarioDirector
    {
        private static CurrentFilter currentFilter;
        public static CurrentFilter CurrentFilter => currentFilter;
        
        private static readonly List<SO_MaidStartScenario> StartScns = new();
        private static readonly List<SO_MaidDailyScenario> DailyScns = new();
        private static readonly List<SO_MaidFavorScenario> FavorScns = new();
        private static readonly List<SO_MaidGiftScenario> GiftScns = new();
        private static readonly List<SO_MaidToyScenario> ToyScns = new();
        
        private static readonly List<SO_MaidStartScenario> StartBuffer = new (capacity: 128);
        private static readonly List<SO_MaidDailyScenario> DailyBuffer = new (capacity: 128);
        private static readonly List<SO_MaidFavorScenario> FavorBuffer = new (capacity: 128);
        private static readonly List<SO_MaidGiftScenario> GiftBuffer = new (capacity: 128);
        private static readonly List<SO_MaidToyScenario> ToyBuffer = new (capacity: 128);
        
        public static void SetScenarioBuffers(CurrentFilter curFilter,
            IReadOnlyList<SO_MaidStartScenario> startScns, 
            IReadOnlyList<SO_MaidDailyScenario> dailyScns,
            IReadOnlyList<SO_MaidFavorScenario> favorScns,
            IReadOnlyList<SO_MaidGiftScenario> giftScns,
            IReadOnlyList<SO_MaidToyScenario> toyScns)
        {
            currentFilter = curFilter;
            
            StartScns?.Clear();
            DailyScns?.Clear();
            FavorScns?.Clear();
            GiftScns?.Clear();
            ToyScns?.Clear();
            
            StartScns?.AddRange(startScns);
            DailyScns?.AddRange(dailyScns);
            FavorScns?.AddRange(favorScns);
            GiftScns?.AddRange(giftScns);
            ToyScns?.AddRange(toyScns);
        }

        public static bool StartFilter(in SStartFilter sFilter, in CurrentFilter f)
        {
            var filter = sFilter;
                
            // 대상 누군지
            if(filter.MaidName != f.MaidName) return false;
                
            // 수치 비교
            var figure = filter.Figure;
                
            if(figure.Level < f.CurrentFigure.Level) return false;
            
            return true;
        }
        
        public static bool DailyFilter(in SDailyFilter dFilter, in CurrentFilter f)
        {
            var filter = dFilter;
                
            // 대상 누군지
            if(filter.MaidName != f.MaidName) return false;
                
            // Daily 오픈 플래그 조건
            var requiredFlags = filter.RequiredFlags;
            var forbiddenFlags = filter.ForbiddenFlags;
                
            if((requiredFlags & f.CurrentFlags) != requiredFlags) return false;
            if((forbiddenFlags & f.CurrentFlags) != 0) return false;
                
            // 음식 오픈 조건
            if((filter.FoodFlags & f.FoodFlags) != filter.FoodFlags) return false;
                
            // 수치 비교
            var figure = filter.Figure;
                
            if(figure.Bond < f.CurrentFigure.Bond) return false;
            if(figure.Relationship < f.CurrentFigure.Relationship) return false;
            if(figure.Level < f.CurrentFigure.Level) return false;
            
            return true;
        }

        public static bool FavorFilter(in SFavorFilter fFilter, in CurrentFilter f)
        {
            var filter = fFilter;
                
            // 대상 누군지
            if(filter.MaidName != f.MaidName) return false;
                
            // 수치 비교
            var figure = filter.Figure;
                
            if(figure.Relationship < f.CurrentFigure.Relationship) return false;
            if(figure.Level < f.CurrentFigure.Level) return false;
            
            return true;
        }

        public static bool GiftFilter(in SGiftFilter gFilter, in CurrentFilter f)
        {
            var filter = gFilter;
                
            // 대상 누군지
            if(filter.MaidName != f.MaidName) return false;
                
            // 플래그 비교
            var requiredFlags = filter.RequiredGifts;
                
            if((requiredFlags & f.CurrentGift) != requiredFlags) return false;
            
            return true;
        }

        public static bool ToyFilter(in SToyFilter tFilter, in CurrentFilter f)
        {
            var filter = tFilter;
                
            // 대상 누군지
            if(filter.MaidName != f.MaidName) return false;
                
            // 플래그 비교
            var requiredFlags = filter.RequiredToys;
                
            if((requiredFlags & f.CurrentToy) != requiredFlags) return false;

            return true;
        }
        
        /// <summary> 시작 대화 필터링 </summary>
        public static List<SO_MaidStartScenario> StartScnFilter(in CurrentFilter f)
        {
            StartBuffer.Clear();
            var count = StartScns.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = StartScns[i];

                var pass = StartFilter(scenario.SFilter, f);
                
                if(!pass) continue;

                StartBuffer.Add(scenario);
            }
            
            return StartBuffer;
        }

        /// <summary> 일상 대화 필터링 </summary>
        public static List<SO_MaidDailyScenario> DailyScnFilter(in CurrentFilter f)
        {
            DailyBuffer.Clear();
            var count = DailyScns.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = DailyScns[i];
                
                var pass = DailyFilter(scenario.DFilter, f);
                
                if(!pass) continue;
                
                DailyBuffer.Add(scenario);
            }
            
            return DailyBuffer;
        }

        /// <summary> 호감도 대화 필터링 </summary>
        public static List<SO_MaidFavorScenario> FavorScnFilter(in CurrentFilter f)
        {
            FavorBuffer.Clear();
            var count = FavorScns.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = FavorScns[i];

                var pass = FavorFilter(scenario.FFilter, f);
                
                if(!pass) continue;

                FavorBuffer.Add(scenario);
            }
            
            return FavorBuffer;
        }
        
        /// <summary> 선물 대화 필터링 </summary>
        public static List<SO_MaidGiftScenario> GiftScnFilter(in CurrentFilter f)
        {
            GiftBuffer.Clear();
            var count = GiftScns.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = GiftScns[i];
                
                var pass = GiftFilter(scenario.GFilter, f);
                
                if(!pass) continue;

                GiftBuffer.Add(scenario);
            }
            
            return GiftBuffer;
        }
        
        /// <summary> 장난감 대화 필터링 </summary>
        public static List<SO_MaidToyScenario> ToyScnFilter(in CurrentFilter f)
        {
            ToyBuffer.Clear();
            var count = ToyScns.Count;

            for (int i = 0; i < count; ++i)
            {
                var scenario = ToyScns[i];
                
                var pass = ToyFilter(scenario.TFilter, f);

                if(!pass) continue;
                
                ToyBuffer.Add(scenario);
            }
            
            return ToyBuffer;
        }
        
        
        
    }
    
    public static partial class MaidScenarioDirector
    {
        public static Action<SO_MaidScenarioBase> addScenario;
        public static GameObject mainActor;
        public static int mainUnumber;
        public static void PublishAddScenario(SO_MaidScenarioBase scenario) => addScenario?.Invoke(scenario);

        public static void SetScenarioMainActor(GameObject actor, int uNum)
        {
            mainActor = actor;
            mainUnumber = uNum;
        } 
        
        
    } 

}
