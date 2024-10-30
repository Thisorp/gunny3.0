// Decompiled with JetBrains decompiler
// Type: Game.Server.Packets.Client.ItemFusionHandler
// Assembly: Game.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9A9F5134-773C-4699-91D0-85A9A4DA47CA
// Assembly location: C:\Gunny3.0SV\Gunny 3.4\Release\Start-OLD\Road\Game.Server.dll

using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
  [PacketHandler(78, "熔化")]
  public class ItemFusionHandler : IPacketHandler
  {
    public static List<int> FusionFormulID = new List<int>()
    {
      11302,
      11303,
      11304,
      11301,
      11201,
      11202,
      11203,
      11204
    };

    public int HandlePacket(GameClient client, GSPacketIn packet)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = (int) packet.ReadByte();
      int num2 = packet.ReadInt();
      int MinValid = int.MaxValue;
      List<SqlDataProvider.Data.ItemInfo> Items = new List<SqlDataProvider.Data.ItemInfo>();
      List<SqlDataProvider.Data.ItemInfo> AppendItems = new List<SqlDataProvider.Data.ItemInfo>();
      List<eBageType> eBageTypeList1 = new List<eBageType>();
      if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
      {
        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
        return 1;
      }
      for (int index = 0; index < num2; ++index)
      {
        eBageType bagType = (eBageType) packet.ReadByte();
        int place = packet.ReadInt();
        SqlDataProvider.Data.ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
        if (itemAt != null)
        {
          if (!Items.Contains(itemAt))
          {
            stringBuilder.Append(itemAt.ItemID.ToString() + ":" + (object) itemAt.TemplateID + ",");
            Items.Add(itemAt);
            eBageTypeList1.Add(bagType);
            if (itemAt.ValidDate < MinValid && itemAt.ValidDate != 0)
              MinValid = itemAt.ValidDate;
          }
          else
          {
            client.Out.SendMessage(eMessageType.Normal, "Bad Input");
            return 1;
          }
        }
      }
      if (MinValid == int.MaxValue)
      {
        MinValid = 0;
        Items.Clear();
      }
      PlayerInventory storeBag2 = client.Player.StoreBag2;
      SqlDataProvider.Data.ItemInfo Formul = storeBag2.GetItemAt(0);
      SqlDataProvider.Data.ItemInfo itemInfo1 = (SqlDataProvider.Data.ItemInfo) null;
      string Property = (string) null;
      string AddItem = "";
      for (int slot = 1; slot <= 4; ++slot)
        Items.Add(storeBag2.GetItemAt(slot));
      using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
      {
        foreach (SqlDataProvider.Data.ItemInfo itemInfo2 in Items)
          itemRecordBussiness.FusionItem(itemInfo2, ref Property);
      }
      int num3 = packet.ReadInt();
      List<eBageType> eBageTypeList2 = new List<eBageType>();
      for (int index = 0; index < num3; ++index)
      {
        eBageType bagType = (eBageType) packet.ReadByte();
        int place = packet.ReadInt();
        SqlDataProvider.Data.ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
        if (itemAt != null)
        {
          if (!Items.Contains(itemAt) && !AppendItems.Contains(itemAt))
          {
            stringBuilder.Append(itemAt.ItemID.ToString() + ":" + (object) itemAt.TemplateID + ",");
            AppendItems.Add(itemAt);
            eBageTypeList2.Add(bagType);
            AddItem = AddItem + (object) itemAt.ItemID + ":" + itemAt.Template.Name + "," + (object) itemAt.IsBinds + "|";
          }
          else
          {
            client.Out.SendMessage(eMessageType.Normal, "Bad Input");
            return 1;
          }
        }
      }
      if (0 == num1)
      {
        bool isBind = false;
        Dictionary<int, double> previewItemList = (Dictionary<int, double>) null;
        foreach (int templateId in ItemFusionHandler.FusionFormulID)
        {
          SqlDataProvider.Data.ItemInfo fromTemplate = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 105);
          previewItemList = FusionMgr.FusionPreview(Items, AppendItems, fromTemplate, ref isBind);
          if (previewItemList != null && previewItemList.Count > 0)
            break;
        }
        if (previewItemList != null && previewItemList.Count > 0)
        {
          if (previewItemList.Count != 0)
            client.Out.SendFusionPreview(client.Player, previewItemList, isBind, MinValid);
        }
        else
          client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.ItemNotEnough"));
        return 0;
      }
      storeBag2.ClearBag();
      int num4 = (num2 + num3) * 400;
      if (client.Player.PlayerCharacter.Gold < num4)
      {
        client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney"));
        return 0;
      }
      bool isBind1 = false;
      bool result = false;
      ItemTemplateInfo goods = (ItemTemplateInfo) null;
      foreach (int templateId in ItemFusionHandler.FusionFormulID)
      {
        Formul = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 105);
        goods = FusionMgr.Fusion(Items, AppendItems, Formul, ref isBind1, ref result);
        if (goods != null)
          break;
      }
      if (goods != null)
      {
        client.Player.RemoveGold(num4);
        for (int index = 0; index < Items.Count; ++index)
        {
          --Items[index].Count;
          client.Player.UpdateItem(Items[index]);
        }
        --Formul.Count;
        client.Player.UpdateItem(Formul);
        for (int index = 0; index < AppendItems.Count; ++index)
        {
          --AppendItems[index].Count;
          client.Player.UpdateItem(AppendItems[index]);
        }
        if (result)
        {
          stringBuilder.Append(goods.TemplateID.ToString() + ",");
          SqlDataProvider.Data.ItemInfo fromTemplate = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, 1, 105);
          if (fromTemplate == null)
            return 0;
          itemInfo1 = fromTemplate;
          fromTemplate.IsBinds = isBind1;
          fromTemplate.ValidDate = MinValid;
          client.Player.OnItemFusion(fromTemplate.Template.FusionType);
          client.Out.SendFusionResult(client.Player, result);
          client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1") + fromTemplate.Template.Name);
          if (fromTemplate.TemplateID >= 8300 && fromTemplate.TemplateID <= 8999 || fromTemplate.TemplateID >= 9300 && fromTemplate.TemplateID <= 9999 || fromTemplate.TemplateID >= 14300 && fromTemplate.TemplateID <= 14999)
          {
            string translation = LanguageMgr.GetTranslation("ItemFusionHandler.Notice", (object) client.Player.PlayerCharacter.NickName, (object) fromTemplate.Template.Name);
            GSPacketIn packet1 = new GSPacketIn((short) 10);
            packet1.WriteInt(1);
            packet1.WriteString(translation);
            GameServer.Instance.LoginServer.SendPacket(packet1);
            foreach (GamePlayer allPlayer in WorldMgr.GetAllPlayers())
              allPlayer.Out.SendTCP(packet1);
          }
          if (!client.Player.AddTemplate(fromTemplate, fromTemplate.Template.BagType, fromTemplate.Count))
          {
            stringBuilder.Append("NoPlace");
            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(fromTemplate.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));
          }
        }
        else
        {
          stringBuilder.Append("false");
          client.Out.SendFusionResult(client.Player, result);
          client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed"));
        }
        LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Fusion, Property, itemInfo1, AddItem, Convert.ToInt32(result));
        client.Player.SaveIntoDatabase();
      }
      else
        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
      return 0;
    }
  }
}
