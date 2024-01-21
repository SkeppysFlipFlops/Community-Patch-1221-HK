using MonoMod;

#pragma warning disable CS0626 //for orig_x functions
#pragma warning disable IDE0051 // Remove unused private members
namespace CommunityPatch
{

    [MonoModPatch("global::GameManager")]
    public class GameManagerPatch : global::GameManager
    {
        private void OnGUI()
        {
            CommunityPatch.ShowGUI();
        }

        public extern void orig_Start();

        public void Start()
        {
            orig_Start();
            CommunityPatch.Init();
        }
    }
}