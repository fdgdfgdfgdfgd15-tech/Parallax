using System;
using System.Collections.Generic;
using CompMenu.Core;
using CompMenu.Mods;
using UnityEngine;

namespace CompMenu.GUI
{
    public static class MenuGUI
    {
        private const float WINDOW_WIDTH = 980f;
        private const float WINDOW_HEIGHT = 640f;

        private static Rect mainWindow = new Rect(20, 20, WINDOW_WIDTH, WINDOW_HEIGHT);
        private static Vector2 scrollPosition = Vector2.zero;

        // Styles
        private static GUIStyle windowStyle;
        private static GUIStyle tabActiveStyle;
        private static GUIStyle tabInactiveStyle;
        private static GUIStyle headerStyle;
        private static GUIStyle subHeaderStyle;
        private static GUIStyle labelStyle;
        private static GUIStyle toggleOnStyle;
        private static GUIStyle toggleOffStyle;
        private static GUIStyle sliderStyle;
        private static GUIStyle sliderThumbStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle boxStyle;
        private static GUIStyle sectionBoxStyle;
        private static GUIStyle creditCardStyle;
        private static GUIStyle footerStyle;
        private static GUIStyle gearButtonStyle;
        
        // Cached styles for per-frame drawing (avoid GC)
        private static GUIStyle animatedHeaderStyle;
        private static GUIStyle valueDisplayStyle;
        private static GUIStyle infoBoxStyle;
        private static GUIStyle panicButtonStyle;
        private static GUIStyle colorPreviewStyle;

        // Theme colors
        private static Color primaryColor;
        private static Color darkColor;
        private static Color accentColor;
        private static Color bgColor;
        private static Color panelColor;
        private static Color cardColor;
        private static Color textWhite = new Color(1f, 1f, 1f, 1f);
        private static Color textGray = new Color(0.7f, 0.7f, 0.7f, 0.9f);
        private static Color textDim = new Color(0.5f, 0.5f, 0.5f, 0.7f);

        private static string[] tabNames = { "PULL", "WALL", "TAG", "PSA", "SPEED", "WEATHER", "BYPASS", "SAFETY", "CONTROL", "ESP", "CONFIG", "CREDITS" };
        private static string[] tabIcons = { "◈", "◈", "◈", "◈", "◈", "◈", "◈", "◈", "◈", "◈", "◈", "★" };
        private static bool stylesInitialized = false;

        // Animation state
        private static float pulseTimer = 0f;
        private static float glowIntensity = 0f;
        private static float titleGlow = 0f;
        private static float menuOpenAnim = 0f;
        private static float contentFade = 1f;
        private static int prevTab = 0;
        private static float redBarTimer = 0f;

        // Particles (for Starry theme)
        private static List<Star> stars = new List<Star>();
        private static float starSpawnTimer = 0f;

        // Rainbow animation
        private static float rainbowHue = 0f;

        // UI transient animations
        private static Dictionary<string, float> toggleFlash = new Dictionary<string, float>();
        private static Dictionary<string, float> elementPressAnim = new Dictionary<string, float>();
        private static Dictionary<string, float> visibilityAnim = new Dictionary<string, float>();
        private static Dictionary<string, float> dropdownAnimations = new Dictionary<string, float>();

        // Cached textures
        private static Dictionary<string, Texture2D> texCache = new Dictionary<string, Texture2D>();

        // Reusable list for dictionary iteration (avoids GC allocation)
        private static List<string> tempKeys;
        
        // Track theme changes to avoid rebuilding every frame
        private static ThemeOption lastTheme = (ThemeOption)(-1);

        private class Star
        {
            public Vector2 position;
            public float speed;
            public float size;
            public float alpha;
        }

        private static void UpdateThemeColors()
        {
            // CRITICAL: Only rebuild when theme actually changes
            bool themeChanged = (lastTheme != Settings.CurrentTheme);
            
            // For rainbow theme, we update colors but DON'T rebuild textures
            if (Settings.CurrentTheme == ThemeOption.Rainbow)
            {
                Color rainbow = Color.HSVToRGB(rainbowHue, 0.8f, 1f);
                primaryColor = rainbow;
                darkColor = Color.HSVToRGB(rainbowHue, 0.8f, 0.6f);
                accentColor = Color.HSVToRGB((rainbowHue + 0.1f) % 1f, 0.9f, 1f);
                
                if (themeChanged)
                {
                    bgColor = new Color(0.04f, 0.04f, 0.06f, 0.97f);
                    panelColor = new Color(0.08f, 0.08f, 0.10f, 0.85f);
                    cardColor = new Color(0.12f, 0.12f, 0.14f, 0.9f);
                }
            }
            else if (themeChanged)
            {
                // Only set colors when theme actually changed
                switch (Settings.CurrentTheme)
                {
                    case ThemeOption.Starry:
                        primaryColor = new Color(1f, 1f, 1f, 1f);
                        darkColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                        accentColor = new Color(1f, 1f, 1f, 1f);
                        bgColor = new Color(0.02f, 0.02f, 0.04f, 0.97f);
                        panelColor = new Color(0.05f, 0.05f, 0.08f, 0.85f);
                        cardColor = new Color(0.08f, 0.08f, 0.12f, 0.9f);
                        break;
                    case ThemeOption.Ocean:
                        primaryColor = new Color(0.1f, 0.6f, 0.9f, 1f);
                        darkColor = new Color(0.05f, 0.3f, 0.6f, 1f);
                        accentColor = new Color(0.3f, 0.85f, 1f, 1f);
                        bgColor = new Color(0.02f, 0.04f, 0.08f, 0.97f);
                        panelColor = new Color(0.04f, 0.08f, 0.14f, 0.85f);
                        cardColor = new Color(0.06f, 0.12f, 0.2f, 0.9f);
                        break;
                    case ThemeOption.Neon:
                        primaryColor = new Color(1f, 0.1f, 0.6f, 1f);
                        darkColor = new Color(0.6f, 0.05f, 0.4f, 1f);
                        accentColor = new Color(0.2f, 1f, 0.9f, 1f);
                        bgColor = new Color(0.02f, 0.01f, 0.04f, 0.97f);
                        panelColor = new Color(0.06f, 0.03f, 0.1f, 0.85f);
                        cardColor = new Color(0.1f, 0.05f, 0.15f, 0.9f);
                        break;
                    case ThemeOption.Forest:
                        primaryColor = new Color(0.2f, 0.75f, 0.3f, 1f);
                        darkColor = new Color(0.1f, 0.5f, 0.15f, 1f);
                        accentColor = new Color(0.5f, 1f, 0.4f, 1f);
                        bgColor = new Color(0.02f, 0.05f, 0.02f, 0.97f);
                        panelColor = new Color(0.04f, 0.1f, 0.05f, 0.85f);
                        cardColor = new Color(0.08f, 0.15f, 0.08f, 0.9f);
                        break;
                    case ThemeOption.Sunset:
                        primaryColor = new Color(1f, 0.5f, 0.1f, 1f);
                        darkColor = new Color(0.8f, 0.25f, 0.1f, 1f);
                        accentColor = new Color(1f, 0.7f, 0.3f, 1f);
                        bgColor = new Color(0.06f, 0.03f, 0.04f, 0.97f);
                        panelColor = new Color(0.12f, 0.06f, 0.08f, 0.85f);
                        cardColor = new Color(0.18f, 0.1f, 0.1f, 0.9f);
                        break;
                    case ThemeOption.Midnight:
                        primaryColor = new Color(0.5f, 0.3f, 0.9f, 1f);
                        darkColor = new Color(0.3f, 0.15f, 0.6f, 1f);
                        accentColor = new Color(0.7f, 0.5f, 1f, 1f);
                        bgColor = new Color(0.02f, 0.02f, 0.06f, 0.97f);
                        panelColor = new Color(0.05f, 0.04f, 0.12f, 0.85f);
                        cardColor = new Color(0.08f, 0.06f, 0.18f, 0.9f);
                        break;
                    default: // Default red/black
                        primaryColor = new Color(0.85f, 0.12f, 0.12f, 1f);
                        darkColor = new Color(0.55f, 0.05f, 0.05f, 1f);
                        accentColor = new Color(1f, 0.3f, 0.3f, 1f);
                        bgColor = new Color(0.04f, 0.04f, 0.06f, 0.97f);
                        panelColor = new Color(0.08f, 0.08f, 0.10f, 0.85f);
                        cardColor = new Color(0.12f, 0.12f, 0.14f, 0.9f);
                        break;
                }
            }
            
            // Only rebuild styles and textures when theme ACTUALLY changed
            if (themeChanged)
            {
                lastTheme = Settings.CurrentTheme;
                stylesInitialized = false;
                ClearTextureCache();
            }
        }

