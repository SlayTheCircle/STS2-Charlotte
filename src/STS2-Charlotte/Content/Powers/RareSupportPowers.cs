using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
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

/// <summary>剪贴相册效果:当你打出[留影纪念]时,抽 1 张牌。</summary>
[RegisterPower]
public sealed class ScrapbookPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner && Snapshot.IsMemento(cardPlay.Card))
        {
            await CardPileCmd.Draw(choiceContext, base.Amount, base.Owner.Player);
        }
    }
}

/// <summary>谨遵事实效果:每当场上的[聚焦]造成伤害时,你获得与那个伤害值相同的格挡。</summary>
[RegisterPower]
public sealed class AdherencePower : CharlottePowerBase, IFocusDamageObserver
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnFocusDamage(PlayerChoiceContext ctx, decimal amount)
    {
        if (amount > 0m)
        {
            await CreatureCmd.GainBlock(base.Owner, amount, ValueProp.Unpowered, null);
        }
    }
}

/// <summary>时效原则效果:每当你的[新闻]进入消耗牌堆,对全体敌人造成 Amount 点伤害。</summary>
[RegisterPower]
public sealed class TimelinessPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != base.Owner || !News.IsNews(card) || base.Owner.CombatState is not { } combatState)
        {
            return;
        }
        await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, base.Amount, ValueProp.Unpowered, base.Owner);
    }
}

/// <summary>复制胶卷效果:每回合你打出的第一张[留影纪念]将会打出 2 次(EchoForm 同型重放钩子)。</summary>
[RegisterPower]
public sealed class ReplayRollPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int FirstInSeriesPlaysThisTurn(Func<CardModel, bool> filter)
    {
        return CombatManager.Instance.History.CardPlaysStarted.Count(e =>
            e.Actor == base.Owner && e.CardPlay.IsFirstInSeries && e.HappenedThisTurn(base.CombatState) && filter(e.CardPlay.Card));
    }

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner || !Snapshot.IsMemento(card))
        {
            return playCount;
        }
        return FirstInSeriesPlaysThisTurn(Snapshot.IsMemento) >= base.Amount ? playCount : playCount + 1;
    }
}

/// <summary>摄影技巧效果:每回合你打出的第一张攻击牌会被打出 2 次(EchoForm 同型,攻击限定;待确认项#2 默认理解)。</summary>
[RegisterPower]
public sealed class PhotographySkillPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private static bool IsAttack(CardModel card) => card.Type == CardType.Attack;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner || !IsAttack(card))
        {
            return playCount;
        }
        int played = CombatManager.Instance.History.CardPlaysStarted.Count(e =>
            e.Actor == base.Owner && e.CardPlay.IsFirstInSeries && e.HappenedThisTurn(base.CombatState) && IsAttack(e.CardPlay.Card));
        return played >= base.Amount ? playCount : playCount + 1;
    }
}

/// <summary>妙笔生花效果:每个回合开始时,将一张随机的[新闻]牌添加到你的手牌。</summary>
[RegisterPower]
public sealed class FloweryPenPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner))
        {
            return;
        }
        for (int i = 0; i < base.Amount; i++)
        {
            if (base.Owner.Player is { } player && player.Creature.CombatState is { } cs)
            {
                CardModel news = News.CreateRandom(player, cs);
                await CardPileCmd.AddGeneratedCardsToCombat(new[] { news }, PileType.Hand, player);
            }
        }
    }
}

/// <summary>切中要害效果:每当你给予[聚焦],对全体敌人造成 Amount 点伤害。</summary>
[RegisterPower]
public sealed class SharpReportPower : CharlottePowerBase, IFocusApplyObserver
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnFocusApplied(Player player)
    {
        if (player.Creature != base.Owner || base.Owner.CombatState is not { } combatState)
        {
            return;
        }
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), combatState.HittableEnemies, base.Amount, ValueProp.Unpowered, base.Owner);
    }
}

/// <summary>摄影形态效果:你每消耗 1 张牌,对全体敌人造成 Amount 点伤害。</summary>
[RegisterPower]
public sealed class PhotographyFormPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != base.Owner || base.Owner.CombatState is not { } combatState)
        {
            return;
        }
        await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, base.Amount, ValueProp.Unpowered, base.Owner);
    }
}

/// <summary>真实至上效果:你每打出 1 张牌,获得 Amount 点格挡。</summary>
[RegisterPower]
public sealed class TruthAbovePower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner)
        {
            await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);
        }
    }
}

/// <summary>职业素养效果:每个回合开始时,获得 Amount 点能量。</summary>
[RegisterPower]
public sealed class ProfessionalismPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player && participants.Contains(base.Owner))
        {
            await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
        }
    }
}

/// <summary>最佳记者效果:战斗结束时,获得 Amount 点最大生命值(局内持久)。</summary>
[RegisterPower]
public sealed class BestJournalistPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom _)
    {
        await CreatureCmd.GainMaxHp(base.Owner, base.Amount);
    }
}

/// <summary>洛阳纸贵效果:每回合你第一次打出[新闻]时,将这张[新闻]的 1 张复制品加入你的手牌。</summary>
[RegisterPower]
public sealed class LuoyangPaperPower : CharlottePowerBase, INewsObserver
{
    private bool _triggeredThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            _triggeredThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public async Task OnNewsPlayed(PlayerChoiceContext ctx, CardModel news, Player player)
    {
        if (player.Creature != base.Owner || _triggeredThisTurn)
        {
            return;
        }
        _triggeredThisTurn = true;
        for (int i = 0; i < base.Amount; i++)
        {
            await CardPileCmd.AddGeneratedCardsToCombat(new[] { news.CreateClone() }, PileType.Hand, player);
        }
    }
}

/// <summary>三审三校效果:每回合开始时,从 3 张[新闻]中选择 1 张加入你的手牌。</summary>
[RegisterPower]
public sealed class TripleCheckPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner) || base.Owner.Player is not { } player)
        {
            return;
        }
        List<CardModel> options = News.CreateRandomDistinct(3, player, combatState);
        if (options.Count == 0)
        {
            return;
        }
        if (options.Count == 1)
        {
            await CardPileCmd.AddGeneratedCardsToCombat(options, PileType.Hand, player);
            return;
        }
        CardModel? chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, player);
        if (chosen != null)
        {
            await CardPileCmd.AddGeneratedCardsToCombat(new[] { chosen }, PileType.Hand, player);
        }
    }
}
