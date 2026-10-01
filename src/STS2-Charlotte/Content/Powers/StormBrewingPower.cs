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

/// <summary>风雨满楼效果:每回合你第一次失去生命时,将 Amount 张随机[新闻]添加到你的手牌。</summary>
[RegisterPower]
public sealed class StormBrewingPower : CharlottePowerBase
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

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || _triggeredThisTurn || result.UnblockedDamage <= 0)
        {
            return;
        }
        _triggeredThisTurn = true;
        for (int i = 0; i < base.Amount; i++)
        {
            if (base.Owner.Player is { } player && player.Creature.CombatState is { } combatState)
            {
                CardModel news = News.CreateRandom(player, combatState);
                await CardPileCmd.AddGeneratedCardsToCombat(new[] { news }, PileType.Hand, player);
            }
        }
    }
}
