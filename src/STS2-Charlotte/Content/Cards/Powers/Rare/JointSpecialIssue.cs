using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>同行特刊(稀有,2/1 费能力,联机限定):双方每回合首次消耗分别触发队友抽牌与全体格挡。</summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class JointSpecialIssue : CharlotteCardBase
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new PowerVar<JointSpecialIssuePower>(6m) };

    public JointSpecialIssue()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.AnyAlly)
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

        await PowerCmd.Apply<JointSpecialIssuePower>(choiceContext, cardPlay.Target,
            base.DynamicVars["JointSpecialIssuePower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
