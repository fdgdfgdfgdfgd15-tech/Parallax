using CompMenu.Core;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class SpeedBoostMods
    {
        private static bool pidegoApplied = false;
        private static bool velmaxApplied = false;
        private static GorillaSurfaceOverride[] cachedSurfaces = null;
        private static float lastSurfaceCheck = -999f;
        private static bool surfacesNeedUpdate = true;

        public static void Execute()
        {
            // Only check for surfaces once every 30 seconds or when first enabled
            if (surfacesNeedUpdate || (cachedSurfaces == null && (Settings.PidegoEnabled || Settings.VelmaxEnabled)))
            {
                if (Time.time - lastSurfaceCheck > 30f)
                {
                    cachedSurfaces = Object.FindObjectsOfType<GorillaSurfaceOverride>();
                    lastSurfaceCheck = Time.time;
                    surfacesNeedUpdate = false;
                }
            }

            ExecutePidego();
            ExecuteVelmax();
        }

        private static void ExecutePidego()
        {
            if (!Settings.PidegoEnabled)
            {
                if (pidegoApplied) ResetPidego();
                return;
            }

            if (cachedSurfaces == null) return;

            // Only apply once, not every frame
            if (pidegoApplied) return;
            pidegoApplied = true;

            foreach (GorillaSurfaceOverride surface in cachedSurfaces)
            {
                if (surface == null) continue;
                
                string name = surface.gameObject.name.ToLower();
                float multiplier = 1f;
                bool shouldApply = false;

                if (Settings.PidegoGroundEnabled && (name.Contains("ground") || name.Contains("floor")))
                {
                    multiplier = Settings.PidegoGroundMultiplier;
                    shouldApply = true;
                }
                else if (Settings.PidegoWallEnabled && name.Contains("wall") && !name.Contains("vertical") && !name.Contains("double"))
                {
                    multiplier = Settings.PidegoWallMultiplier;
                    shouldApply = true;
                }
                else if (Settings.PidegoDoubleWallEnabled && (name.Contains("vertical") || name.Contains("double")))
                {
                    multiplier = Settings.PidegoDoubleWallMultiplier;
                    shouldApply = true;
                }
                else if (Settings.PidegoSlipEnabled && (name.Contains("slip") || name.Contains("ice") || name.Contains("slippery")))
                {
                    multiplier = Settings.PidegoSlipMultiplier;
                    shouldApply = true;
                }

                if (shouldApply)
                {
                    surface.extraVelMaxMultiplier = multiplier;
                    surface.extraVelMultiplier = multiplier;
                }
            }
        }

        private static void ResetPidego()
        {
            pidegoApplied = false;
            
            if (cachedSurfaces == null) return;

            foreach (GorillaSurfaceOverride surface in cachedSurfaces)
            {
                if (surface == null) continue;
                surface.extraVelMaxMultiplier = 1f;
                surface.extraVelMultiplier = 1f;
            }
        }

        private static void ExecuteVelmax()
        {
            if (!Settings.VelmaxEnabled)
            {
                if (velmaxApplied) ResetVelmax();
                return;
            }

            if (cachedSurfaces == null) return;

            // Only apply once, not every frame
            if (velmaxApplied) return;
            velmaxApplied = true;

            foreach (GorillaSurfaceOverride surface in cachedSurfaces)
            {
                if (surface != null)
                {
                    surface.extraVelMaxMultiplier = Settings.VelmaxMultiplier;
                    surface.extraVelMultiplier = Settings.VelmaxMultiplier;
                }
            }
        }

        private static void ResetVelmax()
        {
            velmaxApplied = false;
            
            if (cachedSurfaces == null) return;

            foreach (GorillaSurfaceOverride surface in cachedSurfaces)
            {
                if (surface != null)
                {
                    surface.extraVelMaxMultiplier = 1f;
                    surface.extraVelMultiplier = 1f;
                }
            }
        }

        // Call this when changing maps or when you want to refresh surfaces
        public static void RefreshSurfaces()
        {
            surfacesNeedUpdate = true;
            cachedSurfaces = null;
        }
    }
}
