using UnityEngine;
using MonoMod;
using System.Reflection;

#pragma warning disable CS0626

namespace CommunityPatch
{

    [MonoModPatch("global::GameManager")]
    public class GameManagerPatch : global::GameManager
    {

        private void OnGUI()
        {
            if (this.GetSceneNameString() == Constants.MENU_SCENE)
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

                string WarningText = "Community Patch Beta";

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
        }


        private static void errorLogging(string message)
        {
            GUI.Label(
                   new Rect(20f, 20f, 200f, 200f),
                   message,
                   new GUIStyle
                   {
                       fontSize = 30,
                       normal = new GUIStyleState
                       {
                           textColor = Color.white,
                       }
                   }
               );
        }

        public extern void orig_Start();

        public void Start()
        {
            errorLogging("asdasdasd");
            orig_Start();
            On.GameManager.SetState += (orig,state) =>
            {
                orig(state)
            }
        }
        private void UpdateTimer()
        {

        }
    }
}