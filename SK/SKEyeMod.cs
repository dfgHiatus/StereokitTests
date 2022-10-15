using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace SKEyeTracking
{
    public class StereokitEyeTracking : NeosMod
    {
        public override string Name => "StereokitEyeTracking";
        public override string Author => "dfgHiatus";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/GithubUsername/RepoName/";
        public override void OnEngineInit()
        {
            new Harmony("net.dfgHiatus.StereokitEyeTracking").PatchAll();
            Engine.Current.InputInterface.RegisterInputDriver(new SKEyeDevice());
        }
    }
}