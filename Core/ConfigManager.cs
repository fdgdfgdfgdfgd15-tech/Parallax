using System;
using System.IO;
using BepInEx;
using UnityEngine;

namespace CompMenu.Core
{
    [Serializable]
    public class ConfigData
    {
        // Theme
        public int CurrentTheme = 0;

        // Pull Mod Settings
        public bool PullModEnabled = true;
        public float PullStrength = 1f;
        public float PullThreshold = 0f;
        public float PullTpTime = 0.013f;
        public int PullActivation = 13; // LeftJoystickClick
        public bool PullObstacleDetection = false;
        public float PullObstacleRadius = 0.5f;
        public bool WallPullEnabled = false;
        public bool UseD1vineTagFreeze = false;
        public float D1vineTagFreezeMultiplier = 5f;

        // Wall Walk Settings
        public bool WallWalkEnabled = false;
        public float WallWalkDistance = 0.5f;
        public float WallWalkPower = 2f;
        public bool WallWalkLeftHand = true;
        public bool WallWalkRightHand = true;

        // Tag Mod Settings
        public bool DCFlickEnabled = false;
        public bool TagAuraEnabled = false;
        public float TagAuraDistance = 1f;
        public bool HitboxExpanderEnabled = false;
        public float HitboxExpanderSize = 0.5f;
        public float HitboxExpanderOpacity = 0.5f;

        // PSA Tab Settings
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

        // SpeedBoost Tab Settings
        public bool PidegoEnabled = false;
        public bool PidegoGroundEnabled = false;
        public float PidegoGroundMultiplier = 1f;
        public bool PidegoWallEnabled = false;
        public float PidegoWallMultiplier = 1f;
        public bool PidegoDoubleWallEnabled = false;
        public float PidegoDoubleWallMultiplier = 1f;
        public bool PidegoSlipEnabled = false;
        public float PidegoSlipMultiplier = 1f;
        public bool VelmaxEnabled = false;
        public float VelmaxMultiplier = 1.2f;

        // Bypass Tab Settings
        public bool WorldScaleBypassEnabled = false;
        public int WorldScaleBypassActivation = 1; // LeftGrip
        public bool VelmaxCheckEnabled = false;
        public int VelmaxCheckActivation = 2; // RightGrip
        public bool ForceTagFreezeEnabled = false;

        // Safety Tab Settings
        public bool AntiReportEnabled = false;

        // Controller Mods Tab Settings
        public bool DCModEnabled = false;
        public float DCModLeftDistance = 0f;
        public float DCModRightDistance = 0f;
        public float DCModLeftThreshold = 0f;
        public float DCModRightThreshold = 0f;
        public int DCModLeftDuration = 1;
        public int DCModRightDuration = 1;
        public int DCModKeybind = 4;
        public bool PredsEnabled = false;
        public float PredsSmoothing = 0.1f;

        // ESP Tab Settings
        public bool ESPEnabled = false;
        public float ESPColorHue = 0f;
        public float ESPColorSat = 1f;
        public float ESPColorVal = 1f;
        public bool TracersEnabled = false;
        public bool HitboxesEnabled = false;
        public bool NameTagsEnabled = false;
        public bool CornerESPEnabled = false;
    }

    public static class ConfigManager
    {
        private static string ConfigFolder => Path.Combine(Paths.PluginPath, "PARALLAX");
        private static string ConfigPath => Path.Combine(ConfigFolder, "config.json");
        public static string LastSaveTime = "Never";
        public static string LastLoadTime = "Never";

        private static void EnsureConfigFolder()
        {
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }
        }

