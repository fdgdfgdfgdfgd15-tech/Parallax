using System;
using BepInEx;
using CompMenu.Core;
using CompMenu.GUI;
using CompMenu.Mods;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace CompMenu
{
    [BepInPlugin("com.parallax.mod", "Parallax", "2.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            DetectVRPlatform();
            Logger.LogInfo("Parallax v2.0 loaded! Press LEFT CTRL or INSERT to toggle menu.");
        }

        private void DetectVRPlatform()
        {
            try
            {
                string xrDevice = XRSettings.loadedDeviceName;
                InputHandler.IsOculus = xrDevice.ToLower().Contains("oculus") || 
                                        xrDevice.ToLower().Contains("meta");
            }
            catch
            {
                InputHandler.IsOculus = false;
            }
        }

        private void Update()
        {
            // Toggle menu with Left Ctrl or Insert
            if (Keyboard.current != null)
            {
                if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || 
                    Keyboard.current.insertKey.wasPressedThisFrame)
                {
                    Settings.ShowMenu = !Settings.ShowMenu;
                }
            }

            // OPTIMIZED: Only call mods that are enabled
            if (Settings.WallWalkEnabled) WallWalk.Execute();
            
            if (Settings.DCFlickEnabled) Tag.DCFlick();
            if (Settings.TagAuraEnabled) Tag.TagAura();
            if (Settings.HitboxExpanderEnabled) Tag.HitboxExpanderMethod();
            
            if (Settings.PSAEnabled) PSAMods.PSA();
            if (Settings.HighJumpEnabled) PSAMods.HighJump();
            if (Settings.RecRoomEnabled) PSAMods.RecRoom();
            
            if (Settings.PidegoEnabled || Settings.VelmaxEnabled) SpeedBoostMods.Execute();
            
            if (Settings.AntiReportEnabled) SafetyMods.Execute();
            
            if (Settings.DCModEnabled || Settings.PredsEnabled) ControllerMods.Execute();
            
            if (Settings.ESPEnabled) Visuals.Execute();
        }

        private void FixedUpdate()
        {
            // Reserved for physics-based mods
        }

        private void LateUpdate()
        {
            // OPTIMIZED: Only call if enabled
            if (Settings.PullModEnabled) PullMod.Execute();
            
            // ALWAYS call BypassMods - it handles cleanup internally
            BypassMods.Execute();
        }

        private void OnGUI()
        {
            try { MenuGUI.Draw(); }
            catch (Exception ex) { Debug.LogWarning("[Parallax] GUI Error: " + ex.Message); }
        }
    }
}


