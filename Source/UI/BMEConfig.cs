using BrilliantSkies.PlayerProfiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace BMEffects_Remaster.UI
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
            public bool e_laserSound;
            public bool e_pulseSound;
            public bool e_pacSound;
        }
        public override ModuleType ModuleType => ModuleType.Options;
        protected override string FilenameAndExtension => "profile.BMEConfig";

        public Mode mode
        {
            get { return Internal.mode; }
            set { Internal.mode = value; }
        }

        public bool e_laserSound
        {
            get { return Internal.e_laserSound; }
            set { Internal.e_laserSound = value; }
        }

        public bool e_pulseSound
        {
            get { return Internal.e_pulseSound; }
            set { Internal.e_pulseSound = value; }
        }

        public bool e_pacSound
        {
            get { return Internal.e_pacSound; }
            set { Internal.e_pacSound = value; }
        }
    }
}
