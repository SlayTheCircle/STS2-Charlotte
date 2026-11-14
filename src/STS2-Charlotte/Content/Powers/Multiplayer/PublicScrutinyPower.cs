using System.Collections.Generic;
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
using CharlotteMod.Content.Compat;

namespace CharlotteMod.Content.Powers;

/// <summary>众目睽睽:本回合每名其他玩家首次以攻击牌对目标造成未格挡伤害后，该玩家抽 Amount 张。</summary>
[RegisterPower]
public sealed class PublicScrutinyPower : CharlottePowerBase
{
    private class Data
    {
        public readonly HashSet<Player> RewardedPlayers = new();
    }

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override object InitInternalData() => new Data();

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (target != base.Owner || dealer is not { IsAlive: true, Player: { } attacker }
            || attacker == base.Applier?.Player || cardSource?.Type != CardType.Attack
            || result.UnblockedDamage <= 0)
        {
            return;
        }
        if (!GetInternalData<Data>().RewardedPlayers.Add(attacker))
        {
            return;
        }

        Flash();
        await CharlotteCardPileCommandShim.DrawForPlayer(choiceContext, base.Amount, attacker, cardSource);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            await PowerCmd.Remove(this);
        }
    }
}
