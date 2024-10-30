using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
  [PacketHandler(59, "物品强化")]
  public class ItemStrengthenHandler : IPacketHandler
  {
    public int HandlePacket(GameClient client, GSPacketIn packet)
    {
      GSPacketIn packet1 = packet.Clone();
      packet1.ClearContext();
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = false;
      int priceStrenghtnGold = GameProperties.PRICE_STRENGHTN_GOLD;
      if (client.Player.PlayerCharacter.Gold < priceStrenghtnGold)
      {
        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoMoney"));
        return 0;
      }
      bool flag2 = packet.ReadBoolean();
      List<ItemInfo> itemInfoList = new List<ItemInfo>();
      ItemInfo itemInfo1 = client.Player.StoreBag2.GetItemAt(5);
      ItemInfo itemInfo2 = (ItemInfo) null;
      ItemInfo itemInfo3 = (ItemInfo) null;
      string Property = (string) null;
      string AddItem = "";
      using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
        itemRecordBussiness.PropertyString(itemInfo1, ref Property);
      if (itemInfo1 != null && itemInfo1.Template.CanStrengthen && itemInfo1.Template.CategoryID < 18 && itemInfo1.Count == 1)
      {
        bool flag3 = flag1 || itemInfo1.IsBinds;
        stringBuilder.Append(itemInfo1.ItemID.ToString() + ":" + (object) itemInfo1.TemplateID + ",");
        ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();
        int num1 = 1;
        double num2 = 0.0;
        bool flag4 = false;
        StrengthenGoodsInfo strengthenGoodsInfo = (StrengthenGoodsInfo) null;
        StrengthenInfo strengthenInfo;
        if (itemInfo1.StrengthenLevel != 9 && itemInfo1.StrengthenLevel != 11 && itemInfo1.StrengthenLevel != 14)
        {
          strengthenInfo = StrengthenMgr.FindRefineryStrengthenInfo(itemInfo1.StrengthenLevel + 1);
        }
        else
        {
          strengthenGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(itemInfo1.StrengthenLevel, itemInfo1.TemplateID);
          strengthenInfo = StrengthenMgr.FindStrengthenInfo(itemInfo1.StrengthenLevel + 1);
        }
        if (strengthenInfo == null)
        {
          client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoStrength"));
          return 0;
        }
        int itemId;
        if (client.Player.StoreBag2.GetItemAt(3) != null)
        {
          itemInfo3 = client.Player.StoreBag2.GetItemAt(3);
          string[] strArray1 = new string[5]
          {
            AddItem,
            ",",
            null,
            null,
            null
          };
          string[] strArray2 = strArray1;
          int index = 2;
          itemId = itemInfo3.ItemID;
          string str = itemId.ToString();
          strArray2[index] = str;
          strArray1[3] = ":";
          strArray1[4] = itemInfo3.Template.Name;
          AddItem = string.Concat(strArray1);
          if (itemInfo3 != null && itemInfo3.Template.CategoryID == 11 && itemInfo3.Template.Property1 == 7)
          {
            flag3 = flag3 || itemInfo3.IsBinds;
            stringBuilder.Append(itemInfo3.ItemID.ToString() + ":" + (object) itemInfo3.TemplateID + ",");
            flag4 = true;
          }
          else
            itemInfo3 = (ItemInfo) null;
        }
        ItemInfo itemAt1 = client.Player.StoreBag2.GetItemAt(0);
        if (itemAt1 != null && itemAt1.Template.CategoryID == 11 && (itemAt1.Template.Property1 == 2 || itemAt1.Template.Property1 == 35) && !itemInfoList.Contains(itemAt1))
        {
          flag3 = flag3 || itemAt1.IsBinds;
          string[] strArray1 = new string[5]
          {
            AddItem,
            ",",
            null,
            null,
            null
          };
          string[] strArray2 = strArray1;
          int index = 2;
          itemId = itemAt1.ItemID;
          string str = itemId.ToString();
          strArray2[index] = str;
          strArray1[3] = ":";
          strArray1[4] = itemAt1.Template.Name;
          AddItem = string.Concat(strArray1);
          itemInfoList.Add(itemAt1);
          num2 += (double) itemAt1.Template.Property2;
        }
        ItemInfo itemAt2 = client.Player.StoreBag2.GetItemAt(1);
        if (itemAt2 != null && itemAt2.Template.CategoryID == 11 && (itemAt2.Template.Property1 == 2 || itemAt2.Template.Property1 == 35) && !itemInfoList.Contains(itemAt2))
        {
          flag3 = flag3 || itemAt2.IsBinds;
          string[] strArray1 = new string[5]
          {
            AddItem,
            ",",
            null,
            null,
            null
          };
          string[] strArray2 = strArray1;
          int index = 2;
          itemId = itemAt2.ItemID;
          string str = itemId.ToString();
          strArray2[index] = str;
          strArray1[3] = ":";
          strArray1[4] = itemAt2.Template.Name;
          AddItem = string.Concat(strArray1);
          itemInfoList.Add(itemAt2);
          num2 += (double) itemAt2.Template.Property2;
        }
        ItemInfo itemAt3 = client.Player.StoreBag2.GetItemAt(2);
        if (itemAt3 != null && itemAt3.Template.CategoryID == 11 && (itemAt3.Template.Property1 == 2 || itemAt3.Template.Property1 == 35) && !itemInfoList.Contains(itemAt3))
        {
          flag3 = flag3 || itemAt3.IsBinds;
          AddItem = AddItem + "," + (object) itemAt3.ItemID + ":" + itemAt3.Template.Name;
          itemInfoList.Add(itemAt3);
          num2 += (double) itemAt3.Template.Property2;
        }
        double num3;
        if (client.Player.StoreBag2.GetItemAt(4) != null)
        {
          itemInfo2 = client.Player.StoreBag2.GetItemAt(4);
          string[] strArray1 = new string[5]
          {
            AddItem,
            ",",
            null,
            null,
            null
          };
          string[] strArray2 = strArray1;
          int index = 2;
          itemId = itemInfo2.ItemID;
          string str = itemId.ToString();
          strArray2[index] = str;
          strArray1[3] = ":";
          strArray1[4] = itemInfo2.Template.Name;
          AddItem = string.Concat(strArray1);
          if (itemInfo2 != null && itemInfo2.Template.CategoryID == 11 && itemInfo2.Template.Property1 == 3)
          {
            flag3 = flag3 || itemInfo2.IsBinds;
            stringBuilder.Append(itemInfo2.ItemID.ToString() + ":" + (object) itemInfo2.TemplateID + ",");
            num3 = num2 * (double) (itemInfo2.Template.Property2 + 100);
          }
          else
          {
            num3 = num2 * 100.0;
            itemInfo2 = (ItemInfo) null;
          }
        }
        else
          num3 = num2 * 100.0;
        bool flag5 = false;
        ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
        if (flag2)
        {
          ConsortiaEquipControlInfo consortiaEuqipRiches = new ConsortiaBussiness().GetConsortiaEuqipRiches(client.Player.PlayerCharacter.ConsortiaID, 0, 2);
          if (consortiaInfo == null)
          {
            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail"));
          }
          else
          {
            if (client.Player.PlayerCharacter.Riches < consortiaEuqipRiches.Riches)
            {
              client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission"));
              return 1;
            }
            flag5 = true;
          }
        }
        if (itemInfoList.Count >= 1)
        {
          double num4 = num3 / (double) strengthenInfo.Rock;
          for (int index = 0; index < itemInfoList.Count; ++index)
          {
            stringBuilder.Append(itemInfoList[index].ItemID.ToString() + ":" + (object) itemInfoList[index].TemplateID + ",");
            AbstractInventory itemInventory = (AbstractInventory) client.Player.GetItemInventory(itemInfoList[index].Template);
            --itemInfoList[index].Count;
            itemInventory.UpdateItem(itemInfoList[index]);
          }
          if (itemInfo2 != null)
            client.Player.GetItemInventory(itemInfo2.Template).RemoveItem(itemInfo2);
          if (itemInfo3 != null)
            client.Player.GetItemInventory(itemInfo3.Template).RemoveItem(itemInfo3);
          if (flag5)
            num4 *= 1.0 + 0.1 * (double) consortiaInfo.SmithLevel;
          itemInfo1.IsBinds = flag3;
          client.Player.StoreBag2.ClearBag();
          if (num4 > (double) threadSafeRandom.Next(10000))
          {
            stringBuilder.Append("true");
            packet1.WriteByte((byte) 0);
            if (strengthenGoodsInfo != null)
            {
              ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(strengthenGoodsInfo.GainEquip);
              if (itemTemplate != null)
              {
                ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(itemTemplate, 1, 116);
                fromTemplate.StrengthenLevel = itemInfo1.StrengthenLevel + 1;
                ItemInfo.OpenHole(ref fromTemplate);
                StrengthenMgr.InheritProperty(itemInfo1, ref fromTemplate);
                client.Player.StoreBag2.AddItemTo(fromTemplate, 5);
                itemInfo1 = fromTemplate;
                if (itemInfo1.StrengthenLevel == 3 || itemInfo1.StrengthenLevel == 6 || itemInfo1.StrengthenLevel == 9 || itemInfo1.StrengthenLevel == 12)
                  packet1.WriteBoolean(true);
                else
                  packet1.WriteBoolean(false);
              }
            }
            else
            {
              ++itemInfo1.StrengthenLevel;
              ItemInfo.OpenHole(ref itemInfo1);
              client.Player.StoreBag2.AddItemTo(itemInfo1, 5);
              if (itemInfo1.StrengthenLevel == 3 || itemInfo1.StrengthenLevel == 6 || itemInfo1.StrengthenLevel == 9 || itemInfo1.StrengthenLevel == 12)
                packet1.WriteBoolean(true);
              else
                packet1.WriteBoolean(false);
            }
            client.Player.OnItemStrengthen(itemInfo1.Template.CategoryID, itemInfo1.StrengthenLevel);
            LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Strengthen, Property, itemInfo1, AddItem, 1);
            client.Player.SaveIntoDatabase();
            if (itemInfo1.StrengthenLevel >= 10)
            {
              string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation", (object) client.Player.PlayerCharacter.NickName, (object) itemInfo1.Template.Name, (object) itemInfo1.StrengthenLevel);
              GSPacketIn packet2 = new GSPacketIn((short) 10);
              packet2.WriteInt(1);
              packet2.WriteString(translation);
              GameServer.Instance.LoginServer.SendPacket(packet2);
              foreach (GamePlayer allPlayer in WorldMgr.GetAllPlayers())
                allPlayer.Out.SendTCP(packet2);
            }
          }
          else
          {
            stringBuilder.Append("false");
            packet1.WriteByte((byte) 1);
            packet1.WriteBoolean(false);
            if (!flag4)
            {
              if (itemInfo1.Template.Level == 3)
              {
                itemInfo1.StrengthenLevel = itemInfo1.StrengthenLevel == 0 ? 0 : itemInfo1.StrengthenLevel - 1;
                client.Player.StoreBag2.AddItemTo(itemInfo1, 5);
              }
              else
              {
                --itemInfo1.Count;
                client.Player.StoreBag2.AddItemTo(itemInfo1, 5);
              }
            }
            else
              client.Player.StoreBag2.AddItemTo(itemInfo1, 5);
            LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Strengthen, Property, itemInfo1, AddItem, 0);
            client.Player.SaveIntoDatabase();
          }
          client.Out.SendTCP(packet1);
          stringBuilder.Append(itemInfo1.StrengthenLevel);
          client.Player.BeginChanges();
          client.Player.RemoveGold(priceStrenghtnGold);
          client.Player.CommitChanges();
        }
        else
          client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1") + (object) num1 + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2"));
        if (itemInfo1.Place < 31)
          client.Player.MainBag.UpdatePlayerProperties();
      }
      else
        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Success"));
      return 0;
    }
  }
}
