using GlobalEnums;
using System.Reflection;
using System;
using UnityEngine;
using MonoMod;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
namespace CommunityPatch
{
    internal static class Timer2
    {
        private static readonly FieldInfo TeleportingFieldInfo = typeof(CameraController).GetField(
            "teleporting",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        private static readonly FieldInfo TilemapDirtyFieldInfo = typeof(GameManager).GetField(
            "tilemapDirty",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        private static bool TimeStart = false;
        private static bool TimeEnd = false;
        private static bool LastFramePaused = true;
        public static string FormattedTime
        {
            get
            {
                return GetFormattedTime(CurrentTime);
            }
        }
        private static float CurrentTime
        {
            get
            {
                if (LastFramePaused)
                {
                    return RoomEnterTime;
                }
                return RoomEnterTime + Time.unscaledTime - RoomEnterTimestamp;
            }
        }
        private static readonly int minorVersion = int.Parse(Constants.GAME_VERSION.Substring(2, 1));
        private static float RoomEnterTime = 0f;
        private static float RoomEnterTimestamp= 0f;
        public static string FormattedRoomTime
        {
            get
            {
                return GetFormattedTime(RoomTime);
            }
        }
        private static float RoomTime = 0;

        private static GameState lastGameState;
        private static bool lookForTeleporting;

        public static void Init()
        {
            //reset timer?
            //On.GameManager.StartNewGame += (orig, self, param) => { orig(self, param); ResetTimer(); }; //1 Param

            On.UIManager.SetState += (orig, self, param) => { orig(self, param); CheckTimer(); }; //1 Param
            On.GameManager.SetState += (orig, self, param) => { orig(self, param); CheckTimer(); }; //1 Param
            On.GameManager.UpdateSceneName += (orig, self) => { orig(self); CheckTimer(); }; //No Param
            On.GameManager.RefreshTilemapInfo += (orig, self, param) => { orig(self, param); CheckTimer(); }; //1 Param
            On.InputHandler.StartAcceptingInput += (orig, self) => { orig(self); CheckTimer(); }; //No Param
            On.InputHandler.StopAcceptingInput += (orig, self) => { orig(self); CheckTimer(); }; //No Param
            On.InputHandler.StartUIInput += (orig, self) => { orig(self); CheckTimer(); }; //No Param
            On.InputHandler.StopUIInput += (orig, self) => { orig(self); CheckTimer(); }; //No Param
            On.HeroController.SetCState += (orig, self, param, param2) => { orig(self, param, param2); CheckTimer(); }; // 2 param
            CommunityPatch.AddLine("loaded hooks");
            // we r missing Herocontroller.Transitionstate. 
        }
        public static void ResetTimer()
        {
            //TODO FIX
            TimeStart= false;
            TimeEnd= false;
            //InGameTime = 0f;
        }
        public static void StartTimer()
        {
            TimeStart = true;
        }
        private static void TickedGameplay()
        {
            if (LastFramePaused)
            {
                CommunityPatch.AddLine("ticked Gameplay");
                RoomEnterTimestamp = Time.unscaledTime;
            }
            LastFramePaused = false;
        }
        private static void TickedLoading()
        {
            if (!LastFramePaused)
            {
                RoomTime  = Time.unscaledTime - RoomEnterTimestamp;
                CommunityPatch.AddLine("ticked Load");
                RoomEnterTime += Time.unscaledTime - RoomEnterTimestamp;
            }
            RoomEnterTimestamp = Time.unscaledTime;
            LastFramePaused = true;
        }
        public static void CheckTimer()
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
                    || minorVersion < 3 && (bool)TilemapDirtyFieldInfo.GetValue(gameManager);
            }
            catch
            {
                // ignore
            }

            lastGameState = gameState;

            if (TimeStart && !timePaused && !TimeEnd)
            {
                TickedGameplay();
            }
            else
            {
                TickedLoading();
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
