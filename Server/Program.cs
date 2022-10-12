using Common;
using Newtonsoft.Json.Linq;
using Server.Properties;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    /*
            Console.WriteLine("Min. 5 players. Fascists = ( num_players / 2 - 1, Hitler included )");
            Console.WriteLine("Example: 5 players: 5 / 2 - 1 = 1.5 ~ 2 (fascists)");
            Console.WriteLine("         8 players: 8 / 2 - 1 = 3 (fascists)\n\n");

    */

    /*				COLORS
 *				
 *				1 - White
 *				2 - Blue
 *				3 - Red
 *				4 - Green
 *				5 - Yellow
 *				6 - DarkYellow
 *				7 - DarkRed
 *				8 - DarkGrey
 *				9 - DarkCyan
 *				
 *			
 * */
    internal class Program
    {

        static SimpleTcpServer server;
        static Game gameMaster = new Game();

        static void broadcastMessage(string data)
        {
            try
            {
                server.BroadcastLine(data);
            }
            catch(Exception e)
            {
                print("@7An exception was thrown while broadcasting data!\nMessage: " + e.Message + "\n");
            }
        }

        public static int getConnectedPlayers()
        {
            int count = 0;
            foreach (Game.Player p in gameMaster.players)
            {
                if (p.tcpIdentifier != null) count++;
            }
            return count;
        }

        public static void finishGame()
        {
            gameMaster.gStarted = false;
        }

        static void sendPlayersData()
        {
            gameMaster.game_started_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string message = string.Empty;
            switch(gameMaster.current_action)
            {
                case Game.Actions.CHOOSING_CHANCELLOR:
                    {
                        foreach (Game.Player p in gameMaster.players)
                        {
                            if(p.isPresident)
                            {
                                message =
                                    "[!!!]" + gameMaster.players.Count + "[!!!]" + gameMaster.game_started_time + "[!!!]" + (gameMaster.lastChancellor != null ? gameMaster.lastChancellor.pName : "X") + "[!!!]"
                                    + (gameMaster.lastPresident != null ? gameMaster.lastPresident.pName : "X") + "[!!!]" + (ushort)gameMaster.current_action + "[!!!]" + p.pName +"[!!!]";
                                break;
                            }
                        }
                        break;
                    }
                case Game.Actions.VOTE_CHANCELLOR:
                    {
                        message =
                            "[!!!]" + gameMaster.players.Count + "[!!!]" + gameMaster.game_started_time + "[!!!]" + (gameMaster.lastChancellor != null ? gameMaster.lastChancellor.pName : "X") + "[!!!]"
                            + (gameMaster.lastPresident != null ? gameMaster.lastPresident.pName : "X") + "[!!!]" + (ushort)gameMaster.current_action + "[!!!]" + gameMaster.voteChancellor.pName + "[!!!]";
                        break;
                    }
            }
            foreach (Game.Player p in gameMaster.players)
            {
                string hlp = p.pName + "[&]" + (p.isLiberal ? "true" : "false") + "[&]" + (p.isFascist ? "true" : "false") + "[&]" + (p.isHitler ? "true" : "false") + "[&]" +
                    (p.isChancellor ? "true" : "false") + "[&]" + (p.isPresident ? "true" : "false") + "[&]" + (p.isDead ? "true" : "false") + "[&]" + (p.votedYes ? "true" : "false") + "[&]" +
                    (p.votedNo ? "true" : "false") + "[&]";
                message += hlp;
            }
            message += "[!!!]";
            broadcastMessage(message);
        }

        static void Main(string[] args)
        {
            Console.Title = "SEH Server";

            Console.WriteLine(" ");
            Console.WriteLine(" ");
            print("@7****            SECRET HITLER               ****\n");
            print("@3                    by F0X                      \n");
            print("@6                  v0.1 (Alpha)                       \n\n");
            print("@8              SEH = Secret Hitler                 \n\n");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit); // redirecting the ProcessExit point to our method

            server = new SimpleTcpServer();
            server.Delimiter = 0x13;
            server.StringEncoder = Encoding.UTF8;
            server.DelimiterDataReceived += server_dataReceived; // this method is used by the server to receive data from clients
            server.ClientDisconnected += server_clientDisconnected; // this method is called when a client disconnects. ( client.Disconnect(); )


            // CONFIG.JSON
            if (!File.Exists("config.json"))
            {
                File.WriteAllBytes("config.json", Resources.config);
                print("@5No config file found. Creating one...\n");
            }
            print("@5Parsing config file...\n");
            JObject data = JObject.Parse(File.ReadAllText("config.json")); // parsing config
            if (data["IP"].ToString() != null) gameMaster.gConfig.IP = data["IP"].ToString();
            else gameMaster.gConfig.IP = "AUTO"; // default setting
            print("@8IP: \"" + gameMaster.gConfig.IP + "\"\n");

            if (data["Port"].ToString() != null) gameMaster.gConfig.PORT = int.Parse(data["Port"].ToString());
            else gameMaster.gConfig.PORT = 6666; // default setting
            print("@8Port: \"" + gameMaster.gConfig.PORT + "\"\n");

            if (data["Password"].ToString() != null) gameMaster.gConfig.PASSWORD = data["Password"].ToString();
            else gameMaster.gConfig.PASSWORD = "0"; // default setting
            print("@8Password: \"" + gameMaster.gConfig.PASSWORD + "\"\n");

            if (data["Gamemode"].ToString() != null) gameMaster.gConfig.GAMEMODE = data["Gamemode"].ToString();
            else gameMaster.gConfig.GAMEMODE = "CLASIC"; // default setting
            print("@8Gamemode: \"" + gameMaster.gConfig.GAMEMODE + "\"\n");

            if (data["Admins"].ToString() != null) gameMaster.gConfig.ADMINS = JArray.Parse(data["Admins"].ToString());
            else gameMaster.gConfig.ADMINS = new JArray(); // no admins
            print("@8Admins: \"" + gameMaster.gConfig.ADMINS + "\"\n");

            if (data["MaxPlayers"].ToString() != null) gameMaster.gConfig.MAX_PLAYERS = ushort.Parse(data["MaxPlayers"].ToString());
            if (gameMaster.gConfig.MAX_PLAYERS < gameMaster.gConfig.MIN_PLAYERS || gameMaster.gConfig.MAX_PLAYERS > 10) gameMaster.gConfig.MAX_PLAYERS = 10; // correcting the min/max players number
            print("@8MaxPlayers: \"" + gameMaster.gConfig.MAX_PLAYERS + "\"\n");

            if (data["DebugMode"].ToString() != null) gameMaster.gConfig.DEBUG_MODE = bool.Parse(data["DebugMode"].ToString());
            else gameMaster.gConfig.DEBUG_MODE = true;
            print("@8DebugMode: \"" + (gameMaster.gConfig.DEBUG_MODE ? "true" : "false") + "\"\n");

            if (data["MOTD"].ToString() != null) gameMaster.gConfig.MOTD = data["MOTD"].ToString();
            else gameMaster.gConfig.MOTD = "Welcome to my SEH Server!"; // default setting
            print("@8MOTD: \"" + gameMaster.gConfig.MOTD + "\"\n\n\n");
            print("@7****                                        ****\n\n");

            // SERVER SETUP
            if (gameMaster.gConfig.IP.Equals("AUTO"))
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().StartsWith("192.") && ip.ToString().Contains(".0"))
                    {
                        try
                        {
                            server.Start(ip, gameMaster.gConfig.PORT);
                            print(string.Format("@9SEH Server started on IP {0} (port {1})\n", ip.ToString(), gameMaster.gConfig.PORT));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
                }
            }
            else
            {
                try
                {
                    server.Start(IPAddress.Parse(gameMaster.gConfig.IP), gameMaster.gConfig.PORT);
                    print(string.Format("@9SEH Server started on IP {0} (port {1})\n", gameMaster.gConfig.IP, gameMaster.gConfig.PORT));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (!server.IsStarted) return; // exit server if start was unsuccessful
            while(true) // loop that handles the game start countdown
            {
                if (!gameMaster.gStarted)
                {
                    Thread.Sleep(2000);
                    broadcastMessage("[#]");
                    if (gameMaster.gStarting > 0)
                    {
                        if (gameMaster.players.Count < gameMaster.gConfig.MIN_PLAYERS)
                        {
                            print("@8Game starting canceled because there are not enough players...\n");
                            broadcastMessage("[!!]Game starting canceled because there are not enough players...");
                            foreach (Game.Player p in gameMaster.players) p.isReady = false;
                            gameMaster.gStarting = 0;
                        }
                        if (gameMaster.gStarting > 0) // game countdown until it begins
                        {
                            gameMaster.gStarting -= 2;
                            if (gameMaster.gStarting == 0)
                            {
                                gameMaster.gStarted = true;
                                gameMaster.points_count_time = 0;

                                foreach (Game.Player p in gameMaster.players)
                                {
                                    p.isHitler = false;
                                    p.isChancellor = false;
                                    p.isLiberal = false;
                                    p.isFascist = false;
                                    p.isPresident = false;
                                    p.isDead = false;
                                    p.isReady = false;
                                    p.votedYes = false;
                                    p.votedNo = false;
                                }

                                ushort total_fascists = (ushort)Math.Ceiling((double)gameMaster.players.Count / 2.0 - 2.0);
                                int total_players = gameMaster.players.Count;

                                // set hitler
                                Random rand = new Random();
                                int index = rand.Next(0, gameMaster.players.Count);
                                gameMaster.players[index].isHitler = true;
                                gameMaster.players[index].isFascist = true;
                                total_players--;
                                if (gameMaster.gConfig.DEBUG_MODE) print("@8[DEBUG] Hitler index=" + index + "\n");

                                index = rand.Next(0, gameMaster.players.Count);
                                gameMaster.players[index].isPresident = true;
                                gameMaster.pTurn = gameMaster.players[index];
                                gameMaster.current_action = Game.Actions.CHOOSING_CHANCELLOR;
                                if (gameMaster.gConfig.DEBUG_MODE) print("@8[DEBUG] President index=" + index + "\n");
                                if (total_players > 0)
                                {
                                    // set fascists
                                    for (int i = 0; i < total_fascists; i++)
                                    {
                                        do
                                        {
                                            index = rand.Next(0, gameMaster.players.Count);
                                            if (gameMaster.gConfig.DEBUG_MODE) print("@8[DEBUG] Fascist index i=" + i + ", index=" + index + "\n");
                                        }
                                        while (gameMaster.players[index].isHitler || gameMaster.players[index].isLiberal || gameMaster.players[index].isFascist);
                                        gameMaster.players[index].isFascist = true;
                                        total_players--;
                                        if (total_players == 0) break;
                                    }
                                }

                                gameMaster.policies.Clear();
                                for (int i = 0; i < 17; i++) // preparing the policies
                                {
                                    Game.Policy policy = new Game.Policy();
                                    policy.isFascist = false;
                                    policy.isPlaced = false;
                                    policy.isDiscarded = false;
                                    gameMaster.policies.Add(policy);
                                }
                                for (int i = 0; i < 11; i++) // set fascist policies
                                {
                                    do
                                    {
                                        index = rand.Next(0, gameMaster.policies.Count);
                                        if (gameMaster.gConfig.DEBUG_MODE) print("@8[DEBUG] Fascist Policy index i=" + i + ", index=" + index + "\n");
                                    }
                                    while (gameMaster.policies[index].isFascist);
                                    gameMaster.policies[index].isFascist = true;
                                }
                                if (gameMaster.gConfig.DEBUG_MODE)
                                    print("@8[DEBUG] ");
                                foreach (Game.Player p in gameMaster.players)
                                {
                                    if (!p.isHitler && !p.isFascist) // set liberals
                                        p.isLiberal = true;
                                    if (gameMaster.gConfig.DEBUG_MODE) print(String.Format("{0}{1} ", (p.isFascist && !p.isHitler ? "@3" : (p.isHitler ? "@7" : "@9")), p.pName));
                                }
                                if (gameMaster.gConfig.DEBUG_MODE) print("\n");
                                if (gameMaster.gConfig.DEBUG_MODE)
                                {
                                    print("@8[DEBUG] ");
                                    foreach (Game.Policy gp in gameMaster.policies)
                                    {
                                        print(String.Format("{0}Policy ", (gp.isFascist ? "@3" : "@9")));
                                    }
                                    print("\n");
                                }
                                sendPlayersData();

                                print("@6Everything is ready! Preparing the players to join the game... (@1" + gameMaster.players.Count + " @6players)\n");
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(5000);
                    broadcastMessage("[#]");
                    gameMaster.points_count_time += 5;
                    if (gameMaster.points_count_time == 30)
                    {
                        string message = "[!!!!]" + gameMaster.players.Count + "[!!!!]";
                        foreach(Game.Player p in gameMaster.players)
                        {
                            if (!p.isDead)
                            {
                                if (p.isFascist)
                                    p.pScore += 15;
                                else p.pScore += 10;
                            }
                            string hlp = p.pName + "[&]" + p.pScore + "[&]";
                            message += hlp;
                        }
                        broadcastMessage(message);
                        if(gameMaster.gConfig.DEBUG_MODE) print("@8[DEBUG] Giving fascists 15 points and liberals 10 points.\n");
                        gameMaster.points_count_time = 0;
                    }

                }
            }
        }

        private static void server_clientDisconnected(object sender, TcpClient e)
        {
            foreach(Game.Player p in gameMaster.players.ToList())
            {
                if(p.tcpIdentifier == e)
                {
                    p.tcpIdentifier = null;
                    print(String.Format("@6Player '{0}{1}@6' left the server. (@1{2}@6/{3})\n", (p.isAdmin ? "@3" : "@1"), p.pName, getConnectedPlayers(), gameMaster.gConfig.MAX_PLAYERS));
                    broadcastMessage("[!]Player '" + p.pName + "' left the server. (" + getConnectedPlayers() + "/" + gameMaster.gConfig.MAX_PLAYERS + ")");
                    if(gameMaster.gStarting > 0)
                    {
                        gameMaster.gStarting = 0;
                        print("@8Game starting canceled because a player disconnected.\n");
                        broadcastMessage("[!!]Game starting canceled because a player disconnected.");
                        foreach (Game.Player pp in gameMaster.players) pp.isReady = false;
                    }
                    
                    if (gameMaster.gStarted)
                    {
                        p.isDead = true;
                        if (gameMaster.players.Count == 0 || p.isHitler)
                        {
                            if (gameMaster.gConfig.DEBUG_MODE) print("@8[DEBUG] Finishing game...\n");
                            finishGame();
                        }
                        if(gameMaster.gStarted)
                            sendPlayersData();
                    }
                    else gameMaster.players.Remove(p);
                    break;
                }
            }
        }

        private static void server_dataReceived(object sender, Message e)
        {
            if (e.MessageString.StartsWith("[*]")) // player connects message from client
            {
                string[] split = e.MessageString.Split(new[] { "[*]" }, StringSplitOptions.None);
                if (!gameMaster.gConfig.PASSWORD.Equals("0") && !split[2].Equals(gameMaster.gConfig.PASSWORD))
                {
                    broadcastMessage("[X]" + e.TcpClient.Client.RemoteEndPoint.ToString() + "[X]Wrong server password.");
                    return;
                }
                if (gameMaster.gStarted)
                {
                    broadcastMessage("[X]" + e.TcpClient.Client.RemoteEndPoint.ToString() + "[X]This server's game already started.");
                    return;
                }
                if (gameMaster.players.Count == gameMaster.gConfig.MAX_PLAYERS)
                {
                    broadcastMessage("[X]" + e.TcpClient.Client.RemoteEndPoint.ToString() + String.Format("[X]This server is already full. ({0}/{1})", 
                        gameMaster.gConfig.MAX_PLAYERS, gameMaster.gConfig.MAX_PLAYERS));
                    return;
                }
                if (split[1].Length < 3 || split[1].Length > 8)
                {
                    broadcastMessage("[X]" + e.TcpClient.Client.RemoteEndPoint.ToString() + "[X]Invalid name.");
                    return;
                }
                foreach (Game.Player pp in gameMaster.players)
                {
                    if (pp.pName.Equals(split[1]))
                    {
                        broadcastMessage("[X]" + e.TcpClient.Client.RemoteEndPoint.ToString() + "[X]There is already a player with the same name.");
                        return;
                    }
                }

                Game.Player p = new Game.Player();
                p.pName = split[1];
                p.pScore = 0;
                p.tcpIdentifier = e.TcpClient;
                p.isLiberal = false;
                p.isFascist = false;
                p.isHitler = false;
                p.isChancellor = false;
                p.isPresident = false;
                p.isDead = false;
                p.isReady = false;
                p.votedNo = false;
                p.votedYes = false;
                foreach (JToken jt in gameMaster.gConfig.ADMINS)
                {
                    if (p.pName.Equals(jt.ToString()))
                    {
                        p.isAdmin = true;
                        break;
                    }
                }
                gameMaster.players.Add(p);
                print(String.Format("@6Player '{0}{1}@6' joined the server. (@1{2}@6/{3})\n", (p.isAdmin ? "@3" : "@1"), p.pName, gameMaster.players.Count, gameMaster.gConfig.MAX_PLAYERS));
                broadcastMessage("[!]Player '" + p.pName + "' joined the server. (" + gameMaster.players.Count + "/" + gameMaster.gConfig.MAX_PLAYERS + ")");
                if (gameMaster.gStarting > 0)
                {
                    gameMaster.gStarting = 0;
                    print("@8Game starting canceled because a player joined the server.\n");
                    broadcastMessage("[!!]Game starting canceled because a player joined the server.");
                    foreach (Game.Player pp in gameMaster.players) pp.isReady = false;
                }
            }
            else if (e.MessageString.StartsWith("[**]")) // player ready message from client
            {
                bool starting = true;
                string namep = string.Empty;
                ushort readyp = 0;
                foreach (Game.Player p in gameMaster.players)
                {
                    if (p.tcpIdentifier == e.TcpClient)
                    {
                        p.isReady = true;
                        namep = p.pName;
                    }
                    if (!p.isReady) starting = false;
                    else readyp++;
                }
                print(String.Format("@9Player '@1{0}@9' is now ready. (@1{1}@9/@1{2}@9)\n", namep, readyp, gameMaster.players.Count));
                broadcastMessage("[!]Player '" + namep + "' is now ready. (" + readyp + "/" + gameMaster.players.Count + ")");
                if (gameMaster.players.Count < gameMaster.gConfig.MIN_PLAYERS) starting = false;
                if (starting)
                {
                    print(String.Format("@8Every player is ready. Starting the game in a few seconds! (@1{0} @8players)\n", gameMaster.players.Count));
                    broadcastMessage(String.Format("[!]Every player is ready. Starting the game in a few seconds! ({0} players)", gameMaster.players.Count));
                    gameMaster.gStarting = 6;
                }
            }
            else if (e.MessageString.StartsWith("[***]")) // player chat message from client
            {
                string[] split = e.MessageString.Split(new[] { "[***]" }, StringSplitOptions.None);
                foreach (Game.Player p in gameMaster.players)
                {
                    if (p.tcpIdentifier == e.TcpClient)
                    {
                        print(String.Format("(CHAT) {0}{1}@1: {2}\n", (p.isAdmin ? "@3" : "@1"), p.pName, split[1]));
                        broadcastMessage(String.Format("[!]{0}: {1}", p.pName, split[1]));
                        break;
                    }
                }
            }
            else if (e.MessageString.StartsWith("[****]"))
            {
                string[] split = e.MessageString.Split(new[] { "[****]" }, StringSplitOptions.None);
                Game.Player target = null;
                if (split[1].Length >= 3)
                {
                    foreach (Game.Player p in gameMaster.players)
                    {
                        if (p.pName.Equals(split[1]))
                        {
                            if (p.tcpIdentifier == e.TcpClient)
                                return;
                            target = p;
                        }
                    }
                }
                switch (gameMaster.current_action) {
                    case Game.Actions.CHOOSING_CHANCELLOR:
                        {
                            foreach (Game.Player p in gameMaster.players)
                            {
                                if(p.tcpIdentifier == e.TcpClient && !p.isPresident) 
                                    return;
                                if(gameMaster.lastChancellor == target || gameMaster.lastPresident == target || p.isDead)
                                    return;
                                if(p.tcpIdentifier == e.TcpClient)
                                {
                                    p.votedYes = true;
                                }
                            }
                            gameMaster.voteChancellor = target;
                            gameMaster.current_action = Game.Actions.VOTE_CHANCELLOR;
                            sendPlayersData();
                            break;
                        }
                    }
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            broadcastMessage("[X]");
            server.Stop();
        }

        static void print(string message)
        {
            using (StreamWriter sw = File.AppendText(@"server_log.txt"))
            {
                sw.Write(message);
            }
            bool amp = false;
            for (int i = 0; i < message.Length; i++)
            {
                bool hold = false;
                if (!amp)
                {
                    if (message[i].Equals('@'))
                    {
                        amp = true;
                        hold = true;
                    }
                }
                else
                {
                    switch (message[i] - 48)
                    {
                        case 1:
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            }
                        case 2:
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            }
                        case 3:
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            }
                        case 4:
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            }
                        case 5:
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            }
                        case 6:
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                break;
                            }
                        case 7:
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                            }
                        case 8:
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                            }
                        case 9:
                            {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                break;
                            }
                    }
                    amp = false;
                    hold = true;
                }
                if (!hold)
                {
                    Console.Write(message[i]);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
