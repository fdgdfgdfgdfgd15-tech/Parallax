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
        
        private static GUIStyle animatedHeaderStyle;
        private static GUIStyle valueDisplayStyle;
        private static GUIStyle infoBoxStyle;
        private static GUIStyle colorPreviewStyle;

        private static Color primaryColor;
        private static Color darkColor;
        private static Color accentColor;
        private static Color bgColor;
        private static Color panelColor;
        private static Color cardColor;
        private static Color textWhite = new Color(1f, 1f, 1f, 1f);
        private static Color textGray = new Color(0.7f, 0.7f, 0.7f, 0.9f);
        private static Color textDim = new Color(0.5f, 0.5f, 0.5f, 0.7f);

        private static string[] tabNames = { "PULL", "WALL", "TAG", "PSA", "SPEED", "WEATHER", "GAMEPLAY", "ESP", "BYPASS", "ROOM", "CONFIG", "CREDITS" };
        private static string[] tabIcons = { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
        private static bool stylesInitialized = false;

        private static float pulseTimer = 0f;
        private static float glowIntensity = 0f;
        private static float titleGlow = 0f;
        private static float menuOpenAnim = 0f;
        private static float contentFade = 1f;
        private static int prevTab = 0;
        private static float redBarTimer = 0f;

        private static List<Star> stars = new List<Star>();
        private static float starSpawnTimer = 0f;

        private static float rainbowHue = 0f;

        private static Dictionary<string, float> toggleFlash = new Dictionary<string, float>();
        private static Dictionary<string, float> elementPressAnim = new Dictionary<string, float>();
        private static Dictionary<string, float> visibilityAnim = new Dictionary<string, float>();
        private static Dictionary<string, float> dropdownAnimations = new Dictionary<string, float>();

        private static Dictionary<string, Texture2D> texCache = new Dictionary<string, Texture2D>();

        private static List<string> tempKeys;
        
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
            bool themeChanged = (lastTheme != Settings.CurrentTheme);
            
            if (themeChanged)
            {
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
                    case ThemeOption.Midnight:
                        primaryColor = new Color(0.5f, 0.3f, 0.9f, 1f);
                        darkColor = new Color(0.3f, 0.15f, 0.6f, 1f);
                        accentColor = new Color(0.7f, 0.5f, 1f, 1f);
                        bgColor = new Color(0.02f, 0.02f, 0.06f, 0.97f);
                        panelColor = new Color(0.05f, 0.04f, 0.12f, 0.85f);
                        cardColor = new Color(0.08f, 0.06f, 0.18f, 0.9f);
                        break;
                    case ThemeOption.Parallax:
                    default:
                        primaryColor = new Color(0.85f, 0.12f, 0.12f, 1f);
                        darkColor = new Color(0.55f, 0.05f, 0.05f, 1f);
                        accentColor = new Color(1f, 0.3f, 0.3f, 1f);
                        bgColor = new Color(0.04f, 0.04f, 0.06f, 0.97f);
                        panelColor = new Color(0.08f, 0.08f, 0.10f, 0.85f);
                        cardColor = new Color(0.12f, 0.12f, 0.14f, 0.9f);
                        break;
                }
                
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
                    
                    bool inCorner = false;
                    float dist = 0f;
                    
                    if (x < radius && y < radius)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius));
                        inCorner = true;
                    }
                    else if (x >= texSize - radius && y < radius)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), new Vector2(texSize - radius - 1, radius));
                        inCorner = true;
                    }
                    else if (x < radius && y >= texSize - radius)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, texSize - radius - 1));
                        inCorner = true;
                    }
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
            tabActiveStyle.normal.textColor = (Settings.CurrentTheme == ThemeOption.Starry) ? Color.black : textWhite;
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
            toggleOnStyle.normal.textColor = (Settings.CurrentTheme == ThemeOption.Starry) ? Color.black : textWhite;
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
            if (Time.time - lastTextureCheck < 5f) return;
            lastTextureCheck = Time.time;

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

            rainbowHue = (rainbowHue + Time.deltaTime * 0.1f) % 1f;

            if (tempKeys == null) tempKeys = new List<string>(16);
            tempKeys.Clear();
            tempKeys.AddRange(toggleFlash.Keys);
            foreach (string k in tempKeys)
            {
                toggleFlash[k] = Mathf.Max(0f, toggleFlash[k] - Time.deltaTime * 2.5f);
                if (toggleFlash[k] <= 0f) toggleFlash.Remove(k);
            }

            tempKeys.Clear();
            tempKeys.AddRange(elementPressAnim.Keys);
            foreach (string k in tempKeys)
            {
                elementPressAnim[k] = Mathf.Max(0f, elementPressAnim[k] - Time.deltaTime * 6f);
                if (elementPressAnim[k] <= 0f) elementPressAnim.Remove(k);
            }

            tempKeys.Clear();
            tempKeys.AddRange(visibilityAnim.Keys);
            foreach (string k in tempKeys)
            {
                float current = visibilityAnim[k];
                float targetVal = GetVisibilityTargetValue(k);
                visibilityAnim[k] = Mathf.MoveTowards(current, targetVal, Time.deltaTime * 4.5f);
            }

            if (Settings.CurrentTheme == ThemeOption.Starry)
            {
                UpdateStars();
            }
            else if (stars.Count > 0)
            {
                stars.Clear();
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
                    case "velmax_options": return Settings.VelmaxEnabled ? 1f : 0f;
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

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (animatedHeaderStyle == null) animatedHeaderStyle = new GUIStyle(headerStyle);
            if (Settings.CurrentTheme == ThemeOption.Midnight)
                animatedHeaderStyle.normal.textColor = accentColor;
            else
                animatedHeaderStyle.normal.textColor = Color.Lerp(primaryColor, accentColor, titleGlow * 0.5f);
            GUILayout.Label("PARALLAX", animatedHeaderStyle);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("◐", gearButtonStyle, GUILayout.Width(30), GUILayout.Height(30)))
            {
                int next = ((int)Settings.CurrentTheme + 1) % 4;
                Settings.CurrentTheme = (ThemeOption)next;
                stylesInitialized = false;
                texCache.Clear();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            DrawRedLine();
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();

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
                case 6: DrawControllerModsTab(); break;
                case 7: DrawESPTab(); break;
                case 8: DrawBypassTab(); break;
                case 9: DrawRoomTab(); break;
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
            GUIStyle toggleLabelStyle = new GUIStyle(labelStyle);
            toggleLabelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(label, toggleLabelStyle, GUILayout.Width(200), GUILayout.Height(28));
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
            GUIStyle sliderLabelStyle = new GUIStyle(labelStyle);
            sliderLabelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(label, sliderLabelStyle, GUILayout.Width(160), GUILayout.Height(20));
            GUILayout.FlexibleSpace();

            if (valueDisplayStyle == null)
            {
                valueDisplayStyle = new GUIStyle(labelStyle);
                valueDisplayStyle.fontStyle = FontStyle.Bold;
                valueDisplayStyle.alignment = TextAnchor.MiddleRight;
            }
            valueDisplayStyle.normal.textColor = accentColor;
            GUILayout.Label(value.ToString(format), valueDisplayStyle, GUILayout.Width(60), GUILayout.Height(20));
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

        private static void DrawPullTab()
        {
            DrawSectionHeader("PULL MOD");

            Settings.PullModEnabled = DrawToggle("Enable Pull Mod", Settings.PullModEnabled);
            EnsureVis("pull_settings");
            float vis = visibilityAnim["pull_settings"];

            if (vis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * vis);

                GUILayout.BeginVertical(sectionBoxStyle);
                Settings.PullObstacleDetection = DrawToggle("Raycast", Settings.PullObstacleDetection);
                Settings.WallPullEnabled = DrawToggle("Wall Pull (Double Walls)", Settings.WallPullEnabled);
                Settings.TagFreezeTpEnabled = DrawToggle("Tag Freeze Tp", Settings.TagFreezeTpEnabled);
                GUILayout.EndVertical();

                if (Settings.PullObstacleDetection)
                    Settings.PullObstacleRadius = DrawSlider("Detection Radius", Settings.PullObstacleRadius, 0.1f, 2f);

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

            Settings.DCFlickEnabled = DrawToggle("Unpatch DC flicking", Settings.DCFlickEnabled);

            GUILayout.Space(6);
            Settings.TagAuraEnabled = DrawToggle("Tag Aura", Settings.TagAuraEnabled);
            EnsureVis("tag_aura");
            float auraVis = visibilityAnim["tag_aura"];
            if (auraVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * auraVis);
                Settings.TagAuraDistance = DrawSlider("Aura Distance", Settings.TagAuraDistance, 0.5f, 15f);
                GUILayout.Label("Right Joystick TagAura", labelStyle);
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
            DrawSectionHeader("VELMAX");

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

        private static void DrawControllerModsTab()
        {
            DrawSectionHeader("Hz Slider");
            
            Settings.HzSliderEnabled = DrawToggle("Enable Hz Slider", Settings.HzSliderEnabled);
            
            if (Settings.HzSliderEnabled)
            {
                GUILayout.Space(8);
                Settings.TargetHz = (int)DrawSlider("Target Hz", Settings.TargetHz, 1, 144, "F0");
                UnityEngine.Application.targetFrameRate = Settings.TargetHz;
            }
            
            GUILayout.Space(15);
            DrawSectionHeader("Preds Slider");

            Settings.PredsEnabled = DrawToggle("Enable Preds", Settings.PredsEnabled);
            EnsureVis("preds_options");
            float predVis = visibilityAnim["preds_options"];

            if (predVis > 0.01f)
            {
                Color saved = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(saved.r, saved.g, saved.b, saved.a * predVis);

                GUILayout.Space(8);
                Settings.PredsAmount = DrawSlider("Pred Speed", Settings.PredsAmount, 0f, 50f);

                GUILayout.Space(8);
                if (GUILayout.Button("◇ SET PREDS", buttonStyle, GUILayout.Height(32)))
                {
                }

                GUILayout.Space(10);
                GUILayout.Label("Pred Presets", subHeaderStyle);
                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Legit", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PredsAmount = Settings.PredsLegit;
                }
                GUILayout.Space(4);
                if (GUILayout.Button("Valve", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PredsAmount = Settings.PredsValve;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(4);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("RiftS", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PredsAmount = Settings.PredsRiftS;
                }
                GUILayout.Space(4);
                if (GUILayout.Button("Pico", buttonStyle, GUILayout.Height(32)))
                {
                    Settings.PredsAmount = Settings.PredsPico;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                GUILayout.BeginVertical(sectionBoxStyle);
                GUILayout.Label("Preset Settings", subHeaderStyle);
                GUILayout.Space(4);
                Settings.PredsLegit = DrawSlider("Legit Value", Settings.PredsLegit, 4.6f, 50f);
                Settings.PredsValve = DrawSlider("Valve Value", Settings.PredsValve, 6f, 50f);
                Settings.PredsRiftS = DrawSlider("RiftS Value", Settings.PredsRiftS, 9.5f, 50f);
                Settings.PredsPico = DrawSlider("Pico Value", Settings.PredsPico, 12f, 50f);
                GUILayout.EndVertical();

                GUILayout.Space(10);
                Settings.PredsAlwaysOn = DrawToggle("Enable Preds Always", Settings.PredsAlwaysOn);
                if (Settings.PredsAlwaysOn)
                {
                    Settings.PredsAlwaysAmount = DrawSlider("Always Amount", Settings.PredsAlwaysAmount, 0f, 50f);
                }

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Apply To:", labelStyle, GUILayout.Width(100));
                if (GUILayout.Button(Settings.PredsHand.ToString(), buttonStyle, GUILayout.Width(120), GUILayout.Height(28)))
                {
                    int next = ((int)Settings.PredsHand + 1) % 3;
                    Settings.PredsHand = (PredsHandOption)next;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                UnityEngine.GUI.color = saved;
            }

            GUILayout.Space(15);
            DrawSectionHeader("FORCE TAG FREEZE");

            Settings.ForceTagFreezeEnabled = DrawToggle("Force Tag Freeze", Settings.ForceTagFreezeEnabled);
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

            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("ESP Color", subHeaderStyle);
            GUILayout.Space(8);

            DrawColorPicker();

            GUILayout.EndVertical();

            GUILayout.Space(15);
            DrawSectionHeader("VISUAL FEATURES");

            Settings.TracersEnabled = DrawToggle("Tracers", Settings.TracersEnabled);
            if (Settings.TracersEnabled)
            {
                DrawInfoBox("Lines from your hand to other players");
            }

            GUILayout.Space(8);

            Settings.CornerESPEnabled = DrawToggle("Corner ESP", Settings.CornerESPEnabled);
            if (Settings.CornerESPEnabled)
            {
                DrawInfoBox("Corner brackets around players");
            }

            GUILayout.Space(8);

            Settings.HitboxesEnabled = DrawToggle("Hitboxes", Settings.HitboxesEnabled);
            if (Settings.HitboxesEnabled)
            {
                DrawInfoBox("Box around player heads");
            }

            GUILayout.Space(8);

            Settings.NameTagsEnabled = DrawToggle("Name Tags", Settings.NameTagsEnabled);
            if (Settings.NameTagsEnabled)
            {
                DrawInfoBox("Player names above their heads");
            }
        }

        private static Texture2D satValTexture = null;
        private static Texture2D hueBarTexture = null;
        private static Texture2D previewTexture = null;
        private static float lastHueForTexture = -1f;
        private static Color lastPreviewColor = Color.black;
        private static Texture2D whiteTex = null;
        private static Texture2D blackTex = null;

        private static void DrawColorPicker()
        {
            if (Settings.CurrentTab != 7) return;

            float pickerSize = 120f;
            float hueBarWidth = 20f;
            float spacing = 10f;
            float previewSize = 30f;

            Color currentColor = Color.HSVToRGB(Settings.ESPColorHue, Settings.ESPColorSat, Settings.ESPColorVal);
            
            GUILayout.BeginHorizontal();
            
            Rect svRect = GUILayoutUtility.GetRect(pickerSize, pickerSize);
            
            if (satValTexture == null || Mathf.Abs(lastHueForTexture - Settings.ESPColorHue) > 0.001f)
            {
                if (satValTexture != null) UnityEngine.Object.Destroy(satValTexture);
                satValTexture = GenerateSatValTexture(32, 32, Settings.ESPColorHue);
                lastHueForTexture = Settings.ESPColorHue;
            }
            
            UnityEngine.GUI.DrawTexture(svRect, satValTexture);
            
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
            
            float crossX = svRect.x + Settings.ESPColorSat * svRect.width;
            float crossY = svRect.y + (1f - Settings.ESPColorVal) * svRect.height;
            DrawCrosshairCached(crossX, crossY);

            GUILayout.Space(spacing);

            Rect hueRect = GUILayoutUtility.GetRect(hueBarWidth, pickerSize);
            if (hueBarTexture == null)
            {
                hueBarTexture = GenerateHueBarTexture(1, 64);
            }
            UnityEngine.GUI.DrawTexture(hueRect, hueBarTexture);
            
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
            {
                if (hueRect.Contains(Event.current.mousePosition))
                {
                    float localY = Event.current.mousePosition.y - hueRect.y;
                    Settings.ESPColorHue = Mathf.Clamp01(localY / hueRect.height);
                    Event.current.Use();
                }
            }
            
            if (whiteTex == null) whiteTex = MakeTex(1, 1, Color.white);
            float hueIndicatorY = hueRect.y + Settings.ESPColorHue * hueRect.height;
            UnityEngine.GUI.DrawTexture(new Rect(hueRect.x - 2, hueIndicatorY - 2, hueRect.width + 4, 4), whiteTex);

            GUILayout.Space(spacing);

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
            
            // Hex color input
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hex:", labelStyle, GUILayout.Width(35));
            
            string currentHex = ColorToHex(currentColor);
            string newHex = GUILayout.TextField(hexColorInput.Length > 0 ? hexColorInput : currentHex, 7, GUILayout.Width(80), GUILayout.Height(24));
            
            if (newHex != hexColorInput)
            {
                hexColorInput = newHex;
                TryApplyHexColor(newHex);
            }
            
            if (GUILayout.Button("Apply", buttonStyle, GUILayout.Width(60), GUILayout.Height(24)))
            {
                TryApplyHexColor(hexColorInput);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        private static string hexColorInput = "";
        
        private static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255);
            int g = Mathf.RoundToInt(color.g * 255);
            int b = Mathf.RoundToInt(color.b * 255);
            return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }
        
        private static void TryApplyHexColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return;
            
            hex = hex.TrimStart('#');
            if (hex.Length != 6) return;
            
            try
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                
                Color color = new Color(r / 255f, g / 255f, b / 255f);
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                
                Settings.ESPColorHue = h;
                Settings.ESPColorSat = s;
                Settings.ESPColorVal = v;
            }
            catch { }
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
                // y=0 is bottom of texture (displayed at bottom of rect) = dark (val=0)
                // y=height-1 is top of texture (displayed at top of rect) = bright (val=1)
                float val = (float)y / (height - 1);
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
                // y=0 is bottom of texture (displayed at bottom of rect) = magenta (hue=1)
                // y=height-1 is top of texture (displayed at top of rect) = red (hue=0)
                float hue = 1f - (float)y / (height - 1);
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
            
            UnityEngine.GUI.DrawTexture(new Rect(x - 6, y - 1, 12, 2), blackTex);
            UnityEngine.GUI.DrawTexture(new Rect(x - 1, y - 6, 2, 12), blackTex);
            
            UnityEngine.GUI.DrawTexture(new Rect(x - 5, y, 10, 1), whiteTex);
            UnityEngine.GUI.DrawTexture(new Rect(x, y - 5, 1, 10), whiteTex);
        }

        private static void DrawBypassTab()
        {
            DrawSectionHeader("NO SLIP");

            Settings.NoSlipEnabled = DrawToggle("Enable No Slip", Settings.NoSlipEnabled);
            
            if (Settings.NoSlipEnabled)
            {
                DrawInfoBox("Removes slippery surfaces in the pit");
            }

            GUILayout.Space(15);
            DrawSectionHeader("VELMAX BYPASS");

            Settings.VelmaxBypassEnabled = DrawToggle("Enable Velmax Bypass", Settings.VelmaxBypassEnabled);
            
            if (Settings.VelmaxBypassEnabled)
            {
                GUILayout.Space(8);
                Settings.VelmaxBypassKeybind = DrawKeybindSelector("Keybind", Settings.VelmaxBypassKeybind);
                DrawInfoBox("Hold keybind to freeze player (bypasses velocity detection)");
            }
        }

        private static void DrawRoomTab()
        {
            DrawSectionHeader("ROOM JOINER");

            GUILayout.BeginVertical(sectionBoxStyle);
            
            GUILayout.BeginHorizontal();
            GUIStyle codeLabelStyle = new GUIStyle(labelStyle);
            codeLabelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("Room Code:", codeLabelStyle, GUILayout.Width(100), GUILayout.Height(28));
            RoomMods.RoomCode = GUILayout.TextField(RoomMods.RoomCode, 20, GUILayout.Width(200), GUILayout.Height(28));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("JOIN ROOM", buttonStyle, GUILayout.Height(35), GUILayout.Width(150)))
            {
                if (!RoomMods.IsJoining)
                {
                    RoomMods.JoinRoom();
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("DISCONNECT", buttonStyle, GUILayout.Height(35), GUILayout.Width(150)))
            {
                RoomMods.Disconnect();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            
            if (!string.IsNullOrEmpty(RoomMods.JoinStatus))
            {
                GUILayout.Space(10);
                GUILayout.Label("Status: " + RoomMods.JoinStatus, labelStyle);
            }
            
            GUILayout.Space(10);
            DrawInfoBox("Enter a room code and click JOIN to join that room");
        }

        private static string newConfigName = "";
        
        private static void DrawConfigTab()
        {
            DrawSectionHeader("CONFIG SELECTION");

            GUILayout.Space(10);

            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.Label("Current Config: " + ConfigManager.CurrentConfigName, subHeaderStyle);
            GUILayout.Space(8);
            
            GUILayout.BeginHorizontal();
            GUIStyle selectLabelStyle = new GUIStyle(labelStyle);
            selectLabelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("Select Config:", selectLabelStyle, GUILayout.Width(100), GUILayout.Height(28));
            if (GUILayout.Button("◀", buttonStyle, GUILayout.Width(30), GUILayout.Height(28)))
            {
                ConfigManager.SelectedConfigIndex--;
                if (ConfigManager.SelectedConfigIndex < 0)
                    ConfigManager.SelectedConfigIndex = ConfigManager.GetConfigCount() - 1;
            }
            GUIStyle configNameStyle = new GUIStyle(labelStyle);
            configNameStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(ConfigManager.GetSelectedConfigName(), configNameStyle, GUILayout.Width(120), GUILayout.Height(28));
            if (GUILayout.Button("▶", buttonStyle, GUILayout.Width(30), GUILayout.Height(28)))
            {
                ConfigManager.CycleSelectedConfig();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(8);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("LOAD", buttonStyle, GUILayout.Height(32), GUILayout.Width(100)))
            {
                ConfigManager.LoadConfig(ConfigManager.GetSelectedConfigName());
                stylesInitialized = false;
                texCache.Clear();
            }
            GUILayout.Space(4);
            if (GUILayout.Button("SAVE", buttonStyle, GUILayout.Height(32), GUILayout.Width(100)))
            {
                ConfigManager.SaveConfig(ConfigManager.GetSelectedConfigName());
            }
            GUILayout.Space(4);
            if (GUILayout.Button("DELETE", buttonStyle, GUILayout.Height(32), GUILayout.Width(100)))
            {
                if (ConfigManager.GetSelectedConfigName() != "default")
                {
                    ConfigManager.DeleteConfig(ConfigManager.GetSelectedConfigName());
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(4);
            GUILayout.Label("Configs: " + ConfigManager.GetConfigCount() + " | Last Save: " + ConfigManager.LastSaveTime + " | Last Load: " + ConfigManager.LastLoadTime, labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(15);
            DrawSectionHeader("CREATE NEW CONFIG");

            GUILayout.BeginVertical(sectionBoxStyle);
            GUILayout.BeginHorizontal();
            GUIStyle nameLabelStyle = new GUIStyle(labelStyle);
            nameLabelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("Name:", nameLabelStyle, GUILayout.Width(50), GUILayout.Height(28));
            newConfigName = GUILayout.TextField(newConfigName, 20, GUILayout.Width(150), GUILayout.Height(28));
            GUILayout.Space(8);
            if (GUILayout.Button("CREATE", buttonStyle, GUILayout.Height(28)))
            {
                if (!string.IsNullOrEmpty(newConfigName) && newConfigName.Length > 0)
                {
                    ConfigManager.SaveConfig(newConfigName.Trim());
                    newConfigName = "";
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(15);
            DrawSectionHeader("AUTO-LOAD ON STARTUP");

            GUILayout.BeginVertical(sectionBoxStyle);
            
            bool autoLoadBefore = ConfigManager.AutoLoadEnabled;
            ConfigManager.AutoLoadEnabled = DrawToggle("Enable Auto-Load", ConfigManager.AutoLoadEnabled);
            
            if (ConfigManager.AutoLoadEnabled)
            {
                GUILayout.Space(8);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Auto-Load Config:", labelStyle, GUILayout.Width(130));
                if (GUILayout.Button(ConfigManager.AutoLoadConfigName, buttonStyle, GUILayout.Width(120), GUILayout.Height(28)))
                {
                    var configs = ConfigManager.GetConfigList();
                    int currentIndex = configs.IndexOf(ConfigManager.AutoLoadConfigName);
                    currentIndex = (currentIndex + 1) % configs.Count;
                    ConfigManager.AutoLoadConfigName = configs[currentIndex];
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(8);
                if (GUILayout.Button("Set Current Config as Auto-Load", buttonStyle, GUILayout.Height(28)))
                {
                    ConfigManager.AutoLoadConfigName = ConfigManager.CurrentConfigName;
                }
            }
            
            if (autoLoadBefore != ConfigManager.AutoLoadEnabled || ConfigManager.AutoLoadEnabled)
            {
                ConfigManager.SaveAutoLoadSettings();
            }
            
            GUILayout.EndVertical();
        }

        private static void DrawCreditsTab()
        {
            DrawSectionHeader("DEVELOPMENT TEAM");

            GUILayout.Space(6);

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

            GUILayout.BeginVertical(creditCardStyle);
            GUILayout.BeginHorizontal();
            GUIStyle blueName2 = new GUIStyle(labelStyle);
            blueName2.normal.textColor = new Color(0.4f, 0.8f, 1f, 1f);
            blueName2.fontStyle = FontStyle.Bold;
            blueName2.fontSize = 18;
            GUILayout.Label("★ FTSyxcal", blueName2);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("Developer", labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(6);

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

            GUILayout.Space(15);

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
