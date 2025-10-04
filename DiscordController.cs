using System;
using System.Linq;
using System.Text;
using Discord;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.UI.DateAndTime;
using Il2CppMonomiPark.SlimeRancher.World;
using MelonLoader;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Object = Il2CppSystem.Object;

namespace RichPresenceMod
{
    public class DiscordController : MonoBehaviour
    {
        public DiscordController(IntPtr value)
            : base(value)
        {
        }

        public void Awake()
        {
            if (ifExist && Instance == null)
            {
                MelonLogger.Msg("is it somewhere?");
                Destroy(this);
            }
            else
            {
                gameObject.hideFlags |= HideFlags.HideAndDontSave;
                ifExist = true;
                Instance = this;
            }
        }

        public void Start()
        {
            try
            {
                _discord = new Discord.Discord(applicationId, 1UL);

            }
            catch (Exception e)
            {
                string message = "Please report this exception to the main developer of the mod: " + e.ToString();
                MelonLogger.Error(message);
                Destroy(this);
            }
        }

        public void Update()
        {
            try
            {
                _discord.RunCallbacks();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                Destroy(gameObject);
            }
        }

        public void LateUpdate()
        {
            if (ifZone && TimeLocalized == null)
            {
                TimeDisplay timeDisplay = Resources.FindObjectsOfTypeAll<TimeDisplay>().First(x => x._text != null);
                DayDisplay dayDisplay = Resources.FindObjectsOfTypeAll<DayDisplay>().First();
                dayDisplay.Start();
                TimeLocalized = timeDisplay._text.StringReference;
                TimeDisplay = timeDisplay;
                DayLocalized = dayDisplay._text.StringReference;
                DayDisplay = dayDisplay;

            }
            try
            {
                if (_activity != null)
                {
                    if (ifZone)
                    {
                        ActivityManager activityManager = _discord.GetActivityManager();
                        Activity value = _activity.Value;
                        DayDisplay.Update();
                        TimeDisplay.Update();
                        
                        value.State = EncodeUtf8(DayLocalized.GetLocalizedString() + " " + TimeLocalized.GetLocalizedString());

                        activityManager.UpdateActivity(value, delegate(Result result)
                        {
                            if (result > Result.Ok)
                            {
                                MelonLogger.Msg("Failed to connect to Discord");
                            }
                        });
                    }
                    else
                    {
                        ActivityManager activityManager2 = _discord.GetActivityManager();
                        Activity value2 = _activity.Value;
                        activityManager2.UpdateActivity(value2, delegate(Result result)
                        {
                            if (result > Result.Ok)
                            {
                                MelonLogger.Msg("Failed to connect to Discord");
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                throw;
            }
        }

        public void SetRichPresence(ZoneDefinition zone)
        {
            ifZone = true;
            if (_timeDirector == null)
            {
                _timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
            }
            string text = zone.RichPresenceId + "_image";
            Activity activity = new Activity
            {
                Details = EncodeUtf8(zone._localizedName.GetLocalizedString()),
                State = null,
                Timestamps =
                {
                    Start = EntryPoint.startTimestamp
                },
                Assets = new ActivityAssets
                {
                    LargeImage = text,
                    LargeText = "Slime Rancher 2",
                    SmallText = "Rich Presence Mod",
                    // SmallImage = "logo"
                }
            };
            _activity = activity;
        }
        public void SetRichPresence()
        {
            ifZone = false;
            Activity activity = new Activity
            {
                Details = EncodeUtf8("Main Menu"),
                // State = "",
                Timestamps =
                {
                    Start = EntryPoint.startTimestamp
                },
                Assets = new ActivityAssets
                {
                    LargeImage = "logo",
                    LargeText = "Slime Rancher 2",
                    SmallText = "Rich Presence Mod",
                    SmallImage = "logo"
                }
            };
            ;
            _activity = activity;
        }

        public static byte[] EncodeUtf8(string input, int size = 128)
        {
            if (size <= 0)
                throw new ArgumentException("Size must be positive");

            byte[] buffer = new byte[size]; // fixed-size buffer
            if (string.IsNullOrEmpty(input))
                return buffer; // empty buffer (all zeros)

            byte[] utf8Bytes = Encoding.UTF8.GetBytes(input);

            // Copy as much as fits (reserve 1 byte for null terminator)
            int count = Math.Min(utf8Bytes.Length, buffer.Length - 1);
            Array.Copy(utf8Bytes, buffer, count);

            // Null-terminate
            buffer[count] = 0;

            return buffer;
        }

        public long applicationId = 1049351538713309266L;
        public static DiscordController Instance;
        public bool ifExist;
        private Discord.Discord _discord;
        private static TimeDirector _timeDirector;
        private Activity? _activity;
        public bool ifZone;
        public static LocalizedString TimeLocalized;
        public static LocalizedString DayLocalized;

        public static TimeDisplay TimeDisplay;
        public static DayDisplay DayDisplay;

    }
    
}
