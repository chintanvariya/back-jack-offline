using UnityEngine;

namespace BlackJackOffline
{
    public class BlackJackDataManager : MonoBehaviour
    {
        public static string userName
        {
            get { return PlayerPrefs.GetString("userName", ""); }
            set { PlayerPrefs.SetString("userName", value); }
        }

        public static float creditPoint
        {
            get { return PlayerPrefs.GetFloat("creditPoint", 10000f); }
            set { PlayerPrefs.SetFloat("creditPoint", value); }
        }

        public static int ProfileImage
        {
            get { return PlayerPrefs.GetInt("ProfileImage", 1); }
            set { PlayerPrefs.SetInt("ProfileImage", value); }
        }

        public static string lobbyName
        {
            get { return PlayerPrefs.GetString("lobbyName", ""); }
            set { PlayerPrefs.SetString("lobbyName", value); }
        }

        public static float lobbyMinValue
        {
            get { return PlayerPrefs.GetFloat("lobbyMinValue", 0f); }
            set { PlayerPrefs.SetFloat("lobbyMinValue", value); }
        }

        public static float lobbyMaxValue
        {
            get { return PlayerPrefs.GetFloat("lobbyMaxValue", 0f); }
            set { PlayerPrefs.SetFloat("lobbyMaxValue", value); }
        }

        public static int lobbyIndex
        {
            get { return PlayerPrefs.GetInt("lobbyIndex", 0); }
            set { PlayerPrefs.SetInt("lobbyIndex", value); }
        }

        public static float RoundPlay
        {
            get { return PlayerPrefs.GetFloat("RoundPlay", 0f); }
            set { PlayerPrefs.SetFloat("RoundPlay", value); }
        }

        public static float RoundWin
        {
            get { return PlayerPrefs.GetFloat("RoundWin", 0f); }
            set { PlayerPrefs.SetFloat("RoundWin", value); }
        }

        public static float RoundLoss
        {
            get { return PlayerPrefs.GetFloat("RoundLoss", 0f); }
            set { PlayerPrefs.SetFloat("RoundLoss", value); }
        }

    }
}
