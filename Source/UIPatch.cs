using BrilliantSkies.PlayerProfiles;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Examples;
using BrilliantSkies.Ui.Consoles.Interpretters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Styles;
using BrilliantSkies.Ui.Layouts.DropDowns;
using BrilliantSkies.Ui.Tips;
using HarmonyLib;
using MTMTVFX.UI;
using UnityEngine;

namespace BMEffects_Remaster
{
    public enum Mode
    {
        Light = 0,
        Dark = 1,
        Plain = 2
    }

    public class BMEConfig : ProfileModule<BMEConfig.InternalData>
    {
        public class InternalData
        {
            public Mode mode;
        }
        public override ModuleType ModuleType => ModuleType.Options;
        protected override string FilenameAndExtension => "profile.BMEConfig";

        public Mode mode
        {
            get { return Internal.mode; }
            set { Internal.mode = value; }
        }
    }

    [HarmonyPatch(typeof(OptionsMenuUi), "BuildInterface")]
    public class UIPatch
    {
        private static void Postfix(ref ConsoleWindow __result)
        {
            __result.AllScreens.Add(new UITab(__result, ProfileManager.Instance.GetModule<BMEConfig>()));
        }
    }

    public class UITab : SuperScreen<BMEConfig>
    {
        public UITab(ConsoleWindow window, BMEConfig config) : base(window, config) { }
        public override Content Name => new Content("<color=#F80>BMEffects Remaster</color> Settings", new ToolTip("Adjust the configuration for <color=#F80>BMEffects Remaster</color>"));

        public override void Build()
        {
            ScreenSegmentTable screenSegmentTable = CreateTableSegment(3, 1);
            screenSegmentTable.SqueezeTable = false;
            screenSegmentTable.SpaceBelow = 40f;
            screenSegmentTable.SetColumnFractionalWidths(new float[] { 0.3f, 0.4f, 0.3f });
            screenSegmentTable.BackgroundStyleWhereApplicable = ConsoleStyles.Instance.Styles.Segments.OptionalSegmentDarkBackgroundWithHeader.Style;
            screenSegmentTable.NameWhereApplicable = "<color=#F00>RESTART THE GAME TO APPLY ANY CHANGES</color>";

            screenSegmentTable.AddInterpretter(new Blank(), 0, 0);
            screenSegmentTable.AddInterpretter(new Blank(), 0, 2);
            DropDownMenuAlt<Mode> modeDropdown = new DropDownMenuAlt<Mode>();
            modeDropdown.SetItems(
                new DropDownMenuAltItem<Mode>()
                {
                    Name = "Use DARK laser mode",
                    ObjectForAction = Mode.Dark,
                    ToolTip = "Set the center part of lasers to BLACK"
                },
                new DropDownMenuAltItem<Mode>()
                {
                    Name = "Use LIGHT laser mode",
                    ObjectForAction = Mode.Light,
                    ToolTip = "Set the center part of lasers to WHITE"
                },
                new DropDownMenuAltItem<Mode>()
                {
                    Name = "Use PLAIN laser mode",
                    ObjectForAction = Mode.Plain,
                    ToolTip = "Set the center part of lasers to the same color as the laser"
                });
            screenSegmentTable.AddInterpretter(new DropDown<BMEConfig, Mode>(_focus, modeDropdown, (BMEConfig I, Mode e) => I.mode == e, delegate (BMEConfig I, Mode e)
            {
                I.mode = e;
            }));
        }
    }
}
