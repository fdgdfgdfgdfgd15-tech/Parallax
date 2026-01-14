using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class SpeedBoostMods
    {
        private static bool velmaxApplied = false;
        private static GorillaSurfaceOverride[] cachedSurfaces = null;
        private static float lastSurfaceCheck = -999f;
        private static bool surfacesNeedUpdate = true;
        
        private static float originalMaxJumpSpeed = -1f;
        private static float originalJumpMultiplier = -1f;

        public static bool NeedsUpdate => velmaxApplied || Settings.VelmaxEnabled;

        public static void Execute()
        {
            if (surfacesNeedUpdate || (cachedSurfaces == null && Settings.VelmaxEnabled))
            {
                if (Time.time - lastSurfaceCheck > 30f)
                {
                    cachedSurfaces = Object.FindObjectsOfType<GorillaSurfaceOverride>();
                    lastSurfaceCheck = Time.time;
                    surfacesNeedUpdate = false;
                }
            }

            ExecuteVelmax();
        }

        private static void ExecuteVelmax()
        {
            if (!Settings.VelmaxEnabled)
            {
                if (velmaxApplied) ResetVelmax();
                return;
            }

            GTPlayer player = GTPlayer.Instance;
            if (player == null) return;

            // Store original values on first apply
            if (originalMaxJumpSpeed < 0)
            {
                originalMaxJumpSpeed = player.maxJumpSpeed;
                originalJumpMultiplier = player.jumpMultiplier;
            }

            if (velmaxApplied) return;
            velmaxApplied = true;

            // Apply to player base settings (works everywhere)
            player.maxJumpSpeed = originalMaxJumpSpeed * Settings.VelmaxMultiplier;
            player.jumpMultiplier = originalJumpMultiplier * Settings.VelmaxMultiplier;

            // Also apply to surface overrides
            if (cachedSurfaces != null)
            {
                foreach (GorillaSurfaceOverride surface in cachedSurfaces)
                {
                    if (surface != null)
                    {
                        surface.extraVelMaxMultiplier = Settings.VelmaxMultiplier;
                        surface.extraVelMultiplier = Settings.VelmaxMultiplier;
                    }
                }
            }
        }

        private static void ResetVelmax()
        {
            velmaxApplied = false;
            
            GTPlayer player = GTPlayer.Instance;
            if (player != null && originalMaxJumpSpeed > 0)
            {
                player.maxJumpSpeed = originalMaxJumpSpeed;
                player.jumpMultiplier = originalJumpMultiplier;
            }

            if (cachedSurfaces != null)
            {
                foreach (GorillaSurfaceOverride surface in cachedSurfaces)
                {
                    if (surface != null)
                    {
                        surface.extraVelMaxMultiplier = 1f;
                        surface.extraVelMultiplier = 1f;
                    }
                }
            }
        }

        public static void RefreshSurfaces()
        {
            surfacesNeedUpdate = true;
            cachedSurfaces = null;
        }
    }
}
