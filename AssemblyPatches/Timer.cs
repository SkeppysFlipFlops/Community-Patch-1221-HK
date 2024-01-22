using GlobalEnums;
using System.Reflection;
using System;
using UnityEngine;
namespace CommunityPatch
{
    internal static class Timer
    {
        private static readonly FieldInfo TeleportingFieldInfo = typeof(CameraController).GetField(
            "teleporting",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        private static readonly FieldInfo TilemapDirtyFieldInfo = typeof(GameManager).GetField(
            "tilemapDirty",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        private static string nextScene ="";
        public static bool TimeStart = false;
        public static bool TimeEnd = false;
        private static float InGameTime = 0f;
        private static readonly int MinorVersion = int.Parse(Constants.GAME_VERSION.Substring(2, 1));
        private static float RoomEnterTime = 0f;
        public static string ShowOnUI = "";
        public static string FormattedTime
        {
            get
            {
                return GetFormattedTime(InGameTime);
            }
        }

        private static GameState lastGameState;
        private static bool lookForTeleporting;
        public static void ResetTimer()
        {
            TimeStart= false;
            TimeEnd= false;
            InGameTime = 0f;
        }
        public static void StartTimer()
        {
            TimeStart = true;
        }
        public static void Checktimer()//, StringBuilder infoBuilder)
        {
            GameManager gameManager = GameManager.instance;
            string currentScene = gameManager.sceneName;
            string nextScene = gameManager.nextSceneName;
            GameState gameState = gameManager.gameState;

            if (!TimeStart && (nextScene.Equals("Tutorial_01", StringComparison.OrdinalIgnoreCase) && gameState == GameState.ENTERING_LEVEL ||
                               nextScene is "GG_Vengefly_V" or "GG_Boss_Door_Entrance" or "GG_Entrance_Cutscene"))
            {
                TimeStart = true;
                TimeEnd = false;
            }

            if (TimeStart && !TimeEnd && (nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase) ||
                                          nextScene == "GG_End_Sequence"))
            {
                TimeEnd = true;
            }

            bool timePaused = false;

            // thanks ShootMe, in game time logic copy from https://github.com/ShootMe/LiveSplit.HollowKnight
            try
            {
                UIState uiState = gameManager.ui.uiState;
                bool loadingMenu = currentScene != "Menu_Title" && string.IsNullOrEmpty(nextScene) ||
                                   currentScene != "Menu_Title" && nextScene == "Menu_Title";
                if (gameState == GameState.PLAYING && lastGameState == GameState.MAIN_MENU)
                {
                    lookForTeleporting = true;
                }

                bool teleporting = (bool)TeleportingFieldInfo.GetValue(gameManager.cameraCtrl);
                if (lookForTeleporting && (teleporting || gameState != GameState.PLAYING && gameState != GameState.ENTERING_LEVEL))
                {
                    lookForTeleporting = false;
                }

                timePaused =
                    gameState == GameState.PLAYING && teleporting && gameManager.hero_ctrl?.cState.hazardRespawning == false
                    || lookForTeleporting
                    || gameState is GameState.PLAYING or GameState.ENTERING_LEVEL && uiState != UIState.PLAYING
                    || gameState != GameState.PLAYING && !gameManager.inputHandler.acceptingInput
                    || gameState is GameState.EXITING_LEVEL or GameState.LOADING
                    || gameManager.hero_ctrl?.transitionState == HeroTransitionState.WAITING_TO_ENTER_LEVEL
                    || uiState != UIState.PLAYING &&
                    (loadingMenu || uiState != UIState.PAUSED && (!string.IsNullOrEmpty(nextScene) || currentScene == "_test_charms")) &&
                    nextScene != currentScene
                    || MinorVersion < 3 && (bool)TilemapDirtyFieldInfo.GetValue(gameManager);
            }
            catch
            {
                // ignore
            }

            lastGameState = gameState;

            if (TimeStart && !timePaused && !TimeEnd)
            {
                InGameTime += Time.unscaledDeltaTime;
            }
            else
            {
                if (RoomEnterTime != InGameTime && nextScene != currentScene)
                {
                    ShowOnUI = GetFormattedTime(InGameTime - RoomEnterTime);
                    RoomEnterTime = InGameTime;
                }
            }
        }
        private static string GetFormattedTime(float time)
        {
            if (time == 0)
            {
                return string.Empty;
            }
            else if (time < 60)
            {
                return time.ToString("F3");
            }
            else if (time < 3600)
            {
                int minute = (int)(time / 60);
                float second = time - minute * 60;
                return $"{minute}:{second.ToString("F3").PadLeft(5, '0')}";
            }
            else
            {
                int hour = (int)(time / 3600);
                int minute = (int)((time - hour * 3600) / 60);
                float second = time - hour * 3600 - minute * 60;
                return $"{hour}:{minute.ToString().PadLeft(2, '0')}:{second.ToString("F3").PadLeft(5, '0')}";
            }
        }
    }
}
