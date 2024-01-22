using MonoMod;

#pragma warning disable CS0626 //for orig_x functions
#pragma warning disable IDE0051 // Remove unused private members
namespace CommunityPatch
{

    [MonoModPatch("global::GameManager")]
    public class GameManagerPatch : global::GameManager
    {
        private void OnGUI() { UI.ShowGUI(); }

        private void Update() { CommunityPatch.Update(); }

        public void Start()
        {
            orig_Start();
            CommunityPatch.Init();
        }
        public extern void orig_Start();
    }
}