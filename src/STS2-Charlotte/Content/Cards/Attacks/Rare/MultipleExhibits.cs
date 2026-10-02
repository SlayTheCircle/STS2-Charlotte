using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 多重取证(稀有,2 费攻击):造成 4 点伤害 3 次。将此卡的 1 张费用降低 1 点的复制品放入你的弃牌堆。消耗。升级:4 次。
/// 2026-10-03 审阅 #8:补 CardKeyword.Exhaust——设计案(基础+升级参考)均以「消耗。」结尾,
/// 漏挂会让减费复制品无限自我复制;双语卡面同步补写消耗。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class MultipleExhibits : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new HashSet<CardKeyword> { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new CardsVar(3),
    };

    public MultipleExhibits()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitCount(base.DynamicVars.Cards.IntValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        CardModel copy = CreateClone();
        copy.EnergyCost.UpgradeBy(-1);
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Discard, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}
