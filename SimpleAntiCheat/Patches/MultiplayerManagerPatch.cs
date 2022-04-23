using System;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace SimpleAntiCheat
{
    class MultiplayerManagerPatch
    {
       
        public static void Patch(Harmony harmonyInstance) // Fighting() methods to patch with the harmony instance
        {
            var onServerJoinedMethod = AccessTools.Method(typeof(MultiplayerManager), "OnServerJoined");
            var onServerJoinedMethodPrefix = new HarmonyMethod(typeof(MultiplayerManagerPatch).GetMethod(nameof(OnServerJoinedMethodPrefix)));
            harmonyInstance.Patch(onServerJoinedMethod, prefix: onServerJoinedMethodPrefix); // Patches JoinServer() with prefix method

            var onServerCreatedMethod = AccessTools.Method(typeof(MultiplayerManager), "OnServerCreated");
            var onServerCreatedMethodPostfix = new HarmonyMethod(typeof(MultiplayerManagerPatch).GetMethod(nameof(OnServerCreatedMethodPostfix)));
            harmonyInstance.Patch(onServerCreatedMethod, postfix: onServerCreatedMethodPostfix); // Patches JoinServer() with prefix method
        }

        public static bool OnServerJoinedMethodPrefix(MultiplayerManager __instance, ref LobbyEnter_t param, ref bool bIOFailure)
        {
            if (bIOFailure) return false;

            string lobbyData = SteamMatchmaking.GetLobbyData(new CSteamID(param.m_ulSteamIDLobby), "Version");
            Debug.Log("attempting to join lobby. version key is: " + lobbyData);

            if (lobbyData != Plugin.Guid)
            {
                
                UnityEngine.Object.FindObjectOfType<LoadingScreenManager>().LoadThenFail(ConnectionErrorType.InvalidVersion, "\n<color=red>Please directly join the lobby of a person using the mod. This is to prevent an unfair advantage.</color>\n\t<font=Bangers SDF><#f08>-Monky");
                return false;
            }

            return true;
        }

        public static void OnServerCreatedMethodPostfix(ref LobbyCreated_t param, ref bool bIOFailure)
        {
            SteamMatchmaking.SetLobbyData(new CSteamID(param.m_ulSteamIDLobby), StickFightConstants.VERSION_KEY, Plugin.Guid);
            string lobbyData = SteamMatchmaking.GetLobbyData(new CSteamID(param.m_ulSteamIDLobby), StickFightConstants.VERSION_KEY);
            Debug.Log("Created lobby of ver: " + lobbyData);
        }
    }
}
