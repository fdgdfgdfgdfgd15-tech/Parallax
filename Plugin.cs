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
        private bool hasAutoLoaded = false;
        
        private void Awake()
        {
            DetectVRPlatform();
            Logger.LogInfo("Parallax v2.0 loaded! Initializing auth...");
        }

        private void Start()
        {
            try
            {
                AuthManager.Initialize();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[Parallax] Auth init error: " + ex.Message);
                AuthManager.InitFailed = true;
                AuthManager.StatusMessage = "Error: " + ex.Message;
            }
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
            if (!AuthManager.IsAuthenticated) return;
            
            if (!hasAutoLoaded)
            {
                hasAutoLoaded = true;
                ConfigManager.TryAutoLoad();
            }

            if (Keyboard.current != null)
            {
                if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || 
                    Keyboard.current.insertKey.wasPressedThisFrame)
                {
                    Settings.ShowMenu = !Settings.ShowMenu;
                }
            }

            if (Settings.WallWalkEnabled) WallWalk.Execute();
            
            if (Settings.DCFlickEnabled) Tag.DCFlick();
            if (Settings.TagAuraEnabled) Tag.TagAura();
            Tag.HitboxExpanderMethod(); // Always call so cleanup can run when disabled
            
            if (Settings.PSAEnabled) PSAMods.PSA();
            if (Settings.HighJumpEnabled) PSAMods.HighJump();
            if (Settings.RecRoomEnabled) PSAMods.RecRoom();
            
            if (SpeedBoostMods.NeedsUpdate) SpeedBoostMods.Execute();
            
            
            if (Settings.PredsEnabled || Settings.PredsAlwaysOn) ControllerMods.Execute();
            
            if (Settings.ESPEnabled) Visuals.Execute();
        }

        private void FixedUpdate()
        {
        }

        private void LateUpdate()
        {
            if (!AuthManager.IsAuthenticated) return;

            if (Settings.PullModEnabled) PullMod.Execute();
            BypassMods.Execute();
        }

        private void OnGUI()
        {
            try
            {
                if (!AuthManager.IsAuthenticated)
                {
                    LoginGUI.Draw();
                }
                else
                {
                    MenuGUI.Draw();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Parallax] GUI Error: " + ex.Message);
            }
        }
    }
}
