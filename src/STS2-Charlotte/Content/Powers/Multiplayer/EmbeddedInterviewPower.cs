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
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Compat;

namespace CharlotteMod.Content.Powers;

/// <summary>随行采访:持有者下一次消耗卡牌后，施加者抽 Amount 张；虚无消耗延后到回合末冲刷后抽。</summary>
[RegisterPower]
public sealed class EmbeddedInterviewPower : CharlottePowerBase
{
    private class Data
    {
        public bool Claimed;
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
        Player? interviewer = base.Applier?.Player;
        if (data.Claimed || card.Owner != base.Owner.Player || interviewer == null || !interviewer.Creature.IsAlive)
        {
            return;
        }

        data.Claimed = true;
        Flash();
        if (causedByEthereal)
        {
            data.DeferredSource = card;
            return;
        }

        int amount = base.Amount;
        await PowerCmd.Remove(this);
        await CharlotteCardPileCommandShim.DrawForPlayer(choiceContext, amount, interviewer, card);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        Data data = GetInternalData<Data>();
        if (!participants.Contains(base.Owner) || data.DeferredSource == null
            || base.Applier?.Player is not { } interviewer || !interviewer.Creature.IsAlive)
        {
            return;
        }

        int amount = base.Amount;
        CardModel source = data.DeferredSource;
        data.DeferredSource = null;
        await PowerCmd.Remove(this);
        await CharlotteCardPileCommandShim.DrawForPlayer(choiceContext, amount, interviewer, source);
    }
}
