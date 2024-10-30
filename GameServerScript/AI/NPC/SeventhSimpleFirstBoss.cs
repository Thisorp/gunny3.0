using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using System.Drawing;
using Game.Logic.Actions;
using Bussiness;


namespace GameServerScript.AI.NPC
{
    public class SeventhSimpleFirstBoss : ABrain
    {
        private int m_attackTurn = 0;
		
		private PhysicalObj moive;

        #region NPC 说话内容
        private static string[] AllAttackChat = new string[] {
            LanguageMgr.GetTranslation("Ddtank super là số 1"),
        };

        private static string[] ShootChat = new string[]{
            LanguageMgr.GetTranslation("Anh em tiến lên !"),
        };

        private static string[] KillPlayerChat = new string[]{
            LanguageMgr.GetTranslation("Anh em tiến lên !")
        };

        private static string[] CallChat = new string[]{
            LanguageMgr.GetTranslation("Ai giết được chúng sẻ được ban thưởng !"),

        };

        private static string[] JumpChat = new string[]{
             LanguageMgr.GetTranslation("Ai giết được chúng sẻ được ban thưởng !"),

        };

        private static string[] KillAttackChat = new string[]{
             LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg13"),

              LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg14")
        };

        private static string[] ShootedChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg15"),

            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg16")

        };

        private static string[] DiedChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg17")
        };

        #endregion

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

            Body.CurrentDamagePlus = 1;
            Body.CurrentShootMinus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            Body.Direction = Game.FindlivingbyDir(Body);
            bool result = false;
            int maxdis = 0;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > 1344)
                {
                    int dis = (int)Body.Distance(player.X, player.Y);
                    if (dis > maxdis)
                    {
                        maxdis = dis;
                    }
                    result = true;
                }
            }

            if (result)
            {
                KillAttack(1344, Game.Map.Info.ForegroundWidth + 1);

                return;
            }
			if (m_attackTurn == 0)
            {
                Summon(0);
                m_attackTurn++;
            }           
			else if (m_attackTurn == 1)
            {
                Shield();					
                m_attackTurn++;
            }
			else if (m_attackTurn == 2)
            {
                Summon(1);			
                m_attackTurn++;
            }
            else if (m_attackTurn == 3)
            {
                Shield();
                m_attackTurn++;
            }
            else if (m_attackTurn == 4)
            {
                Summon(2);
                m_attackTurn++;
            }
            else
            {
                Shield();
                m_attackTurn = 0;
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        private void KillAttack(int fx, int tx)
        {
            int index = Game.Random.Next(0, KillAttackChat.Length);
            Body.Say(KillAttackChat[index], 1, 1000);
            Body.CurrentDamagePlus = 10;
            //Body.PlayMovie("speak", 3000, 0);
            Body.PlayMovie("beatB", 3000, 0);
            Body.RangeAttacking(fx, tx, "cry", 5000, null);
        }

		public void Summon(int type)
        {
            Body.PlayMovie("Ato", 100, 0);
            ((SimpleBoss)Body).SetRelateDemagemRect(-56, -122, 124, 129);
            //((SimpleBoss)Body).Delay = ((SimpleBoss)Body).Delay + ((SimpleBoss)Body).DefaultDelay * 50 / 100;
            switch (type)
            {
                case 1:
                    Body.CallFuction(new LivingCallBack(PersonalAttackDame), 2500);
                    break;
                case 2:
                    Body.CallFuction(new LivingCallBack(AllAttack), 2500);
                    break;
                default:
                    Body.CallFuction(new LivingCallBack(PersonalAttack), 2500);
                    break;
            }
        }

		private void AllAttack()
        {
            Body.PlayMovie("beatB", 3000, 0);
             //SetRelateDemagemRect(-21, -87, 72, 59);
            Body.RangeAttacking(0, Body.X, "cry", 6000, null);
			Body.CallFuction(new LivingCallBack(GoMovie), 5000);
        }
		
		private void GoMovie()
        {
		    List<Player> targets = Game.GetAllFightPlayers();
            foreach (Player p in targets)
            {
                moive = ((PVEGame)Game).Createlayer(p.X, p.Y, "moive", "asset.game.seven.cao", "out", 1, 0);
                moive.PlayMovie("in", 1000, 0);
            }			
		}

        private void PersonalAttack()
        {
            Player target = Game.FindRandomPlayer();
            if (target != null)
            {
                Body.CurrentDamagePlus = 0.8f;
                //int mtX = Game.Random.Next(target.X - 0, target.X + 0);
				if (Body.ShootPoint(target.X, target.Y, 84, 1200, 10000, 1, 3.0f, 2550))
                {
                    Body.PlayMovie("beatA", 1700, 0);
                }                
            }
        }

        public void Shield()
        {
            Body.State = 1;
            Body.PlayMovie("toA", 2700, 0);
            ((SimpleBoss)Body).SetRelateDemagemRect(0, 0, 124, 129);
        }
		
		private void PersonalAttackDame()
        {
            Player target = Game.FindRandomPlayer();
            if (target != null)
            {
                Body.CurrentDamagePlus = 1.0f;
                
                int mtX = Game.Random.Next(target.X - 0, target.X + 0);

                if (Body.ShootPoint(target.X, target.Y, 84, 1200, 10000, 1, 3.0f, 2650))
                {
                    Body.PlayMovie("beat", 1700, 0);
                }
            }
        }
    }
}
