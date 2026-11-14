using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>合影留念(罕见,1 费技能,联机限定):另一名玩家获得 0 费咔嚓；基础附虚无，升级移除虚无。</summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class GroupPhoto : CharlotteCardBase
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<CharlotteKacha>(),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
    };

    public GroupPhoto()
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

        CharlotteKacha kacha = base.CombatState!.CreateCard<CharlotteKacha>(ally);
        kacha.EnergyCost.UpgradeBy(-kacha.EnergyCost.GetWithModifiers(CostModifiers.None));
        if (!IsUpgraded)
        {
            CardCmd.ApplyKeyword(kacha, CardKeyword.Ethereal);
        }
        await CardPileCmd.AddGeneratedCardToCombat(kacha, PileType.Hand, base.Owner);
    }
}
