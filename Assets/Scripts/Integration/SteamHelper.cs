using UnityEngine;

namespace LetterBattle
{
    public static class SteamHelper
    {
        private static bool LostSteamApi = false;
        public static void Unlock(string achievement)
        {
            try //not the best but time to check how to check if steamworks is inited
            {
                Steamworks.SteamUserStats.GetAchievement(achievement, out bool achieved);
                if (achieved) return;
                
                Steamworks.SteamUserStats.SetAchievement(achievement);
                Steamworks.SteamUserStats.StoreStats();
                
            }
            catch
            {
                LostSteamApi = true;
            }
         
            
        }
    }
}