using System;
using System.Reflection;
using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    internal class PullMod
    {
        // Previous frame's collision state
        public static bool leftHandWasColliding;
        public static bool rightHandWasColliding;
        
        public static float timer = 35f;
        
        // Cache reflection - do it ONCE not every frame
        private static PropertyInfo velocityProperty = null;
        private static bool velocityPropertyCached = false;
        private static Rigidbody cachedRigidbody = null;

        public static void Execute()
        {
            if (!Settings.PullModEnabled) return;

            GTPlayer instance = GTPlayer.Instance;
            if (instance == null) return;
            
            // Obstacle detection - disable pull if obstacle detected
            if (Settings.PullObstacleDetection && IsObstacleInFront(instance))
            {
                return;
            }

            // Standard Pull Mod
            timer += Time.deltaTime;
            float threshold = (instance.maxJumpSpeed - 0.01f) * Settings.PullThreshold;
            bool activation = Settings.GetActivation(Settings.PullActivation);
            
            if (activation)
            {
                bool leftColliding = instance.LeftHand.wasColliding;
                bool rightColliding = instance.RightHand.wasColliding;
                
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
                        bool velocityCheck = velocity.magnitude / instance.scale > threshold || 
                                            (instance.disableMovement && 
                                             (Vector3.Distance(instance.GetControllerTransform(true).position, instance.headCollider.transform.position) > 1f || 
                                              Vector3.Distance(instance.GetControllerTransform(false).position, instance.headCollider.transform.position) > 1f));
                        
                        if (velocityCheck)
                        {
                            timer = 0f;
                        }
                    }
                }
                
                if (timer < Settings.PullTpTime)
                {
                    bool isTagFrozen = instance.disableMovement;
                    
                    if (isTagFrozen && Settings.UseD1vineTagFreeze)
                    {
                        ExecuteD1vineTagFreeze(instance);
                    }
                    else
                    {
                        ExecuteStandardPull(instance);
                    }
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

        private static void ExecuteD1vineTagFreeze(GTPlayer instance)
        {
            if (instance == null || instance.bodyCollider == null) return;
            
            float multiplier = Settings.D1vineTagFreezeMultiplier;
            Vector3 position = instance.transform.position;
            Vector3 forward = instance.bodyCollider.transform.forward;
            
            RaycastHit hit;
            
            if (Physics.Raycast(position, Vector3.back, out hit))
            {
                instance.transform.position -= forward * multiplier * Time.deltaTime;
            }
            
            if (!Physics.Raycast(position, Vector3.forward, out hit))
            {
                instance.transform.position += forward * multiplier * Time.deltaTime;
            }
        }

        private static bool IsObstacleInFront(GTPlayer instance)
        {
            if (instance == null) return false;

            RaycastHit hit;
            return Physics.SphereCast(instance.transform.position, Settings.PullObstacleRadius, 
                instance.transform.forward, out hit, Settings.PullObstacleRadius * 2f, 
                instance.locomotionEnabledLayers);
        }

        // OPTIMIZED: Cache reflection result
        private static Vector3 GetVelocityFast(Rigidbody rb)
        {
            if (rb == null) return Vector3.zero;
            
            // Cache the property info once
            if (!velocityPropertyCached)
            {
                velocityPropertyCached = true;
                // Try linearVelocity first (newer Unity)
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
