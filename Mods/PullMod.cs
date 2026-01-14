using System;
using System.Reflection;
using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    internal class PullMod
    {
        public static bool leftHandWasColliding;
        public static bool rightHandWasColliding;
        
        public static float timer = 35f;
        
        private static PropertyInfo velocityProperty = null;
        private static bool velocityPropertyCached = false;
        private static Rigidbody cachedRigidbody = null;

        public static void Execute()
        {
            if (!Settings.PullModEnabled) return;

            GTPlayer instance = GTPlayer.Instance;
            if (instance == null) return;
            
            if (Settings.PullObstacleDetection && IsObstacleInFront(instance))
            {
                return;
            }

            timer += Time.deltaTime;
            float threshold = (instance.maxJumpSpeed - 0.01f) * Settings.PullThreshold;
            bool activation = Settings.GetActivation(Settings.PullActivation);
            
            if (activation)
            {
                bool leftColliding = instance.LeftHand.wasColliding;
                bool rightColliding = instance.RightHand.wasColliding;
                
                // Tag Freeze TP mode - always allow pulling when frozen
                bool tagFreezePullMode = Settings.TagFreezeTpEnabled && instance.disableMovement;
                
                if (tagFreezePullMode)
                {
                    // When tag frozen with TagFreezeTp enabled, continuously pull
                    timer = 0f;
                }
                else
                {
                    if (leftColliding || rightColliding)
                    {
                        timer = 0f;
                    }
                    
                    bool handJustReleased = (leftHandWasColliding && !leftColliding) || 
                                            (rightHandWasColliding && !rightColliding);
                    
                    if (handJustReleased)
                    {
                        Rigidbody rb = GetCachedRigidbody(instance);
                        if (rb != null)
                        {
                            Vector3 velocity = GetVelocityFast(rb);
                            bool velocityCheck = velocity.magnitude / instance.scale > threshold;
                            
                            if (velocityCheck)
                            {
                                timer = 0f;
                            }
                        }
                    }
                }
                
                float tpTime = Settings.PullTpTime;
                if (tagFreezePullMode)
                {
                    tpTime *= 2f;
                }
                
                if (timer < tpTime)
                {
                    ExecuteStandardPull(instance);
                }
            }
            
            leftHandWasColliding = instance.LeftHand.wasColliding;
            rightHandWasColliding = instance.RightHand.wasColliding;
        }

        private static Rigidbody GetCachedRigidbody(GTPlayer instance)
        {
            if (cachedRigidbody == null)
            {
                cachedRigidbody = instance.GetComponent<Rigidbody>();
            }
            return cachedRigidbody;
        }

        private static void ExecuteStandardPull(GTPlayer instance)
        {
            Rigidbody rb = GetCachedRigidbody(instance);
            if (rb == null) return;

            Vector3 velocity = GetVelocityFast(rb);
            Vector3 moveDir = velocity * Settings.PullStrength * (Settings.PullTpTime * 4f);
            
            if (Settings.WallPullEnabled)
            {
                instance.transform.position += moveDir;
                return;
            }
            
            RaycastHit hit;
            Transform leftHand = instance.GetControllerTransform(true);
            
            if (leftHand != null && Physics.Raycast(leftHand.position, Vector3.down, out hit, 999f))
            {
                Vector3 sv = velocity * Settings.PullStrength * (Settings.PullTpTime * 4f);
                moveDir = sv - Vector3.Project(sv, hit.normal);
            }
            
            float horizontalMag = Mathf.Sqrt(moveDir.x * moveDir.x + moveDir.z * moveDir.z);
            
            if (moveDir.y < -horizontalMag * 0.5f) moveDir.y = -horizontalMag * 0.5f;
            if (moveDir.y > horizontalMag * 1.5f) moveDir.y = horizontalMag * 1.5f;
            
            instance.transform.position += moveDir;
        }

        private static bool IsObstacleInFront(GTPlayer instance)
        {
            if (instance == null) return false;

            RaycastHit hit;
            return Physics.SphereCast(instance.transform.position, Settings.PullObstacleRadius, 
                instance.transform.forward, out hit, Settings.PullObstacleRadius * 2f, 
                instance.locomotionEnabledLayers);
        }

        private static Vector3 GetVelocityFast(Rigidbody rb)
        {
            if (rb == null) return Vector3.zero;
            
            if (!velocityPropertyCached)
            {
                velocityPropertyCached = true;
                velocityProperty = typeof(Rigidbody).GetProperty("linearVelocity", 
                    BindingFlags.Instance | BindingFlags.Public);
                
                if (velocityProperty == null)
                {
                    velocityProperty = typeof(Rigidbody).GetProperty("velocity", 
                        BindingFlags.Instance | BindingFlags.Public);
                }
            }
            
            if (velocityProperty != null)
            {
                try
                {
                    return (Vector3)velocityProperty.GetValue(rb);
                }
                catch { }
            }
            
            return Vector3.zero;
        }
    }
}
