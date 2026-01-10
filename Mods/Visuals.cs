using System.Collections.Generic;
using CompMenu.Core;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class Visuals
    {
        // Cached materials
        private static Material espMaterial;
        
        // Tracer lines - reuse instead of create/destroy
        private static Dictionary<int, LineRenderer> tracerLines = new Dictionary<int, LineRenderer>();
        private static List<int> tracerKeysToRemove = new List<int>();
        
        // Hitbox cubes - pooled
        private static Dictionary<int, GameObject> hitboxCubes = new Dictionary<int, GameObject>();
        private static List<int> hitboxKeysToRemove = new List<int>();
        
        // Corner ESP - pooled
        private static Dictionary<int, GameObject> cornerObjects = new Dictionary<int, GameObject>();
        private static List<int> cornerKeysToRemove = new List<int>();

        public static void Execute()
        {
            if (!Settings.ESPEnabled)
            {
                CleanupAll();
                return;
            }

            if (Settings.TracersEnabled) DrawTracers();
            else CleanupTracers();
            
            if (Settings.HitboxesEnabled) DrawHitboxes();
            else CleanupHitboxes();
            
            if (Settings.CornerESPEnabled) DrawCornerESP();
            else CleanupCorners();
        }

        public static Color GetESPColor()
        {
            return Color.HSVToRGB(Settings.ESPColorHue, Settings.ESPColorSat, Settings.ESPColorVal);
        }

        private static Material GetESPMaterial()
        {
            if (espMaterial == null)
            {
                // Use unlit color shader that shows through walls
                espMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                if (espMaterial != null)
                {
                    espMaterial.hideFlags = HideFlags.HideAndDontSave;
                    espMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    espMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    espMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    espMaterial.SetInt("_ZWrite", 0);
                    espMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                }
            }
            return espMaterial;
        }

        // ==================== TRACERS ====================
        private static void DrawTracers()
        {
            if (GorillaParent.instance == null || GorillaParent.instance.vrrigs == null) return;
            if (GorillaTagger.Instance == null) return;

            Color color = GetESPColor();
            Vector3 handPos = GorillaTagger.Instance.rightHandTransform.position;
            HashSet<int> activeRigs = new HashSet<int>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == null || vrrig == GorillaTagger.Instance.offlineVRRig) continue;

                int id = vrrig.GetInstanceID();
                activeRigs.Add(id);

                LineRenderer lr;
                if (!tracerLines.TryGetValue(id, out lr) || lr == null)
                {
                    GameObject go = new GameObject("ESP_Tracer_" + id);
                    go.hideFlags = HideFlags.HideAndDontSave;
                    lr = go.AddComponent<LineRenderer>();
                    lr.material = GetESPMaterial();
                    lr.startWidth = 0.01f;
                    lr.endWidth = 0.01f;
                    lr.positionCount = 2;
                    tracerLines[id] = lr;
                }

                // Update color every frame for real-time updates
                lr.startColor = color;
                lr.endColor = color;
                lr.SetPosition(0, handPos);
                lr.SetPosition(1, vrrig.transform.position);
                lr.enabled = true;
            }

            // Remove tracers for players who left
            tracerKeysToRemove.Clear();
            foreach (var kvp in tracerLines)
            {
                if (!activeRigs.Contains(kvp.Key))
                {
                    if (kvp.Value != null) Object.Destroy(kvp.Value.gameObject);
                    tracerKeysToRemove.Add(kvp.Key);
                }
            }
            foreach (int key in tracerKeysToRemove)
            {
                tracerLines.Remove(key);
            }
        }

        private static void CleanupTracers()
        {
            foreach (var kvp in tracerLines)
            {
                if (kvp.Value != null) Object.Destroy(kvp.Value.gameObject);
            }
            tracerLines.Clear();
        }

        // ==================== HITBOXES ====================
        private static void DrawHitboxes()
        {
            if (GorillaParent.instance == null || GorillaParent.instance.vrrigs == null) return;
            if (GorillaTagger.Instance == null) return;

            Color color = GetESPColor();
            color.a = 0.4f;
            HashSet<int> activeRigs = new HashSet<int>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == null || vrrig == GorillaTagger.Instance.offlineVRRig) continue;

                int id = vrrig.GetInstanceID();
                activeRigs.Add(id);

                GameObject cube;
                if (!hitboxCubes.TryGetValue(id, out cube) || cube == null)
                {
                    cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "ESP_Hitbox_" + id;
                    cube.hideFlags = HideFlags.HideAndDontSave;
                    
                    // Remove collider so it doesn't interfere
                    Object.Destroy(cube.GetComponent<Collider>());
                    
                    // Set up material
                    Renderer rend = cube.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        rend.material = GetESPMaterial();
                    }
                    
                    cube.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                    hitboxCubes[id] = cube;
                }

                // Update position and color every frame
                if (vrrig.headMesh != null)
                {
                    cube.transform.position = vrrig.headMesh.transform.position;
                }
                else
                {
                    cube.transform.position = vrrig.transform.position + Vector3.up * 0.3f;
                }
                
                // Update color
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = color;
                }
                
                cube.SetActive(true);
            }

            // Remove hitboxes for players who left
            hitboxKeysToRemove.Clear();
            foreach (var kvp in hitboxCubes)
            {
                if (!activeRigs.Contains(kvp.Key))
                {
                    if (kvp.Value != null) Object.Destroy(kvp.Value);
                    hitboxKeysToRemove.Add(kvp.Key);
                }
            }
            foreach (int key in hitboxKeysToRemove)
            {
                hitboxCubes.Remove(key);
            }
        }

        private static void CleanupHitboxes()
        {
            foreach (var kvp in hitboxCubes)
            {
                if (kvp.Value != null) Object.Destroy(kvp.Value);
            }
            hitboxCubes.Clear();
        }

        // ==================== CORNER ESP ====================
        private static void DrawCornerESP()
        {
            if (GorillaParent.instance == null || GorillaParent.instance.vrrigs == null) return;
            if (GorillaTagger.Instance == null) return;

            Color color = GetESPColor();
            HashSet<int> activeRigs = new HashSet<int>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == null || vrrig == GorillaTagger.Instance.offlineVRRig) continue;

                int id = vrrig.GetInstanceID();
                activeRigs.Add(id);

                GameObject container;
                if (!cornerObjects.TryGetValue(id, out container) || container == null)
                {
                    container = CreateCornerBrackets(id);
                    cornerObjects[id] = container;
                }

                // Update position and rotation
                Vector3 pos = vrrig.transform.position;
                Vector3 lookDir = pos - GorillaTagger.Instance.headCollider.transform.position;
                if (lookDir != Vector3.zero)
                {
                    container.transform.rotation = Quaternion.LookRotation(lookDir);
                }
                container.transform.position = pos;
                
                // Update color on all child renderers
                foreach (Renderer rend in container.GetComponentsInChildren<Renderer>())
                {
                    if (rend.material != null)
                    {
                        rend.material.color = color;
                    }
                }
                
                container.SetActive(true);
            }

            // Remove corners for players who left
            cornerKeysToRemove.Clear();
            foreach (var kvp in cornerObjects)
            {
                if (!activeRigs.Contains(kvp.Key))
                {
                    if (kvp.Value != null) Object.Destroy(kvp.Value);
                    cornerKeysToRemove.Add(kvp.Key);
                }
            }
            foreach (int key in cornerKeysToRemove)
            {
                cornerObjects.Remove(key);
            }
        }

        private static GameObject CreateCornerBrackets(int id)
        {
            GameObject container = new GameObject("ESP_Corners_" + id);
            container.hideFlags = HideFlags.HideAndDontSave;

            // Corner positions and scales
            Vector3[] positions = new Vector3[]
            {
                new Vector3(0.24f, 0.35f, 0f),   // Top-right horizontal
                new Vector3(0.33f, 0.26f, 0f),   // Top-right vertical
                new Vector3(-0.24f, 0.35f, 0f),  // Top-left horizontal
                new Vector3(-0.33f, 0.26f, 0f),  // Top-left vertical
                new Vector3(-0.24f, -0.55f, 0f), // Bottom-left horizontal
                new Vector3(-0.33f, -0.46f, 0f), // Bottom-left vertical
                new Vector3(0.24f, -0.55f, 0f),  // Bottom-right horizontal
                new Vector3(0.33f, -0.46f, 0f)   // Bottom-right vertical
            };

            Vector3[] scales = new Vector3[]
            {
                new Vector3(0.18f, 0.02f, 0.01f), // Horizontal
                new Vector3(0.02f, 0.18f, 0.01f), // Vertical
                new Vector3(0.18f, 0.02f, 0.01f),
                new Vector3(0.02f, 0.18f, 0.01f),
                new Vector3(0.18f, 0.02f, 0.01f),
                new Vector3(0.02f, 0.18f, 0.01f),
                new Vector3(0.18f, 0.02f, 0.01f),
                new Vector3(0.02f, 0.18f, 0.01f)
            };

            Material mat = GetESPMaterial();

            for (int i = 0; i < 8; i++)
            {
                GameObject bracket = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bracket.name = "Bracket_" + i;
                bracket.hideFlags = HideFlags.HideAndDontSave;
                bracket.transform.SetParent(container.transform, false);
                bracket.transform.localPosition = positions[i];
                bracket.transform.localScale = scales[i];
                
                Object.Destroy(bracket.GetComponent<Collider>());
                
                Renderer rend = bracket.GetComponent<Renderer>();
                if (rend != null && mat != null)
                {
                    rend.material = mat;
                }
            }

            return container;
        }

        private static void CleanupCorners()
        {
            foreach (var kvp in cornerObjects)
            {
                if (kvp.Value != null) Object.Destroy(kvp.Value);
            }
            cornerObjects.Clear();
        }

        // ==================== CLEANUP ALL ====================
        private static void CleanupAll()
        {
            CleanupTracers();
            CleanupHitboxes();
            CleanupCorners();
        }
    }
}
