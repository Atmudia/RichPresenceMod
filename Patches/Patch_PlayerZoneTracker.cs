using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.World;

namespace RichPresenceMod.Patches;


[HarmonyPatch(typeof(PlayerZoneTracker), "OnEntered")]
public static class Patch_PlayerZoneTracker
{
    public static void Prefix(ZoneDefinition zone)
    {
        if (DiscordController.Instance != null)
        {
            DiscordController.Instance.SetRichPresence(zone);
        }
    }
    
}