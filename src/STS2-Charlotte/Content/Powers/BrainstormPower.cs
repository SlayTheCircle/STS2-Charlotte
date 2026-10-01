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

/// <summary>脑洞大开效果:接下来的 Amount 个回合开始时,将存储新闻的 0 费带消耗复制品加入你的手牌(NightmarePower 同型)。</summary>
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
        if (player.Creature != base.Owner)
        {
            return;
        }
        CardModel? news = GetInternalData<Data>().news;
        for (int i = 0; i < base.Amount; i++)
        {
            CardModel copy = news!.CreateClone();
            copy.EnergyCost.UpgradeBy(-copy.EnergyCost.Canonical);
            CardCmd.ApplyKeyword(copy, CardKeyword.Exhaust);
            await CardPileCmd.AddGeneratedCardsToCombat(new[] { copy }, PileType.Hand, player);
        }
        await PowerCmd.Remove(this);
    }
}
