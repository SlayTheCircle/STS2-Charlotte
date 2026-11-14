using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Compat;

namespace CharlotteMod.Content.Powers;

/// <summary>同行特刊:双方每回合首次消耗分别使队友抽 1、所有存活玩家获得 Amount 格挡。</summary>
[RegisterPower]
public sealed class JointSpecialIssuePower : CharlottePowerBase
{
    private class Data
    {
        public bool InterviewerTriggered;
        public bool AllyTriggered;
        public int DeferredDraws;
        public CardModel? DeferredSource;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    protected override object InitInternalData() => new Data();

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        Data data = GetInternalData<Data>();
        if (card.Owner == base.Applier?.Player && !data.InterviewerTriggered)
        {
            data.InterviewerTriggered = true;
            Flash();
            if (base.Owner.IsAlive && base.Owner.Player is { } ally)
            {
                if (causedByEthereal)
                {
                    data.DeferredDraws++;
                    data.DeferredSource = card;
                }
                else
                {
                    await CharlotteCardPileCommandShim.DrawForPlayer(choiceContext, 1, ally, card);
                }
            }
        }

        if (card.Owner == base.Owner.Player && !data.AllyTriggered)
        {
            data.AllyTriggered = true;
            Flash();
            foreach (Creature teammate in base.Owner.CombatState!.GetTeammatesOf(base.Owner)
                         .Where(c => c.IsAlive && c.Player != null))
            {
                await CreatureCmd.GainBlock(teammate, base.Amount, ValueProp.Unpowered, null);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        Data data = GetInternalData<Data>();
        if (!participants.Contains(base.Owner) || data.DeferredDraws == 0 || data.DeferredSource == null)
        {
            return;
        }

        int count = data.DeferredDraws;
        CardModel source = data.DeferredSource;
        data.DeferredDraws = 0;
        data.DeferredSource = null;
        if (base.Owner.IsAlive && base.Owner.Player is { } ally)
        {
            await CharlotteCardPileCommandShim.DrawForPlayer(choiceContext, count, ally, source);
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            Data data = GetInternalData<Data>();
            data.InterviewerTriggered = false;
            data.AllyTriggered = false;
            data.DeferredDraws = 0;
            data.DeferredSource = null;
        }
        return Task.CompletedTask;
    }
}
