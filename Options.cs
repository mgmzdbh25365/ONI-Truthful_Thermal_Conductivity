namespace ONI_Truthful_Thermal_Conductivity {
    public class Options {
        public int mode = 1;
        public Custom custom = null;
        public class Custom {
            public bool scaleChanged = false;
            public bool tipsChanged = false;
            public string scale_format = "0.000";
            public string tips_format = "(DTU/(m*s))/";
            public Custom() {
            }
        }
        public Options() { 
        }
        public Custom GetCustomOrCreate() {
            if (custom == null)
               custom = new Custom();
            return custom;
        }
        public bool IsEnableThermalConductivity() {
            if (custom == null)
                return true;
            return custom.scaleChanged;
        }
        public bool IsEnableUnit() {
            if (custom == null)
                return false;
            return custom.tipsChanged;
        }
    }
}
