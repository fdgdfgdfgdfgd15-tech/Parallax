using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;

namespace CompMenu.Core
{
    [Serializable]
    public class ConfigData
    {
        public int CurrentTheme = 0;

        public bool PullModEnabled = true;
        public float PullStrength = 1f;
        public float PullThreshold = 0f;
        public float PullTpTime = 0.013f;
        public int PullActivation = 13;
        public bool PullObstacleDetection = false;
        public float PullObstacleRadius = 0.5f;
        public bool WallPullEnabled = false;
        public bool TagFreezeTpEnabled = false;

        public bool WallWalkEnabled = false;
        public float WallWalkDistance = 0.5f;
        public float WallWalkPower = 2f;
        public bool WallWalkLeftHand = true;
        public bool WallWalkRightHand = true;

        public bool DCFlickEnabled = false;
        public bool TagAuraEnabled = false;
        public float TagAuraDistance = 1f;
        public bool HitboxExpanderEnabled = false;
        public float HitboxExpanderSize = 0.5f;
        public float HitboxExpanderOpacity = 0.5f;

        public bool PSAEnabled = false;
        public float PSASpeed = 1f;
        public int PSAKeybind = 1;
        public bool HighJumpEnabled = false;
        public float HighJumpSpeed = 5f;
        public int HighJumpKeybind = 7;
        public bool RecRoomEnabled = false;
        public float RecRoomSpeed = 1f;
        public int RecRoomForwardKeybind = 5;
        public int RecRoomBackwardKeybind = 6;

        public bool VelmaxEnabled = false;
        public float VelmaxMultiplier = 1.2f;

        public bool ForceTagFreezeEnabled = false;

        public bool HzSliderEnabled = false;
        public int TargetHz = 72;

        public bool NoSlipEnabled = false;
        
        public bool VelmaxBypassEnabled = false;
        public int VelmaxBypassKeybind = 4;

        public bool PredsEnabled = false;
        public float PredsAmount = 0f;
        public float PredsLegit = 4.6f;
        public float PredsValve = 6f;
        public float PredsRiftS = 9.5f;
        public float PredsPico = 12f;
        public bool PredsAlwaysOn = false;
        public float PredsAlwaysAmount = 20f;
        public int PredsHand = 0;

        public bool ESPEnabled = false;
        public float ESPColorHue = 0f;
        public float ESPColorSat = 1f;
        public float ESPColorVal = 1f;
        public bool TracersEnabled = false;
        public bool HitboxesEnabled = false;
        public bool NameTagsEnabled = false;
        public bool CornerESPEnabled = false;
    }

    [Serializable]
    public class AutoLoadSettings
    {
        public bool AutoLoadEnabled = false;
        public string AutoLoadConfigName = "default";
    }

    public static class ConfigManager
    {
        private static string ConfigFolder => Path.Combine(Paths.PluginPath, "PARALLAX");
        private static string ConfigsFolder => Path.Combine(ConfigFolder, "configs");
        private static string AutoLoadPath => Path.Combine(ConfigFolder, "autoload.json");
        
        public static string LastSaveTime = "Never";
        public static string LastLoadTime = "Never";
        public static string CurrentConfigName = "default";
        
        public static bool AutoLoadEnabled = false;
        public static string AutoLoadConfigName = "default";
        
        private static List<string> cachedConfigList = new List<string>();
        public static int SelectedConfigIndex = 0;

