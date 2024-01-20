using CommunityPatch;
using MonoMod;

// ReSharper disable All
public class patch_CameraController : CameraController
{
    private void OnPreRender()
    {
        if (GameManager.instance is not { } gameManager)
        {
            return;
        }
        Timer.OnPreRender(gameManager);
    }
}