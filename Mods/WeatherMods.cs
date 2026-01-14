using System;
using CompMenu.Core;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class WeatherMods
    {
        public static readonly string[] TimeNames = 
        { 
            "None", 
            "Day", 
            "Dawn", 
            "Night", 
            "Night Fall", 
            "Mid Night" 
        };
        
        public static readonly string[] WeatherNames = 
        { 
            "None", 
            "Rain", 
            "Clear" 
        };

        public static void ApplyTimeOfDay()
        {
            try
            {
                if (BetterDayNightManager.instance == null) return;
                
                switch (Settings.CurrentTimeOfDay)
                {
                    case 1:
                        BetterDayNightManager.instance.SetTimeOfDay(3);
                        break;
                    case 2:
                        BetterDayNightManager.instance.SetTimeOfDay(1);
                        break;
                    case 3:
                        BetterDayNightManager.instance.SetTimeOfDay(0);
                        break;
                    case 4:
                        BetterDayNightManager.instance.SetTimeOfDay(6);
                        break;
                    case 5:
                        BetterDayNightManager.instance.SetTimeOfDay(8);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] Time Change Error: " + ex.Message);
            }
        }

        public static void ApplyWeather()
        {
            try
            {
                if (BetterDayNightManager.instance == null) return;
                if (BetterDayNightManager.instance.weatherCycle == null) return;
                
                switch (Settings.CurrentWeather)
                {
                    case 1:
                        for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                        {
                            BetterDayNightManager.instance.weatherCycle[i] = (BetterDayNightManager.WeatherType)1;
                        }
                        break;
                    case 2:
                        for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                        {
                            BetterDayNightManager.instance.weatherCycle[i] = (BetterDayNightManager.WeatherType)0;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] Weather Change Error: " + ex.Message);
            }
        }

        public static void CycleTimeOfDay()
        {
            Settings.CurrentTimeOfDay++;
            if (Settings.CurrentTimeOfDay >= TimeNames.Length)
            {
                Settings.CurrentTimeOfDay = 0;
            }
            ApplyTimeOfDay();
        }

        public static void CycleWeather()
        {
            Settings.CurrentWeather++;
            if (Settings.CurrentWeather >= WeatherNames.Length)
            {
                Settings.CurrentWeather = 0;
            }
            ApplyWeather();
        }

        public static string GetCurrentTimeName()
        {
            if (Settings.CurrentTimeOfDay >= 0 && Settings.CurrentTimeOfDay < TimeNames.Length)
            {
                return "Time: " + TimeNames[Settings.CurrentTimeOfDay];
            }
            return "Time: Unknown";
        }

        public static string GetCurrentWeatherName()
        {
            if (Settings.CurrentWeather >= 0 && Settings.CurrentWeather < WeatherNames.Length)
            {
                return "Weather: " + WeatherNames[Settings.CurrentWeather];
            }
            return "Weather: Unknown";
        }
    }
}