        private static void EnsureConfigFolder()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);
            if (!Directory.Exists(ConfigsFolder))
                Directory.CreateDirectory(ConfigsFolder);
        }

        public static List<string> GetConfigList()
        {
            EnsureConfigFolder();
            cachedConfigList.Clear();
            
            string[] files = Directory.GetFiles(ConfigsFolder, "*.json");
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                cachedConfigList.Add(name);
            }
            
            if (cachedConfigList.Count == 0)
                cachedConfigList.Add("default");
                
            return cachedConfigList;
        }

        public static void RefreshConfigList()
        {
            GetConfigList();
            
            int index = cachedConfigList.IndexOf(CurrentConfigName);
            if (index >= 0)
                SelectedConfigIndex = index;
            else
                SelectedConfigIndex = 0;
        }

        public static string GetSelectedConfigName()
        {
            if (cachedConfigList.Count == 0)
                GetConfigList();
            if (SelectedConfigIndex >= 0 && SelectedConfigIndex < cachedConfigList.Count)
                return cachedConfigList[SelectedConfigIndex];
            return "default";
        }

        public static void CycleSelectedConfig()
        {
            if (cachedConfigList.Count == 0)
                GetConfigList();
            SelectedConfigIndex = (SelectedConfigIndex + 1) % cachedConfigList.Count;
        }

        public static void SaveConfig(string configName = null)
        {
            try
            {
                EnsureConfigFolder();
                
                if (string.IsNullOrEmpty(configName))
                    configName = CurrentConfigName;
                
                ConfigData config = new ConfigData
                {
                    CurrentTheme = (int)Settings.CurrentTheme,

                    PullModEnabled = Settings.PullModEnabled,
                    PullStrength = Settings.PullStrength,
                    PullThreshold = Settings.PullThreshold,
                    PullTpTime = Settings.PullTpTime,
                    PullActivation = (int)Settings.PullActivation,
                    PullObstacleDetection = Settings.PullObstacleDetection,
                    PullObstacleRadius = Settings.PullObstacleRadius,
                    WallPullEnabled = Settings.WallPullEnabled,
                    TagFreezeTpEnabled = Settings.TagFreezeTpEnabled,

                    WallWalkEnabled = Settings.WallWalkEnabled,
                    WallWalkDistance = Settings.WallWalkDistance,
                    WallWalkPower = Settings.WallWalkPower,
                    WallWalkLeftHand = Settings.WallWalkLeftHand,
                    WallWalkRightHand = Settings.WallWalkRightHand,

                    DCFlickEnabled = Settings.DCFlickEnabled,
                    TagAuraEnabled = Settings.TagAuraEnabled,
                    TagAuraDistance = Settings.TagAuraDistance,
                    HitboxExpanderEnabled = Settings.HitboxExpanderEnabled,
                    HitboxExpanderSize = Settings.HitboxExpanderSize,
                    HitboxExpanderOpacity = Settings.HitboxExpanderOpacity,

                    PSAEnabled = Settings.PSAEnabled,
                    PSASpeed = Settings.PSASpeed,
                    PSAKeybind = (int)Settings.PSAKeybind,
                    HighJumpEnabled = Settings.HighJumpEnabled,
                    HighJumpSpeed = Settings.HighJumpSpeed,
                    HighJumpKeybind = (int)Settings.HighJumpKeybind,
                    RecRoomEnabled = Settings.RecRoomEnabled,
                    RecRoomSpeed = Settings.RecRoomSpeed,
                    RecRoomForwardKeybind = (int)Settings.RecRoomForwardKeybind,
                    RecRoomBackwardKeybind = (int)Settings.RecRoomBackwardKeybind,

                    VelmaxEnabled = Settings.VelmaxEnabled,
                    VelmaxMultiplier = Settings.VelmaxMultiplier,

                    ForceTagFreezeEnabled = Settings.ForceTagFreezeEnabled,

                    HzSliderEnabled = Settings.HzSliderEnabled,
                    TargetHz = Settings.TargetHz,

                    NoSlipEnabled = Settings.NoSlipEnabled,
                    
                    VelmaxBypassEnabled = Settings.VelmaxBypassEnabled,
                    VelmaxBypassKeybind = (int)Settings.VelmaxBypassKeybind,

                    PredsEnabled = Settings.PredsEnabled,
                    PredsAmount = Settings.PredsAmount,
                    PredsLegit = Settings.PredsLegit,
                    PredsValve = Settings.PredsValve,
                    PredsRiftS = Settings.PredsRiftS,
                    PredsPico = Settings.PredsPico,
                    PredsAlwaysOn = Settings.PredsAlwaysOn,
                    PredsAlwaysAmount = Settings.PredsAlwaysAmount,
                    PredsHand = (int)Settings.PredsHand,

                    ESPEnabled = Settings.ESPEnabled,
                    ESPColorHue = Settings.ESPColorHue,
                    ESPColorSat = Settings.ESPColorSat,
                    ESPColorVal = Settings.ESPColorVal,
                    TracersEnabled = Settings.TracersEnabled,
                    HitboxesEnabled = Settings.HitboxesEnabled,
                    NameTagsEnabled = Settings.NameTagsEnabled,
                    CornerESPEnabled = Settings.CornerESPEnabled
                };

                string configPath = Path.Combine(ConfigsFolder, configName + ".json");
                string json = JsonUtility.ToJson(config, true);
                File.WriteAllText(configPath, json);
                
                CurrentConfigName = configName;
                LastSaveTime = DateTime.Now.ToString("HH:mm:ss");
                RefreshConfigList();
                
                Debug.Log("[Parallax] Config saved: " + configName);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Parallax] Failed to save config: " + ex.Message);
            }
        }

        public static void LoadConfig(string configName = null)
        {
            try
            {
                EnsureConfigFolder();
                
                if (string.IsNullOrEmpty(configName))
                    configName = GetSelectedConfigName();
                
                string configPath = Path.Combine(ConfigsFolder, configName + ".json");
                
                if (!File.Exists(configPath))
                {
                    Debug.Log("[Parallax] Config not found: " + configName);
                    return;
                }

                string json = File.ReadAllText(configPath);
                ConfigData config = JsonUtility.FromJson<ConfigData>(json);

                if (config == null)
                {
                    Debug.LogError("[Parallax] Failed to parse config: " + configName);
                    return;
                }

                Settings.CurrentTheme = (ThemeOption)config.CurrentTheme;

                Settings.PullModEnabled = config.PullModEnabled;
                Settings.PullStrength = config.PullStrength;
                Settings.PullThreshold = config.PullThreshold;
                Settings.PullTpTime = config.PullTpTime;
                Settings.PullActivation = (ActivationMode)config.PullActivation;
                Settings.PullObstacleDetection = config.PullObstacleDetection;
                Settings.PullObstacleRadius = config.PullObstacleRadius;
                Settings.WallPullEnabled = config.WallPullEnabled;
                Settings.TagFreezeTpEnabled = config.TagFreezeTpEnabled;

                Settings.WallWalkEnabled = config.WallWalkEnabled;
                Settings.WallWalkDistance = config.WallWalkDistance;
                Settings.WallWalkPower = config.WallWalkPower;
                Settings.WallWalkLeftHand = config.WallWalkLeftHand;
                Settings.WallWalkRightHand = config.WallWalkRightHand;

                Settings.DCFlickEnabled = config.DCFlickEnabled;
                Settings.TagAuraEnabled = config.TagAuraEnabled;
                Settings.TagAuraDistance = config.TagAuraDistance;
                Settings.HitboxExpanderEnabled = config.HitboxExpanderEnabled;
                Settings.HitboxExpanderSize = config.HitboxExpanderSize;
                Settings.HitboxExpanderOpacity = config.HitboxExpanderOpacity;

                Settings.PSAEnabled = config.PSAEnabled;
                Settings.PSASpeed = config.PSASpeed;
                Settings.PSAKeybind = (KeybindOption)config.PSAKeybind;
                Settings.HighJumpEnabled = config.HighJumpEnabled;
                Settings.HighJumpSpeed = config.HighJumpSpeed;
                Settings.HighJumpKeybind = (KeybindOption)config.HighJumpKeybind;
                Settings.RecRoomEnabled = config.RecRoomEnabled;
                Settings.RecRoomSpeed = config.RecRoomSpeed;
                Settings.RecRoomForwardKeybind = (KeybindOption)config.RecRoomForwardKeybind;
                Settings.RecRoomBackwardKeybind = (KeybindOption)config.RecRoomBackwardKeybind;

                Settings.VelmaxEnabled = config.VelmaxEnabled;
                Settings.VelmaxMultiplier = config.VelmaxMultiplier;

                Settings.ForceTagFreezeEnabled = config.ForceTagFreezeEnabled;

                Settings.HzSliderEnabled = config.HzSliderEnabled;
                Settings.TargetHz = config.TargetHz;

                Settings.NoSlipEnabled = config.NoSlipEnabled;
                
                Settings.VelmaxBypassEnabled = config.VelmaxBypassEnabled;
                Settings.VelmaxBypassKeybind = (KeybindOption)config.VelmaxBypassKeybind;

                Settings.PredsEnabled = config.PredsEnabled;
                Settings.PredsAmount = config.PredsAmount;
                Settings.PredsLegit = config.PredsLegit;
                Settings.PredsValve = config.PredsValve;
                Settings.PredsRiftS = config.PredsRiftS;
                Settings.PredsPico = config.PredsPico;
                Settings.PredsAlwaysOn = config.PredsAlwaysOn;
                Settings.PredsAlwaysAmount = config.PredsAlwaysAmount;
                Settings.PredsHand = (PredsHandOption)config.PredsHand;

                Settings.ESPEnabled = config.ESPEnabled;
                Settings.ESPColorHue = config.ESPColorHue;
                Settings.ESPColorSat = config.ESPColorSat;
                Settings.ESPColorVal = config.ESPColorVal;
                Settings.TracersEnabled = config.TracersEnabled;
                Settings.HitboxesEnabled = config.HitboxesEnabled;
                Settings.NameTagsEnabled = config.NameTagsEnabled;
                Settings.CornerESPEnabled = config.CornerESPEnabled;

                CurrentConfigName = configName;
                LastLoadTime = DateTime.Now.ToString("HH:mm:ss");
                RefreshConfigList();
                
                Debug.Log("[Parallax] Config loaded: " + configName);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Parallax] Failed to load config: " + ex.Message);
            }
        }

        public static void DeleteConfig(string configName)
        {
            try
            {
                if (configName == "default")
                {
                    Debug.Log("[Parallax] Cannot delete default config");
                    return;
                }
                
                string configPath = Path.Combine(ConfigsFolder, configName + ".json");
                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                    Debug.Log("[Parallax] Config deleted: " + configName);
                    
                    if (CurrentConfigName == configName)
                        CurrentConfigName = "default";
                    if (AutoLoadConfigName == configName)
                        AutoLoadConfigName = "default";
                        
                    RefreshConfigList();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[Parallax] Failed to delete config: " + ex.Message);
            }
        }

        public static void SaveAutoLoadSettings()
        {
            try
            {
                EnsureConfigFolder();
                
                AutoLoadSettings settings = new AutoLoadSettings
                {
                    AutoLoadEnabled = AutoLoadEnabled,
                    AutoLoadConfigName = AutoLoadConfigName
                };
                
                string json = JsonUtility.ToJson(settings, true);
                File.WriteAllText(AutoLoadPath, json);
                
                Debug.Log("[Parallax] Auto-load settings saved");
            }
            catch (Exception ex)
            {
                Debug.LogError("[Parallax] Failed to save auto-load settings: " + ex.Message);
            }
        }

        public static void LoadAutoLoadSettings()
        {
            try
            {
                if (!File.Exists(AutoLoadPath))
                    return;
                
                string json = File.ReadAllText(AutoLoadPath);
                AutoLoadSettings settings = JsonUtility.FromJson<AutoLoadSettings>(json);
                
                if (settings != null)
                {
                    AutoLoadEnabled = settings.AutoLoadEnabled;
                    AutoLoadConfigName = settings.AutoLoadConfigName;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[Parallax] Failed to load auto-load settings: " + ex.Message);
            }
        }

        public static void TryAutoLoad()
        {
            LoadAutoLoadSettings();
            
            if (AutoLoadEnabled && !string.IsNullOrEmpty(AutoLoadConfigName))
            {
                string configPath = Path.Combine(ConfigsFolder, AutoLoadConfigName + ".json");
                if (File.Exists(configPath))
                {
                    LoadConfig(AutoLoadConfigName);
                    Debug.Log("[Parallax] Auto-loaded config: " + AutoLoadConfigName);
                }
            }
        }

        public static bool ConfigExists(string configName = null)
        {
            if (string.IsNullOrEmpty(configName))
                configName = CurrentConfigName;
            string configPath = Path.Combine(ConfigsFolder, configName + ".json");
            return File.Exists(configPath);
        }

        public static string GetConfigPath()
        {
            return Path.Combine(ConfigsFolder, CurrentConfigName + ".json");
        }

        public static int GetConfigCount()
        {
            if (cachedConfigList.Count == 0)
                GetConfigList();
            return cachedConfigList.Count;
        }
    }
}
