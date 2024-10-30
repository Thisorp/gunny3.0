// Decompiled with JetBrains decompiler
// Type: Game.Server.Packets.Client.ItemInlayHandle
// Assembly: Game.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9A9F5134-773C-4699-91D0-85A9A4DA47CA
// Assembly location: C:\Gunny3.0SV\Gunny 3.4\Release\Start-OLD\Road\Game.Server.dll

using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
  [PacketHandler(121, "物品镶嵌")]
  public class ItemInlayHandle : IPacketHandler
  {
    public int HandlePacket(GameClient client, GSPacketIn packet)
    {
      GSPacketIn pkg = packet.Clone();
      pkg.ClearContext();
      int num1 = packet.ReadInt();
      int place1 = packet.ReadInt();
      int num2 = packet.ReadInt();
      int num3 = packet.ReadInt();
      int place2 = packet.ReadInt();
      SqlDataProvider.Data.ItemInfo itemAt1 = client.Player.GetItemAt((eBageType) num1, place1);
      SqlDataProvider.Data.ItemInfo itemAt2 = client.Player.GetItemAt((eBageType) num3, place2);
      string Property = (string) null;
      string AddItem = "";
      using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
        itemRecordBussiness.PropertyString(itemAt1, ref Property);
      int num4 = 2000;
      if (itemAt1 == null || itemAt2 == null || itemAt2.Template.Property1 != 31)
        return 0;
      if (client.Player.PlayerCharacter.Gold > num4)
      {
        string[] strArray = itemAt1.Template.Hole.Split('|');
        if (num2 > 0 && num2 < 7)
        {
          client.Player.RemoveGold(num4);
          bool flag = false;
          switch (num2)
          {
            case 1:
              if (itemAt1.Hole1 >= 0)
              {
                if (Convert.ToInt32(strArray[0].Split(',')[1]) == itemAt2.Template.Property2)
                {
                  itemAt1.Hole1 = itemAt2.TemplateID;
                  AddItem = AddItem + "," + (object) itemAt2.ItemID + "," + itemAt2.Template.Name;
                  flag = true;
                }
                break;
              }
              break;
            case 2:
              if (itemAt1.Hole2 >= 0)
              {
                if (Convert.ToInt32(strArray[1].Split(',')[1]) == itemAt2.Template.Property2)
                {
                  itemAt1.Hole2 = itemAt2.TemplateID;
                  AddItem = AddItem + "," + (object) itemAt2.ItemID + "," + itemAt2.Template.Name;
                  flag = true;
                }
                break;
              }
              break;
            case 3:
              if (itemAt1.Hole3 >= 0)
              {
                if (Convert.ToInt32(strArray[2].Split(',')[1]) == itemAt2.Template.Property2)
                {
                  itemAt1.Hole3 = itemAt2.TemplateID;
                  AddItem = AddItem + "," + (object) itemAt2.ItemID + "," + itemAt2.Template.Name;
                  flag = true;
                }
                break;
              }
              break;
            case 4:
              if (itemAt1.Hole4 >= 0)
              {
                if (Convert.ToInt32(strArray[3].Split(',')[1]) == itemAt2.Template.Property2)
                {
                  itemAt1.Hole4 = itemAt2.TemplateID;
                  AddItem = AddItem + "," + (object) itemAt2.ItemID + "," + itemAt2.Template.Name;
                  flag = true;
                }
                break;
              }
              break;
            case 5:
              if (itemAt1.Hole5 >= 0)
              {
                if (Convert.ToInt32(strArray[4].Split(',')[1]) == itemAt2.Template.Property2)
                {
                  itemAt1.Hole5 = itemAt2.TemplateID;
                  AddItem = AddItem + "," + (object) itemAt2.ItemID + "," + itemAt2.Template.Name;
                  flag = true;
                }
                break;
              }
              break;
            case 6:
              if (itemAt1.Hole6 >= 0)
              {
                if (Convert.ToInt32(strArray[5].Split(',')[1]) == itemAt2.Template.Property2)
                {
                  itemAt1.Hole6 = itemAt2.TemplateID;
                  AddItem = AddItem + "," + (object) itemAt2.ItemID + "," + itemAt2.Template.Name;
                  flag = true;
                }
                break;
              }
              break;
          }
          if (flag)
          {
            client.Player.StoreBag2.MoveToStore(client.Player.StoreBag2, 0, client.Player.MainBag.FindFirstEmptySlot(32), (PlayerInventory) client.Player.MainBag, 9);
            pkg.WriteInt(0);
            --itemAt2.Count;
            client.Player.UpdateItem(itemAt2);
            client.Player.UpdateItem(itemAt1);
          }
          LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Insert, Property, itemAt1, AddItem, Convert.ToInt32(flag));
        }
        else
        {
          pkg.WriteByte((byte) 1);
          client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemInlayHandle.NoPlace"));
        }
        client.Player.SendTCP(pkg);
        client.Player.SaveIntoDatabase();
      }
      else
        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
      return 0;
    }
  }
}
