

using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
  [PacketHandler(63, "打开物品")]
  public class OpenUpArkHandler : IPacketHandler
  {
    public int HandlePacket(GameClient client, GSPacketIn packet)
    {
      int num1 = (int) packet.ReadByte();
      int slot = packet.ReadInt();
      SqlDataProvider.Data.ItemInfo itemAt = client.Player.GetInventory((eBageType) num1).GetItemAt(slot);
      string str = "";
      List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
      if (itemAt != null && itemAt.IsValidItem() && (itemAt.Template.CategoryID == 11 && itemAt.Template.Property1 == 6) && client.Player.PlayerCharacter.Grade >= itemAt.Template.NeedLevel)
      {
        int money = 0;
        int gold = 0;
        int giftToken = 0;
        int[] bag = new int[3];
        this.OpenUpItem(itemAt.Template.Data, bag, infos, ref gold, ref money, ref giftToken);
        --bag[itemAt.GetBagType()];
        bool flag;
        if (itemAt.Count > 1)
        {
          --itemAt.Count;
          client.Player.UpdateItem(itemAt);
          flag = true;
        }
        else
          flag = client.Player.RemoveItem(itemAt);
        if (flag)
        {
          StringBuilder stringBuilder1 = new StringBuilder();
          int num2 = 0;
          StringBuilder stringBuilder2 = new StringBuilder();
          stringBuilder2.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start"));
          if (money != 0)
          {
            stringBuilder2.Append(money.ToString() + LanguageMgr.GetTranslation("OpenUpArkHandler.Money"));
            client.Player.AddMoney(money);
            LogMgr.LogMoneyAdd(LogMoneyType.Box, LogMoneyType.Box_Open, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, 0, 0, "", "", "");
          }
          if (gold != 0)
          {
            stringBuilder2.Append(gold.ToString() + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold"));
            client.Player.AddGold(gold);
          }
          if (giftToken != 0)
          {
            stringBuilder2.Append(giftToken.ToString() + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken"));
            client.Player.AddGiftToken(giftToken);
          }
          StringBuilder stringBuilder3 = new StringBuilder();
          int gp = 0;
          foreach (SqlDataProvider.Data.ItemInfo cloneItem in infos)
          {
            if (cloneItem.TemplateID == 11107)
            {
              gp += cloneItem.Count;
            }
            else
            {
              stringBuilder3.Append(cloneItem.Template.Name + "x" + cloneItem.Count.ToString() + ",");
              if (cloneItem.Template.Quality >= itemAt.Template.Property2 & itemAt.Template.Property2 != 0)
              {
                stringBuilder1.Append(cloneItem.Template.Name + ",");
                ++num2;
              }
              if (!client.Player.AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count))
              {
                using (PlayerBussiness playerBussiness = new PlayerBussiness())
                {
                  cloneItem.UserID = 0;
                  playerBussiness.AddGoods(cloneItem);
                  MailInfo mail = new MailInfo();
                  mail.Annex1 = cloneItem.ItemID.ToString();
                  mail.Content = LanguageMgr.GetTranslation("OpenUpArkHandler.Content1") + cloneItem.Template.Name + LanguageMgr.GetTranslation("OpenUpArkHandler.Content2");
                  mail.Gold = 0;
                  mail.Money = 0;
                  mail.Receiver = client.Player.PlayerCharacter.NickName;
                  mail.ReceiverID = client.Player.PlayerCharacter.ID;
                  mail.Sender = mail.Receiver;
                  mail.SenderID = mail.ReceiverID;
                  mail.Title = LanguageMgr.GetTranslation("OpenUpArkHandler.Title") + cloneItem.Template.Name + "]";
                  mail.Type = 1;
                  playerBussiness.SendMail(mail);
                  str = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail");
                }
              }
            }
          }
          if (gp > 0)
          {
            stringBuilder2.Append(gp.ToString() + " Kinh nghiệm.");
            client.Player.AddGP(gp);
          }
          if (stringBuilder3.Length > 0)
          {
            stringBuilder3.Remove(stringBuilder3.Length - 1, 1);
            string[] strArray = stringBuilder3.ToString().Split(',');
            for (int index1 = 0; index1 < strArray.Length; ++index1)
            {
              int num3 = 1;
              for (int index2 = index1 + 1; index2 < strArray.Length; ++index2)
              {
                if (strArray[index1].Contains(strArray[index2]) && strArray[index2].Length == strArray[index1].Length)
                {
                  ++num3;
                  strArray[index2] = index2.ToString();
                }
              }
              if (num3 > 1)
              {
                strArray[index1] = strArray[index1].Remove(strArray[index1].Length - 1, 1);
                strArray[index1] = strArray[index1] + num3.ToString();
              }
              if (strArray[index1] != index1.ToString())
              {
                strArray[index1] = strArray[index1] + ",";
                stringBuilder2.Append(strArray[index1]);
              }
            }
          }
          if (itemAt.Template.Property2 != 0 & num2 > 0)
          {
            string translation = LanguageMgr.GetTranslation("OpenUpArkHandler.Notice", (object) client.Player.PlayerCharacter.NickName, (object) itemAt.Template.Name, (object) stringBuilder1, (object) stringBuilder1.Remove(stringBuilder1.Length - 1, 1));
            GSPacketIn packet1 = new GSPacketIn((short) 10);
            packet1.WriteInt(2);
            packet1.WriteString(translation);
            GameServer.Instance.LoginServer.SendPacket(packet1);
            foreach (GamePlayer allPlayer in WorldMgr.GetAllPlayers())
            {
              if (allPlayer != client.Player)
                allPlayer.Out.SendTCP(packet1);
            }
          }
          stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
          stringBuilder2.Append(".");
          client.Out.SendMessage(eMessageType.Normal, str + stringBuilder2.ToString());
          if (!string.IsNullOrEmpty(str))
            client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
        }
      }
      return 1;
    }

    public void OpenUpItem(string data, int[] bag, List<SqlDataProvider.Data.ItemInfo> infos, ref int gold, ref int money, ref int giftToken)
    {
      if (string.IsNullOrEmpty(data))
        return;
      ItemBoxMgr.CreateItemBox(Convert.ToInt32(data), infos, ref gold, ref money, ref giftToken);
    }
  }
}