        public static void SaveConfig()
        {
            try
            {
                EnsureConfigFolder();
                
                ConfigData config = new ConfigData
                {
                    // Theme
                    CurrentTheme = (int)Settings.CurrentTheme,

                    // Pull Mod
                    PullModEnabled = Settings.PullModEnabled,
                    PullStrength = Settings.PullStrength,
                    PullThreshold = Settings.PullThreshold,
                    PullTpTime = Settings.PullTpTime,
                    PullActivation = (int)Settings.PullActivation,
                    PullObstacleDetection = Settings.PullObstacleDetection,
                    PullObstacleRadius = Settings.PullObstacleRadius,
                    WallPullEnabled = Settings.WallPullEnabled,
                    UseD1vineTagFreeze = Settings.UseD1vineTagFreeze,
                    D1vineTagFreezeMultiplier = Settings.D1vineTagFreezeMultiplier,

                    // Wall Walk
                    WallWalkEnabled = Settings.WallWalkEnabled,
                    WallWalkDistance = Settings.WallWalkDistance,
                    WallWalkPower = Settings.WallWalkPower,
                    WallWalkLeftHand = Settings.WallWalkLeftHand,
                    WallWalkRightHand = Settings.WallWalkRightHand,

                    // Tag Mod
                    DCFlickEnabled = Settings.DCFlickEnabled,
                    TagAuraEnabled = Settings.TagAuraEnabled,
                    TagAuraDistance = Settings.TagAuraDistance,
                    HitboxExpanderEnabled = Settings.HitboxExpanderEnabled,
                    HitboxExpanderSize = Settings.HitboxExpanderSize,
                    HitboxExpanderOpacity = Settings.HitboxExpanderOpacity,

                    // PSA
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

                    // SpeedBoost
                    PidegoEnabled = Settings.PidegoEnabled,
                    PidegoGroundEnabled = Settings.PidegoGroundEnabled,
                    PidegoGroundMultiplier = Settings.PidegoGroundMultiplier,
                    PidegoWallEnabled = Settings.PidegoWallEnabled,
                    PidegoWallMultiplier = Settings.PidegoWallMultiplier,
                    PidegoDoubleWallEnabled = Settings.PidegoDoubleWallEnabled,
                    PidegoDoubleWallMultiplier = Settings.PidegoDoubleWallMultiplier,
                    PidegoSlipEnabled = Settings.PidegoSlipEnabled,
                    PidegoSlipMultiplier = Settings.PidegoSlipMultiplier,
                    VelmaxEnabled = Settings.VelmaxEnabled,
                    VelmaxMultiplier = Settings.VelmaxMultiplier,

                    // Bypass
                    WorldScaleBypassEnabled = Settings.WorldScaleBypassEnabled,
                    WorldScaleBypassActivation = (int)Settings.WorldScaleBypassActivation,
                    VelmaxCheckEnabled = Settings.VelmaxCheckEnabled,
                    VelmaxCheckActivation = (int)Settings.VelmaxCheckActivation,
                    ForceTagFreezeEnabled = Settings.ForceTagFreezeEnabled,

                    // Safety
                    AntiReportEnabled = Settings.AntiReportEnabled,

                    // Controller Mods
                    DCModEnabled = Settings.DCModEnabled,
                    DCModLeftDistance = Settings.DCModLeftDistance,
                    DCModRightDistance = Settings.DCModRightDistance,
                    DCModLeftThreshold = Settings.DCModLeftThreshold,
                    DCModRightThreshold = Settings.DCModRightThreshold,
                    DCModLeftDuration = Settings.DCModLeftDuration,
                    DCModRightDuration = Settings.DCModRightDuration,
                    DCModKeybind = (int)Settings.DCModKeybind,
                    PredsEnabled = Settings.PredsEnabled,
                    PredsSmoothing = Settings.PredsSmoothing,

                    // ESP
                    ESPEnabled = Settings.ESPEnabled,
                    ESPColorHue = Settings.ESPColorHue,
                    ESPColorSat = Settings.ESPColorSat,
                    ESPColorVal = Settings.ESPColorVal,
                    TracersEnabled = Settings.TracersEnabled,
                    HitboxesEnabled = Settings.HitboxesEnabled,
                    NameTagsEnabled = Settings.NameTagsEnabled,
                    CornerESPEnabled = Settings.CornerESPEnabled
                };

                string json = JsonUtility.ToJson(config, true);
                File.WriteAllText(ConfigPath, json);
                LastSaveTime = DateTime.Now.ToString("HH:mm:ss");
                Debug.Log("[CompMenu] Config saved to: " + ConfigPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("[CompMenu] Failed to save config: " + ex.Message);
            }
        }

        public static void LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    Debug.Log("[CompMenu] No config file found at: " + ConfigPath);
                    return;
                }

                string json = File.ReadAllText(ConfigPath);
                ConfigData config = JsonUtility.FromJson<ConfigData>(json);

                if (config == null)
                {
                    Debug.LogError("[CompMenu] Failed to parse config file");
                    return;
                }

                // Theme
                Settings.CurrentTheme = (ThemeOption)config.CurrentTheme;

