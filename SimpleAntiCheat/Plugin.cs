﻿using System;
using BepInEx;
using HarmonyLib;

namespace SimpleAntiCheat
{
    [BepInPlugin(Guid, "SimpleAntiCheat", VersionNumber)]
    [BepInProcess("StickFight.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo("Plugin " + Guid + " is loaded! [v" + VersionNumber + "]");
            try
            {
                Harmony harmony = new Harmony(Guid); // Creates harmony instance with identifier

                Logger.LogInfo("Applying MultiplayerManager patch...");
                MultiplayerManagerPatch.Patch(harmony);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception on applying patches: " + ex.InnerException + " " + ex.Message + " " +
                                ex.TargetSite + " " + ex.Source);
            }
        }

        public const string VersionNumber = "1.0.0"; // Version string of plugin
        public const string Guid = "monky.plugins.SimpleAntiCheat";
    }
}