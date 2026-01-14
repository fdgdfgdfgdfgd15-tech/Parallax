using System;
using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class PSAMods
    {
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

        public static void RecRoom()
        {
            if (!Settings.RecRoomEnabled) return;
            
            try
            {
                if (GTPlayer.Instance == null) return;
                
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
