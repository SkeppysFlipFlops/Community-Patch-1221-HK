using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CommunityPatch
{
    [Serializable]
    internal class Config
    {
        /// <summary>
        /// ScreenShakeModifier, set to 0 for no screenshake
        /// </summary>
        public FloatRef ScreenShake = new();

        /// <summary>
        /// reset bind, will not work if the reset bind modifier key is bound and not pressed.
        /// </summary>
        public KeyCodeRef ResetBind = new();

        /// <summary>
        /// optional modifier key so if you fafinger it you dont reset
        /// </summary>
        public KeyCodeRef ResetBindModifierKey = new();

        /// <summary>
        /// holds left stick left
        /// </summary>
        public KeyCodeRef LAnalogLeft = new();

        /// <summary>
        /// holds left stick right
        /// </summary>
        public KeyCodeRef LAnalogRight = new();

        /// <summary>
        /// holds right stick up
        /// </summary>
        public KeyCodeRef RAnalogUp = new();

        /// <summary>
        /// opens menu to change keybinds
        /// </summary>
        public KeyCodeRef OpenConfigMenu = new(KeyCode.F11);

        /// <summary>
        /// modifier key (optional) to open config menu, so you have to press this too, so no fatfinger.
        /// </summary>
        public KeyCodeRef OpenConfigMenuModifier = new();

        public KeyValuePair<KeyCodeRef, string>[] ControlRef() => [
            new(ResetBind, "Reset Bind"),
            new(ResetBindModifierKey, "Reset Bind Modifier"),
            new(LAnalogLeft, "Left Analog Left"),
            new(LAnalogRight, "Left Analog Right"),
            new(RAnalogUp, "Right Analog Up"),
            new(OpenConfigMenu, "Open Config Menu"),
            new(OpenConfigMenuModifier, "Open Config Menu Modifier")];
    }

    [Serializable]
    public struct KeyCodeRef(KeyCode key = KeyCode.None)
    {
        public KeyCode keycode = key;
    }

    [Serializable]
    struct FloatRef(float val = 0)
    {
        public float value = val;
    }


    static class ControlsHelper
    {
        private static string path = Application.persistentDataPath + "1221CommunityPatchConfig.json";

        public static Config LoadBindsFromFile()
        {
            if (File.Exists(path)) return JsonUtility.FromJson<Config>(File.ReadAllText(path));
            Config conf = new();
            SaveBindsToFile(conf);
            return conf;
        }

        public static void SaveBindsToFile(Config conf)
        {
            File.WriteAllText(path, JsonUtility.ToJson(conf, prettyPrint: true));
        }
    }
}
