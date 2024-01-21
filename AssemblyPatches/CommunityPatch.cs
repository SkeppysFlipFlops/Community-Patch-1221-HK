using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunityPatch
{
    internal static class CommunityPatch
    {
        private static int messagecount = 0;   
        private static string checker = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString("00");
        private static List<string> DebugLog = [];
        public static void Init()
        {
            Timer2.Init();
        }
        public static void ShowGUI()
        {
            UI.ShowTextBox(new UI.TextBoxData("Community Patch BETA, compiled @ " + checker, 20, 20, 30));
            UI.ShowTextBox(new UI.TextBoxData(string.Join("\n", DebugLog.ToArray()), 500, 300, 30));
            UI.ShowTextBox(new UI.TextBoxData(Timer.FormattedTime + " | " + Timer2.FormattedTime, 1600, 20, 30));
        }
        public static void AddLine(string Line)
        {
            if (DebugLog.Count>= 5)
            {
                DebugLog.RemoveAt(0);   
            }
            DebugLog.Add(messagecount.ToString() + "|" + Line);
            messagecount++;
            if (messagecount >= 5) {
                messagecount = 0;
            }
        }
    }
}
