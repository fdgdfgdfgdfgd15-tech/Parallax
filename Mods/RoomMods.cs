using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace CompMenu.Mods
{
    public static class RoomMods
    {
        public static string RoomCode = "";
        public static bool IsJoining = false;
        public static string JoinStatus = "";
        
        private static object cachedController = null;
        private static MethodInfo joinMethod = null;
        private static Type controllerType = null;
        private static bool typeCached = false;

        private static void FindControllerType()
        {
            if (typeCached) return;
            typeCached = true;
            
            try
            {
                // Search all assemblies for PhotonNetworkController
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        controllerType = assembly.GetTypes().FirstOrDefault(t => t.Name == "PhotonNetworkController");
                        if (controllerType != null) break;
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static object GetController()
        {
            FindControllerType();
            
            if (controllerType == null) return null;
            
            try
            {
                // Try Instance property
                var instanceProp = controllerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                if (instanceProp != null)
                {
                    return instanceProp.GetValue(null);
                }
                
                // Try instance field
                var instanceField = controllerType.GetField("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                if (instanceField != null)
                {
                    return instanceField.GetValue(null);
                }
                
                // Try FindObjectOfType
                var findMethod = typeof(UnityEngine.Object).GetMethod("FindObjectOfType", new Type[] { typeof(Type) });
                if (findMethod != null)
                {
                    return findMethod.Invoke(null, new object[] { controllerType });
                }
            }
            catch { }
            
            return null;
        }

        public static async void JoinRoom()
        {
            if (string.IsNullOrEmpty(RoomCode))
            {
                JoinStatus = "Enter a room code";
                return;
            }

            try
            {
                IsJoining = true;
                JoinStatus = "Disconnecting...";
                
                PhotonNetwork.Disconnect();
                await Task.Delay(3000);
                
                JoinStatus = "Joining " + RoomCode + "...";
                
                cachedController = GetController();
                
                if (cachedController != null)
                {
                    if (joinMethod == null)
                    {
                        joinMethod = controllerType.GetMethod("AttemptToJoinSpecificRoom", 
                            BindingFlags.Public | BindingFlags.Instance);
                    }
                    
                    if (joinMethod != null)
                    {
                        joinMethod.Invoke(cachedController, new object[] { RoomCode, 0 });
                        JoinStatus = "Join request sent";
                    }
                    else
                    {
                        JoinStatus = "Join method not found";
                    }
                }
                else
                {
                    JoinStatus = "Controller not found";
                }
                
                IsJoining = false;
            }
            catch (Exception ex)
            {
                JoinStatus = "Error: " + ex.Message;
                IsJoining = false;
                Debug.LogError("[Parallax] JoinRoom error: " + ex);
            }
        }

        public static void Disconnect()
        {
            try
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Disconnect();
                    JoinStatus = "Disconnected";
                }
                else
                {
                    JoinStatus = "Not in a room";
                }
            }
            catch (Exception ex)
            {
                JoinStatus = "Error: " + ex.Message;
                Debug.LogError("[Parallax] Disconnect error: " + ex);
            }
        }
    }
}

