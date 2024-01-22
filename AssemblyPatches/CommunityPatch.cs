using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace CommunityPatch
{
    internal static class CommunityPatch
    {
        public static UI.LogList DebugLog { private set; get; } = new();
        private static int MessageCount = 0;
        public static Config Config = new();

        public static void Init()
        {
            Config = ControlsHelper.LoadBindsFromFile();
            Timer2.Init();
            UI.Init();
        }


        public static void Update()
        {
            UI.Update();
            // slowly remove log messages
            DebugLog.entries.ForEach(entry => { entry.Time -= Time.deltaTime; });
            DebugLog.entries.RemoveAll(entry => entry.Time < 0);


        }

        public static void AddLine(string Line, float length = 10)
        {
            MessageCount++;
            DebugLog.entries.Add(new(MessageCount.ToString() + "|" + Line, length));
            if (MessageCount >= 5 ) MessageCount = 0;
        }
    }
}
