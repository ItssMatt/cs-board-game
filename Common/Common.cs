using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Common
{
    public class Game
    {
        public enum Actions
        {
            NONE = 0,
            CHOOSING_CHANCELLOR = 1,
            VOTE_CHANCELLOR = 2,
            PRESIDENT_DISCARDS_POLICY = 3,
            CHANCELLOR_PICKS_POLICY = 4,
            PRESIDENT_INVESTIGATES_A_PLAYER = 5,
            PRESIDENT_PICKS_NEXT_PRESIDENT = 6,
            PRESIDENT_SHOOTS_PLAYER = 7
        }

        public string getActionName(Actions action)
        {
            switch (action)
            {
                case Actions.NONE: return "INVALID_ACTION"; // NONE
                case Actions.CHOOSING_CHANCELLOR: return "choosing a chancellor"; // PRESIDENT
                case Actions.VOTE_CHANCELLOR: return "everyone votes chancellor"; // EVERYONE
                case Actions.PRESIDENT_DISCARDS_POLICY: return "discards 1 policy"; // PRESIDENT
                case Actions.CHANCELLOR_PICKS_POLICY: return "chooses 1 policy"; // CHANCELLOR
                case Actions.PRESIDENT_INVESTIGATES_A_PLAYER: return "investigates a player"; // PRESIDENT
                case Actions.PRESIDENT_PICKS_NEXT_PRESIDENT: return "chooses the next president"; // PRESIDENT
                case Actions.PRESIDENT_SHOOTS_PLAYER: return "shoots a player"; // PRESIDENT
            }
            return string.Empty;
        }

        public class Player
        {
            public TcpClient tcpIdentifier = null;
            public string pName = string.Empty;
            public int pScore = 0;

            public bool isLiberal = false;
            public bool isFascist = false;
            public bool isHitler = false;
            public bool isChancellor = false;
            public bool isPresident = false;
            public bool isDead = false;
            public bool isReady = false;
            public bool isAdmin = false;
            public bool votedYes = false;
            public bool votedNo = false;
        }

        public bool gStarted = false;
        public ushort gStarting = 0;
        public Player pTurn = null;
        public Player lastChancellor = null;
        public Player lastPresident = null;
        public Player voteChancellor = null;
        public Actions current_action = Actions.NONE;
        public long game_started_time = 0;
        public ushort points_count_time = 0;

        public class Config
        {
            public string IP = "AUTO";
            public int PORT = 6666;
            public string PASSWORD = "0";
            public string GAMEMODE = "CLASIC";
            public JArray ADMINS;
            public ushort MAX_PLAYERS = 10;
            public ushort MIN_PLAYERS = 2;
            public bool DEBUG_MODE = true;
            public string MOTD = "Welcome to my SEH Server!";
        };
        public Config gConfig = new Config();

        public class Policy
        {
            public bool isFascist = false;
            public bool isPlaced = false;
            public bool isDiscarded = false;
        }

        public List<Policy> policies = new List<Policy>();
        public List<Player> players = new List<Player>();
    }
}
