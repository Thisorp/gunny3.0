

using Bussiness;
using Game.Server;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Base.Packets
{
  [PacketLib(1)]
  public class AbstractPacketLib : IPacketLib
  {
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    protected readonly GameClient m_gameClient;

    public AbstractPacketLib(GameClient client)
    {
      this.m_gameClient = client;
    }

    public static IPacketLib CreatePacketLibForVersion(int rawVersion, GameClient client)
    {
      foreach (Type derivedClass in ScriptMgr.GetDerivedClasses(typeof (IPacketLib)))
      {
        foreach (PacketLibAttribute customAttribute in derivedClass.GetCustomAttributes(typeof (PacketLibAttribute), false))
        {
          if (customAttribute.RawVersion == rawVersion)
          {
            try
            {
              return (IPacketLib) Activator.CreateInstance(derivedClass, new object[1]
              {
                (object) client
              });
            }
            catch (Exception ex)
            {
              if (AbstractPacketLib.log.IsErrorEnabled)
                AbstractPacketLib.log.Error((object) ("error creating packetlib (" + derivedClass.FullName + ") for raw version " + (object) rawVersion), ex);
            }
          }
        }
      }
      return (IPacketLib) null;
    }

    public void SendTCP(GSPacketIn packet)
    {
      this.m_gameClient.SendTCP(packet);
    }

    public void SendLoginFailed(string msg)
    {
      GSPacketIn packet = new GSPacketIn((short) 1);
      packet.WriteByte((byte) 1);
      packet.WriteString(msg);
      this.SendTCP(packet);
    }

    public void SendLoginSuccess()
    {
      if (this.m_gameClient.Player == null)
        return;
      GSPacketIn packet = new GSPacketIn((short) 1, this.m_gameClient.Player.PlayerCharacter.ID);
      packet.WriteByte((byte) 0);
      packet.WriteInt(4);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Attack);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Defence);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Agility);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Luck);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.GP);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Repute);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Gold);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Money);
      packet.WriteInt(this.m_gameClient.Player.PropBag.GetItemCount(7001));
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Hide);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.FightPower);
      packet.WriteInt(5);
      packet.WriteInt(-1);
      packet.WriteString("Master");
      packet.WriteInt(5);
      packet.WriteString("HoNorMaster");
      GSPacketIn gsPacketIn1 = packet;
      DateTime now = DateTime.Now;
      DateTime date1 = now.AddDays(50.0);
      gsPacketIn1.WriteDateTime(date1);
      packet.WriteBoolean(true);
      packet.WriteInt(5);
      packet.WriteInt(50000);
      GSPacketIn gsPacketIn2 = packet;
      now = DateTime.Now;
      DateTime date2 = now.AddDays(50.0);
      gsPacketIn2.WriteDateTime(date2);
      GSPacketIn gsPacketIn3 = packet;
      now = DateTime.Now;
      DateTime date3 = now.AddDays(50.0);
      gsPacketIn3.WriteDateTime(date3);
      packet.WriteInt(50);
      packet.WriteDateTime(DateTime.Now);
      packet.WriteBoolean(false);
      packet.WriteInt(1599);
      packet.WriteInt(1599);
      packet.WriteString("honor");
      packet.WriteInt(0);
      packet.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.Sex);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.Style + "&" + this.m_gameClient.Player.PlayerCharacter.Colors);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.Skin);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaID);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.ConsortiaName);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.DutyLevel);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.DutyName);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Right);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.ChairmanName);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaHonor);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaRiches);
      packet.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.HasBagPassword);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest1);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest2);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.FailedPasswordAttemptCount);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.UserName);
      packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Nimbus);
      packet.WriteString(this.m_gameClient.Player.PlayerCharacter.PvePermission);
      packet.WriteString("1111111");
      packet.WriteInt(99999);
      packet.WriteInt(1000);
      packet.WriteInt(2000);
      packet.WriteInt(3000);
      GSPacketIn gsPacketIn4 = packet;
      now = DateTime.Now;
      DateTime date4 = now.AddDays(-5.0);
      gsPacketIn4.WriteDateTime(date4);
      GSPacketIn gsPacketIn5 = packet;
      now = DateTime.Now;
      DateTime date5 = now.AddDays(-5.0);
      gsPacketIn5.WriteDateTime(date5);
      this.SendTCP(packet);
    }

    public void SendLoginSuccess2()
    {
    }

    public void SendRSAKey(byte[] m, byte[] e)
    {
      GSPacketIn packet = new GSPacketIn((short) 7);
      packet.Write(m);
      packet.Write(e);
      this.SendTCP(packet);
    }

    public void SendCheckCode()
    {
      if (this.m_gameClient.Player == null || this.m_gameClient.Player.PlayerCharacter.CheckCount < GameProperties.CHECK_MAX_FAILED_COUNT)
        return;
      if (this.m_gameClient.Player.PlayerCharacter.CheckError == 0)
        this.m_gameClient.Player.PlayerCharacter.CheckCount += 10000;
      GSPacketIn packet = new GSPacketIn((short) 200, this.m_gameClient.Player.PlayerCharacter.ID, 10240);
      if (this.m_gameClient.Player.PlayerCharacter.CheckError < 1)
        packet.WriteByte((byte) 0);
      else
        packet.WriteByte((byte) 2);
      packet.WriteBoolean(true);
      this.m_gameClient.Player.PlayerCharacter.CheckCode = CheckCode.GenerateCheckCode();
      packet.Write(CheckCode.CreateImage(this.m_gameClient.Player.PlayerCharacter.CheckCode));
      this.SendTCP(packet);
    }

    public void SendKitoff(string msg)
    {
      GSPacketIn packet = new GSPacketIn((short) 2);
      packet.WriteString(msg);
      this.SendTCP(packet);
    }

    public void SendEditionError(string msg)
    {
      GSPacketIn packet = new GSPacketIn((short) 12);
      packet.WriteString(msg);
      this.SendTCP(packet);
    }

    public void SendWaitingRoom(bool result)
    {
      GSPacketIn packet = new GSPacketIn((short) 16);
      packet.WriteByte(result ? (byte) 1 : (byte) 0);
      this.SendTCP(packet);
    }

    public GSPacketIn SendPlayerState(int id, byte state)
    {
      GSPacketIn packet = new GSPacketIn((short) 32, id);
      packet.WriteByte(state);
      this.SendTCP(packet);
      return packet;
    }

    public virtual GSPacketIn SendMessage(eMessageType type, string message)
    {
      GSPacketIn packet = new GSPacketIn((short) 3);
      packet.WriteInt((int) type);
      packet.WriteString(message);
      this.SendTCP(packet);
      return packet;
    }

    public void SendReady()
    {
      this.SendTCP(new GSPacketIn((short) 0));
    }

    public void SendUpdatePrivateInfo(PlayerInfo info)
    {
      GSPacketIn packet = new GSPacketIn((short) 38, info.ID);
      packet.WriteInt(info.Money);
      packet.WriteInt(m_gameClient.Player.PropBag.GetItemCount(11408));
      packet.WriteInt(info.Gold);
      packet.WriteInt(info.GiftToken);
      this.SendTCP(packet);
    }

    public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info)
    {
      GSPacketIn packet = new GSPacketIn((short) 67, info.ID);
      packet.WriteInt(info.GP);
      packet.WriteInt(info.Offer);
      packet.WriteInt(info.RichesOffer);
      packet.WriteInt(info.RichesRob);
      packet.WriteInt(info.Win);
      packet.WriteInt(info.Total);
      packet.WriteInt(info.Escape);
      packet.WriteInt(info.Attack);
      packet.WriteInt(info.Defence);
      packet.WriteInt(info.Agility);
      packet.WriteInt(info.Luck);
      packet.WriteInt(info.Hide);
      packet.WriteString(info.Style);
      packet.WriteString(info.Colors);
      packet.WriteString(info.Skin);
      packet.WriteInt(info.ConsortiaID);
      packet.WriteString(info.ConsortiaName);
      packet.WriteInt(info.ConsortiaLevel);
      packet.WriteInt(info.ConsortiaRepute);
      packet.WriteInt(info.Nimbus);
      packet.WriteString(info.PvePermission);
      packet.WriteString("1");
      packet.WriteInt(info.FightPower);
      packet.WriteInt(1);
      packet.WriteInt(-1);
      packet.WriteString("ss");
      packet.WriteInt(1);
      packet.WriteString("ss");
      packet.WriteInt(0);
      packet.WriteString("honor");
      if (info.ExpendDate.HasValue)
        packet.WriteDateTime(info.ExpendDate.Value);
      else
        packet.WriteDateTime(DateTime.MinValue);
      packet.WriteInt(100);
      packet.WriteInt(100);
      packet.WriteDateTime(DateTime.MinValue);
      packet.WriteInt(10001);
      packet.WriteInt(0);
      packet.WriteInt(info.AnswerSite);
      this.SendTCP(packet);
      return packet;
    }

    public void SendPingTime(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 4);
      player.PingStart = DateTime.Now.Ticks;
      packet.WriteInt(player.PlayerCharacter.AntiAddiction);
      this.SendTCP(packet);
    }

    public GSPacketIn SendNetWork(int id, long delay)
    {
      GSPacketIn packet = new GSPacketIn((short) 6, id);
      packet.WriteInt((int) delay / 1000 / 10);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendUserEquip(PlayerInfo player, List<SqlDataProvider.Data.ItemInfo> items)
    {
      GSPacketIn packet = new GSPacketIn((short) 74, player.ID, 10240);
      packet.WriteInt(player.ID);
      packet.WriteInt(player.Agility);
      packet.WriteInt(player.Attack);
      packet.WriteString(player.Colors);
      packet.WriteString(player.Skin);
      packet.WriteInt(player.Defence);
      packet.WriteInt(player.GP);
      packet.WriteInt(player.Grade);
      packet.WriteInt(player.Luck);
      packet.WriteInt(player.Hide);
      packet.WriteInt(player.Repute);
      packet.WriteBoolean(player.Sex);
      packet.WriteString(player.Style);
      packet.WriteInt(player.Offer);
      packet.WriteString(player.NickName);
      packet.WriteBoolean(true);
      packet.WriteInt(5);
      packet.WriteInt(player.Win);
      packet.WriteInt(player.Total);
      packet.WriteInt(player.Escape);
      packet.WriteInt(player.ConsortiaID);
      packet.WriteString(player.ConsortiaName);
      packet.WriteInt(player.RichesOffer);
      packet.WriteInt(player.RichesRob);
      packet.WriteBoolean(player.IsMarried);
      packet.WriteInt(player.SpouseID);
      packet.WriteString(player.SpouseName);
      packet.WriteString(player.DutyName);
      packet.WriteInt(player.Nimbus);
      packet.WriteInt(player.FightPower);
      packet.WriteInt(5);
      packet.WriteInt(-1);
      packet.WriteString("Master");
      packet.WriteInt(5);
      packet.WriteString("HoNorMaster");
      packet.WriteInt(9999);
      packet.WriteString("Honor");
      packet.WriteDateTime(DateTime.Now.AddDays(-2.0));
      packet.WriteInt(items.Count);
      foreach (SqlDataProvider.Data.ItemInfo itemInfo in items)
      {
        packet.WriteByte((byte) itemInfo.BagType);
        packet.WriteInt(itemInfo.UserID);
        packet.WriteInt(itemInfo.ItemID);
        packet.WriteInt(itemInfo.Count);
        packet.WriteInt(itemInfo.Place);
        packet.WriteInt(itemInfo.TemplateID);
        packet.WriteInt(itemInfo.AttackCompose);
        packet.WriteInt(itemInfo.DefendCompose);
        packet.WriteInt(itemInfo.AgilityCompose);
        packet.WriteInt(itemInfo.LuckCompose);
        packet.WriteInt(itemInfo.StrengthenLevel);
        packet.WriteBoolean(itemInfo.IsBinds);
        packet.WriteBoolean(itemInfo.IsJudge);
        packet.WriteDateTime(itemInfo.BeginDate);
        packet.WriteInt(itemInfo.ValidDate);
        packet.WriteString(itemInfo.Color);
        packet.WriteString(itemInfo.Skin);
        packet.WriteBoolean(itemInfo.IsUsed);
        packet.WriteInt(itemInfo.Hole1);
        packet.WriteInt(itemInfo.Hole2);
        packet.WriteInt(itemInfo.Hole3);
        packet.WriteInt(itemInfo.Hole4);
        packet.WriteInt(itemInfo.Hole5);
        packet.WriteInt(itemInfo.Hole6);
        packet.WriteString(itemInfo.Template.Pic);
        packet.WriteInt(itemInfo.Template.RefineryLevel);
        packet.WriteDateTime(DateTime.Now);
        packet.WriteByte((byte) 5);
        packet.WriteInt(5);
        packet.WriteByte((byte) 5);
        packet.WriteInt(5);
      }
      packet.Compress();
      this.SendTCP(packet);
      return packet;
    }

    public void SendDateTime()
    {
      GSPacketIn packet = new GSPacketIn((short) 5);
      packet.WriteDateTime(DateTime.Now);
      this.SendTCP(packet);
    }

    public GSPacketIn SendDailyAward(GamePlayer player)
    {
      bool val = false;
      DateTime dateTime = DateTime.Now;
      DateTime date1 = dateTime.Date;
      dateTime = player.PlayerCharacter.LastAward;
      DateTime date2 = dateTime.Date;
      if (date1 != date2)
        val = true;
      GSPacketIn packet = new GSPacketIn((short) 13);
      packet.WriteBoolean(val);
      packet.WriteInt(0);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendUpdateRoomList(BaseRoom room)
    {
      GSPacketIn packet = new GSPacketIn((short) 95);
      packet.WriteInt(1);
      packet.WriteInt(1);
      packet.WriteInt(room.RoomId);
      packet.WriteByte((byte) room.RoomType);
      packet.WriteByte(room.TimeMode);
      packet.WriteByte((byte) room.PlayerCount);
      packet.WriteByte((byte) room.PlacesCount);
      packet.WriteBoolean(!string.IsNullOrEmpty(room.Password));
      packet.WriteInt(room.MapId);
      packet.WriteBoolean(room.IsPlaying);
      packet.WriteString(room.Name);
      packet.WriteByte((byte) room.GameType);
      packet.WriteByte((byte) room.HardLevel);
      packet.WriteInt(room.LevelLimits);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendUpdateRoomList(List<BaseRoom> roomlist)
    {
      GSPacketIn packet = new GSPacketIn((short) 95);
      packet.WriteInt(roomlist.Count);
      int val = roomlist.Count < 10 ? roomlist.Count : 10;
      packet.WriteInt(val);
      for (int index = 0; index < val; ++index)
      {
        BaseRoom baseRoom = roomlist[index];
        packet.WriteInt(baseRoom.RoomId);
        packet.WriteByte((byte) baseRoom.RoomType);
        packet.WriteByte(baseRoom.TimeMode);
        packet.WriteByte((byte) baseRoom.PlayerCount);
        packet.WriteByte((byte) baseRoom.PlacesCount);
        packet.WriteBoolean(!string.IsNullOrEmpty(baseRoom.Password));
        packet.WriteInt(baseRoom.MapId);
        packet.WriteBoolean(baseRoom.IsPlaying);
        packet.WriteString(baseRoom.Name);
        packet.WriteByte((byte) baseRoom.GameType);
        packet.WriteByte((byte) baseRoom.HardLevel);
        packet.WriteInt(baseRoom.LevelLimits);
      }
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendSceneAddPlayer(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 18, player.PlayerCharacter.ID);
      packet.WriteInt(player.PlayerCharacter.Grade);
      packet.WriteBoolean(player.PlayerCharacter.Sex);
      packet.WriteString(player.PlayerCharacter.NickName);
      packet.WriteBoolean(true);
      packet.WriteInt(5);
      packet.WriteString(player.PlayerCharacter.ConsortiaName);
      packet.WriteInt(player.PlayerCharacter.Offer);
      packet.WriteInt(player.PlayerCharacter.Win);
      packet.WriteInt(player.PlayerCharacter.Total);
      packet.WriteInt(player.PlayerCharacter.Escape);
      packet.WriteInt(player.PlayerCharacter.ConsortiaID);
      packet.WriteInt(player.PlayerCharacter.Repute);
      packet.WriteBoolean(player.PlayerCharacter.IsMarried);
      if (player.PlayerCharacter.IsMarried)
      {
        packet.WriteInt(player.PlayerCharacter.SpouseID);
        packet.WriteString(player.PlayerCharacter.SpouseName);
      }
      packet.WriteString(player.PlayerCharacter.UserName);
      packet.WriteInt(player.PlayerCharacter.FightPower);
      packet.WriteInt(5);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 21, player.PlayerCharacter.ID);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 82, player.PlayerId);
      bool val = false;
      if (player.CurrentRoom.Game != null)
        val = true;
      packet.WriteBoolean(val);
      packet.WriteByte((byte) player.CurrentRoomIndex);
      packet.WriteByte((byte) player.CurrentRoomTeam);
      packet.WriteInt(player.PlayerCharacter.Grade);
      packet.WriteInt(player.PlayerCharacter.Hide);
      packet.WriteInt(player.PlayerCharacter.Repute);
      packet.WriteInt((int) player.PingTime / 1000 / 10);
      packet.WriteInt(player.PlayerCharacter.ID);
      packet.WriteInt(4);
      packet.WriteInt(player.PlayerCharacter.ID);
      packet.WriteString(player.PlayerCharacter.NickName);
      packet.WriteBoolean(true);
      packet.WriteInt(5);
      packet.WriteBoolean(player.PlayerCharacter.Sex);
      packet.WriteString(player.PlayerCharacter.Style);
      packet.WriteString(player.PlayerCharacter.Colors);
      packet.WriteString(player.PlayerCharacter.Skin);
      SqlDataProvider.Data.ItemInfo itemAt = player.MainBag.GetItemAt(6);
      packet.WriteInt(itemAt == null ? -1 : itemAt.TemplateID);
      packet.WriteInt(10001);
      packet.WriteInt(player.PlayerCharacter.ConsortiaID);
      packet.WriteString(player.PlayerCharacter.ConsortiaName);
      packet.WriteInt(player.PlayerCharacter.Win);
      packet.WriteInt(player.PlayerCharacter.Total);
      packet.WriteInt(player.PlayerCharacter.Escape);
      packet.WriteInt(player.PlayerCharacter.ConsortiaLevel);
      packet.WriteInt(player.PlayerCharacter.ConsortiaRepute);
      packet.WriteBoolean(player.PlayerCharacter.IsMarried);
      if (player.PlayerCharacter.IsMarried)
      {
        packet.WriteInt(player.PlayerCharacter.SpouseID);
        packet.WriteString(player.PlayerCharacter.SpouseName);
      }
      packet.WriteString(player.PlayerCharacter.UserName);
      packet.WriteInt(player.PlayerCharacter.Nimbus);
      packet.WriteInt(player.PlayerCharacter.FightPower);
      packet.WriteInt(1);
      packet.WriteInt(0);
      packet.WriteString("Master");
      packet.WriteInt(5);
      packet.WriteString("HonorOfMaster");
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 83, player.PlayerId);
      packet.Parameter1 = player.PlayerId;
      packet.WriteInt(4);
      packet.WriteInt(4);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
    {
      GSPacketIn packet = new GSPacketIn((short) 87);
      for (int index = 0; index < states.Length; ++index)
        packet.WriteByte(states[index]);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
    {
      GSPacketIn packet = new GSPacketIn((short) 100);
      for (int index = 0; index < states.Length; ++index)
        packet.WriteInt(states[index]);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 102, player.PlayerId);
      packet.WriteByte((byte) player.CurrentRoomTeam);
      packet.WriteByte((byte) player.CurrentRoomIndex);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomCreate(BaseRoom room)
    {
      GSPacketIn packet = new GSPacketIn((short) 94);
      packet.WriteInt(room.RoomId);
      packet.WriteByte((byte) room.RoomType);
      packet.WriteByte((byte) room.HardLevel);
      packet.WriteByte(room.TimeMode);
      packet.WriteByte((byte) room.PlayerCount);
      packet.WriteByte((byte) room.PlacesCount);
      packet.WriteBoolean(!string.IsNullOrEmpty(room.Password));
      packet.WriteInt(room.MapId);
      packet.WriteBoolean(room.IsPlaying);
      packet.WriteString(room.Name);
      packet.WriteByte((byte) room.GameType);
      packet.WriteInt(room.LevelLimits);
      packet.WriteBoolean(false);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomLoginResult(bool result)
    {
      GSPacketIn packet = new GSPacketIn((short) 81);
      packet.WriteBoolean(result);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomPairUpStart(BaseRoom room)
    {
      GSPacketIn packet = new GSPacketIn((short) 208);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendGameRoomInfo(GamePlayer player, BaseRoom game)
    {
      return new GSPacketIn((short) 94, player.PlayerCharacter.ID);
    }

    public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
    {
      GSPacketIn packet = new GSPacketIn((short) 211);
      packet.WriteByte((byte) game.GameStyle);
      packet.WriteInt((int) game.GameType);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
    {
      GSPacketIn packet = new GSPacketIn((short) 210);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomClear(GamePlayer player, BaseRoom game)
    {
      GSPacketIn packet = new GSPacketIn((short) 96, player.PlayerCharacter.ID);
      packet.WriteInt(game.RoomId);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
    {
      GSPacketIn packet = new GSPacketIn((short) 66, player.PlayerCharacter.ID);
      packet.WriteByte((byte) place);
      packet.WriteInt(goodsID);
      packet.WriteString(style);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRoomChange(BaseRoom room)
    {
      GSPacketIn packet = new GSPacketIn((short) 107);
      packet.WriteInt(room.MapId);
      packet.WriteByte((byte) room.RoomType);
      packet.WriteByte(room.TimeMode);
      packet.WriteByte((byte) room.HardLevel);
      packet.WriteInt(room.LevelLimits);
      packet.WriteBoolean(false);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isbind, int MinValid)
    {
      GSPacketIn packet = new GSPacketIn((short) 76, player.PlayerCharacter.ID);
      packet.WriteInt(previewItemList.Count);
      foreach (KeyValuePair<int, double> previewItem in previewItemList)
      {
        packet.WriteInt(previewItem.Key);
        packet.WriteInt(MinValid);
        int int32 = Convert.ToInt32(previewItem.Value);
        packet.WriteInt(int32 > 100 ? 100 : (int32 < 0 ? 0 : int32));
      }
      packet.WriteBoolean(isbind);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendFusionResult(GamePlayer player, bool result)
    {
      GSPacketIn packet = new GSPacketIn((short) 78, player.PlayerCharacter.ID);
      packet.WriteBoolean(result);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, SqlDataProvider.Data.ItemInfo item)
    {
      GSPacketIn packet = new GSPacketIn((short) 111, player.PlayerCharacter.ID);
      packet.WriteInt(templateid);
      packet.WriteInt(item.ValidDate);
      packet.WriteBoolean(isbind);
      packet.WriteInt(item.AgilityCompose);
      packet.WriteInt(item.AttackCompose);
      packet.WriteInt(item.DefendCompose);
      packet.WriteInt(item.LuckCompose);
      this.SendTCP(packet);
      return packet;
    }

    public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
    {
      if (this.m_gameClient.Player == null)
        return;
      GSPacketIn packet = new GSPacketIn((short) 64, this.m_gameClient.Player.PlayerCharacter.ID, 10240);
      packet.WriteInt(bag.BagType);
      packet.WriteInt(updatedSlots.Length);
      foreach (int updatedSlot in updatedSlots)
      {
        packet.WriteInt(updatedSlot);
        SqlDataProvider.Data.ItemInfo itemAt = bag.GetItemAt(updatedSlot);
        if (itemAt == null)
        {
          packet.WriteBoolean(false);
        }
        else
        {
          packet.WriteBoolean(true);
          packet.WriteInt(itemAt.UserID);
          packet.WriteInt(itemAt.ItemID);
          packet.WriteInt(itemAt.Count);
          packet.WriteInt(itemAt.Place);
          packet.WriteInt(itemAt.TemplateID);
          packet.WriteInt(itemAt.AttackCompose);
          packet.WriteInt(itemAt.DefendCompose);
          packet.WriteInt(itemAt.AgilityCompose);
          packet.WriteInt(itemAt.LuckCompose);
          packet.WriteInt(itemAt.StrengthenLevel);
          packet.WriteBoolean(itemAt.IsBinds);
          packet.WriteBoolean(itemAt.IsJudge);
          packet.WriteDateTime(itemAt.BeginDate);
          packet.WriteInt(itemAt.ValidDate);
          packet.WriteString(itemAt.Color == null ? "" : itemAt.Color);
          packet.WriteString(itemAt.Skin == null ? "" : itemAt.Skin);
          packet.WriteBoolean(itemAt.IsUsed);
          packet.WriteInt(itemAt.Hole1);
          packet.WriteInt(itemAt.Hole2);
          packet.WriteInt(itemAt.Hole3);
          packet.WriteInt(itemAt.Hole4);
          packet.WriteInt(itemAt.Hole5);
          packet.WriteInt(itemAt.Hole6);
          packet.WriteString(itemAt.Template.Pic);
          packet.WriteInt(5);
          packet.WriteDateTime(DateTime.Now.AddDays(5.0));
          packet.WriteInt(5);
          packet.WriteByte((byte) 5);
          packet.WriteInt(5);
          packet.WriteByte((byte) 5);
          packet.WriteInt(5);
        }
      }
      this.SendTCP(packet);
    }

    public GSPacketIn SendFriendRemove(int FriendID)
    {
      GSPacketIn packet = new GSPacketIn((short) 161, FriendID);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendFriendState(int playerID, bool state)
    {
      GSPacketIn packet = new GSPacketIn((short) 165, playerID);
      packet.WriteInt(1);
      packet.WriteBoolean(state);
      packet.WriteInt(playerID);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] infos)
    {
      if (this.m_gameClient.Player == null)
        return (GSPacketIn) null;
      try
      {
        int length = infos.Length;
        int index1 = 0;
        do
        {
          GSPacketIn packet = new GSPacketIn((short) 178, this.m_gameClient.Player.PlayerCharacter.ID);
          int val = length > 7 ? 7 : length;
          packet.WriteInt(val);
          int num = 0;
          while (num < val)
          {
            BaseQuest info = infos[index1];
            if (info.Data.IsExist)
            {
              packet.WriteInt(info.Data.QuestID);
              packet.WriteBoolean(info.Data.IsComplete);
              packet.WriteInt(info.Data.Condition1);
              packet.WriteInt(info.Data.Condition2);
              packet.WriteInt(info.Data.Condition3);
              packet.WriteInt(info.Data.Condition4);
              packet.WriteDateTime(info.Data.CompletedDate);
              packet.WriteInt(info.Data.RepeatFinish);
              packet.WriteInt(info.Data.RandDobule);
              packet.WriteBoolean(info.Data.IsExist);
            }
            ++num;
            ++index1;
          }
          for (int index2 = 0; index2 < states.Length; ++index2)
            packet.WriteByte(states[index2]);
          length -= val;
          this.SendTCP(packet);
        }
        while (index1 < infos.Length);
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex.InnerException);
      }
      return new GSPacketIn((short) 178, this.m_gameClient.Player.PlayerCharacter.ID);
    }

    public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)
    {
      GSPacketIn packet = new GSPacketIn((short) 185, player.PlayerId);
      packet.WriteInt(infos.Length);
      foreach (BufferInfo info in infos)
      {
        packet.WriteInt(info.Type);
        packet.WriteBoolean(info.IsExist);
        packet.WriteDateTime(info.BeginDate);
        packet.WriteInt(info.ValidDate);
        packet.WriteInt(info.Value);
      }
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
    {
      GSPacketIn packet = new GSPacketIn((short) 186, player.PlayerId);
      packet.WriteInt(infos.Count);
      foreach (AbstractBuffer info1 in infos)
      {
        BufferInfo info2 = info1.Info;
        packet.WriteInt(info2.Type);
        packet.WriteBoolean(info2.IsExist);
        packet.WriteDateTime(info2.BeginDate);
        packet.WriteInt(info2.ValidDate);
        packet.WriteInt(info2.Value);
      }
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
    {
      GSPacketIn packet = new GSPacketIn((short) 117);
      packet.WriteInt(playerID);
      packet.WriteInt((int) type);
      GameServer.Instance.LoginServer.SendPacket(packet);
      return packet;
    }

    public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, SqlDataProvider.Data.ItemInfo item)
    {
      GSPacketIn packet = new GSPacketIn((short) 195);
      packet.WriteInt(auctionID);
      packet.WriteBoolean(isExist);
      if (isExist)
      {
        packet.WriteInt(info.AuctioneerID);
        packet.WriteString(info.AuctioneerName);
        packet.WriteDateTime(info.BeginDate);
        packet.WriteInt(info.BuyerID);
        packet.WriteString(info.BuyerName);
        packet.WriteInt(info.ItemID);
        packet.WriteInt(info.Mouthful);
        packet.WriteInt(info.PayType);
        packet.WriteInt(info.Price);
        packet.WriteInt(info.Rise);
        packet.WriteInt(info.ValidDate);
        packet.WriteBoolean(item != null);
        if (item != null)
        {
          packet.WriteInt(item.Count);
          packet.WriteInt(item.TemplateID);
          packet.WriteInt(item.AttackCompose);
          packet.WriteInt(item.DefendCompose);
          packet.WriteInt(item.AgilityCompose);
          packet.WriteInt(item.LuckCompose);
          packet.WriteInt(item.StrengthenLevel);
          packet.WriteBoolean(item.IsBinds);
          packet.WriteBoolean(item.IsJudge);
          packet.WriteDateTime(item.BeginDate);
          packet.WriteInt(item.ValidDate);
          packet.WriteString(item.Color);
          packet.WriteString(item.Skin);
          packet.WriteBoolean(item.IsUsed);
        }
      }
      packet.Compress();
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendAASState(bool result)
    {
      GSPacketIn packet = new GSPacketIn((short) 224);
      packet.WriteBoolean(result);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendIDNumberCheck(bool result)
    {
      GSPacketIn packet = new GSPacketIn((short) 226);
      packet.WriteBoolean(result);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendAASInfoSet(bool result)
    {
      GSPacketIn packet = new GSPacketIn((short) 224);
      packet.WriteBoolean(result);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
    {
      GSPacketIn packet = new GSPacketIn((short) 227);
      packet.WriteBoolean(true);
      packet.WriteInt(1);
      packet.WriteBoolean(true);
      packet.WriteBoolean(IsMinor);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
    {
      GSPacketIn packet = new GSPacketIn((short) 241, player.PlayerCharacter.ID);
      bool val = room != null;
      packet.WriteBoolean(val);
      if (val)
      {
        packet.WriteInt(room.Info.ID);
        packet.WriteBoolean(room.Info.IsHymeneal);
        packet.WriteString(room.Info.Name);
        packet.WriteBoolean(!(room.Info.Pwd == ""));
        packet.WriteInt(room.Info.MapIndex);
        packet.WriteInt(room.Info.AvailTime);
        packet.WriteInt(room.Count);
        packet.WriteInt(room.Info.PlayerID);
        packet.WriteString(room.Info.PlayerName);
        packet.WriteInt(room.Info.GroomID);
        packet.WriteString(room.Info.GroomName);
        packet.WriteInt(room.Info.BrideID);
        packet.WriteString(room.Info.BrideName);
        packet.WriteDateTime(room.Info.BeginTime);
        packet.WriteByte((byte) room.RoomState);
        packet.WriteString(room.Info.RoomIntroduction);
      }
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result)
    {
      GSPacketIn packet = new GSPacketIn((short) 242, player.PlayerCharacter.ID);
      packet.WriteBoolean(result);
      if (result)
      {
        packet.WriteInt(player.CurrentMarryRoom.Info.ID);
        packet.WriteString(player.CurrentMarryRoom.Info.Name);
        packet.WriteInt(player.CurrentMarryRoom.Info.MapIndex);
        packet.WriteInt(player.CurrentMarryRoom.Info.AvailTime);
        packet.WriteInt(player.CurrentMarryRoom.Count);
        packet.WriteInt(player.CurrentMarryRoom.Info.PlayerID);
        packet.WriteString(player.CurrentMarryRoom.Info.PlayerName);
        packet.WriteInt(player.CurrentMarryRoom.Info.GroomID);
        packet.WriteString(player.CurrentMarryRoom.Info.GroomName);
        packet.WriteInt(player.CurrentMarryRoom.Info.BrideID);
        packet.WriteString(player.CurrentMarryRoom.Info.BrideName);
        packet.WriteDateTime(player.CurrentMarryRoom.Info.BeginTime);
        packet.WriteBoolean(player.CurrentMarryRoom.Info.IsHymeneal);
        packet.WriteByte((byte) player.CurrentMarryRoom.RoomState);
        packet.WriteString(player.CurrentMarryRoom.Info.RoomIntroduction);
        packet.WriteBoolean(player.CurrentMarryRoom.Info.GuestInvite);
        packet.WriteInt(player.MarryMap);
        packet.WriteBoolean(player.CurrentMarryRoom.Info.IsGunsaluteUsed);
      }
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 243, player.PlayerCharacter.ID);
      packet.WriteInt(player.PlayerCharacter.Grade);
      packet.WriteInt(player.PlayerCharacter.Hide);
      packet.WriteInt(player.PlayerCharacter.Repute);
      packet.WriteInt(player.PlayerCharacter.ID);
      packet.WriteString(player.PlayerCharacter.NickName);
      packet.WriteBoolean(player.PlayerCharacter.Sex);
      packet.WriteString(player.PlayerCharacter.Style);
      packet.WriteString(player.PlayerCharacter.Colors);
      packet.WriteString(player.PlayerCharacter.Skin);
      packet.WriteInt(player.X);
      packet.WriteInt(player.Y);
      packet.WriteInt(player.PlayerCharacter.FightPower);
      packet.WriteInt(player.PlayerCharacter.Win);
      packet.WriteInt(player.PlayerCharacter.Total);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
    {
      GSPacketIn packet = new GSPacketIn((short) 239);
      packet.WriteInt(ID);
      packet.WriteBoolean(isExist);
      if (isExist)
      {
        packet.WriteInt(info.UserID);
        packet.WriteBoolean(info.IsPublishEquip);
        packet.WriteString(info.Introduction);
      }
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
    {
      GSPacketIn packet = new GSPacketIn((short) 246, player.PlayerCharacter.ID);
      packet.WriteInt(userID);
      packet.WriteBoolean(isMarried);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int id)
    {
      GSPacketIn packet = new GSPacketIn((short) 247, player.PlayerCharacter.ID);
      packet.WriteInt(userID);
      packet.WriteString(userName);
      packet.WriteString(loveProclamation);
      packet.WriteInt(id);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
    {
      GSPacketIn packet = new GSPacketIn((short) 248, player.PlayerCharacter.ID);
      packet.WriteBoolean(result);
      packet.WriteBoolean(isProposer);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int id)
    {
      GSPacketIn packet = new GSPacketIn((short) 250, player.PlayerCharacter.ID);
      packet.WriteInt(UserID);
      packet.WriteBoolean(result);
      packet.WriteString(UserName);
      packet.WriteBoolean(isApplicant);
      packet.WriteInt(id);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendBigSpeakerMsg(GamePlayer player, string msg)
    {
      GSPacketIn packet = new GSPacketIn((short) 72, player.PlayerCharacter.ID);
      packet.WriteInt(player.PlayerCharacter.ID);
      packet.WriteString(player.PlayerCharacter.NickName);
      packet.WriteString(msg);
      GameServer.Instance.LoginServer.SendPacket(packet);
      foreach (GamePlayer allPlayer in WorldMgr.GetAllPlayers())
        allPlayer.Out.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
    {
      GSPacketIn packet = new GSPacketIn((short) 244, player.PlayerCharacter.ID);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info)
    {
      GSPacketIn packet = new GSPacketIn((short) 252, player.PlayerCharacter.ID);
      packet.WriteBoolean(state);
      if (state)
      {
        packet.WriteInt(info.ID);
        packet.WriteString(info.Name);
        packet.WriteInt(info.MapIndex);
        packet.WriteInt(info.AvailTime);
        packet.WriteInt(info.PlayerID);
        packet.WriteInt(info.GroomID);
        packet.WriteInt(info.BrideID);
        packet.WriteDateTime(info.BeginTime);
        packet.WriteBoolean(info.IsGunsaluteUsed);
      }
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
    {
      GSPacketIn packet = new GSPacketIn((short) 235, player.PlayerCharacter.ID);
      packet.WriteString(info.Introduction);
      packet.WriteBoolean(info.IsPublishEquip);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
    {
      GSPacketIn packet = new GSPacketIn((short) 249, player.PlayerCharacter.ID);
      packet.WriteByte((byte) 3);
      packet.WriteInt(info.AvailTime);
      this.SendTCP(packet);
      return packet;
    }

    public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
    {
      GSPacketIn packet = new GSPacketIn((short) 234, player.PlayerCharacter.ID);
      packet.WriteBoolean(info.IsMarried);
      packet.WriteInt(info.SpouseID);
      packet.WriteString(info.SpouseName);
      packet.WriteBoolean(info.IsCreatedMarryRoom);
      packet.WriteInt(info.SelfMarryRoomID);
      packet.WriteBoolean(info.IsGotRing);
      this.SendTCP(packet);
      return packet;
    }
  }
}
