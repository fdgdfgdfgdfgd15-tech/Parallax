using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class BypassMods
    {
        private static bool wasForceTagFreezeEnabled = false;
        private static bool wasVelmaxBypassActive = false;
        private static bool noSlipApplied = false;
        
        public static void Execute()
        {
            ForceTagFreeze();
            NoSlip();
            VelmaxBypass();
        }

        private static bool GetKeybindInput(KeybindOption keybind)
        {
            switch (keybind)
            {
                case KeybindOption.RightTrigger: return InputHandler.RightTrigger();
                case KeybindOption.LeftTrigger: return InputHandler.LeftTrigger();
                case KeybindOption.RightGrip: return InputHandler.RightGrip();
                case KeybindOption.LeftGrip: return InputHandler.LeftGrip();
                case KeybindOption.RightPrimary: return InputHandler.RightPrimary();
                case KeybindOption.LeftPrimary: return InputHandler.LeftPrimary();
                case KeybindOption.RightSecondary: return InputHandler.RightSecondary();
                case KeybindOption.LeftSecondary: return InputHandler.LeftSecondary();
                case KeybindOption.RightJoystickClick: return InputHandler.RightJoystickClick();
                case KeybindOption.LeftJoystickClick: return InputHandler.LeftJoystickClick();
                default: return false;
            }
        }

        public static void ForceTagFreeze()
        {
            try
            {
                GTPlayer player = GTPlayer.Instance;
                if (player == null) return;
                
                if (Settings.ForceTagFreezeEnabled)
                {
                    player.disableMovement = true;
                    wasForceTagFreezeEnabled = true;
                }
                else if (wasForceTagFreezeEnabled)
                {
                    player.disableMovement = false;
                    wasForceTagFreezeEnabled = false;
                }
            }
            catch { }
        }

        public static void NoSlip()
        {
            try
            {
                if (!Settings.NoSlipEnabled)
                {
                    noSlipApplied = false;
                    return;
                }
                
                if (noSlipApplied) return;
                
                GameObject lowerWall = GameObject.Find("pit lower slippery wall");
                GameObject upperWall = GameObject.Find("pit upper slippery wall");
                
                if (lowerWall != null)
                {
                    GorillaSurfaceOverride surface = lowerWall.GetComponent<GorillaSurfaceOverride>();
                    if (surface != null)
                    {
                        surface.slidePercentageOverride = 0f;
                        surface.overrideIndex = 0;
                    }
                }
                
                if (upperWall != null)
                {
                    GorillaSurfaceOverride surface = upperWall.GetComponent<GorillaSurfaceOverride>();
                    if (surface != null)
                    {
                        surface.slidePercentageOverride = 0f;
                        surface.overrideIndex = 0;
                    }
                }
                
                noSlipApplied = true;
            }
            catch { }
        }

        public static void VelmaxBypass()
        {
            try
            {
                GTPlayer player = GTPlayer.Instance;
                if (player == null) return;
                
                if (Settings.VelmaxBypassEnabled && GetKeybindInput(Settings.VelmaxBypassKeybind))
                {
                    player.disableMovement = true;
                    wasVelmaxBypassActive = true;
                }
                else if (wasVelmaxBypassActive)
                {
                    player.disableMovement = false;
                    wasVelmaxBypassActive = false;
                }
            }
            catch { }
        }
    }
}
