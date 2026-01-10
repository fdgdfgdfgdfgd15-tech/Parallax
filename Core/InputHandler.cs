using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

namespace CompMenu.Core
{
    public static class InputHandler
    {
        public static bool IsOculus { get; set; } = false;

        public static bool RightJoystickClick()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
            }
            catch { return false; }
        }

        public static bool LeftJoystickClick()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
            }
            catch { return false; }
        }

        public static bool RightTrigger()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_RightTriggerClick.GetState(SteamVR_Input_Sources.RightHand);
            }
            catch { return false; }
        }

        public static bool LeftTrigger()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_LeftTriggerClick.GetState(SteamVR_Input_Sources.LeftHand);
            }
            catch { return false; }
        }

        public static bool RightGrip()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.gripButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_RightGripClick.GetState(SteamVR_Input_Sources.RightHand);
            }
            catch { return false; }
        }

        public static bool LeftGrip()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.gripButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_LeftGripClick.GetState(SteamVR_Input_Sources.LeftHand);
            }
            catch { return false; }
        }

        public static bool RightPrimary()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
            }
            catch { return false; }
        }

        public static bool LeftPrimary()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
            }
            catch { return false; }
        }

        public static bool RightSecondary()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
            }
            catch { return false; }
        }

        public static bool LeftSecondary()
        {
            try
            {
                if (IsOculus)
                {
                    bool state = false;
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.secondaryButton, out state);
                    return state;
                }
                return SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
            }
            catch { return false; }
        }

        public static Vector2 RightJoystickAxis()
        {
            try
            {
                if (IsOculus)
                {
                    Vector2 axis = Vector2.zero;
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);
                    return axis;
                }
                return SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
            }
            catch { return Vector2.zero; }
        }

        public static Vector2 LeftJoystickAxis()
        {
            try
            {
                if (IsOculus)
                {
                    Vector2 axis = Vector2.zero;
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);
                    return axis;
                }
                return SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
            }
            catch { return Vector2.zero; }
        }
        
        // Alternative input method using ControllerInputPoller (game's built-in input)
        public static bool GetButtonFromPoller(string buttonName, bool leftHand)
        {
            try
            {
                var poller = ControllerInputPoller.instance;
                if (poller == null) return false;
                
                switch (buttonName.ToLower())
                {
                    case "grip":
                        return leftHand ? poller.leftGrab : poller.rightGrab;
                    case "trigger":
                        return leftHand ? poller.leftControllerIndexFloat > 0.5f : poller.rightControllerIndexFloat > 0.5f;
                    case "primary":
                        return leftHand ? poller.leftControllerPrimaryButton : poller.rightControllerPrimaryButton;
                    case "secondary":
                        return leftHand ? poller.leftControllerSecondaryButton : poller.rightControllerSecondaryButton;
                }
            }
            catch { }
            return false;
        }
    }
}

