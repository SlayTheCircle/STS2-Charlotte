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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Powers;

/// <summary>忠实记录效果:本回合内,你打出其他攻击牌时,给予记录的目标 N 层虚弱。回合结束移除。</summary>
[RegisterPower]
public sealed class FaithfulRecordPower : CharlottePowerBase
{
    private sealed class Data
    {
        public Creature? target;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void SetTarget(Creature target)
    {
        GetInternalData<Data>().target = target;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || cardPlay.Card.Type != CardType.Attack || cardPlay.Card is Cards.FaithfulRecord)
        {
            return;
        }
        Creature? target = GetInternalData<Data>().target;
        if (target is { IsAlive: true })
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, target, base.Amount, base.Owner, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
