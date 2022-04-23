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
            harmonyInstance.Patch(onServerCreatedMethod, postfix: onServerCreatedMethodPostfix); // Patches OnServerCreated() with postfix method
        }

        public static bool OnServerJoinedMethodPrefix(MultiplayerManager __instance, ref LobbyEnter_t param, ref bool bIOFailure)
        {
            if (bIOFailure) return false;

            CSteamID lobbyID =  new (param.m_ulSteamIDLobby);
            string lobbyData = SteamMatchmaking.GetLobbyData(lobbyID, "Version");
            Debug.Log("Attempting to join lobby with version key of: " + lobbyData);

            if (lobbyData != Plugin.Guid)
            {
                UnityEngine.Object.FindObjectOfType<LoadingScreenManager>().LoadThenFail(ConnectionErrorType.InvalidVersion, "\n<color=red>Please directly join the lobby of a person using a game-play altering mod. This is to prevent an unfair advantage.</color>\n\t<font=Bangers SDF><#f08>-Monky");
                UnityEngine.Object.FindObjectOfType<MultiplayerManager>().OnDisconnected();
                SteamMatchmaking.LeaveLobby(lobbyID);
                return false;
            }

            return true;
        }

        public static void OnServerCreatedMethodPostfix(ref LobbyCreated_t param, ref bool bIOFailure)
        {
            CSteamID lobbyID = new(param.m_ulSteamIDLobby);

            SteamMatchmaking.SetLobbyData(lobbyID, StickFightConstants.VERSION_KEY, Plugin.Guid);
            string lobbyData = SteamMatchmaking.GetLobbyData(lobbyID, StickFightConstants.VERSION_KEY);
            Debug.Log("Created lobby of version: " + lobbyData);
        }
    }
}
