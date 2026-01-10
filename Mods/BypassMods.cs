using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class BypassMods
    {
        private static bool wasForceTagFreezeEnabled = false;
        
        // Always call this - handles cleanup even when mods are disabled
        public static void Execute()
        {
            WorldScaleBypass();
            VelmaxCheckBypass();
            ForceTagFreeze();
        }

        public static void WorldScaleBypass()
        {
            if (!Settings.WorldScaleBypassEnabled) return;
            
            // Use GetActivation (same as PullMod) - more reliable
            bool keybindHeld = Settings.GetActivation(Settings.WorldScaleBypassActivation);
            if (!keybindHeld) return;

            try
            {
                GTPlayer player = GTPlayer.Instance;
                if (player == null) return;
                if (player.bodyCollider == null) return;

                // Move hands close to body to bypass world scale check
                Vector3 bodyPos = player.bodyCollider.transform.position;
                
                if (player.LeftHand.controllerTransform != null)
                    player.LeftHand.controllerTransform.position = bodyPos + new Vector3(-0.15f, 0f, 0.1f);
                if (player.RightHand.controllerTransform != null)
                    player.RightHand.controllerTransform.position = bodyPos + new Vector3(0.15f, 0f, 0.1f);
            }
            catch { }
        }

        public static void VelmaxCheckBypass()
        {
            if (!Settings.VelmaxCheckEnabled) return;
            
            // Use GetActivation (same as PullMod) - more reliable
            bool keybindHeld = Settings.GetActivation(Settings.VelmaxCheckActivation);
            if (!keybindHeld) return;

            try
            {
                GTPlayer player = GTPlayer.Instance;
                if (player == null) return;
                
                // Reset movement state to bypass velmax checks
                player.disableMovement = false;
            }
            catch { }
        }

        public static void ForceTagFreeze()
        {
            try
            {
                GTPlayer player = GTPlayer.Instance;
                if (player == null) return;
                
                if (Settings.ForceTagFreezeEnabled)
                {
                    // Force tag freeze state (locks movement)
                    player.disableMovement = true;
                    wasForceTagFreezeEnabled = true;
                }
                else if (wasForceTagFreezeEnabled)
                {
                    // When disabled, IMMEDIATELY restore movement
                    player.disableMovement = false;
                    wasForceTagFreezeEnabled = false;
                }
            }
            catch { }
        }
    }
}
