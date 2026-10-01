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
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 精彩纷呈(普通,0 费攻击):固有。造成 3 点伤害,然后将这张牌[留影]。
/// 自我留影:打出中的牌在 Play 堆,经 ExhaustOnNextPlay 交给引擎路由进消耗堆,
/// 纪念克隆走 Snapshot.Memento(只生成不消耗)。升级:5 伤。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class PhotoOp : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword> { CardKeyword.Innate };
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(3m, ValueProp.Move) };

    public PhotoOp()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        base.ExhaustOnNextPlay = true;
        await Snapshot.Memento(choiceContext, this, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
