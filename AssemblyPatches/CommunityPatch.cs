using UnityEngine;
namespace CommunityPatch
{
    internal static class CommunityPatch
    {
        public static UI.LogList DebugLog { private set; get; } = new();
        private static int MessageCount = 0;
        public static Config Config = new();
        private static bool initialized = false;

        public static void Init()
        {
            if (initialized) return;
            Config = ControlsHelper.LoadBindsFromFile();
            Timer2.Init();
            //UI.Init();
            AddLine("Initialized");
            initialized = true;
        }


        public static void Update()
        {
            UI.Update();
            // slowly remove log messages
            DebugLog.entries.ForEach(entry => { entry.Time -= Time.deltaTime; });
            DebugLog.entries.RemoveAll(entry => entry.Time < 0);

            if (ControlsHelper.ShouldDoAction(Config.StartBind.keycode, Config.StartBindModifier.keycode))
            {
                if (!Timer.TimeStart) 
                Timer.ResetTimer();
                Timer.StartTimer();//doesnt matter that start timer always runs it does nothing if time is alr started
                if (!Timer2.TimeStart) 
                Timer2.ResetTimer();
                Timer2.StartTimer();
            }
            if (ControlsHelper.ShouldDoAction(Config.ResetBind.keycode, Config.ResetBindModifier.keycode))
            {
                if (Timer.TimeStart) Timer.ResetTimer();
                if (Timer2.TimeStart) Timer2.ResetTimer();
            }
        }

        public static void AddLine(string Line, float length = 10)
        {
            MessageCount++;
            DebugLog.entries.Add(new(MessageCount.ToString() + "|" + Line, length));
        }
    }
}
