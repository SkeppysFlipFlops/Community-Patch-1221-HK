using MonoMod;
#pragma warning disable CS0626 //for orig_x functions
#pragma warning disable IDE0051 // Remove unused private members
namespace CommunityPatch
{

    [MonoModPatch("global::CameraController")]
    public class CameraControllerPatch : global::CameraController
    {
        private void OnPreRender()
        {
            if (GameManager.instance is not { } gameManager)
            {
                return;
            }
            Timer.Checktimer();
        }
    }
}