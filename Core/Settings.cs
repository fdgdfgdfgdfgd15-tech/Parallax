using System;

namespace CompMenu.Core
{
    public enum ActivationMode
    {
        Always,
        LeftGrip,
        RightGrip,
        BothGrip,
        LeftTrigger,
        RightTrigger,
        BothTrigger,
        LeftPrimary,
        RightPrimary,
        BothPrimary,
        LeftSecondary,
        RightSecondary,
        BothSecondary,
        LeftJoystickClick,
        RightJoystickClick,
        BothJoystickClick
    }

    public enum KeybindOption
    {
        RightTrigger = 1,
        LeftTrigger = 2,
        RightGrip = 3,
        LeftGrip = 4,
        RightSecondary = 5,
        LeftSecondary = 6,
        RightPrimary = 7,
        LeftPrimary = 8,
        RightJoystickClick = 9,
        LeftJoystickClick = 10
    }

    public enum ThemeOption
    {
        Starry,
        Parallax,
        Ocean,
        Midnight
    }

    public enum PredsHandOption
    {
        Both,
        LeftOnly,
        RightOnly
    }

    public static class Settings
    {
        public static ThemeOption CurrentTheme = ThemeOption.Starry;
        public static bool RoundedCorners = true;

        public static bool PullModEnabled = true;
        public static float PullStrength = 1f;
        public static float PullThreshold = 0f;
        public static float PullTpTime = 0.013f;
        public static ActivationMode PullActivation = ActivationMode.LeftJoystickClick;
        public static bool PullAntiCheatBypass = false;
        public static bool PullObstacleDetection = false;
        public static float PullObstacleRadius = 0.5f;
        public static bool WallPullEnabled = false;
        public static bool TagFreezeTpEnabled = false;

        public static bool WallWalkEnabled = false;
        public static float WallWalkDistance = 0.5f;
        public static float WallWalkPower = 2f;
        public static bool WallWalkLeftHand = true;
        public static bool WallWalkRightHand = true;

        public static bool DCFlickEnabled = false;
        public static bool TagAuraEnabled = false;
        public static float TagAuraDistance = 1f;
        public static bool HitboxExpanderEnabled = false;
        public static float HitboxExpanderSize = 0.5f;
        public static float HitboxExpanderOpacity = 0.5f;

        public static bool PSAEnabled = false;
        public static float PSASpeed = 1f;
        public static KeybindOption PSAKeybind = KeybindOption.RightTrigger;
        public static bool HighJumpEnabled = false;
        public static float HighJumpSpeed = 5f;
        public static KeybindOption HighJumpKeybind = KeybindOption.RightPrimary;
        public static bool RecRoomEnabled = false;
        public static float RecRoomSpeed = 1f;
        public static KeybindOption RecRoomForwardKeybind = KeybindOption.RightSecondary;
        public static KeybindOption RecRoomBackwardKeybind = KeybindOption.LeftSecondary;

        public static bool VelmaxEnabled = false;
        public static float VelmaxMultiplier = 1.2f;

        public static int CurrentTimeOfDay = 0;
        public static int CurrentWeather = 0;

        public static bool ForceTagFreezeEnabled = false;

        public static bool HzSliderEnabled = false;
        public static int TargetHz = 72;

        public static bool NoSlipEnabled = false;
        
        public static bool VelmaxBypassEnabled = false;
        public static KeybindOption VelmaxBypassKeybind = KeybindOption.LeftGrip;

        public static bool PredsEnabled = false;
        public static float PredsAmount = 0f;
        public static float PredsLegit = 4.6f;
        public static float PredsValve = 6f;
        public static float PredsRiftS = 9.5f;
        public static float PredsPico = 12f;
        public static bool PredsAlwaysOn = false;
        public static float PredsAlwaysAmount = 20f;
        public static PredsHandOption PredsHand = PredsHandOption.Both;

        public static bool ESPEnabled = false;
        public static float ESPColorHue = 0f;
        public static float ESPColorSat = 1f;
        public static float ESPColorVal = 1f;
        public static bool TracersEnabled = false;
        public static bool HitboxesEnabled = false;
        public static bool NameTagsEnabled = false;
        public static bool CornerESPEnabled = false;

        public static bool ShowMenu = true;
        public static int CurrentTab = 0;

