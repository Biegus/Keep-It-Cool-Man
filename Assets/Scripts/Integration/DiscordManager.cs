using Discord;
using LetterBattle.Utility;
using System;
using UnityEngine;

namespace LetterBattle
{

    public class DiscordController : MonoBehaviour
    {
        public Discord.Discord DiscordManager { get; private set; } = null;
        private ActivityManager activityManager = null;

        private bool lost = false;

        private long gameStartTime;
        private DiscordController instance;
        private Ticker updateTicker;

        private readonly string[] planetNamesToChapters = new string[] { "Earth", "Moon", "Spaceballs", "Mars", "Earth?" };

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            GameObject g = new GameObject();
            DontDestroyOnLoad(g);
            g.hideFlags |= HideFlags.HideInHierarchy;
            g.AddComponent<DiscordController>();
        }
        private void Awake()
        {
            updateTicker = TickerCreator.CreateNormalTime(5f);
            if (instance != null)
            {
                Debug.LogError("Double discord manager");
                Destroy(this.gameObject);
            }
            instance = this;
        }

        protected void Start()
        {
            CreateDiscordManager();
            RefreshActivity();



        }
        private void Disconnect()
        {
            if (lost) return;
            lost = true;
        }


        private void CreateDiscordManager()
        {
            DiscordManager?.Dispose();
            try
            {
                DiscordManager = new Discord.Discord(997168088388751402, (UInt64)CreateFlags.NoRequireDiscord);
                activityManager = DiscordManager.GetActivityManager();
                gameStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            catch (ResultException)
            {
                Disconnect();
            }

        }



        private void RefreshActivity()
        {
            Activity activity = GetUpdatedActivity();

            activityManager?.UpdateActivity(activity, CheckGood);
        }

        private Activity GetUpdatedActivity()
        {

            Activity activity = new Activity();

            activity.Timestamps.Start = gameStartTime;

            if (!GameManager.IsGameOn)
            {

                activity.Details = "Wandering around in the menu";
                activity.Assets.LargeImage = $"ch_1";
            }

            else
            {
                int lvlNum = GameManager.CurrentLevelNumber;
                int chapter = (lvlNum >= 0) ? GameAsset.Current.GetChapter(lvlNum) : 0;
                string levelName = GameManager.CurrentLevel.LevelName;
                activity.Details = $"In Level {lvlNum} ({levelName})";
                activity.Assets.LargeText = planetNamesToChapters[chapter];
                activity.Assets.LargeImage = $"ch_{chapter + 1}";
            }

            return activity;
        }

        protected void OnApplicationQuit()
        {
            DiscordManager?.Dispose();
        }

        private static void CheckGood(Result res)
        {
            if (res != Result.Ok)
            {
                Debug.LogWarning($"DiscordController: {res.ToString()}");
            }
        }

        protected void Update()
        {
            if (lost)
                return;
            try
            {
                if (updateTicker.Push())
                {
                    RefreshActivity();
                }
                DiscordManager.RunCallbacks();

            }
            catch (ResultException)
            {
                Debug.Log("Discord disconnected");
                Disconnect();
            }

        }
    }
}