using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class ControllerMods
    {
        private static Vector3 lastLeft;
        private static Vector3 lastRight;
        private static Vector3 velLeft;
        private static Vector3 velRight;

        public static void Execute()
        {
            ExecutePreds();
        }

        public static void PredController(float amount)
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f) return;

            GTPlayer player = GTPlayer.Instance;
            if (player == null) return;

            Vector3 position = player.LeftHand.controllerTransform.position;
            Vector3 position2 = player.RightHand.controllerTransform.position;
            
            Vector3 vector = (position - lastLeft) / deltaTime;
            Vector3 vector2 = (position2 - lastRight) / deltaTime;
            
            velLeft = Vector3.Lerp(velLeft, vector, 0.2f);
            velRight = Vector3.Lerp(velRight, vector2, 0.2f);
            
            // Apply based on hand selection
            bool applyLeft = Settings.PredsHand == PredsHandOption.Both || Settings.PredsHand == PredsHandOption.LeftOnly;
            bool applyRight = Settings.PredsHand == PredsHandOption.Both || Settings.PredsHand == PredsHandOption.RightOnly;
            
            if (applyLeft)
                player.LeftHand.controllerTransform.position = position + velLeft * amount * deltaTime;
            if (applyRight)
                player.RightHand.controllerTransform.position = position2 + velRight * amount * deltaTime;
            
            lastLeft = position;
            lastRight = position2;
        }

        private static void ExecutePreds()
        {
            try
            {
                if (Settings.PredsAlwaysOn)
                {
                    PredController(Settings.PredsAlwaysAmount);
                }
                else if (Settings.PredsEnabled)
                {
                    PredController(Settings.PredsAmount);
                }
            }
            catch { }
        }
    }
}