        public static bool GetActivation(ActivationMode mode)
        {
            if (mode == ActivationMode.Always) return true;
            
            try
            {
                var poller = ControllerInputPoller.instance;
                if (poller != null)
                {
                    switch (mode)
                    {
                        case ActivationMode.LeftGrip: return poller.leftGrab;
                        case ActivationMode.RightGrip: return poller.rightGrab;
                        case ActivationMode.BothGrip: return poller.leftGrab && poller.rightGrab;
                        case ActivationMode.LeftTrigger: return poller.leftControllerIndexFloat > 0.7f;
                        case ActivationMode.RightTrigger: return poller.rightControllerIndexFloat > 0.7f;
                        case ActivationMode.BothTrigger: return poller.leftControllerIndexFloat > 0.7f && poller.rightControllerIndexFloat > 0.7f;
                        case ActivationMode.LeftPrimary: return poller.leftControllerPrimaryButton;
                        case ActivationMode.RightPrimary: return poller.rightControllerPrimaryButton;
                        case ActivationMode.BothPrimary: return poller.leftControllerPrimaryButton && poller.rightControllerPrimaryButton;
                        case ActivationMode.LeftSecondary: return poller.leftControllerSecondaryButton;
                        case ActivationMode.RightSecondary: return poller.rightControllerSecondaryButton;
                        case ActivationMode.BothSecondary: return poller.leftControllerSecondaryButton && poller.rightControllerSecondaryButton;
                        case ActivationMode.LeftJoystickClick: return InputHandler.LeftJoystickClick();
                        case ActivationMode.RightJoystickClick: return InputHandler.RightJoystickClick();
                        case ActivationMode.BothJoystickClick: return InputHandler.LeftJoystickClick() && InputHandler.RightJoystickClick();
                    }
                }
            }
            catch { }
            
            switch (mode)
            {
                case ActivationMode.LeftGrip: return InputHandler.LeftGrip();
                case ActivationMode.RightGrip: return InputHandler.RightGrip();
                case ActivationMode.BothGrip: return InputHandler.LeftGrip() && InputHandler.RightGrip();
                case ActivationMode.LeftTrigger: return InputHandler.LeftTrigger();
                case ActivationMode.RightTrigger: return InputHandler.RightTrigger();
                case ActivationMode.BothTrigger: return InputHandler.LeftTrigger() && InputHandler.RightTrigger();
                case ActivationMode.LeftPrimary: return InputHandler.LeftPrimary();
                case ActivationMode.RightPrimary: return InputHandler.RightPrimary();
                case ActivationMode.BothPrimary: return InputHandler.LeftPrimary() && InputHandler.RightPrimary();
                case ActivationMode.LeftSecondary: return InputHandler.LeftSecondary();
                case ActivationMode.RightSecondary: return InputHandler.RightSecondary();
                case ActivationMode.BothSecondary: return InputHandler.LeftSecondary() && InputHandler.RightSecondary();
                case ActivationMode.LeftJoystickClick: return InputHandler.LeftJoystickClick();
                case ActivationMode.RightJoystickClick: return InputHandler.RightJoystickClick();
                case ActivationMode.BothJoystickClick: return InputHandler.LeftJoystickClick() && InputHandler.RightJoystickClick();
                default: return false;
            }
        }

        public static bool GetKeybind(KeybindOption keybind)
        {
            try
            {
                var poller = ControllerInputPoller.instance;
                if (poller != null)
                {
                    switch (keybind)
                    {
                        case KeybindOption.RightTrigger:
                            return poller.rightControllerIndexFloat > 0.7f;
                        case KeybindOption.LeftTrigger:
                            return poller.leftControllerIndexFloat > 0.7f;
                        case KeybindOption.RightGrip:
                            return poller.rightGrab;
                        case KeybindOption.LeftGrip:
                            return poller.leftGrab;
                        case KeybindOption.RightPrimary:
                            return poller.rightControllerPrimaryButton;
                        case KeybindOption.LeftPrimary:
                            return poller.leftControllerPrimaryButton;
                        case KeybindOption.RightSecondary:
                            return poller.rightControllerSecondaryButton;
                        case KeybindOption.LeftSecondary:
                            return poller.leftControllerSecondaryButton;
                        case KeybindOption.RightJoystickClick:
                            return InputHandler.RightJoystickClick();
                        case KeybindOption.LeftJoystickClick:
                            return InputHandler.LeftJoystickClick();
                    }
                }
            }
            catch { }
            
            switch (keybind)
            {
                case KeybindOption.RightTrigger: return InputHandler.RightTrigger();
                case KeybindOption.LeftTrigger: return InputHandler.LeftTrigger();
                case KeybindOption.RightGrip: return InputHandler.RightGrip();
                case KeybindOption.LeftGrip: return InputHandler.LeftGrip();
                case KeybindOption.RightSecondary: return InputHandler.RightSecondary();
                case KeybindOption.LeftSecondary: return InputHandler.LeftSecondary();
                case KeybindOption.RightPrimary: return InputHandler.RightPrimary();
                case KeybindOption.LeftPrimary: return InputHandler.LeftPrimary();
                case KeybindOption.RightJoystickClick: return InputHandler.RightJoystickClick();
                case KeybindOption.LeftJoystickClick: return InputHandler.LeftJoystickClick();
                default: return false;
            }
        }

        public static string GetKeybindName(KeybindOption keybind)
        {
            switch (keybind)
            {
                case KeybindOption.RightTrigger: return "R Trigger";
                case KeybindOption.LeftTrigger: return "L Trigger";
                case KeybindOption.RightGrip: return "R Grip";
                case KeybindOption.LeftGrip: return "L Grip";
                case KeybindOption.RightSecondary: return "R Secondary";
                case KeybindOption.LeftSecondary: return "L Secondary";
                case KeybindOption.RightPrimary: return "R Primary";
                case KeybindOption.LeftPrimary: return "L Primary";
                case KeybindOption.RightJoystickClick: return "R Joystick";
                case KeybindOption.LeftJoystickClick: return "L Joystick";
                default: return "Unknown";
            }
        }

    }
}
