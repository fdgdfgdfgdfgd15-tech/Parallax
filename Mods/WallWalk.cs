using System;
using CompMenu.Core;
using GorillaLocomotion;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class WallWalk
    {
        public static void Execute()
        {
            if (!Settings.WallWalkEnabled) return;

            try
            {
            GTPlayer player = GTPlayer.Instance;
            if (player == null) return;

                Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
                LayerMask locomotionEnabledLayers = player.locomotionEnabledLayers;

                // Left hand
                if (Settings.WallWalkLeftHand)
                {
                    bool leftGrab = ControllerInputPoller.instance.leftGrab;
                    if (leftGrab)
            {
                        Vector3 leftHandPos = player.LeftHand.controllerTransform.position;
                        ApplyWallForce(rigidbody, leftHandPos, locomotionEnabledLayers);
                    }
                }

                // Right hand
                if (Settings.WallWalkRightHand)
                {
                    bool rightGrab = ControllerInputPoller.instance.rightGrab;
                    if (rightGrab)
                    {
                        Vector3 rightHandPos = player.RightHand.controllerTransform.position;
                        ApplyWallForce(rigidbody, rightHandPos, locomotionEnabledLayers);
                    }
                }
            }
            catch (Exception ex)
                    {
                Debug.LogWarning("[CompMenu] WallWalk Error: " + ex.Message);
            }
        }

        private static void ApplyWallForce(Rigidbody rigidbody, Vector3 handPos, LayerMask layers)
                    {
            // Raycast in multiple horizontal directions from hand to find walls
            Vector3[] directions = new Vector3[]
            {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right,
                (Vector3.forward + Vector3.right).normalized,
                (Vector3.forward + Vector3.left).normalized,
                (Vector3.back + Vector3.right).normalized,
                (Vector3.back + Vector3.left).normalized
            };

            RaycastHit closestHit = new RaycastHit();
            float closestDist = float.MaxValue;
            bool foundWall = false;

            foreach (Vector3 dir in directions)
            {
                    RaycastHit hit;
                if (Physics.Raycast(handPos, dir, out hit, Settings.WallWalkDistance, layers))
                    {
                    // Only count walls (surfaces that are mostly vertical)
                    float verticalDot = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up));
                    if (verticalDot < 0.5f) // Less than ~60 degrees from vertical = wall
                    {
                        if (hit.distance < closestDist)
                        {
                            closestDist = hit.distance;
                            closestHit = hit;
                            foundWall = true;
                        }
                    }
                }
            }

            // Apply force toward the wall
            if (foundWall)
                            {
                rigidbody.AddForce(closestHit.normal * -Settings.WallWalkPower, ForceMode.Acceleration);
            }
        }
    }
}
