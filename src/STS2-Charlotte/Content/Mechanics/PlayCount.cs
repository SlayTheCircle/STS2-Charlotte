using System;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace CharlotteMod.Content.Mechanics;

/// <summary>
/// 战斗计数查询(引擎 CombatHistory 口径):本回合/本场打出的牌数、消耗数。
/// 必须读 History 条目而非私有计数器:History.Changed 直连 CombatStateTracker,
/// 卡牌打出即时刷新手牌面板;私有 Data 在 AfterCardPlayed 里自增不触发任何通知,
/// 面板永远慢一拍(2026-10-02 Mirror 反馈「伤害不能正常提升」的根因)。
/// 读取时点语义:CardPlayFinished 在 OnPlay 之后、AfterCardPlayed 之前记录,
/// OnPlay 内读取恰为「其他牌」数——不含结算中的自身,重播各记一条(与逐次
/// AfterCardPlayed 分发的旧计数等值)。非战斗一律返回 0。
/// </summary>
public static class PlayCount
{
    /// <summary>本回合打出的攻击牌数(结算中的自身不计)。</summary>
    public static int AttacksThisTurn(Player player) => PlaysThisTurn(player, static c => c.Type == CardType.Attack);

    /// <summary>本回合打出的牌数(结算中的自身不计)。</summary>
    public static int CardsThisTurn(Player player) => PlaysThisTurn(player, static _ => true);

    /// <summary>本场战斗累计打出的攻击牌数。</summary>
    public static int AttacksThisCombat(Player player) => PlaysThisCombat(player, static c => c.Type == CardType.Attack);

    private static int PlaysThisTurn(Player player, Func<CardModel, bool> filter)
    {
        if (player.Creature.CombatState is not { } state || !CombatManager.Instance.IsInProgress)
        {
            return 0;
        }
        return CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.Actor == player.Creature && e.HappenedThisTurn(state) && filter(e.CardPlay.Card));
    }

    private static int PlaysThisCombat(Player player, Func<CardModel, bool> filter)
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return 0;
        }
        return CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.Actor == player.Creature && filter(e.CardPlay.Card));
    }

    /// <summary>本回合消耗的牌数。</summary>
    public static int ExhaustsThisTurn(Player player)
    {
        if (player.Creature.CombatState is not { } state || !CombatManager.Instance.IsInProgress)
        {
            return 0;
        }
        return CombatManager.Instance.History.Entries.OfType<CardExhaustedEntry>()
            .Count(e => e.Actor == player.Creature && e.HappenedThisTurn(state));
    }

    /// <summary>本场战斗累计消耗的牌数(追踪调查的费用折扣基准)。</summary>
    public static int ExhaustsThisCombat(Player player)
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return 0;
        }
        return CombatManager.Instance.History.Entries.OfType<CardExhaustedEntry>()
            .Count(e => e.Actor == player.Creature);
    }
}
