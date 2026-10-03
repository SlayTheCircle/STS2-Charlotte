using System;
using System.Collections.Generic;
using System.Linq;
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
/// 连续抓拍(普通,2 费攻击):造成 13 点伤害,那之后为随机 1 张手牌[留影]。升级:17。
/// 结算序=先伤后影(设计师 2026-10-03 C1 复批推翻 0.1.1 代决:两个动作分立、文案改序,
/// 「那之后」成句,避免与结算纠缠)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class RapidSnaps : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(13m, ValueProp.Move) };

    public RapidSnaps()
        : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 先伤后影(设计师 C1 复批):留影结算在后,敌人死于伤害也不影响(留影只动手牌)。
        List<CardModel> hand = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        if (hand.Count > 0)
        {
            CardModel victim = base.Owner.RunState.Rng.CombatCardSelection.NextItem(hand);
            await Snapshot.Card(choiceContext, victim, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
