using BrilliantSkies.Core.Timing;
using BrilliantSkies.Modding;
using MTMTVFX.Core;
using Newtonsoft.Json.Linq;
using Steamworks;
using System.IO;
using System;
using System.Reflection;

namespace BMEffects_Remaster
{
    public static class ModInfo
    {
        public static string ModPath
        {
            get
            {
                if (string.IsNullOrEmpty(_path)) Init();
                return _path;
            }
        }

        public static string ModName { get; private set; } = "BMEffects_Remaster";

        public static string AssetbundleGUID
        {
            get
            {
                if (string.IsNullOrEmpty(_guid)) Init();
                return _guid;
            }
        }

        public static System.Version Version
        {
            get { return _version; }
        }

        private static string _path;
        private static string _guid;
        private static System.Version _version = new System.Version(0, 0, 0);
        private static ulong _workshopID;
        private static int _requestCount;
        private static CallResult<SteamUGCRequestUGCDetailsResult_t> _steamCall;

        private static void Init()
        {
            try
            {
                _path = Assembly.GetExecutingAssembly().Location;
                string modFolder = Path.Combine(Path.GetDirectoryName(ModPath), "Asset Bundles");
                string[] files;
                files = Directory.GetFiles(modFolder, "bmeffects_*.assetBundle");
                if (files.Length < 1) files = Directory.GetFiles(modFolder, "bmeffects*"); //nuclear option
                string json = File.ReadAllText(files[0]);
                JObject obj = JObject.Parse(json);
                _guid = (string)obj["ComponentId"]["Guid"];
            }
            catch (Exception e)
            {
                Utils.LogError<CorePlugin>(e.Message, BrilliantSkies.Core.Logger.LogOptions.PopupDev);
            }
        }

        public static void CheckVersion()
        {
            GameEvents.Twice_Second.RegWithEvent(SteamUGCRequest);

            string pluginPath = Path.Combine(Path.GetDirectoryName(ModPath), "plugin.json");

            if (File.Exists(pluginPath))
            {
                JObject jObject = JObject.Parse(File.ReadAllText(pluginPath));

                JToken jobj1 = jObject["version"];
                JToken jobj2 = jObject["workshop_id"];

                if (jobj1 != null)
                {
                    _version = System.Version.Parse(jobj1.ToString());
                }

                if (jobj2 != null)
                {
                    _workshopID = ulong.Parse(jobj2.ToString());
                }
            }

            ModProblemOverwrite($"{ModName}  v{_version}  Active!", ModPath, string.Empty, false);
        }

        private static void ModProblemOverwrite(string InitMod, string InitModPath, string InitDescription, bool InitIsError)
        {
            ModProblems.AllModProblems.Remove(InitModPath);
            ModProblems.AddModProblem(InitMod, InitModPath, InitDescription, InitIsError);
        }

        private static void SteamUGCRequest(ITimeStep t)
        {
            if (_workshopID != 0 && ++_requestCount <= 5)
            {
                SteamAPICall_t ugcDetails = SteamUGC.RequestUGCDetails(new PublishedFileId_t(_workshopID), 0);
                _steamCall = new CallResult<SteamUGCRequestUGCDetailsResult_t>(Callback);
                _steamCall.Set(ugcDetails);
            }
            else
            {
                GameEvents.Twice_Second.UnregWithEvent(SteamUGCRequest);
            }
        }

        private static void Callback(SteamUGCRequestUGCDetailsResult_t param, bool bIOFailure)
        {
            Utils.LogInfo<CorePlugin>("flag1");
            GameEvents.Twice_Second.UnregWithEvent(SteamUGCRequest);
            Utils.LogInfo<CorePlugin>("flag2");

            string description = param.m_details.m_rgchDescription;
            Utils.LogInfo<CorePlugin>("flag3");

            if (!string.IsNullOrEmpty(description))
            {
                StringReader reader = new StringReader(description);
                string inputLine;
                System.Version latestVersion = null;

                while ((inputLine = reader.ReadLine()) != null)
                {
                    if (inputLine.StartsWith("Latest version "))
                    {
                        latestVersion = System.Version.Parse(inputLine.Remove(0, 15));
                        Utils.LogInfo<CorePlugin>(description);
                        Utils.LogInfo<CorePlugin>(latestVersion.ToString());
                        Utils.LogInfo<CorePlugin>(_version.ToString());
                        break;
                    }
                }

                if (latestVersion != null && _version.CompareTo(latestVersion) == -1)
                {
                    ModProblemOverwrite(ModName, ModPath, "New version released! v" + latestVersion, false);
                }
            }
        }
    }
}