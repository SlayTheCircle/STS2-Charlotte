using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.Afflictions;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;
using MegaCrit.Sts2.Core.HoverTips;
using CharlotteMod.Content.Keywords;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 延时摄影(罕见,1 费攻击):造成 12 点伤害,消耗你手牌中的 1 张[留影纪念]。升级:17。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class TimeLapse : CharlotteCardBase
{
    // 卡面提及[留影纪念],挂 Snapshot 词条(本卡自身非消耗,勿加 Exhaust 横幅)。
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get { var set = new HashSet<CardKeyword>(); CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot); return set; }
    }

    // 「消耗」为文案中的动作/牌堆引用(非本卡自身关键词),挂词条悬停而非 CanonicalKeywords 横幅(BurningPact 同款,悬浮扫描 2026-10-03)。
    // [留影纪念]名词直挂纪念标记悬停(设计师 2026-10-03 D3):与纪念牌本体同一份说明。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) }
            .Concat(HoverTipFactory.FromAffliction<SnapshotMemento>());

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(12m, ValueProp.Move) };

    public TimeLapse()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        // 恰好 1 张(设计无「至多」);手牌无纪念时 FromHand 过滤后为空,自然跳过。
        CardModel? memento = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 1),
            context: choiceContext,
            player: base.Owner,
            filter: Snapshot.IsMemento,
            source: this)).FirstOrDefault();
        if (memento != null)
        {
            await CardCmd.Exhaust(choiceContext, memento);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
