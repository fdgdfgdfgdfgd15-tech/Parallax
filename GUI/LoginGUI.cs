using System.IO;
using BepInEx;
using UnityEngine;
using CompMenu.Core;

namespace CompMenu.GUI
{
    public static class LoginGUI
    {
        private static string licenseKey = "";
        private static GUIStyle titleStyle;
        private static GUIStyle labelStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle textFieldStyle;
        private static GUIStyle statusStyle;
        private static GUIStyle boxStyle;
        private static Texture2D darkBg;
        private static Texture2D buttonBg;
        private static Texture2D buttonHover;
        private static Texture2D inputBg;
        private static bool stylesInit = false;
        private static bool keyLoaded = false;
        private static bool autoLoginAttempted = false;

        private static string LicensePath => Path.Combine(Paths.PluginPath, "PARALLAX", "License.txt");

        public static void LoadSavedKey()
        {
            if (keyLoaded) return;
            keyLoaded = true;
            
            try
            {
                string folder = Path.GetDirectoryName(LicensePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                    
                if (File.Exists(LicensePath))
                {
                    licenseKey = File.ReadAllText(LicensePath).Trim();
                }
            }
            catch { }
        }

        public static void SaveKey(string key)
        {
            try
            {
                string folder = Path.GetDirectoryName(LicensePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                    
                File.WriteAllText(LicensePath, key);
            }
            catch { }
        }

        public static void TryAutoLogin()
        {
            if (autoLoginAttempted) return;
            if (!AuthManager.IsInitialized) return;
            
            autoLoginAttempted = true;
            
            LoadSavedKey();
            
            if (!string.IsNullOrEmpty(licenseKey))
            {
                AuthManager.StatusMessage = "Auto-logging in...";
                AuthManager.Login(licenseKey);
                
                if (AuthManager.IsAuthenticated)
                {
                    SaveKey(licenseKey);
                }
            }
        }

        public static void Draw()
        {
            InitStyles();
            LoadSavedKey();
            
            // Try auto-login if we have a saved key
            if (AuthManager.IsInitialized && !autoLoginAttempted && !string.IsNullOrEmpty(licenseKey))
            {
                TryAutoLogin();
            }

            float windowWidth = 450f;
            float windowHeight = 280f;
            float x = (Screen.width - windowWidth) / 2f;
            float y = (Screen.height - windowHeight) / 2f;

            UnityEngine.GUI.Box(new Rect(x, y, windowWidth, windowHeight), "", boxStyle);

            GUILayout.BeginArea(new Rect(x + 30, y + 25, windowWidth - 60, windowHeight - 50));

            GUILayout.Label("PARALLAX", titleStyle);
            GUILayout.Space(8);
            GUILayout.Label("Authentication Required", labelStyle);
            GUILayout.Space(25);

            GUILayout.Label("License Key:", labelStyle);
            GUILayout.Space(8);

            licenseKey = GUILayout.TextField(licenseKey, 256, textFieldStyle, GUILayout.Height(35), GUILayout.ExpandWidth(true));
            GUILayout.Space(15);

            if (AuthManager.InitFailed)
            {
                if (GUILayout.Button("RETRY CONNECTION", buttonStyle, GUILayout.Height(40)))
                {
                    AuthManager.InitFailed = false;
                    AuthManager.StatusMessage = "Connecting...";
                    AuthManager.Initialize();
                    autoLoginAttempted = false;
                }
            }
            else if (AuthManager.IsInitialized)
            {
                if (GUILayout.Button("LOGIN", buttonStyle, GUILayout.Height(40)))
                {
                    if (!string.IsNullOrEmpty(licenseKey))
                    {
                        AuthManager.Login(licenseKey);
                        if (AuthManager.IsAuthenticated)
                        {
                            SaveKey(licenseKey);
                        }
                    }
                    else
                    {
                        AuthManager.StatusMessage = "Please enter a license key";
                    }
                }
            }
            else
            {
                GUILayout.Button("CONNECTING...", buttonStyle, GUILayout.Height(40));
            }

            GUILayout.Space(15);

            Color msgColor;
            if (AuthManager.InitFailed || AuthManager.StatusMessage.Contains("Failed") || AuthManager.StatusMessage.Contains("Invalid") || AuthManager.StatusMessage.Contains("Error"))
                msgColor = new Color(0.9f, 0.3f, 0.3f);
            else if (AuthManager.IsAuthenticated)
                msgColor = new Color(0.3f, 0.9f, 0.3f);
            else if (AuthManager.IsInitialized)
                msgColor = new Color(0.5f, 0.9f, 0.5f);
            else
                msgColor = new Color(0.7f, 0.7f, 0.7f);

            statusStyle.normal.textColor = msgColor;
            GUILayout.Label(AuthManager.StatusMessage, statusStyle);

            GUILayout.EndArea();
        }

        private static void InitStyles()
        {
            if (stylesInit) return;
            stylesInit = true;

            darkBg = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.12f, 0.98f));
            buttonBg = MakeTex(2, 2, new Color(0.6f, 0.15f, 0.15f, 1f));
            buttonHover = MakeTex(2, 2, new Color(0.7f, 0.2f, 0.2f, 1f));
            inputBg = MakeTex(2, 2, new Color(0.06f, 0.06f, 0.08f, 1f));

            boxStyle = new GUIStyle();
            boxStyle.normal.background = darkBg;

            titleStyle = new GUIStyle();
            titleStyle.fontSize = 32;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(0.85f, 0.2f, 0.2f);

            labelStyle = new GUIStyle();
            labelStyle.fontSize = 14;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);

            buttonStyle = new GUIStyle();
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.normal.background = buttonBg;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.hover.background = buttonHover;
            buttonStyle.active.textColor = Color.white;
            buttonStyle.active.background = buttonHover;

            textFieldStyle = new GUIStyle();
            textFieldStyle.fontSize = 14;
            textFieldStyle.alignment = TextAnchor.MiddleLeft;
            textFieldStyle.padding = new RectOffset(12, 12, 10, 10);
            textFieldStyle.normal.textColor = Color.white;
            textFieldStyle.normal.background = inputBg;
            textFieldStyle.focused.textColor = Color.white;
            textFieldStyle.focused.background = inputBg;
            textFieldStyle.active.textColor = Color.white;
            textFieldStyle.active.background = inputBg;
            textFieldStyle.clipping = TextClipping.Clip;

            statusStyle = new GUIStyle();
            statusStyle.fontSize = 11;
            statusStyle.alignment = TextAnchor.MiddleCenter;
            statusStyle.wordWrap = true;
            statusStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
