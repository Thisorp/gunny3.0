namespace Game.Logic
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic.Actions;
    using Game.Logic.Phy.Maps;
    using Game.Logic.Phy.Object;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Configuration;
    using System.Reflection;

    public class PVPGame : BaseGame
    {
        private int BeginPlayerCount;
        private DateTime beginTime;
        private static readonly int Gift_Rate = int.Parse(ConfigurationSettings.AppSettings["Gift_Rate"]);
        private static readonly int Gold_Rate = int.Parse(ConfigurationSettings.AppSettings["Gold_Rate"]);
        private static readonly int GP_Rate = int.Parse(ConfigurationSettings.AppSettings["GP_Rate"]);
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private float m_blueAvgLevel;
        private List<Player> m_blueTeam;
        private float m_redAvgLevel;
        private List<Player> m_redTeam;
        private string teamAStr;
        private string teamBStr;
        private static readonly int Xu_Rate = int.Parse(ConfigurationSettings.AppSettings["Xu_Rate"]);

        public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType)
            : base(id, roomId, map, roomType, gameType, timeType)
        {
            this.m_redTeam = new List<Player>();
            this.m_blueTeam = new List<Player>();
            StringBuilder sbTeampA = new StringBuilder();
            this.m_redAvgLevel = 0f;
            foreach (IGamePlayer player in red)
            {
                Player fp = new Player(player, base.PhysicalId++, this, 1);
                sbTeampA.Append(player.PlayerCharacter.ID).Append(",");
                fp.Reset();
                fp.Direction = (base.m_random.Next(0, 1) == 0) ? 1 : -1;
                base.AddPlayer(player, fp);
                this.m_redTeam.Add(fp);
                this.m_redAvgLevel += player.PlayerCharacter.Grade;
            }
            this.m_redAvgLevel /= (float)this.m_redTeam.Count;
            this.teamAStr = sbTeampA.ToString();
            StringBuilder sbTeampB = new StringBuilder();
            this.m_blueAvgLevel = 0f;
            foreach (IGamePlayer player2 in blue)
            {
                Player fp2 = new Player(player2, base.PhysicalId++, this, 2);
                sbTeampB.Append(player2.PlayerCharacter.ID).Append(",");
                fp2.Reset();
                fp2.Direction = (base.m_random.Next(0, 1) == 0) ? 1 : -1;
                base.AddPlayer(player2, fp2);
                this.m_blueTeam.Add(fp2);
                this.m_blueAvgLevel += player2.PlayerCharacter.Grade;
            }
            this.m_blueAvgLevel /= (float)blue.Count;
            this.teamBStr = sbTeampB.ToString();
            this.BeginPlayerCount = this.m_redTeam.Count + this.m_blueTeam.Count;
            this.beginTime = DateTime.Now;
        }

        private int CalculateGuildMatchResult(List<Player> players, int winTeam)
        {
            if (base.RoomType == eRoomType.Match)
            {
                StringBuilder winStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
                StringBuilder loseStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
                IGamePlayer winPlayer = null;
                IGamePlayer losePlayer = null;
                int count = 0;
                foreach (Player p in players)
                {
                    if (p.Team == winTeam)
                    {
                        winStr.Append(string.Format("[{0}]", p.PlayerDetail.PlayerCharacter.NickName));
                        winPlayer = p.PlayerDetail;
                    }
                    else
                    {
                        loseStr.Append(string.Format("{0}", p.PlayerDetail.PlayerCharacter.NickName));
                        losePlayer = p.PlayerDetail;
                        count++;
                    }
                }
                if (losePlayer != null)
                {
                    winStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1", new object[0]) + losePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2", new object[0]));
                    loseStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3", new object[0]) + winPlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4", new object[0]));
                    int riches = 0;
                    if (base.GameType == eGameType.Guild)
                    {
                        riches = count + (base.TotalHurt / 0x7d0);
                    }
                    winPlayer.ConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, losePlayer.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, base.TotalHurt, players.Count);
                    if (winPlayer.ServerID != losePlayer.ServerID)
                    {
                        losePlayer.ConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, losePlayer.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, base.TotalHurt, players.Count);
                    }
                    if (base.GameType == eGameType.Guild)
                    {
                        winPlayer.SendConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, riches, winStr.ToString());
                    }
                    return riches;
                }
            }
            return 0;
        }

        public bool CanGameOver()
        {
            bool red = true;
            bool blue = true;
            foreach (Player p in this.m_redTeam)
            {
                if (p.IsLiving)
                {
                    red = false;
                    break;
                }
            }
            foreach (Player p2 in this.m_blueTeam)
            {
                if (p2.IsLiving)
                {
                    blue = false;
                    break;
                }
            }
            return (red || blue);
        }

        public override void CheckState(int delay)
        {
            base.AddAction(new CheckPVPGameStateAction(delay));
        }

        public void GameOver()
        {
            if (base.GameState == eGameState.Playing)
            {
                base.m_gameState = eGameState.GameOver;
                base.ClearWaitTimer();
                base.CurrentTurnTotalDamage = 0;
                List<Player> players = base.GetAllFightPlayers();
                int winTeam = -1;
                foreach (Player p in players)
                {
                    if (p.IsLiving)
                    {
                        winTeam = p.Team;
                        break;
                    }
                }
                if ((winTeam == -1) && (this.CurrentPlayer != null))
                {
                    winTeam = this.CurrentPlayer.Team;
                }
                int riches = this.CalculateGuildMatchResult(players, winTeam);
                if ((base.RoomType == eRoomType.Match) && (base.GameType == eGameType.Guild))
                {
                    int winbaseoffer = 10;
                    int losebaseoffer = -10;
                    winbaseoffer += players.Count / 2;
                    losebaseoffer += (int)Math.Round((double)((players.Count / 2) * 0.5));
                }
                int canBlueTakeOut = 0;
                int canRedTakeOut = 0;
                foreach (Player p2 in players)
                {
                    if (p2.TotalHurt > 0)
                    {
                        if (p2.Team == 1)
                        {
                            canRedTakeOut = 1;
                        }
                        else
                        {
                            canBlueTakeOut = 1;
                        }
                    }
                }
                GSPacketIn pkg = new GSPacketIn(0x5b);
                pkg.WriteByte(100);
                pkg.WriteInt(base.PlayerCount);
                foreach (Player p3 in players)
                {
                    float againstTeamLevel = (p3.Team == 1) ? this.m_blueAvgLevel : this.m_redAvgLevel;
                    float againstTeamCount = (p3.Team == 1) ? ((float)this.m_blueTeam.Count) : ((float)this.m_redTeam.Count);
                    float disLevel = Math.Abs((float)(againstTeamLevel - p3.PlayerDetail.PlayerCharacter.Grade));
                    float winPlus = (p3.Team == winTeam) ? ((float)2) : ((float)0);
                    int gp = 0;
                    int totalShoot = (p3.TotalShootCount == 0) ? 1 : p3.TotalShootCount;
                    if ((base.m_roomType == eRoomType.Match) || (disLevel < 5f))
                    {
                        gp = (int)Math.Ceiling((double)(((((winPlus + (p3.TotalHurt * 0.001)) + (p3.TotalKill * 0.5)) + ((p3.TotalHitTargetCount / totalShoot) * 2)) * againstTeamLevel) * (0.9 + ((againstTeamCount - 1f) * 0.3))));
                    }
                    gp = (gp == 0) ? 1 : gp;
                    int xutang = (int)Math.Round((double)(Xu_Rate * gp));
                    if (xutang > 300)
                    {
                        xutang = 300;
                    }
                    if (base.RoomType != eRoomType.Freedom)
                    {
                        p3.PlayerDetail.AddMoney(xutang);
                        p3.PlayerDetail.AddGold((int)Math.Round((double)(Gold_Rate * gp)));
                        p3.PlayerDetail.AddGiftToken((int)Math.Round((double)(Gift_Rate * gp)));
                    }
                    p3.GainGP = p3.PlayerDetail.AddGP(GP_Rate * gp);
                    string msg = string.Format("Bạn nhận được {0} Xu, {1} V\x00e0ng , {2} Lệ kim v\x00e0 {3} Kinh nhiệm.", new object[] { xutang, (int)Math.Round((double)(Gold_Rate * gp)), (int)Math.Round((double)(Gift_Rate * gp)), GP_Rate * gp });
                    p3.PlayerDetail.SendMessage(msg);
                    p3.CanTakeOut = (p3.Team == 1) ? canRedTakeOut : canBlueTakeOut;
                    riches += p3.GainOffer;
                    pkg.WriteInt(p3.Id);
                    pkg.WriteBoolean(p3.Team == winTeam);
                    pkg.WriteInt(p3.Grade);
                    pkg.WriteInt(p3.PlayerDetail.PlayerCharacter.GP);
                    pkg.WriteInt(p3.TotalKill);
                    pkg.WriteInt(p3.TotalHurt);
                    pkg.WriteInt(p3.TotalShootCount);
                    pkg.WriteInt(p3.TotalCure);
                    pkg.WriteInt(1);
                    pkg.WriteInt(1);
                    pkg.WriteInt(100);
                    pkg.WriteInt(100);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(1);
                    pkg.WriteInt(p3.GainGP);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                    pkg.WriteInt(p3.GainOffer);
                    pkg.WriteInt(p3.CanTakeOut);
                }
                pkg.WriteInt(riches);
                this.SendToAll(pkg);
                new StringBuilder();
                foreach (Player p4 in players)
                {
                    p4.PlayerDetail.OnGameOver(this, p4.Team == winTeam, p4.GainGP);
                }
                string templateIdsStr = "";
                base.OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, this.beginTime, DateTime.Now, this.BeginPlayerCount, base.Map.Info.ID, this.teamAStr, this.teamBStr, templateIdsStr, winTeam, base.BossWarField);
                base.WaitTime(0x3a98);
                base.OnGameOverred();
            }
        }

        public void NextTurn()
        {
            if (base.GameState == eGameState.Playing)
            {
                base.ClearWaitTimer();
                base.ClearDiedPhysicals();
                base.CheckBox();
                base.m_turnIndex++;
                base.UpdateWind(base.GetNextWind(), false);
                List<Box> newBoxes = base.CreateBox();
                List<Physics> list = base.m_map.GetAllPhysicalSafe();
                foreach (Physics p in list)
                {
                    p.PrepareNewTurn();
                }
                base.m_currentLiving = base.FindNextTurnedLiving();
                this.MinusDelays(base.m_currentLiving.Delay);
                base.m_currentLiving.PrepareSelfTurn();
                if (!base.CurrentLiving.IsFrost)
                {
                    base.m_currentLiving.StartAttacking();
                    base.SendGameNextTurn(base.m_currentLiving, this, newBoxes);
                    if (base.m_currentLiving.IsAttacking)
                    {
                        base.AddAction(new WaitLivingAttackingAction(base.m_currentLiving, base.m_turnIndex, (base.m_timeType + 20) * 0x3e8));
                    }
                }
                base.OnBeginNewTurn();
            }
        }

        public void Prepare()
        {      
            if (GameState == eGameState.Inited)
            {
                SendCreateGame();
                m_gameState = eGameState.Prepared;
                this.CheckState(0);
            }
        }

        public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
        {
            Player player = base.RemovePlayer(gp, IsKick);
            if (((player != null) && player.IsLiving) && (base.GameState != eGameState.Loading))
            {
                gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
                string msg = null;
                string msg2 = null;
                if (base.RoomType == eRoomType.Match)
                {
                    if (base.GameType == eGameType.Guild)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[] { gp.PlayerCharacter.Grade * 12, 15 });
                        gp.RemoveOffer(15);
                        msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[] { gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 15 });
                    }
                    else if (base.GameType == eGameType.Free)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[] { gp.PlayerCharacter.Grade * 12, 5 });
                        gp.RemoveOffer(5);
                        msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[] { gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 5 });
                    }
                }
                else
                {
                    msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", new object[] { gp.PlayerCharacter.Grade * 12 });
                    msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", new object[] { gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12 });
                }
                base.SendMessage(gp, msg, msg2, 3);
                if (base.GetSameTeam())
                {
                    base.CurrentLiving.StopAttacking();
                    this.CheckState(0);
                }
            }
            return player;
        }

        public void StartGame()
        {
            if (base.GameState == eGameState.Loading)
            {
                base.m_gameState = eGameState.Playing;
                base.ClearWaitTimer();
                base.SendSyncLifeTime();
                List<Player> list = base.GetAllFightPlayers();
                MapPoint mapPos = MapMgr.GetMapRandomPos(base.m_map.Info.ID);
                GSPacketIn pkg = new GSPacketIn(0x5b);
                pkg.WriteByte(0x63);
                pkg.WriteInt(list.Count);
                foreach (Player p in list)
                {
                    p.Reset();
                    Point pos = base.GetPlayerPoint(mapPos, p.Team);
                    p.SetXY(pos);
                    base.m_map.AddPhysical(p);
                    p.StartMoving();
                    p.StartGame();
                    pkg.WriteInt(p.Id);
                    pkg.WriteInt(p.X);
                    pkg.WriteInt(p.Y);
                    pkg.WriteInt(p.Direction);
                    pkg.WriteInt(p.Blood);
                    pkg.WriteInt(2);
                    pkg.WriteInt(8);
                    pkg.WriteInt(p.Dander);
                    pkg.WriteInt(p.PlayerDetail.EquipEffect.Count);
                    foreach (int templateID in p.PlayerDetail.EquipEffect)
                    {
                        ItemTemplateInfo item = ItemMgr.FindItemTemplate(templateID);
                        if (item.Property3 < 0x1b)
                        {
                            pkg.WriteInt(item.Property3);
                            pkg.WriteInt(item.Property4);
                        }
                        else
                        {
                            pkg.WriteInt(0);
                            pkg.WriteInt(0);
                        }
                    }
                }
                this.SendToAll(pkg);
                base.WaitTime(list.Count * 0x3e8);
                base.OnGameStarted();
            }
        }

        public void StartLoading()
        {
            if (base.GameState == eGameState.Prepared)
            {
                base.ClearWaitTimer();
                base.SendStartLoading(60);
                base.AddAction(new WaitPlayerLoadingAction(this, 0xee48));
                base.m_gameState = eGameState.Loading;
            }
        }

        public override void Stop()
        {
            if (base.GameState == eGameState.GameOver)
            {
                base.m_gameState = eGameState.Stopped;
                List<Player> players = base.GetAllFightPlayers();
                foreach (Player p in players)
                {
                    if ((p.IsActive && !p.FinishTakeCard) && (p.CanTakeOut > 0))
                    {
                        this.TakeCard(p);
                    }
                }
                lock (base.m_players)
                {
                    base.m_players.Clear();
                }
                base.Stop();
            }
        }

        public override bool TakeCard(Player player)
        {
            int index = 0;
            for (int i = 0; i < base.Cards.Length; i++)
            {
                if (base.Cards[i] == 0)
                {
                    index = i;
                    break;
                }
            }
            return this.TakeCard(player, index);
        }

        public override bool TakeCard(Player player, int index)
        {
            if (((!player.IsActive || (index < 0)) || ((index > base.Cards.Length) || player.FinishTakeCard)) || (base.Cards[index] > 0))
            {
                return false;
            }
            int gold = 0;
            int money = 0;
            int giftToken = 0;
            int templateID = 0;
            List<SqlDataProvider.Data.ItemInfo> infos = null;
            if (DropInventory.CardDrop(base.RoomType, ref infos))
            {
                if (infos != null)
                {
                    foreach (SqlDataProvider.Data.ItemInfo info in infos)
                    {
                        SqlDataProvider.Data.ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
                        if (info != null)
                        {
                            templateID = info.TemplateID;
                            player.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count);
                        }
                    }
                }
                else
                {
                    gold = 100;
                }
            }
            if ((base.RoomType == eRoomType.Treasure) || (base.RoomType == eRoomType.Boss))
            {
                player.CanTakeOut--;
                if (player.CanTakeOut == 0)
                {
                    player.FinishTakeCard = true;
                }
            }
            else
            {
                player.FinishTakeCard = true;
            }
            base.Cards[index] = 1;
            base.SendGamePlayerTakeCard(player, index, templateID, gold, money, giftToken);
            return true;
        }

        public Player CurrentPlayer
        {
            get
            {
                return (base.m_currentLiving as Player);
            }
        }
    }
}

