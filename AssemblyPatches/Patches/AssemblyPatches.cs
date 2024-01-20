using UnityEngine;
using MonoMod;
#pragma warning disable CS0626

namespace CommunityPatch
{

    [MonoModPatch("global::GameManager")]
    public class GameManagerPatch : global::GameManager
    {
        private string error = "";
        private void OnGUI()
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
            string WarningText = "beta patch. , Time: " + Timer.FormattedTime + " " + error;

            GUI.Label(
                new Rect(20f, 20f, 200f, 200f),
                WarningText,
                new GUIStyle
                {
                    fontSize = 30,
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


        private void errorLogging(string message)
        {
            error += message;
        }

        public extern void orig_Start();

        public void Start()
        {
            orig_Start();
            errorLogging("test2");
        }
    }
}