                // Pull Mod
                Settings.PullModEnabled = config.PullModEnabled;
                Settings.PullStrength = config.PullStrength;
                Settings.PullThreshold = config.PullThreshold;
                Settings.PullTpTime = config.PullTpTime;
                Settings.PullActivation = (ActivationMode)config.PullActivation;
                Settings.PullObstacleDetection = config.PullObstacleDetection;
                Settings.PullObstacleRadius = config.PullObstacleRadius;
                Settings.WallPullEnabled = config.WallPullEnabled;
                Settings.UseD1vineTagFreeze = config.UseD1vineTagFreeze;
                Settings.D1vineTagFreezeMultiplier = config.D1vineTagFreezeMultiplier;

                // Wall Walk
                Settings.WallWalkEnabled = config.WallWalkEnabled;
                Settings.WallWalkDistance = config.WallWalkDistance;
                Settings.WallWalkPower = config.WallWalkPower;
                Settings.WallWalkLeftHand = config.WallWalkLeftHand;
                Settings.WallWalkRightHand = config.WallWalkRightHand;

                // Tag Mod
                Settings.DCFlickEnabled = config.DCFlickEnabled;
                Settings.TagAuraEnabled = config.TagAuraEnabled;
                Settings.TagAuraDistance = config.TagAuraDistance;
                Settings.HitboxExpanderEnabled = config.HitboxExpanderEnabled;
                Settings.HitboxExpanderSize = config.HitboxExpanderSize;
                Settings.HitboxExpanderOpacity = config.HitboxExpanderOpacity;

                // PSA
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

                // SpeedBoost
                Settings.PidegoEnabled = config.PidegoEnabled;
                Settings.PidegoGroundEnabled = config.PidegoGroundEnabled;
                Settings.PidegoGroundMultiplier = config.PidegoGroundMultiplier;
                Settings.PidegoWallEnabled = config.PidegoWallEnabled;
                Settings.PidegoWallMultiplier = config.PidegoWallMultiplier;
                Settings.PidegoDoubleWallEnabled = config.PidegoDoubleWallEnabled;
                Settings.PidegoDoubleWallMultiplier = config.PidegoDoubleWallMultiplier;
                Settings.PidegoSlipEnabled = config.PidegoSlipEnabled;
                Settings.PidegoSlipMultiplier = config.PidegoSlipMultiplier;
                Settings.VelmaxEnabled = config.VelmaxEnabled;
                Settings.VelmaxMultiplier = config.VelmaxMultiplier;

                // Bypass
                Settings.WorldScaleBypassEnabled = config.WorldScaleBypassEnabled;
                Settings.WorldScaleBypassActivation = (ActivationMode)config.WorldScaleBypassActivation;
                Settings.VelmaxCheckEnabled = config.VelmaxCheckEnabled;
                Settings.VelmaxCheckActivation = (ActivationMode)config.VelmaxCheckActivation;
                Settings.ForceTagFreezeEnabled = config.ForceTagFreezeEnabled;

                // Safety
                Settings.AntiReportEnabled = config.AntiReportEnabled;

                // Controller Mods
                Settings.DCModEnabled = config.DCModEnabled;
                Settings.DCModLeftDistance = config.DCModLeftDistance;
                Settings.DCModRightDistance = config.DCModRightDistance;
                Settings.DCModLeftThreshold = config.DCModLeftThreshold;
                Settings.DCModRightThreshold = config.DCModRightThreshold;
                Settings.DCModLeftDuration = config.DCModLeftDuration;
                Settings.DCModRightDuration = config.DCModRightDuration;
                Settings.DCModKeybind = (KeybindOption)config.DCModKeybind;
                Settings.PredsEnabled = config.PredsEnabled;
                Settings.PredsSmoothing = config.PredsSmoothing;

                // ESP
                Settings.ESPEnabled = config.ESPEnabled;
                Settings.ESPColorHue = config.ESPColorHue;
                Settings.ESPColorSat = config.ESPColorSat;
                Settings.ESPColorVal = config.ESPColorVal;
                Settings.TracersEnabled = config.TracersEnabled;
                Settings.HitboxesEnabled = config.HitboxesEnabled;
                Settings.NameTagsEnabled = config.NameTagsEnabled;
                Settings.CornerESPEnabled = config.CornerESPEnabled;

                LastLoadTime = DateTime.Now.ToString("HH:mm:ss");
                Debug.Log("[CompMenu] Config loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("[CompMenu] Failed to load config: " + ex.Message);
            }
        }

        public static bool ConfigExists()
        {
            return File.Exists(ConfigPath);
        }

        public static string GetConfigPath()
        {
            return ConfigPath;
        }
    }
}

