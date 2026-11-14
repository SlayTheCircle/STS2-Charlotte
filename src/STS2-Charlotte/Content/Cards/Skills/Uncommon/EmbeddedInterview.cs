using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>随行采访(罕见,1 费技能,联机限定):队友获得 8/12 格挡；其下次消耗卡牌后，你抽 2。</summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class EmbeddedInterview : CharlotteCardBase
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override bool GainsBlock => true;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(8m, ValueProp.Move),
        new PowerVar<EmbeddedInterviewPower>(2m),
    };

    public EmbeddedInterview()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        Player? ally = cardPlay.Target.Player;
        if (ally == null || ally == base.Owner || !cardPlay.Target.IsAlive)
        {
            return;
        }

        await CreatureCmd.GainBlock(cardPlay.Target, base.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<EmbeddedInterviewPower>(choiceContext, cardPlay.Target,
            base.DynamicVars["EmbeddedInterviewPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}
