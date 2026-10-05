using BMEffects_Remaster.Core;
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
using static MTMTVFX.UI.SettingsTab;

namespace BMEffects_Remaster.UI
{
    public class UITab : SuperScreen<BMEConfig>
    {
        public UITab(ConsoleWindow window, BMEConfig config) : base(window, config) { }
        public override Content Name => new Content("<color=#F80>BMEffects Remaster</color> Settings", new ToolTip("Adjust the configuration for <color=#F80>BMEffects Remaster</color>"));

        public override void Build()
        {
            LaserMode();
            SoundSettings();
        }

        private void LaserMode()
        {
            ScreenSegmentTable screenSegmentTable = CreateTableSegment(3, 1);
            screenSegmentTable.SqueezeTable = false;
            screenSegmentTable.SpaceBelow = 40f;
            screenSegmentTable.SetColumnFractionalWidths(new float[] { 0.3f, 0.4f, 0.3f });
            screenSegmentTable.BackgroundStyleWhereApplicable = ConsoleStyles.Instance.Styles.Segments.OptionalSegmentDarkBackground.Style;

            screenSegmentTable.AddInterpretter(new Blank(), 0, 0);
            screenSegmentTable.AddInterpretter(new Blank(), 0, 2);
            DropDownMenuAlt<Mode> modeDropdown = new DropDownMenuAlt<Mode>();
            modeDropdown.SetItems(
                new DropDownMenuAltItem<Mode>()
                {
                    Name = "<b>DARK</b> mode",
                    ObjectForAction = Mode.Dark,
                    ToolTip = "Set the center part of lasers to BLACK"
                },
                new DropDownMenuAltItem<Mode>()
                {
                    Name = "<b>LIGHT</b> mode",
                    ObjectForAction = Mode.Light,
                    ToolTip = "Set the center part of lasers to WHITE"
                },
                new DropDownMenuAltItem<Mode>()
                {
                    Name = "<b>PLAIN</b> mode",
                    ObjectForAction = Mode.Plain,
                    ToolTip = "Set the center part of lasers to the same color as the laser"
                });
            screenSegmentTable.AddInterpretter(new DropDown<BMEConfig, Mode>(_focus, modeDropdown, (I, e) => I.mode == e, delegate (BMEConfig I, Mode e)
            {
                I.mode = e;
                Core.CorePatcher.SwapMode(e);
            }));
        }

        private void SoundSettings()
        {
            ScreenSegmentTable screenSegmentTable = CreateTableSegment(3, 1);
            screenSegmentTable.SqueezeTable = false;
            screenSegmentTable.SpaceBelow = 40f;
            screenSegmentTable.BackgroundStyleWhereApplicable = ConsoleStyles.Instance.Styles.Segments.OptionalSegmentDarkBackground.Style;

            screenSegmentTable.AddInterpretter(UIHelper.Bool(_focus, "Enable 0Q laser sounds", "Enable or disable the custom 0Q laser sounds", I => I.e_laserSound));
            screenSegmentTable.AddInterpretter(UIHelper.Bool(_focus, "Enable PULSE laser sounds", "Enable or disable the custom pulse laser sounds (1/2/3/4Q)", I => I.e_pulseSound));
            screenSegmentTable.AddInterpretter(UIHelper.Bool(_focus, "Enable PAC sounds", "Enable or disable the custom sound for pac (one at above 400,000 energy, one at 1,500,000)", I => I.e_pacSound));
        }
    }
}