        private static void ClearTextureCache()
        {
            foreach (var kvp in texCache)
            {
                if (kvp.Value != null)
                {
                    UnityEngine.Object.Destroy(kvp.Value);
                }
            }
            texCache.Clear();
        }

        private static Texture2D MakeTex(int width, int height, Color color)
        {
            string key = $"tex_{width}_{height}_{color.r}_{color.g}_{color.b}_{color.a}";
            if (texCache.TryGetValue(key, out Texture2D cached) && cached != null) return cached;

            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.SetPixels(pixels);
            tex.Apply();
            texCache[key] = tex;
            return tex;
        }

        private static Texture2D MakeGradientTex(int width, int height, Color top, Color bottom)
        {
            string key = $"grad_{width}_{height}_{top}_{bottom}";
            if (texCache.TryGetValue(key, out Texture2D cached) && cached != null) return cached;

            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / height;
                Color color = Color.Lerp(top, bottom, t);
                for (int x = 0; x < width; x++)
                    pixels[y * width + x] = color;
            }
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.SetPixels(pixels);
            tex.Apply();
            texCache[key] = tex;
            return tex;
        }

        private static Texture2D MakeRoundedTex(int width, int height, Color color, int radius)
        {
            if (radius <= 0)
            {
                return MakeTex(width, height, color);
            }

            // For 9-slice to work properly, texture must be at least 2*radius+2 in each dimension
            int texSize = radius * 2 + 2;
            string key = $"rounded9_{texSize}_{color.r}_{color.g}_{color.b}_{color.a}_{radius}";
            if (texCache.TryGetValue(key, out Texture2D cached) && cached != null) return cached;

            Color[] pixels = new Color[texSize * texSize];
            Color transparent = new Color(0, 0, 0, 0);

            for (int y = 0; y < texSize; y++)
            {
                for (int x = 0; x < texSize; x++)
                {
                    int index = y * texSize + x;
                    
                    // Check corners
                    bool inCorner = false;
                    float dist = 0f;
                    
                    // Bottom-left corner
                    if (x < radius && y < radius)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius));
                        inCorner = true;
                    }
                    // Bottom-right corner
                    else if (x >= texSize - radius && y < radius)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), new Vector2(texSize - radius - 1, radius));
                        inCorner = true;
                    }
                    // Top-left corner
                    else if (x < radius && y >= texSize - radius)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, texSize - radius - 1));
                        inCorner = true;
                    }
                    // Top-right corner
                    else if (x >= texSize - radius && y >= texSize - radius)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), new Vector2(texSize - radius - 1, texSize - radius - 1));
                        inCorner = true;
                    }

                    if (inCorner)
                    {
                        if (dist > radius)
                            pixels[index] = transparent;
                        else if (dist > radius - 1.5f)
                            pixels[index] = new Color(color.r, color.g, color.b, color.a * (radius - dist) / 1.5f);
                        else
                            pixels[index] = color;
                    }
                    else
                    {
                        pixels[index] = color;
                    }
                }
            }

            Texture2D tex = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.SetPixels(pixels);
            tex.Apply();
            texCache[key] = tex;
            return tex;
        }

        private static Texture2D MakeCircleTex(int size, Color color)
        {
            string key = $"circle_{size}_{color}";
            if (texCache.TryGetValue(key, out Texture2D cached) && cached != null) return cached;

            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = y * size + x;
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance < radius - 1) pixels[index] = color;
                    else if (distance < radius) pixels[index] = new Color(color.r, color.g, color.b, color.a * (radius - distance));
                    else pixels[index] = new Color(0, 0, 0, 0);
                }
            }

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.SetPixels(pixels);
            tex.Apply();
            texCache[key] = tex;
            return tex;
        }

        private static Texture2D MakeGlowTex(int size, Color color)
        {
            string key = $"glow_{size}_{color}";
            if (texCache.TryGetValue(key, out Texture2D cached) && cached != null) return cached;

            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = y * size + x;
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01(1f - (distance / radius));
                    alpha = alpha * alpha * alpha;
                    pixels[index] = new Color(color.r, color.g, color.b, color.a * alpha);
                }
            }

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.SetPixels(pixels);
            tex.Apply();
            texCache[key] = tex;
            return tex;
        }

        private static void InitStyles()
        {
            if (stylesInitialized) return;

            int cornerRadius = 10;

            windowStyle = new GUIStyle();
            windowStyle.normal.background = MakeRoundedTex(64, 64, panelColor, cornerRadius);
            windowStyle.border = new RectOffset(cornerRadius, cornerRadius, cornerRadius, cornerRadius);
            windowStyle.padding = new RectOffset(12, 12, 12, 12);

            tabActiveStyle = new GUIStyle();
            tabActiveStyle.normal.background = MakeGradientTex(1, 32, primaryColor, darkColor);
            tabActiveStyle.normal.textColor = textWhite;
            tabActiveStyle.fontStyle = FontStyle.Bold;
            tabActiveStyle.alignment = TextAnchor.MiddleLeft;
            tabActiveStyle.fontSize = 12;
            tabActiveStyle.padding = new RectOffset(12, 12, 10, 10);
            tabActiveStyle.margin = new RectOffset(2, 2, 2, 2);

            tabInactiveStyle = new GUIStyle();
            tabInactiveStyle.normal.background = MakeTex(1, 1, new Color(0, 0, 0, 0));
            tabInactiveStyle.normal.textColor = textGray;
            tabInactiveStyle.hover.background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.15f, 0.6f));
            tabInactiveStyle.hover.textColor = accentColor;
            tabInactiveStyle.fontStyle = FontStyle.Bold;
            tabInactiveStyle.alignment = TextAnchor.MiddleLeft;
            tabInactiveStyle.fontSize = 12;
            tabInactiveStyle.padding = new RectOffset(12, 12, 10, 10);
            tabInactiveStyle.margin = new RectOffset(2, 2, 2, 2);

            headerStyle = new GUIStyle();
            headerStyle.normal.textColor = accentColor;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.fontSize = 28;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.padding = new RectOffset(8, 0, 8, 5);

            subHeaderStyle = new GUIStyle();
            subHeaderStyle.normal.textColor = primaryColor;
            subHeaderStyle.fontStyle = FontStyle.Bold;
            subHeaderStyle.fontSize = 14;
            subHeaderStyle.alignment = TextAnchor.MiddleLeft;
            subHeaderStyle.padding = new RectOffset(5, 5, 8, 4);

            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = textWhite;
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.padding = new RectOffset(5, 5, 2, 2);

            toggleOnStyle = new GUIStyle();
            toggleOnStyle.normal.background = MakeTex(64, 24, primaryColor);
            toggleOnStyle.normal.textColor = textWhite;
            toggleOnStyle.fontStyle = FontStyle.Bold;
            toggleOnStyle.alignment = TextAnchor.MiddleCenter;
            toggleOnStyle.fontSize = 11;
            toggleOnStyle.padding = new RectOffset(12, 12, 5, 5);

            toggleOffStyle = new GUIStyle();
            toggleOffStyle.normal.background = MakeTex(64, 24, new Color(0, 0, 0, 0));
            toggleOffStyle.normal.textColor = textGray;
            toggleOffStyle.hover.background = MakeTex(64, 24, new Color(0.2f, 0.2f, 0.2f, 0.6f));
            toggleOffStyle.hover.textColor = textWhite;
            toggleOffStyle.fontStyle = FontStyle.Bold;
            toggleOffStyle.alignment = TextAnchor.MiddleCenter;
            toggleOffStyle.fontSize = 11;
            toggleOffStyle.padding = new RectOffset(12, 12, 5, 5);

            sliderStyle = new GUIStyle();
            sliderStyle.normal.background = MakeTex(128, 8, new Color(0.15f, 0.1f, 0.1f, 0.6f));
            sliderStyle.fixedHeight = 8f;

            sliderThumbStyle = new GUIStyle();
            sliderThumbStyle.normal.background = MakeCircleTex(16, primaryColor);
            sliderThumbStyle.hover.background = MakeCircleTex(16, accentColor);
            sliderThumbStyle.fixedWidth = 16f;
            sliderThumbStyle.fixedHeight = 16f;
            sliderThumbStyle.margin = new RectOffset(0, 0, -4, 0);

            buttonStyle = new GUIStyle();
            buttonStyle.normal.background = MakeTex(64, 28, cardColor);
            buttonStyle.normal.textColor = textGray;
            buttonStyle.hover.background = MakeTex(64, 28, new Color(primaryColor.r * 0.25f, primaryColor.g * 0.25f, primaryColor.b * 0.25f, 1f));
            buttonStyle.hover.textColor = textWhite;
            buttonStyle.active.background = MakeTex(64, 28, new Color(primaryColor.r * 0.6f, primaryColor.g * 0.6f, primaryColor.b * 0.6f, 1f));
            buttonStyle.active.textColor = textWhite;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.fontSize = 12;
            buttonStyle.padding = new RectOffset(12, 12, 6, 6);

            boxStyle = new GUIStyle();
            boxStyle.normal.background = MakeTex(64, 64, new Color(0, 0, 0, 0.06f));
            boxStyle.padding = new RectOffset(12, 12, 12, 12);

            sectionBoxStyle = new GUIStyle();
            sectionBoxStyle.normal.background = MakeTex(64, 64, new Color(0.06f, 0.06f, 0.08f, 0.65f));
            sectionBoxStyle.padding = new RectOffset(10, 10, 8, 8);
            sectionBoxStyle.margin = new RectOffset(0, 0, 5, 5);

            creditCardStyle = new GUIStyle();
            creditCardStyle.normal.background = MakeTex(256, 80, cardColor);
            creditCardStyle.padding = new RectOffset(15, 15, 12, 12);
            creditCardStyle.margin = new RectOffset(5, 5, 8, 8);

            footerStyle = new GUIStyle();
            footerStyle.normal.textColor = textDim;
            footerStyle.fontSize = 10;
            footerStyle.alignment = TextAnchor.MiddleCenter;
            footerStyle.padding = new RectOffset(0, 0, 5, 0);

            gearButtonStyle = new GUIStyle();
            gearButtonStyle.normal.background = MakeTex(1, 1, new Color(0, 0, 0, 0));
            gearButtonStyle.normal.textColor = textGray;
            gearButtonStyle.hover.textColor = accentColor;
            gearButtonStyle.fontSize = 20;
            gearButtonStyle.alignment = TextAnchor.MiddleCenter;

            stylesInitialized = true;
        }

        public static void Draw()
        {
            if (!Settings.ShowMenu) return;

            // Check if textures are still valid, reinitialize if corrupted
            ValidateTextures();

            UpdateThemeColors();
            InitStyles();
            UpdateAnimations();

            mainWindow = GUILayout.Window(9999, mainWindow, DrawMainWindow, "", windowStyle);
            mainWindow.width = WINDOW_WIDTH;
            mainWindow.height = WINDOW_HEIGHT;
        }

        private static float lastTextureCheck = 0f;
        private static void ValidateTextures()
        {
            // Only check every 5 seconds to avoid lag
            if (Time.time - lastTextureCheck < 5f) return;
            lastTextureCheck = Time.time;

            // Quick check - just see if windowStyle background is valid
            if (windowStyle != null && windowStyle.normal.background == null)
            {
                ClearTextureCache();
                stylesInitialized = false;
            }
        }

        private static void UpdateAnimations()
        {
            pulseTimer += Time.deltaTime * 2f;
            glowIntensity = (Mathf.Sin(pulseTimer) + 1f) * 0.5f;
            titleGlow = (Mathf.Sin(pulseTimer * 1.2f) + 1f) * 0.5f;

            if (!Settings.ShowMenu) { menuOpenAnim = 0f; return; }
            menuOpenAnim = Mathf.Lerp(menuOpenAnim, 1f, Time.deltaTime * 10f);

            if (Settings.CurrentTab != prevTab) { prevTab = Settings.CurrentTab; contentFade = 0f; }
            contentFade = Mathf.Lerp(contentFade, 1f, Time.deltaTime * 12f);

            // Rainbow animation
            rainbowHue = (rainbowHue + Time.deltaTime * 0.1f) % 1f;

            // Toggle flash decay - use temp list to avoid allocation
            if (tempKeys == null) tempKeys = new List<string>(16);
            tempKeys.Clear();
            tempKeys.AddRange(toggleFlash.Keys);
            foreach (string k in tempKeys)
            {
                toggleFlash[k] = Mathf.Max(0f, toggleFlash[k] - Time.deltaTime * 2.5f);
                if (toggleFlash[k] <= 0f) toggleFlash.Remove(k);
            }

            // Press animation decay
            tempKeys.Clear();
            tempKeys.AddRange(elementPressAnim.Keys);
            foreach (string k in tempKeys)
            {
                elementPressAnim[k] = Mathf.Max(0f, elementPressAnim[k] - Time.deltaTime * 6f);
                if (elementPressAnim[k] <= 0f) elementPressAnim.Remove(k);
            }

            // Visibility animations
            tempKeys.Clear();
            tempKeys.AddRange(visibilityAnim.Keys);
            foreach (string k in tempKeys)
            {
                float current = visibilityAnim[k];
                float targetVal = GetVisibilityTargetValue(k);
                visibilityAnim[k] = Mathf.MoveTowards(current, targetVal, Time.deltaTime * 4.5f);
            }

            // Stars for starry theme
            if (Settings.CurrentTheme == ThemeOption.Starry)
            {
                UpdateStars();
            }
            else if (stars.Count > 0)
            {
                stars.Clear(); // Clear stars when not using Starry theme
            }
        }

        private static float GetVisibilityTargetValue(string key)
        {
            try
            {
                switch (key)
                {
                    case "pull_settings": return Settings.PullModEnabled ? 1f : 0f;
                    case "tag_aura": return Settings.TagAuraEnabled ? 1f : 0f;
                    case "hitbox_options": return Settings.HitboxExpanderEnabled ? 1f : 0f;
                    case "psa_options": return Settings.PSAEnabled ? 1f : 0f;
                    case "highjump_options": return Settings.HighJumpEnabled ? 1f : 0f;
                    case "recroom_options": return Settings.RecRoomEnabled ? 1f : 0f;
                    case "pidego_options": return Settings.PidegoEnabled ? 1f : 0f;
                    case "velmax_options": return Settings.VelmaxEnabled ? 1f : 0f;
                    case "bypass_wsb": return Settings.WorldScaleBypassEnabled ? 1f : 0f;
                    case "bypass_velcheck": return Settings.VelmaxCheckEnabled ? 1f : 0f;
                    case "dcmod_options": return Settings.DCModEnabled ? 1f : 0f;
                    case "preds_options": return Settings.PredsEnabled ? 1f : 0f;
                    default: return 0f;
                }
            }
            catch { return 0f; }
        }

        private static void EnsureVis(string key)
        {
            if (!visibilityAnim.ContainsKey(key)) visibilityAnim[key] = 0f;
        }

        private static void UpdateStars()
        {
            starSpawnTimer += Time.deltaTime;
            if (starSpawnTimer > 0.1f)
            {
                starSpawnTimer = 0f;
                if (stars.Count < 50)
                {
                    stars.Add(new Star
                    {
                        position = new Vector2(UnityEngine.Random.Range(0f, mainWindow.width), -5f),
                        speed = UnityEngine.Random.Range(30f, 80f),
                        size = UnityEngine.Random.Range(2f, 4f),
                        alpha = UnityEngine.Random.Range(0.3f, 0.9f)
                    });
                }
            }

            for (int i = stars.Count - 1; i >= 0; i--)
            {
                stars[i].position.y += stars[i].speed * Time.deltaTime;
                if (stars[i].position.y > mainWindow.height + 10f) stars.RemoveAt(i);
            }
        }

        private static void DrawStars()
        {
            if (Settings.CurrentTheme != ThemeOption.Starry) return;

            Texture2D starTex = MakeCircleTex(8, Color.white);
            foreach (Star s in stars)
            {
                UnityEngine.GUI.color = new Color(1f, 1f, 1f, s.alpha);
                UnityEngine.GUI.DrawTexture(new Rect(s.position.x - s.size / 2, s.position.y - s.size / 2, s.size, s.size), starTex);
            }
            UnityEngine.GUI.color = Color.white;
        }

        private static void DrawMainWindow(int windowId)
        {
            DrawStars();

            Color savedColor = UnityEngine.GUI.color;
            float alpha = Mathf.Clamp01(menuOpenAnim);
            UnityEngine.GUI.color = new Color(savedColor.r, savedColor.g, savedColor.b, savedColor.a * alpha);

            // Header with title and gear button
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (animatedHeaderStyle == null) animatedHeaderStyle = new GUIStyle(headerStyle);
            animatedHeaderStyle.normal.textColor = Color.Lerp(primaryColor, accentColor, titleGlow);
            GUILayout.Label("PARALLAX", animatedHeaderStyle);

            GUILayout.FlexibleSpace();

            // Theme button
            if (GUILayout.Button("◐", gearButtonStyle, GUILayout.Width(30), GUILayout.Height(30)))
            {
                int next = ((int)Settings.CurrentTheme + 1) % 8;
                Settings.CurrentTheme = (ThemeOption)next;
                stylesInitialized = false;
                texCache.Clear();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            DrawRedLine();
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();

            // Sidebar
            int leftWidth = 160;
            GUILayout.BeginVertical(GUILayout.Width(leftWidth));
            GUILayout.Space(6);

            for (int i = 0; i < tabNames.Length; i++)
            {
                GUIStyle tabStyle = (Settings.CurrentTab == i) ? tabActiveStyle : tabInactiveStyle;
                string tabText = tabIcons[i] + "  " + tabNames[i];

                if (GUILayout.Button(tabText, tabStyle, GUILayout.Height(38), GUILayout.Width(leftWidth - 15)))
                {
                    Settings.CurrentTab = i;
                }
                GUILayout.Space(3);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            // Content area
            GUILayout.BeginVertical(boxStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            Color contentSaved = UnityEngine.GUI.color;
            UnityEngine.GUI.color = new Color(contentSaved.r, contentSaved.g, contentSaved.b, contentSaved.a * contentFade * alpha);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUIStyle.none, GUIStyle.none);

            switch (Settings.CurrentTab)
            {
                case 0: DrawPullTab(); break;
                case 1: DrawWallWalkTab(); break;
                case 2: DrawTagTab(); break;
                case 3: DrawPSATab(); break;
                case 4: DrawSpeedBoostTab(); break;
                case 5: DrawWeatherTab(); break;
                case 6: DrawBypassTab(); break;
                case 7: DrawSafetyTab(); break;
                case 8: DrawControllerModsTab(); break;
                case 9: DrawESPTab(); break;
                case 10: DrawConfigTab(); break;
                case 11: DrawCreditsTab(); break;
            }

            GUILayout.EndScrollView();
            UnityEngine.GUI.color = contentSaved;
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            DrawRedLine();
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string themeText = Settings.CurrentTheme.ToString();
            GUILayout.Label($"Parallax v2.0 | Theme: {themeText} | Press [INSERT] to toggle menu", footerStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            UnityEngine.GUI.color = savedColor;
            UnityEngine.GUI.DragWindow(new Rect(0, 0, mainWindow.width, 50));
        }

        private static void DrawRedLine()
        {
            Texture2D lineTex = MakeGradientTex(256, 2, new Color(0, 0, 0, 0), primaryColor);
            redBarTimer += Time.deltaTime * 0.5f;
            
            Rect lineRect = GUILayoutUtility.GetRect(mainWindow.width - 40, 2);
            float lineWidth = lineRect.width * 0.4f;
            float travelRange = lineRect.width - lineWidth;
            float offset = Mathf.PingPong(redBarTimer * 100f, travelRange);
            
            // Draw centered moving line
            UnityEngine.GUI.DrawTexture(new Rect(lineRect.x + offset, lineRect.y, lineWidth, 2), lineTex);
        }

        private static void DrawSectionHeader(string text)
        {
            GUILayout.Space(6);
            GUILayout.Label("◆ " + text, subHeaderStyle);
            GUILayout.Space(4);
        }

        private static bool DrawToggle(string label, bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, labelStyle, GUILayout.Width(200));
            GUILayout.FlexibleSpace();

            GUIStyle style = value ? toggleOnStyle : toggleOffStyle;
            string text = value ? "● ON" : "○ OFF";

            if (GUILayout.Button(text, style, GUILayout.Width(86), GUILayout.Height(28)))
            {
                value = !value;
                toggleFlash[label] = 1f;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            return value;
        }

        private static float DrawSlider(string label, float value, float min, float max, string format = "F2")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, labelStyle, GUILayout.Width(160));
            GUILayout.FlexibleSpace();

            if (valueDisplayStyle == null)
            {
                valueDisplayStyle = new GUIStyle(labelStyle);
                valueDisplayStyle.fontStyle = FontStyle.Bold;
                valueDisplayStyle.alignment = TextAnchor.MiddleRight;
            }
            valueDisplayStyle.normal.textColor = accentColor;
            GUILayout.Label(value.ToString(format), valueDisplayStyle, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            value = GUILayout.HorizontalSlider(value, min, max, sliderStyle, sliderThumbStyle, GUILayout.Height(16));
            GUILayout.Space(8);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            return value;
        }

        private static void DrawInfoBox(string text)
        {
            GUILayout.BeginVertical(sectionBoxStyle);
            if (infoBoxStyle == null)
            {
                infoBoxStyle = new GUIStyle(labelStyle);
                infoBoxStyle.fontSize = 11;
                infoBoxStyle.alignment = TextAnchor.MiddleCenter;
            }
            infoBoxStyle.normal.textColor = textDim;
            GUILayout.Label(text, infoBoxStyle);
            GUILayout.EndVertical();
        }

        private static KeybindOption DrawKeybindSelector(string label, KeybindOption current)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ":", labelStyle, GUILayout.Width(120));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Settings.GetKeybindName(current), buttonStyle, GUILayout.Width(120), GUILayout.Height(28)))
            {
                int next = ((int)current % 10) + 1;
                current = (KeybindOption)next;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            return current;
        }

        // ==================== TAB DRAWING METHODS ====================

        private static void DrawPullTab()
        {
            DrawSectionHeader("PULL MOD (Mintys)");

            Settings.PullModEnabled = DrawToggle("Pullmod (Mintys)", Settings.PullModEnabled);
            EnsureVis("pull_settings");
            float vis = visibilityAnim["pull_settings"];

            if (vis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * vis);

                GUILayout.BeginVertical(sectionBoxStyle);
                Settings.PullObstacleDetection = DrawToggle("Raycast", Settings.PullObstacleDetection);
                Settings.WallPullEnabled = DrawToggle("Wall Pull (Double Walls)", Settings.WallPullEnabled);
                Settings.UseD1vineTagFreeze = DrawToggle("D1vine Tag Freeze", Settings.UseD1vineTagFreeze);
                GUILayout.EndVertical();

                if (Settings.PullObstacleDetection)
                    Settings.PullObstacleRadius = DrawSlider("Detection Radius", Settings.PullObstacleRadius, 0.1f, 2f);

                if (Settings.UseD1vineTagFreeze)
                {
                    Settings.D1vineTagFreezeMultiplier = DrawSlider("Tag Freeze Multiplier", Settings.D1vineTagFreezeMultiplier, 1f, 15f);
                    DrawInfoBox("Alternative tag freeze method - moves you when frozen");
                }

                if (Settings.WallPullEnabled)
                {
                    DrawInfoBox("Allows pulling straight up on vertical walls");
                }

                GUILayout.Space(8);
                Settings.PullStrength = DrawSlider("Strength", Settings.PullStrength, 0f, 1.5f);
                Settings.PullThreshold = DrawSlider("Threshold", Settings.PullThreshold * 100f, 0f, 100f, "F0") / 100f;
                Settings.PullTpTime = DrawSlider("TP Time (ms)", Settings.PullTpTime * 1000f, 0f, 50f, "F0") / 1000f;

                GUILayout.BeginHorizontal();
                GUILayout.Label("Activation Mode:", labelStyle, GUILayout.Width(140));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Settings.PullActivation.ToString(), buttonStyle, GUILayout.Width(140), GUILayout.Height(28)))
                {
                    int next = ((int)Settings.PullActivation + 1) % Enum.GetValues(typeof(ActivationMode)).Length;
                    Settings.PullActivation = (ActivationMode)next;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("◇ LEGIT", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PullStrength = 0.7f;
                    Settings.PullThreshold = 0.65f;
                    Settings.PullTpTime = 0.013f;
                }
                GUILayout.Space(6);
                if (GUILayout.Button("◆ SEMI-LEGIT", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PullStrength = 1f;
                    Settings.PullThreshold = 0.4f;
                    Settings.PullTpTime = 0.013f;
                }
                GUILayout.Space(6);
                if (GUILayout.Button("★ BLATANT", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PullStrength = 1f;
                    Settings.PullThreshold = 0f;
                    Settings.PullTpTime = 0.016f;
                }
                GUILayout.EndHorizontal();

                UnityEngine.GUI.color = saved;
            }
        }

        private static void DrawWallWalkTab()
        {
            DrawSectionHeader("WALL WALK");

            Settings.WallWalkEnabled = DrawToggle("Enable Wall Walk", Settings.WallWalkEnabled);

            if (Settings.WallWalkEnabled)
            {
                GUILayout.Space(8);
                Settings.WallWalkDistance = DrawSlider("Detection Distance", Settings.WallWalkDistance, 0.1f, 1.5f);
                Settings.WallWalkPower = DrawSlider("Push Power", Settings.WallWalkPower, 0.5f, 5f);

                GUILayout.Space(8);
                GUILayout.BeginVertical(sectionBoxStyle);
                Settings.WallWalkLeftHand = DrawToggle("Left Hand", Settings.WallWalkLeftHand);
                Settings.WallWalkRightHand = DrawToggle("Right Hand", Settings.WallWalkRightHand);
                GUILayout.EndVertical();

                GUILayout.Space(10);
                DrawInfoBox("Hold GRIP near wall to activate");
            }
        }

        private static void DrawTagTab()
        {
            DrawSectionHeader("TAG MODS");

            GUILayout.BeginVertical(sectionBoxStyle);
            Settings.DCFlickEnabled = DrawToggle("DC Flick", Settings.DCFlickEnabled);
            if (Settings.DCFlickEnabled)
                GUILayout.Label("Tags players near your hands (0.3 range)", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(6);
            Settings.TagAuraEnabled = DrawToggle("Tag Aura", Settings.TagAuraEnabled);
            EnsureVis("tag_aura");
            float auraVis = visibilityAnim["tag_aura"];
            if (auraVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * auraVis);
                Settings.TagAuraDistance = DrawSlider("Aura Distance", Settings.TagAuraDistance, 0.5f, 15f);
                GUILayout.Label("Hold RIGHT JOYSTICK to tag nearby players", labelStyle);
                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(6);
            Settings.HitboxExpanderEnabled = DrawToggle("Hitbox Expander", Settings.HitboxExpanderEnabled);
            EnsureVis("hitbox_options");
            float hitVis = visibilityAnim["hitbox_options"];
            if (hitVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * hitVis);
                Settings.HitboxExpanderSize = DrawSlider("Hitbox Size", Settings.HitboxExpanderSize, 0.3f, 2f);
                Settings.HitboxExpanderOpacity = DrawSlider("Opacity", Settings.HitboxExpanderOpacity, 0.1f, 1f);
                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(10);
            DrawInfoBox("Only works when infected");
        }

        private static void DrawPSATab()
        {
            DrawSectionHeader("PSA MODS");

            Settings.PSAEnabled = DrawToggle("PSA", Settings.PSAEnabled);
            EnsureVis("psa_options");
            float psaVis = visibilityAnim["psa_options"];
            if (psaVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * psaVis);
                Settings.PSASpeed = DrawSlider("Speed", Settings.PSASpeed, 0.1f, 10f);
                Settings.PSAKeybind = DrawKeybindSelector("Keybind", Settings.PSAKeybind);
                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(6);
            Settings.HighJumpEnabled = DrawToggle("High Jump", Settings.HighJumpEnabled);
            EnsureVis("highjump_options");
            float hjVis = visibilityAnim["highjump_options"];
            if (hjVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * hjVis);
                Settings.HighJumpSpeed = DrawSlider("Speed", Settings.HighJumpSpeed, 0.5f, 20f);
                Settings.HighJumpKeybind = DrawKeybindSelector("Keybind", Settings.HighJumpKeybind);
                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(6);
            Settings.RecRoomEnabled = DrawToggle("Rec Room", Settings.RecRoomEnabled);
            EnsureVis("recroom_options");
            float rrVis = visibilityAnim["recroom_options"];
            if (rrVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * rrVis);
                Settings.RecRoomSpeed = DrawSlider("Speed", Settings.RecRoomSpeed, 0.1f, 5f);
                Settings.RecRoomForwardKeybind = DrawKeybindSelector("Forward", Settings.RecRoomForwardKeybind);
                Settings.RecRoomBackwardKeybind = DrawKeybindSelector("Backward", Settings.RecRoomBackwardKeybind);
                UnityEngine.GUI.color = saved;
            }
        }

        private static void DrawSpeedBoostTab()
        {
            DrawSectionHeader("PIDEGO (Surface Boosts)");

            Settings.PidegoEnabled = DrawToggle("Enable Pidego", Settings.PidegoEnabled);
            EnsureVis("pidego_options");
            float pVis = visibilityAnim["pidego_options"];

            if (pVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * pVis);

                GUILayout.BeginVertical(sectionBoxStyle);
                Settings.PidegoGroundEnabled = DrawToggle("Ground Boost", Settings.PidegoGroundEnabled);
                if (Settings.PidegoGroundEnabled)
                    Settings.PidegoGroundMultiplier = DrawSlider("Ground Multiplier", Settings.PidegoGroundMultiplier, 1f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(sectionBoxStyle);
                Settings.PidegoWallEnabled = DrawToggle("Wall Boost", Settings.PidegoWallEnabled);
                if (Settings.PidegoWallEnabled)
                    Settings.PidegoWallMultiplier = DrawSlider("Wall Multiplier", Settings.PidegoWallMultiplier, 1f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(sectionBoxStyle);
                Settings.PidegoDoubleWallEnabled = DrawToggle("Double Wall Boost", Settings.PidegoDoubleWallEnabled);
                if (Settings.PidegoDoubleWallEnabled)
                    Settings.PidegoDoubleWallMultiplier = DrawSlider("Double Wall Multiplier", Settings.PidegoDoubleWallMultiplier, 1f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(sectionBoxStyle);
                Settings.PidegoSlipEnabled = DrawToggle("Slip Slap Boost", Settings.PidegoSlipEnabled);
                if (Settings.PidegoSlipEnabled)
                    Settings.PidegoSlipMultiplier = DrawSlider("Slip Multiplier", Settings.PidegoSlipMultiplier, 1f, 2f);
                GUILayout.EndVertical();

                GUILayout.Space(8);
                if (GUILayout.Button("Reset All to Default", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PidegoGroundMultiplier = 1f;
                    Settings.PidegoWallMultiplier = 1f;
                    Settings.PidegoDoubleWallMultiplier = 1f;
                    Settings.PidegoSlipMultiplier = 1f;
                }

                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(15);
            DrawSectionHeader("VELMAX (Global Speed)");

            Settings.VelmaxEnabled = DrawToggle("Enable Velmax", Settings.VelmaxEnabled);
            EnsureVis("velmax_options");
            float vVis = visibilityAnim["velmax_options"];

            if (vVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * vVis);
                Settings.VelmaxMultiplier = DrawSlider("Speed Multiplier", Settings.VelmaxMultiplier, 1f, 2f);
                DrawInfoBox("Applies to all surfaces in Forest");
                UnityEngine.GUI.color = saved;
            }
        }

        private static void DrawWeatherTab()
        {
            DrawSectionHeader("TIME & WEATHER");

            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("Time Changer", subHeaderStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("◇ " + WeatherMods.GetCurrentTimeName(), buttonStyle, GUILayout.Height(36)))
                WeatherMods.CycleTimeOfDay();

            GUILayout.Space(6);
            GUILayout.Label("Cycle: None → Day → Dawn → Night → Nightfall → Midnight", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("Weather Changer", subHeaderStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("◇ " + WeatherMods.GetCurrentWeatherName(), buttonStyle, GUILayout.Height(36)))
                WeatherMods.CycleWeather();

            GUILayout.Space(6);
            GUILayout.Label("Cycle: None → Rain → Clear", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(12);
            DrawInfoBox("Changes apply instantly");
        }

        private static void DrawBypassTab()
        {
            DrawSectionHeader("BYPASS MODS");

            Settings.WorldScaleBypassEnabled = DrawToggle("World Scale Bypass", Settings.WorldScaleBypassEnabled);
            EnsureVis("bypass_wsb");
            float wsbVis = visibilityAnim["bypass_wsb"];
            if (wsbVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * wsbVis);
                
                // Use ActivationMode selector (same as PullMod)
                GUILayout.BeginHorizontal();
                GUILayout.Label("Activation:", labelStyle, GUILayout.Width(120));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Settings.WorldScaleBypassActivation.ToString(), buttonStyle, GUILayout.Width(140), GUILayout.Height(28)))
                {
                    int next = ((int)Settings.WorldScaleBypassActivation + 1) % Enum.GetValues(typeof(ActivationMode)).Length;
                    Settings.WorldScaleBypassActivation = (ActivationMode)next;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
                GUILayout.Label("Moves hands to body when held", labelStyle);
                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(6);
            Settings.VelmaxCheckEnabled = DrawToggle("Velmax Check Bypass", Settings.VelmaxCheckEnabled);
            EnsureVis("bypass_velcheck");
            float vcVis = visibilityAnim["bypass_velcheck"];
            if (vcVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * vcVis);
                
                // Use ActivationMode selector (same as PullMod)
                GUILayout.BeginHorizontal();
                GUILayout.Label("Activation:", labelStyle, GUILayout.Width(120));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Settings.VelmaxCheckActivation.ToString(), buttonStyle, GUILayout.Width(140), GUILayout.Height(28)))
                {
                    int next = ((int)Settings.VelmaxCheckActivation + 1) % Enum.GetValues(typeof(ActivationMode)).Length;
                    Settings.VelmaxCheckActivation = (ActivationMode)next;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
                GUILayout.Label("Resets movement state when held", labelStyle);
                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(6);
            Settings.ForceTagFreezeEnabled = DrawToggle("Force Tag Freeze", Settings.ForceTagFreezeEnabled);
            if (Settings.ForceTagFreezeEnabled)
                GUILayout.Label("Forces tag freeze state (disables movement)", labelStyle);

            GUILayout.Space(12);
            DrawInfoBox("Hold activation button to activate bypass");
        }

        private static void DrawSafetyTab()
        {
            DrawSectionHeader("SAFETY FEATURES");

            GUILayout.BeginVertical(sectionBoxStyle);
            Settings.AntiReportEnabled = DrawToggle("Anti Report", Settings.AntiReportEnabled);
            if (Settings.AntiReportEnabled)
                GUILayout.Label("Disconnects you when someone tries to report", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(15);

            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("PANIC BUTTON", subHeaderStyle);
            GUILayout.Space(8);
            GUILayout.Label("Instantly disables ALL mods and closes GUI", labelStyle);
            GUILayout.Space(8);

            if (panicButtonStyle == null)
            {
                panicButtonStyle = new GUIStyle(buttonStyle);
                panicButtonStyle.normal.background = MakeTex(64, 28, new Color(0.8f, 0.1f, 0.1f, 1f));
                panicButtonStyle.normal.textColor = textWhite;
                panicButtonStyle.fontSize = 14;
            }

            if (GUILayout.Button("⚠ PANIC ⚠", panicButtonStyle, GUILayout.Height(45)))
            {
                Settings.TriggerPanic();
            }
            GUILayout.EndVertical();

            GUILayout.Space(15);
            DrawInfoBox("Use panic if you think you're about to get caught");
        }

        private static void DrawControllerModsTab()
        {
            DrawSectionHeader("DC MOD");

            Settings.DCModEnabled = DrawToggle("Enable DC Mod", Settings.DCModEnabled);
            EnsureVis("dcmod_options");
            float dcVis = visibilityAnim["dcmod_options"];

            if (dcVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * dcVis);

                Settings.DCModKeybind = DrawKeybindSelector("Activation", Settings.DCModKeybind);

                GUILayout.Space(6);
                Settings.DCModLeftDistance = DrawSlider("Left Distance", Settings.DCModLeftDistance, 0f, 10f);
                Settings.DCModRightDistance = DrawSlider("Right Distance", Settings.DCModRightDistance, 0f, 10f);
                Settings.DCModLeftThreshold = DrawSlider("Left Threshold", Settings.DCModLeftThreshold, 0f, 1f);
                Settings.DCModRightThreshold = DrawSlider("Right Threshold", Settings.DCModRightThreshold, 0f, 1f);
                Settings.DCModLeftDuration = (int)DrawSlider("Left Duration (ticks)", Settings.DCModLeftDuration, 1, 100, "F0");
                Settings.DCModRightDuration = (int)DrawSlider("Right Duration (ticks)", Settings.DCModRightDuration, 1, 100, "F0");

                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(15);
            DrawSectionHeader("PREDS (Rotation Smoothing)");

            Settings.PredsEnabled = DrawToggle("Enable Preds", Settings.PredsEnabled);
            EnsureVis("preds_options");
            float predVis = visibilityAnim["preds_options"];

            if (predVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * predVis);
                Settings.PredsSmoothing = DrawSlider("Smoothing Factor", Settings.PredsSmoothing, 0.01f, 1f);
                DrawInfoBox("Smooths controller rotation for better prediction");
                UnityEngine.GUI.color = saved;
            }
        }

        private static void DrawESPTab()
        {
            DrawSectionHeader("ESP SETTINGS");

            Settings.ESPEnabled = DrawToggle("Enable ESP", Settings.ESPEnabled);

            if (!Settings.ESPEnabled)
            {
                GUILayout.Space(10);
                DrawInfoBox("Enable ESP to access all visual features");
                return;
            }

            GUILayout.Space(15);

            // Color Picker
            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("ESP Color", subHeaderStyle);
            GUILayout.Space(8);

            // Draw color picker
            DrawColorPicker();

            GUILayout.EndVertical();

            GUILayout.Space(15);
            DrawSectionHeader("VISUAL FEATURES");

            // Tracers
            Settings.TracersEnabled = DrawToggle("Tracers", Settings.TracersEnabled);
            if (Settings.TracersEnabled)
            {
                DrawInfoBox("Lines from your hand to other players");
            }

            GUILayout.Space(8);

            // Corner ESP
            Settings.CornerESPEnabled = DrawToggle("Corner ESP", Settings.CornerESPEnabled);
            if (Settings.CornerESPEnabled)
            {
                DrawInfoBox("Corner brackets around players");
            }

            GUILayout.Space(8);

            // Hitboxes
            Settings.HitboxesEnabled = DrawToggle("Hitboxes", Settings.HitboxesEnabled);
            if (Settings.HitboxesEnabled)
            {
                DrawInfoBox("Box around player heads");
            }
        }

        // Cached color picker textures (generated once)
        private static Texture2D satValTexture = null;
        private static Texture2D hueBarTexture = null;
        private static Texture2D previewTexture = null;
        private static float lastHueForTexture = -1f;
        private static Color lastPreviewColor = Color.black;
        private static Texture2D whiteTex = null;
        private static Texture2D blackTex = null;

        private static void DrawColorPicker()
        {
            // Only draw if ESP tab is active
            if (Settings.CurrentTab != 9) return;

            float pickerSize = 120f;
            float hueBarWidth = 20f;
            float spacing = 10f;
            float previewSize = 30f;

            Color currentColor = Color.HSVToRGB(Settings.ESPColorHue, Settings.ESPColorSat, Settings.ESPColorVal);
            
            GUILayout.BeginHorizontal();
            
            // Saturation/Value square
            Rect svRect = GUILayoutUtility.GetRect(pickerSize, pickerSize);
            
            // Generate sat/val texture only when hue changes
            if (satValTexture == null || Mathf.Abs(lastHueForTexture - Settings.ESPColorHue) > 0.001f)
            {
                if (satValTexture != null) UnityEngine.Object.Destroy(satValTexture);
                satValTexture = GenerateSatValTexture(32, 32, Settings.ESPColorHue);
                lastHueForTexture = Settings.ESPColorHue;
            }
            
            UnityEngine.GUI.DrawTexture(svRect, satValTexture);
            
            // Handle clicks on saturation/value square
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
            {
                if (svRect.Contains(Event.current.mousePosition))
                {
                    Vector2 localPos = Event.current.mousePosition - new Vector2(svRect.x, svRect.y);
                    Settings.ESPColorSat = Mathf.Clamp01(localPos.x / svRect.width);
                    Settings.ESPColorVal = Mathf.Clamp01(1f - (localPos.y / svRect.height));
                    Event.current.Use();
                }
            }
            
            // Draw crosshair
            float crossX = svRect.x + Settings.ESPColorSat * svRect.width;
            float crossY = svRect.y + (1f - Settings.ESPColorVal) * svRect.height;
            DrawCrosshairCached(crossX, crossY);

            GUILayout.Space(spacing);

            // Hue bar - use cached texture
            Rect hueRect = GUILayoutUtility.GetRect(hueBarWidth, pickerSize);
            if (hueBarTexture == null)
            {
                hueBarTexture = GenerateHueBarTexture(1, 64);
            }
            UnityEngine.GUI.DrawTexture(hueRect, hueBarTexture);
            
            // Handle clicks on hue bar
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
            {
                if (hueRect.Contains(Event.current.mousePosition))
                {
                    float localY = Event.current.mousePosition.y - hueRect.y;
                    Settings.ESPColorHue = Mathf.Clamp01(localY / hueRect.height);
                    Event.current.Use();
                }
            }
            
            // Draw hue indicator using cached white texture
            if (whiteTex == null) whiteTex = MakeTex(1, 1, Color.white);
            float hueIndicatorY = hueRect.y + Settings.ESPColorHue * hueRect.height;
            UnityEngine.GUI.DrawTexture(new Rect(hueRect.x - 2, hueIndicatorY - 2, hueRect.width + 4, 4), whiteTex);

            GUILayout.Space(spacing);

            // Color preview - only update texture when color changes
            GUILayout.BeginVertical();
            GUILayout.Label("Preview:", labelStyle);
            
            if (previewTexture == null || lastPreviewColor != currentColor)
            {
                if (previewTexture != null) UnityEngine.Object.Destroy(previewTexture);
                previewTexture = new Texture2D(1, 1);
                previewTexture.SetPixel(0, 0, currentColor);
                previewTexture.Apply();
                previewTexture.hideFlags = HideFlags.HideAndDontSave;
                lastPreviewColor = currentColor;
            }
            
            if (colorPreviewStyle == null) colorPreviewStyle = new GUIStyle();
            colorPreviewStyle.normal.background = previewTexture;
            GUILayout.Box("", colorPreviewStyle, GUILayout.Width(previewSize), GUILayout.Height(previewSize));
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static Texture2D GenerateSatValTexture(int width, int height, float hue)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.hideFlags = HideFlags.HideAndDontSave;

            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float val = 1f - (float)y / (height - 1);
                for (int x = 0; x < width; x++)
                {
                    float sat = (float)x / (width - 1);
                    pixels[y * width + x] = Color.HSVToRGB(hue, sat, val);
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateHueBarTexture(int width, int height)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.hideFlags = HideFlags.HideAndDontSave;

            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float hue = (float)y / (height - 1);
                Color c = Color.HSVToRGB(hue, 1f, 1f);
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = c;
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static void DrawCrosshairCached(float x, float y)
        {
            if (whiteTex == null) whiteTex = MakeTex(1, 1, Color.white);
            if (blackTex == null) blackTex = MakeTex(1, 1, Color.black);
            
            // Outer black
            UnityEngine.GUI.DrawTexture(new Rect(x - 6, y - 1, 12, 2), blackTex);
            UnityEngine.GUI.DrawTexture(new Rect(x - 1, y - 6, 2, 12), blackTex);
            
            // Inner white
            UnityEngine.GUI.DrawTexture(new Rect(x - 5, y, 10, 1), whiteTex);
            UnityEngine.GUI.DrawTexture(new Rect(x, y - 5, 1, 10), whiteTex);
        }

        private static void DrawConfigTab()
        {
            DrawSectionHeader("CONFIGURATION");

            GUILayout.Space(10);

            // Config file info
            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("Config File Location:", subHeaderStyle);
            GUILayout.Space(4);
            
            GUIStyle pathStyle = new GUIStyle(labelStyle);
            pathStyle.wordWrap = true;
            pathStyle.fontSize = 11;
            GUILayout.Label(ConfigManager.GetConfigPath(), pathStyle);
            
            GUILayout.Space(8);
            
            bool configExists = ConfigManager.ConfigExists();
            GUILayout.Label("Config Status: " + (configExists ? "Found" : "Not Found"), labelStyle);
            GUILayout.Label("Last Save: " + ConfigManager.LastSaveTime, labelStyle);
            GUILayout.Label("Last Load: " + ConfigManager.LastLoadTime, labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(15);
            DrawSectionHeader("SAVE / LOAD");

            GUILayout.Space(10);

            // Save button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("💾  SAVE CONFIG", buttonStyle, GUILayout.Height(40)))
            {
                ConfigManager.SaveConfig();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            DrawInfoBox("Saves all current settings to config file");

            GUILayout.Space(15);

            // Load button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("📂  LOAD CONFIG", buttonStyle, GUILayout.Height(40)))
            {
                ConfigManager.LoadConfig();
                stylesInitialized = false;
                texCache.Clear();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            DrawInfoBox("Loads settings from config file");
        }

        private static void DrawCreditsTab()
        {
            DrawSectionHeader("DEVELOPMENT TEAM");

            GUILayout.Space(6);

            // Techfor1
            GUILayout.BeginVertical(creditCardStyle);
            GUILayout.BeginHorizontal();
            GUIStyle goldName = new GUIStyle(labelStyle);
            goldName.normal.textColor = new Color(1f, 0.85f, 0.3f, 1f);
            goldName.fontStyle = FontStyle.Bold;
            goldName.fontSize = 18;
            GUILayout.Label("♛ Techfor1", goldName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("Developer & Owner", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(6);

            // FTSyxcal
            GUILayout.BeginVertical(creditCardStyle);
            GUILayout.BeginHorizontal();
            GUIStyle goldName2 = new GUIStyle(labelStyle);
            goldName2.normal.textColor = new Color(1f, 0.85f, 0.3f, 1f);
            goldName2.fontStyle = FontStyle.Bold;
            goldName2.fontSize = 18;
            GUILayout.Label("♛ FTSyxcal", goldName2);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("Developer & Co-Owner", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(6);

            // Vortex
            GUILayout.BeginVertical(creditCardStyle);
            GUILayout.BeginHorizontal();
            GUIStyle blueName = new GUIStyle(labelStyle);
            blueName.normal.textColor = new Color(0.4f, 0.8f, 1f, 1f);
            blueName.fontStyle = FontStyle.Bold;
            blueName.fontSize = 18;
            GUILayout.Label("★ Vortex", blueName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("Developer", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(6);

            // Cr1pt
            GUILayout.BeginVertical(creditCardStyle);
            GUILayout.BeginHorizontal();
            GUIStyle blueName2 = new GUIStyle(labelStyle);
            blueName2.normal.textColor = new Color(0.4f, 0.8f, 1f, 1f);
            blueName2.fontStyle = FontStyle.Bold;
            blueName2.fontSize = 18;
            GUILayout.Label("★ Cr1pt", blueName2);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("Developer", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(15);

            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("Special Thanks", subHeaderStyle);
            GUILayout.Space(6);
            GUIStyle thanksStyle = new GUIStyle(labelStyle);
            thanksStyle.alignment = TextAnchor.MiddleCenter;
            thanksStyle.normal.textColor = textGray;
            GUILayout.Label("To all our users and supporters!", thanksStyle);
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(sectionBoxStyle);
            GUIStyle versionStyle = new GUIStyle(labelStyle);
            versionStyle.alignment = TextAnchor.MiddleCenter;
            versionStyle.normal.textColor = accentColor;
            versionStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("Parallax v2.0", versionStyle);
            GUILayout.Label("Edition - 2026", footerStyle);
            GUILayout.EndVertical();
        }
    }
}




