using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CompMenu.Core
{
    public class api
    {
        public string name, ownerid, version;

        public api(string name, string ownerid, string version)
        {
            this.name = name;
            this.ownerid = ownerid;
            this.version = version;
        }

        [Serializable]
        private class response_structure
        {
            public bool success = false;
            public string sessionid = string.Empty;
            public string message = string.Empty;
        }

        [Serializable]
        private class license_response
        {
            public bool success = false;
            public string message = string.Empty;
            public user_info info = new user_info();
        }

        [Serializable]
        private class user_info
        {
            public string username = string.Empty;
            public string ip = string.Empty;
            public string hwid = string.Empty;
        }

        private string sessionid;
        public bool initialized;

        public void init()
        {
            WWWForm form = new WWWForm();
            form.AddField("type", "init");
            form.AddField("ver", version);
            form.AddField("name", name);
            form.AddField("ownerid", ownerid);

            Debug.Log("[KeyAuth] Sending init - name: " + name + ", ownerid: " + ownerid + " (len:" + ownerid.Length + "), ver: " + version);

            string resp = SendRequest(form);

            if (resp == "KeyAuth_Invalid")
            {
                response.success = false;
                response.message = "Connection failed";
                return;
            }

            Debug.Log("[KeyAuth] Init response: " + resp);

            try
            {
                var json = JsonUtility.FromJson<response_structure>(resp);

                if (json == null)
                {
                    response.success = false;
                    response.message = "Invalid response";
                    return;
                }

                response.success = json.success;
                response.message = json.message ?? "Unknown error";

                if (json.success)
                {
                    sessionid = json.sessionid;
                    initialized = true;
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Parse error: " + ex.Message;
                Debug.LogError("[KeyAuth] Parse error: " + ex);
            }
        }

        public void license(string key)
        {
            if (!initialized)
            {
                response.success = false;
                response.message = "Not initialized";
                return;
            }

            string hwid = GetHWID();

            WWWForm form = new WWWForm();
            form.AddField("type", "license");
            form.AddField("key", key);
            form.AddField("hwid", hwid);
            form.AddField("sessionid", sessionid);
            form.AddField("name", name);
            form.AddField("ownerid", ownerid);

            string resp = SendRequest(form);

            if (resp == "KeyAuth_Invalid")
            {
                response.success = false;
                response.message = "Connection failed";
                return;
            }

            Debug.Log("[KeyAuth] License response: " + resp);

            try
            {
                var json = JsonUtility.FromJson<license_response>(resp);

                if (json == null)
                {
                    response.success = false;
                    response.message = "Invalid response";
                    return;
                }

                response.success = json.success;
                response.message = json.message ?? "Unknown error";

                if (json.success && json.info != null)
                {
                    user_data.username = json.info.username;
                    user_data.ip = json.info.ip;
                    user_data.hwid = json.info.hwid;
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Parse error: " + ex.Message;
            }
        }

        private string GetHWID()
        {
            try
            {
                return WindowsIdentity.GetCurrent().User.Value;
            }
            catch
            {
                return SystemInfo.deviceUniqueIdentifier;
            }
        }

        private string SendRequest(WWWForm form)
        {
            try
            {
                using (UnityWebRequest www = UnityWebRequest.Post("https://keyauth.win/api/1.2/", form))
                {
                    www.timeout = 15;
                    var op = www.SendWebRequest();

                    while (!op.isDone)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        return www.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError("[KeyAuth] Request error: " + www.error);
                        return "KeyAuth_Invalid";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[KeyAuth] Exception: " + ex.Message);
                return "KeyAuth_Invalid";
            }
        }

        public user_data_class user_data = new user_data_class();

        public class user_data_class
        {
            public string username { get; set; }
            public string ip { get; set; }
            public string hwid { get; set; }
        }

        public response_class response = new response_class();

        public class response_class
        {
            public bool success { get; set; }
            public string message { get; set; }
        }
    }

    public static class AuthManager
    {
        public static api KeyAuthApp;
        public static bool IsAuthenticated = false;
        public static bool IsInitialized = false;
        public static bool InitFailed = false;
        public static string StatusMessage = "Connecting...";

        public static void Initialize()
        {
            InitFailed = false;
            IsInitialized = false;
            StatusMessage = "Connecting...";

            try
            {
                KeyAuthApp = new api(
                    name: "Parallax",
                    ownerid: "dKG3LOwgsm",
                    version: "1.0"
                );

                KeyAuthApp.init();

                if (KeyAuthApp.response.success)
                {
                    IsInitialized = true;
                    StatusMessage = "Enter your license key";
                }
                else
                {
                    InitFailed = true;
                    StatusMessage = "Failed: " + KeyAuthApp.response.message;
                }
            }
            catch (Exception ex)
            {
                InitFailed = true;
                StatusMessage = "Error: " + ex.Message;
                Debug.LogError("[KeyAuth] Init Exception: " + ex);
            }
        }

        public static bool Login(string key)
        {
            if (!IsInitialized)
            {
                StatusMessage = "Not initialized";
                return false;
            }

            try
            {
                KeyAuthApp.license(key);

                if (KeyAuthApp.response.success)
                {
                    IsAuthenticated = true;
                    StatusMessage = "Welcome, " + (KeyAuthApp.user_data.username ?? "User") + "!";
                    return true;
                }

                StatusMessage = KeyAuthApp.response.message;
                return false;
            }
            catch (Exception ex)
            {
                StatusMessage = "Error: " + ex.Message;
                return false;
            }
        }
    }
}
