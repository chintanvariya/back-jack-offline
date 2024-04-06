using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackGameManager : MonoBehaviour
    {
        public static BlackJackGameManager instance;
        [Header("--------------------- Game Default Values --------------------- ")]
        [SerializeField ,Range(10, 20)]
        internal float placeBetTimer;
        [SerializeField, Range(10, 20)]
        internal float turnTimer , insuranceTimer;
        [SerializeField]
        internal List<Sprite> userTurnSprites;

        [Header("--------------------- Lobby And Screen --------------------- ")]
        [SerializeField]
        internal List<BlackJackLobby> blackJackLobbies;
        [SerializeField]
        internal List<GameObject> gameScreens;
        public SimpleScrollSnap lobbyScroll;

        [Header("--------------------- Managers --------------------- ")]
        [SerializeField]
        internal BlackJackGameBoardManager boradManager;
        [SerializeField]
        internal BlackJackBetButtons betButtons;
        [SerializeField]
        internal BlackJackPooler pooler;
        [SerializeField]
        internal BlackJackDealer dealer;
        [SerializeField]
        internal BlackJackPopupManager popupManager;
        [SerializeField]
        internal BlackjackDailySpin dailySpin;
        [Header("--------------------- User Info ---------------------")]
        [SerializeField]
        internal Text userNameTxt;
        [SerializeField]
        internal Text craditPointTxt;
        [SerializeField]
        private Image profileImage;
        [SerializeField]
        internal List<Sprite> profileSprites;

        //Bot name
        internal List<string> botsName = new List<string> { "Zepp", "Cody", "Kelsy", "Ariel", "Blade", "Ace", "Liam", "Josiah", "Alina", "Ellie", "Anna", "Noah", "Lucero", "Rashad", "Faven", "Estel", "Mya", "Aurora", "Noah", "Caleb", "Emersyn", "Madison", "Iris", "Fatima", "Theo", "Amore", "Dodie", "Bayo", "Winona", "Leah", "Blaze", "Oliver", "Cooper", "Sienna", "Grace", "Bella", "Danna", "Oliver", "Amour", "Bonamy", "Eagor", "Nicole", "Cherry", "Captain", "James", "Lincoln", "Mary", "Isla", "Eloise", "Alessia", "George", "Optimus", "Amicia", "Ryn", "Walter", "Betty Bobo", "Comet", "Elijah", "Miles", "Isabelle", "Willow", "Skylar", "Mckenzie", "Leo", "Trevor", "Carine", "Lyka", "Neil", "Billie", "Guardian", "William", "Christopher", "Alaia", "Zoe", "Jade", "Wynter", "Freddie", "Ulima", "Solada", "Gedith", "Derek", "Jennybot", "Maverick", "Henry", "Nathan", "Esther", "Riley", "Gabriella", "Fiona", "Arthur", "Thaddeus", "Dakota", "Kaida", "Taylor", "Halie", "Pilot", "Lucas", "Isaiah", "Sloane", "Stella", "Ariana", "Brooklynn", "Archie", "Ravyn", "Rona", "Hydra", "Raymond", "Chappie", "Nova", "Benjamin", "Kai", "Mackenzie", "Eliana", "Maria", "Gracelynn", "Alfie", "Ravyn", "Rinc", "Pendragon", "Lincoln", "Harley", "Stellar", "Theodore", "Joshua", "Adelina", "Ivy", "Adeline", "Luciana", "Charlie", "Maxime", "Oreo", "Bruno", "Twinklerry", "Hello Robot", "Titan", "Mateo", "Andrew", "Raya", "Victoria", "Lydia", "Alexis", "Oscar", "Terry", "Felix", "Percy", "Bubble", "UNO", "Jaguar", "Levi", "Angel", "Astrid", "Emilia", "Sarah", "Everlee", "Henry", "Hugo", "Dice", "Silverto", "AmazBot", "BOT", "Hunter", "Sebastian", "Adrian", "Azalea", "Zoey", "Nevaeh", "Laura", "Harry", "Lark", "Eddie", "Mikie", "Sweetie", "Forte Bot", "Dominic", "Daniel", "Cameron", "Samuel", "Naomi", "Serenity", "Selah", "Jack", "Amber", "Stevie", "Flash", "Hermione", "Flying Droid", "Jace", "Jack", "Nolan", "Gabriel", "Hannah", "Liliana", "Reign", "Teddy", "Toyoda", "Sundae", "Support Bot", "Chatbot", "Alphius", "Gael", "Michael", "Waylon", "Eddie", "Lucy", "Ayla", "Alayah", "Finley", "Kamei", "Ivan", "David", "Charles", "Eyebot", "River", "Alexander", "Jaxon", "Bobby", "Elena", "Everleigh", "Rosemary", "Arlo", "Nakai", "Yolkie", "Benjamin", "Stan", "Laughbot", "Thiago", "Owen", "Roman", "Frankie", "Lillian", "Raelynn", "Lilliana", "Luca", "Seki", "Chloe", "Matt", "Rhett", "Airbender", "Kayden", "Asher", "Eli", "Olivia", "Maya", "Allison", "Ariyah", "Jacob", "Sunny", "Lacey", "James", "Steve", "Agnes", "Damian", "Samuel", "Wesley", "Emma", "Leah", "Madeline", "Heidi", "Tommy", "Todd", "Marcus", "Jake", "Cass", "Gossip Girl", "August", "Ethan", "Aaron", "Charlotte", "Paisley", "Vivian", "Esmeralda", "Lucas", "Ted", "Lucas", "Liam", "Gilbert", "Space Nomad", "Carson", "Leo", "Ian", "Amelia", "Addison", "Maeve", "Logan", "Theodore", "Leo", "Charlie", "Ethan", "Cyan", "Bot", "Austin", "Jackson", "Christian", "Sophia", "Natalie", "Lyla", "Amora", "Max", "Andrew", "Mike", "Daniel", "Codie", "Artie", "Myles", "Mason", "Ryan", "Isabella", "Valentina", "Samantha", "Kalani", "Isaac", "Tate", "Will", "Thomas", "Frank", "Muse", "Amir", "Ezra", "Leonardo", "Ava", "Everly", "Rylee", "Leighton", "Albie", "Liam", "Harrison", "John", "Karl", "Picasso", "Declan", "John", "Brooks", "Mia", "Delilah", "Eva", "Cali", "James", "Luke", "Jack", "Maya", "Mary", "Da Vinci", "Emmett", "Hudson", "Axel", "Evelyn", "Leilani", "Melody", "Melissa", "Rupert", "Bob", "Richard", "Bess", "Audrey", "Van Gogh", "Ryder", "Luca", "Walker", "Luna", "Madelyn", "Clara", "Aniyah", "Eli", "Tom", "Oliver", "Maria", "Dion", "Rembrandt", "Luka", "Aiden", "Jonathan", "Harper", "Kinsley", "Hadley", "Izabella", "Myles", "Ross", "Dave", "Sarah", "Anne", "Monet", "Grayson", "Joseph", "Easton", "Camila", "Ruby", "Julia", "Michelle", "Brodie", "Helper Bot", "Vulture", "Ursula", "Juliet", "Warhol", "Elliot", "David", "Everett", "Sofia", "Sophie", "Piper", "Raelyn", "Parker", "Ashley", "Alice", "Diana", "Helen", "Basquiat", "Caleb", "Jacob", "Weston", "Scarlett", "Alice", "Juniper", "Alessandra", "Ralph", "Ava", "Rosa", "Somerset", "Daisy", "Klimt", "Benjamin", "Logan", "Bennett", "Elizabeth", "Genesis", "Parker", "Viviana", "Miles", "Poppy", "Jenny", "Portia", "Scarlett", "Escher", "Zachary", "Luke", "Robert", "Eleanor", "Claire", "Brielle", "Madeleine", "Jayden", "Ella", "Amy", "Marina", "Lola", "Dali", "Brody", "Julian", "Jameson", "Emily", "Audrey", "Eden", "Arielle", "Billy", "Lily", "Phoebe", "Celia", "Lucy", "Jolt Jester", "Jackson", "Gabriel", "Landon", "Chloe", "Sadie", "Remi", "Serena", "Elliott", "Mia", "Kathy", "Pollock", "Robotic Rebel", "Maverick", "Ollie", "Grayson", "Silas", "Mila", "Aaliyah", "Josie", "Francesca", "Jax", "Emily", "Katie", "Hopper", "Synapse Sage", "Phoenix", "Jasper", "Wyatt", "Jose", "Violet", "Josephine", "Rose", "Brynn", "Ryan", "Olivia", "Camellia", "Michelangelo", "Magritte", "Rebel", "Liam", "Matthew", "Beau", "Penelope", "Autumn", "Arya", "Gwendolyn", "Joey", "Isabelle", "Jessie", "Cezanne", "Calder", "Neo", "Stanley", "Maverick", "Micah", "Gianna", "Brooklyn", "Eliza", "Kira", "Blind", "Sophie", "Catherine", "Machine Mind", "Evobot", "Sally", "Sonny", "Dylan", "Colton", "Aria", "Quinn", "Charlie", "Destiny", "Dudley", "Aragog", "Demon", "O�Keeffe", "Storm", "Blade", "Blake", "Isaac", "Jordan", "Abigail", "Kennedy", "Peyton", "Elle", "Duke", "Thanos", "Sluggish", "Rothko", "Titan", "Knight", "Albert", "Elias", "Jeremiah", "Ella", "Cora", "Daisy", "Makayla", "James", "Venom", "HelpDesk Bot", "Quantum Queen", "Vector", "Raven", "Joseph", "Anthony", "Parker", "Avery", "Savannah", "Lucia", "Alaya", "Jonathan", "Cobra", "Dragon", "Crystal", "Voyager", "Odyssey", "Chester", "Thomas", "Greyson", "Hazel", "Caroline", "Millie", "Malani", "Kingston", "Wolf", "Meteor", "Diamond", "Warrior", "Kronos", "Carter", "Jayden", "Rowan", "Nora", "Athena", "Margaret", "Willa", "Laurence", "Shark", "Nebula", "Eclipse", "Zenith", "Oracle", "David", "Carter", "Adam", "Layla", "Natalia", "Freya", "Saige", "Lloyd", "Hawk", "Viper", "Flash", "Blaze", "Phoenix", "Milo", "Santiago", "Nicholas", "Lily", "Hailey", "Melanie", "Makenna", "Norman", "Jaguar", "Mustang", "Galaxy", "Cloud", "Pulse", "Ellis", "Ezekiel", "Theo", "Aurora", "Aubrey", "Elliana", "Remington", "William", "Falcon", "Panther", "Infinity", "Sentinel", "Ranger", "Jenson", "Charles", "Xavier", "Nova", "Emery", "Adalynn", "Demi", "Gray" };

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            Input.multiTouchEnabled = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateUserInfo();
            foreach (var lobby in blackJackLobbies)
            {
                lobby.LobbyJoinButton.onClick.AddListener(() => LobbyJoinClicked(lobby));
            }
        }

        private void LobbyJoinClicked(BlackJackLobby lobby)
        {
            if (BlackJackDataManager.creditPoint < lobby.minAmount)
            {
                popupManager.CoinStoreShow("");
            }
            else
            {
                lobby.LobbyJoinButton.interactable = false;
                BlackJackDataManager.lobbyName = lobby.lobbyName;
                BlackJackDataManager.lobbyMinValue = lobby.minAmount;
                BlackJackDataManager.lobbyMaxValue = lobby.maxAmount;
                BlackJackDataManager.lobbyIndex = lobby.lobbyIndex;
                ScreenChange("GamePlay");
                dealer.StartNewRound(false);
                lobby.LobbyJoinButton.interactable = true;
            }
        }
        [SerializeField]
        internal string currentScreen = "";
        internal void ScreenChange(string screenName)
        {
            UpdateUserInfo();
            if (screenName == "MainManuPanel")
            {
                //BlackJackGoogleAdmobManage.Instance.BannerLoadAd();
            }
            else
            {
                //BlackJackGoogleAdmobManage.Instance.DistoryBanner();
            }
            foreach (var screen in gameScreens)
            {
                if (screenName == screen.name)
                    screen.SetActive(true);
                else
                    screen.SetActive(false);
            }
            currentScreen = screenName;
        }

        [Serializable]
        public struct BlackJackLobby
        {
            public string lobbyName;
            public float minAmount , maxAmount ;
            public Button LobbyJoinButton;
            public Transform lobbyObject;
            public int lobbyIndex;
        }

        #region Balance Format

        internal String SetBalanceFormat(float Amount)
        {
            if (Amount < 1000)
            {
                return Amount.ToString();
            }
            else if (Amount < 100000)
            {
                if (Amount % 1000 == 0)
                {
                    return (Amount / 1000).ToString() + "K";
                }
                else
                {
                    return (Amount / 1000).ToString("F1") + "K";
                }
            }
            else if (Amount < 10000000)
            {
                if (Amount % 100000 == 0)
                {
                    return (Amount / 100000).ToString() + "M";
                }
                else
                {
                    return (Amount / 100000).ToString("F1") + "M";
                }
            }
            else if (Amount < 1000000000)
            {
                if (Amount % 10000000 == 0)
                {
                    return (Amount / 10000000).ToString() + "B";
                }
                else
                {
                    return (Amount / 10000000).ToString("F1") + "B";
                }
            }
            else if (Amount < 100000000000)
            {
                if (Amount % 1000000000 == 0)
                {
                    return (Amount / 1000000000).ToString() + "T";
                }
                else
                {
                    return (Amount / 1000000000).ToString("F1") + "T";
                }
            }

            return Amount.ToString();
        }

        internal float SetRoundFormat(float Amount)
        {
            if (Amount < 1000)
            {
                return Amount;
            }
            else if (Amount < 100000)
            {
                return ((int)(Amount / 100)) * 100;
            }
            else if (Amount < 10000000)
            {
                return ((int)(Amount / 10000)) * 10000;
            }
            else if (Amount < 1000000000)
            {
                return ((int)(Amount / 1000000)) * 1000000;
            }
            else if (Amount < 100000000000)
            {
                return ((int)(Amount / 100000000)) * 100000000;
            }
            return Amount;
        }
        #endregion

        #region User Info

        internal void UpdateUserInfo()
        {
            userNameTxt.text = BlackJackDataManager.userName; 
            craditPointTxt.text = SetBalanceFormat(BlackJackDataManager.creditPoint);
            profileImage.sprite = profileSprites[BlackJackDataManager.ProfileImage];
            betButtons.player.SetPlayerInfo();
        }

        #endregion

        public void SettingButtonClick(bool isLobby)
        {
            BlackJackSettingManager.instance.OpenSettingPanel(isLobby);
        }

        public void RateUs() => Application.OpenURL("https://play.google.com/store/apps/developer?id=Finix+Games+Studio");
        
    }
}
 
