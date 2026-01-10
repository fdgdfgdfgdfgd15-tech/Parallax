using System;
using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class PSAMods
    {
        // PSA - Move forward when keybind is held
        public static void PSA()
        {
            if (!Settings.PSAEnabled) return;
            
            try
            {
                if (GTPlayer.Instance == null) return;
                
                bool isHeld = Settings.GetKeybind(Settings.PSAKeybind);
                
                if (isHeld)
                {
                    GTPlayer.Instance.transform.position += 
                        GTPlayer.Instance.bodyCollider.transform.forward * 
                        Settings.PSASpeed * Time.deltaTime * GTPlayer.Instance.scale;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] PSA Error: " + ex.Message);
            }
        }

        // High Jump - Move upward when keybind is held
        public static void HighJump()
        {
            if (!Settings.HighJumpEnabled) return;
            
            try
            {
                if (GTPlayer.Instance == null) return;
                
                bool isHeld = Settings.GetKeybind(Settings.HighJumpKeybind);
                
                if (isHeld)
                {
                    GTPlayer.Instance.transform.position += 
                        GTPlayer.Instance.bodyCollider.transform.up * 
                        Settings.HighJumpSpeed * Time.deltaTime;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] HighJump Error: " + ex.Message);
            }
        }

        // Rec Room - Move forward+right or backward+left with different keybinds
        public static void RecRoom()
        {
            if (!Settings.RecRoomEnabled) return;
            
            try
            {
                if (GTPlayer.Instance == null) return;
                
                // Forward keybind - move forward and right
                bool forwardHeld = Settings.GetKeybind(Settings.RecRoomForwardKeybind);
                if (forwardHeld)
                {
                    GTPlayer.Instance.transform.position += 
                        GTPlayer.Instance.bodyCollider.transform.forward * 
                        Settings.RecRoomSpeed * Time.deltaTime * GTPlayer.Instance.scale;
                    
                    GTPlayer.Instance.transform.position += 
                        GTPlayer.Instance.bodyCollider.transform.right * 
                        Settings.RecRoomSpeed * Time.deltaTime * GTPlayer.Instance.scale;
                }
                
                // Backward keybind - move backward and left
                bool backwardHeld = Settings.GetKeybind(Settings.RecRoomBackwardKeybind);
                if (backwardHeld)
                {
                    GTPlayer.Instance.transform.position += 
                        GTPlayer.Instance.bodyCollider.transform.forward * 
                        -Settings.RecRoomSpeed * Time.deltaTime * GTPlayer.Instance.scale;
                    
                    GTPlayer.Instance.transform.position += 
                        GTPlayer.Instance.bodyCollider.transform.right * 
                        -Settings.RecRoomSpeed * Time.deltaTime * GTPlayer.Instance.scale;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] RecRoom Error: " + ex.Message);
            }
        }
    }
}


