using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;
using MegaCrit.Sts2.Core.HoverTips;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 神来之笔(稀有,0 费攻击):消耗你抽牌堆中所有的[新闻]。每消耗 1 张,就对随机敌人造成 4 点伤害 1 次。升级:6 点。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class Masterstroke : CharlotteCardBase
{
    // 「消耗」为文案中的动作/牌堆引用(非本卡自身关键词),挂词条悬停而非 CanonicalKeywords 横幅(BurningPact 同款,悬浮扫描 2026-10-03)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(4m, ValueProp.Move) };

    public Masterstroke()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> news = CardPile.GetCards(base.Owner, PileType.Draw).Where(News.IsNews).ToList();
        foreach (CardModel card in news)
        {
            await CardCmd.Exhaust(choiceContext, card);
            Creature? target = base.CombatState?.HittableEnemies?.Count > 0
                ? base.Owner.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies)
                : null;
            if (target != null)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
