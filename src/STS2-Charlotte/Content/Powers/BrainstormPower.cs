using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Powers;

/// <summary>脑洞大开效果:接下来的 Amount 个回合开始时,每回合将 1 张存储新闻的 0 费带消耗复制品加入你的手牌。
/// 2026-10-03 审阅 #10:原实现一次爆发 Amount 张,与卡面「接下来的 N 个回合开始时」(每回合 1 张)不符,
/// 改为逐回合发放 1 张、层数随发递减、发完移除(新克隆无堆,仍走 AddGeneratedCardsToCombat)。</summary>
[RegisterPower]
public sealed class BrainstormPower : CharlottePowerBase
{
    private sealed class Data
    {
        public CardModel? news;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void SetNews(CardModel news)
    {
        // 存克隆而非原牌(NightmarePower 同型),原牌被消耗也不影响后续回合发放。
        CardModel clone = news.CreateClone();
        CardCmd.ClearAffliction(clone);
        GetInternalData<Data>().news = clone;
    }

    public override async Task BeforeHandDraw(MegaCrit.Sts2.Core.Entities.Players.Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Creature != base.Owner || GetInternalData<Data>().news is not { } news)
        {
            return;
        }
        CardModel copy = news.CreateClone();
        copy.EnergyCost.UpgradeBy(-copy.EnergyCost.Canonical);
        CardCmd.ApplyKeyword(copy, CardKeyword.Exhaust);
        await CardPileCmd.AddGeneratedCardsToCombat(new[] { copy }, PileType.Hand, player);
        await PowerCmd.Decrement(this);
        if (base.Amount <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}
