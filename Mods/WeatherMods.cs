using System;
using CompMenu.Core;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class WeatherMods
    {
        // Time of Day names for display
        public static readonly string[] TimeNames = 
        { 
            "None", 
            "Day", 
            "Dawn", 
            "Night", 
            "Night Fall", 
            "Mid Night" 
        };
        
        // Weather names for display
        public static readonly string[] WeatherNames = 
        { 
            "None", 
            "Rain", 
            "Clear" 
        };

        // Apply Time of Day change
        public static void ApplyTimeOfDay()
        {
            try
            {
                if (BetterDayNightManager.instance == null) return;
                
                switch (Settings.CurrentTimeOfDay)
                {
                    case 1: // Day
                        BetterDayNightManager.instance.SetTimeOfDay(3);
                        break;
                    case 2: // Dawn
                        BetterDayNightManager.instance.SetTimeOfDay(1);
                        break;
                    case 3: // Night
                        BetterDayNightManager.instance.SetTimeOfDay(0);
                        break;
                    case 4: // Night Fall
                        BetterDayNightManager.instance.SetTimeOfDay(6);
                        break;
                    case 5: // Mid Night
                        BetterDayNightManager.instance.SetTimeOfDay(8);
                        break;
                    // case 0: None - don't change anything
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] Time Change Error: " + ex.Message);
            }
        }

        // Apply Weather change
        public static void ApplyWeather()
        {
            try
            {
                if (BetterDayNightManager.instance == null) return;
                if (BetterDayNightManager.instance.weatherCycle == null) return;
                
                switch (Settings.CurrentWeather)
                {
                    case 1: // Rain
                        for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                        {
                            BetterDayNightManager.instance.weatherCycle[i] = (BetterDayNightManager.WeatherType)1;
                        }
                        break;
                    case 2: // Clear
                        for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                        {
                            BetterDayNightManager.instance.weatherCycle[i] = (BetterDayNightManager.WeatherType)0;
                        }
                        break;
                    // case 0: None - don't change anything
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] Weather Change Error: " + ex.Message);
            }
        }

        // Cycle to next time of day
        public static void CycleTimeOfDay()
        {
            Settings.CurrentTimeOfDay++;
            if (Settings.CurrentTimeOfDay >= TimeNames.Length)
            {
                Settings.CurrentTimeOfDay = 0;
            }
            ApplyTimeOfDay();
        }

        // Cycle to next weather
        public static void CycleWeather()
        {
            Settings.CurrentWeather++;
            if (Settings.CurrentWeather >= WeatherNames.Length)
            {
                Settings.CurrentWeather = 0;
            }
            ApplyWeather();
        }

        // Get current time display name
        public static string GetCurrentTimeName()
        {
            if (Settings.CurrentTimeOfDay >= 0 && Settings.CurrentTimeOfDay < TimeNames.Length)
            {
                return "Time: " + TimeNames[Settings.CurrentTimeOfDay];
            }
            return "Time: Unknown";
        }

        // Get current weather display name
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

