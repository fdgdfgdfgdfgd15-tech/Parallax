using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class ControllerMods
    {
        // DC Mod state
        private static Vector3 bodyDelta, leftDelta, rightDelta;
        private static Vector3 lStartPos, lTargetPos, rStartPos, rTargetPos;
        private static Quaternion lStartRot, lTargetRot, rStartRot, rTargetRot;
        private static float lElapsed, rElapsed;
        private static bool lLerping, rLerping;
        private static float originalMaxArmLength = -1f;
        private static bool wasArmLengthModified = false;

        // Preds state
        private static Quaternion smoothedRotLeft = Quaternion.identity;
        private static Quaternion smoothedRotRight = Quaternion.identity;

        public static void Execute()
        {
            ExecuteDCMod();
            ExecutePreds();
        }

        private static void ExecuteDCMod()
        {
            try
            {
                GTPlayer player = GTPlayer.Instance;
                if (player == null) return;

                // Store original arm length on first run
                if (originalMaxArmLength < 0f)
                {
                    originalMaxArmLength = player.maxArmLength;
                }

                // If DCMod disabled or keybind not held, restore original arm length
                if (!Settings.DCModEnabled || !Settings.GetKeybind(Settings.DCModKeybind))
                {
                    if (wasArmLengthModified)
                    {
                        player.maxArmLength = originalMaxArmLength;
                        wasArmLengthModified = false;
                    }
                    return;
                }

                player.maxArmLength = float.MaxValue;
                wasArmLengthModified = true;

                Transform body = player.bodyCollider.transform;
                GTPlayer.HandState leftHand = player.LeftHand;
                GTPlayer.HandState rightHand = player.RightHand;

                Vector3 vel = body.position - bodyDelta;
                bodyDelta = body.position;

                Vector3 lv = leftHand.controllerTransform.position - leftDelta - vel;
                leftDelta = leftHand.controllerTransform.position;

                Vector3 rv = rightHand.controllerTransform.position - rightDelta - vel;
                rightDelta = rightHand.controllerTransform.position;

                // Left hand lerp
                if (lLerping && Settings.DCModLeftDuration > 0)
                {
                    lElapsed += Time.deltaTime;
                    float duration = Settings.DCModLeftDuration * Time.deltaTime;
                    float t = Mathf.Clamp01(lElapsed / duration);

                    leftHand.controllerTransform.position = Vector3.Lerp(lStartPos, lTargetPos, t);
                    leftHand.controllerTransform.rotation = Quaternion.Slerp(lStartRot, lTargetRot, t);

                    if (t >= 1f) lLerping = false;
                }

                // Right hand lerp
                if (rLerping && Settings.DCModRightDuration > 0)
                {
                    rElapsed += Time.deltaTime;
                    float duration = Settings.DCModRightDuration * Time.deltaTime;
                    float t = Mathf.Clamp01(rElapsed / duration);

                    rightHand.controllerTransform.position = Vector3.Lerp(rStartPos, rTargetPos, t);
                    rightHand.controllerTransform.rotation = Quaternion.Slerp(rStartRot, rTargetRot, t);

                    if (t >= 1f) rLerping = false;
                }
            }
            catch { }
        }

        private static void ExecutePreds()
        {
            if (!Settings.PredsEnabled) return;

            try
            {
                GTPlayer player = GTPlayer.Instance;
                if (player == null) return;

                Transform xrOrigin = player.headCollider.transform.parent;
                if (xrOrigin == null) return;

                Quaternion rigRotation = xrOrigin.rotation;
                Quaternion rigRotationInverse = Quaternion.Inverse(rigRotation);

                Quaternion currentLeftRot = player.LeftHand.controllerTransform.rotation;
                Quaternion currentRightRot = player.RightHand.controllerTransform.rotation;

                Quaternion localLeftRot = rigRotationInverse * currentLeftRot;
                Quaternion localRightRot = rigRotationInverse * currentRightRot;

                smoothedRotLeft = Quaternion.Slerp(smoothedRotLeft, localLeftRot, Settings.PredsSmoothing);
                smoothedRotRight = Quaternion.Slerp(smoothedRotRight, localRightRot, Settings.PredsSmoothing);

                player.LeftHand.controllerTransform.rotation = rigRotation * smoothedRotLeft;
                player.RightHand.controllerTransform.rotation = rigRotation * smoothedRotRight;
            }
            catch { }
        }
    }
}

