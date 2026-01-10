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
            if (!InputHandler.RightJoystickClick()) return; // Only active when holding right joystick

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
        
        public static void HitboxExpanderMethod()
        {
            if (!Settings.HitboxExpanderEnabled)
            {
                // Clean up if disabled
                if (hitboxExpanderR != null)
                {
                    UnityEngine.Object.Destroy(hitboxExpanderR);
                    hitboxExpanderR = null;
                }
                if (hitboxExpanderL != null)
                {
                    UnityEngine.Object.Destroy(hitboxExpanderL);
                    hitboxExpanderL = null;
                }
                return;
            }
            
            if (PhotonNetwork.InRoom)
            {
                if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.ToLower().Contains("fected"))
                {
                    if (hitboxExpanderR == null)
                    {
                        hitboxExpanderR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        hitboxExpanderR.GetComponent<SphereCollider>().enabled = false;
                        hitboxExpanderR.transform.localScale = Vector3.one * Settings.HitboxExpanderSize;
                        
                        // Set color with opacity
                        Color redWithAlpha = new Color(1f, 0f, 0f, Settings.HitboxExpanderOpacity);
                        hitboxExpanderR.GetComponent<Renderer>().material.color = redWithAlpha;
                        
                        // Enable transparency
                        hitboxExpanderR.GetComponent<Renderer>().material.SetFloat("_Mode", 3);
                        hitboxExpanderR.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        hitboxExpanderR.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        hitboxExpanderR.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
                        hitboxExpanderR.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                        hitboxExpanderR.GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
                        hitboxExpanderR.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        hitboxExpanderR.GetComponent<Renderer>().material.renderQueue = 3000;
                    }
                    else
                    {
                        hitboxExpanderR.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
                        hitboxExpanderR.transform.localScale = Vector3.one * Settings.HitboxExpanderSize;
                        
                        // Update opacity
                        Color redWithAlpha = new Color(1f, 0f, 0f, Settings.HitboxExpanderOpacity);
                        hitboxExpanderR.GetComponent<Renderer>().material.color = redWithAlpha;
                    }
                    
                    if (hitboxExpanderL == null)
                    {
                        hitboxExpanderL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        hitboxExpanderL.GetComponent<SphereCollider>().enabled = false;
                        hitboxExpanderL.transform.localScale = Vector3.one * Settings.HitboxExpanderSize;
                        
                        // Set color with opacity
                        Color redWithAlpha = new Color(1f, 0f, 0f, Settings.HitboxExpanderOpacity);
                        hitboxExpanderL.GetComponent<Renderer>().material.color = redWithAlpha;
                        
                        // Enable transparency
                        hitboxExpanderL.GetComponent<Renderer>().material.SetFloat("_Mode", 3);
                        hitboxExpanderL.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        hitboxExpanderL.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        hitboxExpanderL.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
                        hitboxExpanderL.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                        hitboxExpanderL.GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
                        hitboxExpanderL.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        hitboxExpanderL.GetComponent<Renderer>().material.renderQueue = 3000;
                    }
                    else
                    {
                        hitboxExpanderL.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
                        hitboxExpanderL.transform.localScale = Vector3.one * Settings.HitboxExpanderSize;
                        
                        // Update opacity
                        Color redWithAlpha = new Color(1f, 0f, 0f, Settings.HitboxExpanderOpacity);
                        hitboxExpanderL.GetComponent<Renderer>().material.color = redWithAlpha;
                    }
                    
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (!rig.mainSkin.material.name.ToLower().Contains("fected"))
                        {
                            float tagRadius = Settings.HitboxExpanderSize / 2f; // Match sphere radius
                            if (Vector3.Distance(rig.headMesh.transform.position, GTPlayer.Instance.RightHand.controllerTransform.position) < tagRadius
                                ||
                                Vector3.Distance(rig.headMesh.transform.position, GTPlayer.Instance.LeftHand.controllerTransform.position) < tagRadius)
                            {
                                GorillaGameModes.GameMode.ReportTag(rig.OwningNetPlayer);
                            }
                        }
                    }
                }
                else
                {
                    // Clean up spheres if not infected
                    if (hitboxExpanderR != null)
                    {
                        UnityEngine.Object.Destroy(hitboxExpanderR);
                        hitboxExpanderR = null;
                    }
                    if (hitboxExpanderL != null)
                    {
                        UnityEngine.Object.Destroy(hitboxExpanderL);
                        hitboxExpanderL = null;
                    }
                }
            }
        }
    }
}
