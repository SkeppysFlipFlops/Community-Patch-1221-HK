
using UnityEngine;
namespace CommunityPatch
{
    public class Timer
    {
        float TotalTime = 0f;
        float UnpauseTime = 0f;
        //float PauseTime = 0f;
        bool wasPaused = true;
        public float GetTime()
        {
            return TotalTime + Time.time - UnpauseTime;
        }
        private void RemoveLoadTimer()
        {
            wasPaused = true;
            TotalTime += Time.time - UnpauseTime;
        }
        private void StartTimingAgain()
        {
            wasPaused = false;
            UnpauseTime = Time.time;
        }
        public void Update(bool nowPaused)
        {
            if (!nowPaused && wasPaused)
            {
                StartTimingAgain();
            }
            else if (nowPaused && !wasPaused)
            {
                RemoveLoadTimer();
            }
        }
        public void resetTimer()
        {
            TotalTime = 0f;
            UnpauseTime =0f;
            wasPaused = true;
        }
    }
    public class TimerHelper
    {
        public Timer timer;
        public void Init() {
            timer = new Timer();

            On.GameManager.StartNewGame += (orig, self, param) => { orig(self, param); timer.resetTimer(); };
            On.GameManager.SetState += (orig, self, param) => { orig(self, param); CheckTimer(); };
        }
        public void CheckTimer()
        {
            timer.Update(true);//TODO: check for load
        }
    };
}
   