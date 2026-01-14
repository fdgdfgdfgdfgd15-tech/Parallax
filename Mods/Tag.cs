using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GorillaLocomotion;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using Valve.VR;
using CompMenu.Core;

namespace CompMenu.Mods
{
    internal class Tag
    {
        private static void ReportTag(VRRig rig)
        {
            try
            {
                if (rig != null && rig.OwningNetPlayer != null)
                {
                    GorillaGameModes.GameMode.ReportTag(rig.OwningNetPlayer);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] ReportTag Error: " + ex.Message);
            }
        }

        public static void DCFlick()
        {
            if (!Settings.DCFlickEnabled) return;
            
            if (PhotonNetwork.InRoom)
            {
                if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.ToLower().Contains("fected"))
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (!rig.mainSkin.material.name.ToLower().Contains("fected"))
                        {
                            if (Vector3.Distance(rig.headMesh.transform.position - new Vector3(0f, 0.1f, 0f), GTPlayer.Instance.RightHand.controllerTransform.position) < 0.3f
                                ||
                                Vector3.Distance(rig.headMesh.transform.position - new Vector3(0f, 0.1f, 0f), GTPlayer.Instance.LeftHand.controllerTransform.position) < 0.3f)
                            {
                                GorillaGameModes.GameMode.ReportTag(rig.OwningNetPlayer);
                            }
                        }
                    }
                }
            }
        }
        
        private static bool IsTagged(VRRig rig)
        {
            return rig.mainSkin.material.name.ToLower().Contains("fected");
        }

        public static void TagAura()
        {
            if (!Settings.TagAuraEnabled) return;
            if (!InputHandler.RightJoystickClick()) return;

            try
            {
                foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => 
                    IsTagged(GorillaTagger.Instance.offlineVRRig) && 
                    !IsTagged(vrrig) && 
                    !GTPlayer.Instance.disableMovement && 
                    Vector3.Distance(vrrig.headMesh.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) < Settings.TagAuraDistance))
                {
                    ReportTag(vrrig);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[CompMenu] TagAura Error: " + ex.Message);
            }
        }

        public static GameObject hitboxExpanderR = null;
        public static GameObject hitboxExpanderL = null;
        private static Material hitboxMaterial = null;
        
        private static Material GetHitboxMaterial()
        {
            if (hitboxMaterial == null)
            {
                // Use unlit transparent shader
                hitboxMaterial = new Material(Shader.Find("GUI/Text Shader"));
                if (hitboxMaterial.shader == null)
                {
                    hitboxMaterial = new Material(Shader.Find("Sprites/Default"));
                }
            }
            return hitboxMaterial;
        }
        
        private static GameObject CreateHitboxSphere()
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "HitboxExpander";
            
            // Disable collider
            var collider = sphere.GetComponent<SphereCollider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            
            // Setup transparent material
            var renderer = sphere.GetComponent<Renderer>();
            renderer.material = new Material(GetHitboxMaterial());
            
            return sphere;
        }
        
        public static void DestroyHitboxSpheres()
        {
            if (hitboxExpanderR != null)
            {
                UnityEngine.Object.DestroyImmediate(hitboxExpanderR);
                hitboxExpanderR = null;
            }
            if (hitboxExpanderL != null)
            {
                UnityEngine.Object.DestroyImmediate(hitboxExpanderL);
                hitboxExpanderL = null;
            }
        }
        
        public static void HitboxExpanderMethod()
        {
            if (!Settings.HitboxExpanderEnabled)
            {
                DestroyHitboxSpheres();
                return;
            }
            
            GTPlayer player = GTPlayer.Instance;
            if (player == null)
            {
                DestroyHitboxSpheres();
                return;
            }
            
            bool isInfected = false;
            try
            {
                if (PhotonNetwork.InRoom && GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
                {
                    isInfected = GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.ToLower().Contains("fected");
                }
            }
            catch { }
            
            if (isInfected)
            {
                Color sphereColor = new Color(1f, 0f, 0f, Settings.HitboxExpanderOpacity);
                
                // Get raw controller positions - exact center of VR controller
                Vector3 rightHandPos = player.RightHand.controllerTransform.position;
                Vector3 leftHandPos = player.LeftHand.controllerTransform.position;
                
                // Right hand sphere
                if (hitboxExpanderR == null)
                {
                    hitboxExpanderR = CreateHitboxSphere();
                }
                hitboxExpanderR.transform.position = rightHandPos;
                hitboxExpanderR.transform.localScale = Vector3.one * Settings.HitboxExpanderSize;
                hitboxExpanderR.GetComponent<Renderer>().material.color = sphereColor;
                
                // Left hand sphere
                if (hitboxExpanderL == null)
                {
                    hitboxExpanderL = CreateHitboxSphere();
                }
                hitboxExpanderL.transform.position = leftHandPos;
                hitboxExpanderL.transform.localScale = Vector3.one * Settings.HitboxExpanderSize;
                hitboxExpanderL.GetComponent<Renderer>().material.color = sphereColor;
                
                // Tag detection
                try
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (!rig.mainSkin.material.name.ToLower().Contains("fected"))
                        {
                            float tagRadius = Settings.HitboxExpanderSize / 2f;
                            if (Vector3.Distance(rig.headMesh.transform.position, rightHandPos) < tagRadius
                                ||
                                Vector3.Distance(rig.headMesh.transform.position, leftHandPos) < tagRadius)
                            {
                                GorillaGameModes.GameMode.ReportTag(rig.OwningNetPlayer);
                            }
                        }
                    }
                }
                catch { }
            }
            else
            {
                DestroyHitboxSpheres();
            }
        }
    }
}
