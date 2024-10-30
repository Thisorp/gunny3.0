// Decompiled with JetBrains decompiler
// Type: Game.Server.Managers.LevelMgr
// Assembly: Game.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 79A1F911-6A22-40C4-B522-51CE58070C78
// Assembly location: C:\Gunny3.0SV\Config SV\Release\Road\Game.Server.dll

namespace Game.Server.Managers
{
  public class LevelMgr
  {
    private static int[] levels = new int[61]
    {
      0,
      37,
      162,
      505,
      1283,
      2801,
      5462,
      9771,
      16341,
      25899,
      39291,
      57489,
      81594,
      112847,
      152630,
      202472,
      264058,
      339232,
      430003,
      538554,
      667242,
      818609,
      995383,
      1200490,
      1437053,
      1753103,
      2112735,
      2519637,
      2977665,
      3490849,
      4145185,
      4873978,
      5684269,
      6583537,
      7579710,
      8681174,
      9896788,
      11235892,
      12708322,
      14324419,
      16263735,
      18590915,
      21383531,
      24734669,
      28756036,
      33581676,
      39372443,
      46321365,
      54660070,
      63832646,
      73922480,
      85021297,
      97229996,
      110659565,
      125432090,
      140943242,
      157229951,
      174330996,
      192287093,
      211140995,
      int.MaxValue
    };

    public static int GetLevel(int GP)
    {
      for (int index = 0; index < LevelMgr.levels.Length; ++index)
      {
        if (GP < LevelMgr.levels[index])
          return index;
      }
      return 1;
    }

    public static int GetGP(int level)
    {
      if (LevelMgr.levels.Length > level && level > 0)
        return LevelMgr.levels[level - 1];
      return 0;
    }

    public static int ReduceGP(int level, int totalGP)
    {
      if (LevelMgr.levels.Length <= level || level <= 0)
        return 0;
      totalGP -= LevelMgr.levels[level - 1];
      if (totalGP < level * 12)
        return totalGP < 0 ? 0 : totalGP;
      return level * 12;
    }

    public static int IncreaseGP(int level, int totalGP)
    {
      if (LevelMgr.levels.Length > level && level > 0)
        return level * 12;
      return 0;
    }
  }
}
