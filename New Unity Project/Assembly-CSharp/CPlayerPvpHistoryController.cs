using System;

[MessageHandlerClass]
public class CPlayerPvpHistoryController : Singleton<CPlayerPvpHistoryController>
{
    public static void CreateSettleData()
    {
        Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GenerateStatData();
        DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetEnumerator();
        while (enumerator.MoveNext())
        {
        }
    }

    public override void Init()
    {
    }

    public override void UnInit()
    {
    }
}

