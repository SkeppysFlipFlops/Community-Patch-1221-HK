using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace CommunityPatch
{
    internal static class UI
    {
        private static readonly Vector2 __SIZE = new(200, 200);
        public class TextBoxData(string text, int x, int y, int fontSize)
        {
            public string text = text;
            public int x = x, y = y;
            public int fontSize = fontSize;
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
                position: new Rect(new(data.x,data.y),__SIZE),
                text: data.text,
                new GUIStyle
                {
                    fontSize = data.fontSize,
                    normal = new GUIStyleState
                    {
                        textColor = Color.white,
                    }
                }
            );

            GUI.backgroundColor = oldBackgroundColor;
            GUI.contentColor = oldContentColor;
            GUI.color = oldColor;
            GUI.matrix = oldMatrix;
        }
    }
}
