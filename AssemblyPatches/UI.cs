using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using UnityEngine;
using static MonoMod.InlineRT.MonoModRule;

namespace CommunityPatch
{
    internal static class UI
    {
        private static readonly Color WHITE = new(1, 1, 1, 1);
        private static readonly Vector2 __SIZE = new(200, 200);
        private static bool ConfigMenuOpen = false;
        public static List<Selector> Selectors = [];
        private static int row = 0;
        private static bool selecting = false;

        private static string BoolToText(bool condition, string text)
        {
            if (condition) return text;
            return "";
        }
        public class TextBoxData(string text, int x, int y, int fontSize, Color color)
        {
            public string text = text;
            public int x = x, y = y;
            public int fontSize = fontSize;
            public Color color = color;
        }

        public class LogList
        {
            public List<LogEntry> entries = [];

            public string[] GetLines()
            {
                List<string> lines = [];
                entries.ForEach(entry => { lines.Add(entry.Text); });
                return lines.ToArray();
            }
        }

        public class LogEntry(string text, float time)
        {
            public string Text = text;
            public float Time = time;
            public float GetAlpha()
            {
                return Mathf.Lerp(0, 1, Mathf.Clamp(Time, 0, 3) / 3);
            }
        }

        public class Selector(string text, int x, int y, int fontSize, Color color, KeyCodeRef keycode)
        {
            public string text = text;
            public int x = x, y = y;
            public int fontSize = fontSize;
            public Color color = color;
            public bool selected = false;
            public KeyCodeRef keycode = keycode;
            public TextBoxData GetTextBox()
            {
                return new TextBoxData(BoolToText(selected, "-->") + text.ToLower() + " currently bound to: " + keycode.keycode.ToString(),
                    x, y, fontSize, color);
            }
        }

        public static void ShowTextBox(TextBoxData data)
        {
            var oldBackgroundColor = GUI.backgroundColor;
            var oldContentColor = GUI.contentColor;
            var oldColor = GUI.color;
            var oldMatrix = GUI.matrix;
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            GUI.color = Color.white;
            GUI.matrix = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.identity,
                new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1f)
            );
            GUI.Label(
                position: new Rect(new(data.x, data.y), __SIZE),
                text: data.text,
                new GUIStyle
                {
                    fontSize = data.fontSize,
                    normal = new GUIStyleState
                    {
                        textColor = data.color
                    }
                }
            ) ;

            GUI.backgroundColor = oldBackgroundColor;
            GUI.contentColor = oldContentColor;
            GUI.color = oldColor;
            GUI.matrix = oldMatrix;
        }

        public static void Init()
        {
            KeyValuePair<KeyCodeRef, string>[] controlsRef = CommunityPatch.Config.ControlRef();

            for (int i = 0; i < controlsRef.Length; i++)
            {
                int y = (i * 70) + 30;
                Selectors.Add(new Selector(controlsRef[i].Value, 960, y, 30, WHITE, controlsRef[i].Key));
            }
        }
        public static void ShowGUI()
        {
#if DEBUG
            string warning = "Community Patch BETA v0.1, compiled @ " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString("00");
#elif RELEASE
            string warning = "Community Patch BETA v0.1";
#endif
            if (GameManager.instance.sceneName == "Menu_Title")
            {
                //show warn on menu
                ShowTextBox(new TextBoxData(
                    warning,
                    20, 20, 30,
                    Color.white));


                if (ConfigMenuOpen)
                {
                    ShowTextBox(new TextBoxData(
                        "Press enter while option selected to bind a key. escape to unbind.",
                        960, 50, 30,
                        Color.white));
                    foreach (Selector selec in Selectors)
                    {
                        ShowTextBox(selec.GetTextBox());
                    }
                }
            }
            else
            {
                ConfigMenuOpen = false;
                selecting = false;
            }
#if DEBUG
            //show logs and fade out
            for (int i = 0; i < CommunityPatch.DebugLog.entries.Count; i++)
            {
                float alpha = CommunityPatch.DebugLog.entries[i].GetAlpha();
                string text = CommunityPatch.DebugLog.entries[i].Text;
                int y = 300 + 40 * i;
                ShowTextBox(new TextBoxData(
                        text,
                        20, y, 15,
                        new Color(1, 1, 1, alpha)));
            }
#endif

            ShowTextBox(new TextBoxData(
               Timer.FormattedTime + " | " + Timer2.FormattedTime + "\n"
               + Timer.ShowOnUI + " | " + Timer2.FormattedRoomTime,
               1600, 20, 30,
               Color.white));
        }
        private static void ChooseKey()
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    selecting = false;
                    if (kcode == KeyCode.Escape)
                    {
                        if (row != 4)
                        {
                            Selectors[row].keycode.keycode = KeyCode.None;
                        }
                        else
                        {
                            Selectors[row].keycode.keycode = kcode;
                        }
                    }
                }
            }
        }
        private static void MoveUpDown()
        {
            Selectors[row].selected = false;
            if (Input.GetKeyDown(KeyCode.E))
            {
                row -= 1;
                if (row < 0) { row = Selectors.Count; }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                row += 1;
                if (row > Selectors.Count)
                {
                    row = 0;
                }
            }
            Selectors[row].selected = true;
        }
        public static void Update()
        {
            //open config menu if pressed bind
            if (DoAction(CommunityPatch.Config.OpenConfigMenu.keycode,
                CommunityPatch.Config.OpenConfigMenuModifier.keycode))
                ConfigMenuOpen = !ConfigMenuOpen;

            if (!ConfigMenuOpen) return;

            if (selecting) ChooseKey();

            if (Input.GetKeyDown(KeyCode.Return)) selecting = true;

            if (!selecting) MoveUpDown();
        }
        private static bool DoAction(KeyCode button, KeyCode modifier = KeyCode.None)
        {
            return Input.GetKeyDown(button) && (Input.GetKey(modifier) || modifier == KeyCode.None);
                
        }
    }
}
