using System.Diagnostics;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

[assembly: MelonInfo(typeof(RichPresenceMod.EntryPoint), "RichPresenceMod", "1.0.4", "Atmudia", "https://www.nexusmods.com/slimerancher2/mods/12")]

namespace RichPresenceMod;

public class EntryPoint : MelonMod
{
    public static long startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    [DllImport("kernel32", CharSet = CharSet.Ansi)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    public override void OnInitializeMelon()
    {
        FileInfo discordSDK = new FileInfo(Path.Combine(MelonEnvironment.GameRootDirectory, "SlimeRancher2_Data", "Plugins", "x86_64", "discord_game_sdk.dll"));
        if (!discordSDK.Exists)
        {
            Stream manifestResourceStream = MelonAssembly.Assembly.GetManifestResourceStream("RichPresenceMod.discord_game_sdk.dll");
            byte[] array = new byte[manifestResourceStream.Length];
            _ = manifestResourceStream.Read(array, 0, array.Length);
            File.WriteAllBytes(discordSDK.FullName, array);
        }
        LoadLibrary(discordSDK.FullName);
        try
        {
            ClassInjector.RegisterTypeInIl2Cpp<DiscordController>();
            new GameObject("DiscordRichPresence", Il2CppType.Of<DiscordController>()).hideFlags |= HideFlags.HideAndDontSave;
        }
        catch (Exception ex)
        {
            string message = "Please report this exception to the main developer of the mod: " + ex.ToString();
            MelonLogger.Error(message);
        }

    }
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
     
        if (DiscordController.Instance != null && sceneName.Equals("MainMenuCamera"))
        {
            DiscordController.Instance.SetRichPresence();
        }
    }
}