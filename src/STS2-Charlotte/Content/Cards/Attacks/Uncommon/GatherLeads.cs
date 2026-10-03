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
using MegaCrit.Sts2.Core.HoverTips;
using CharlotteMod.Content.Afflictions;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;
using CharlotteMod.Content.Keywords;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 线索收集(罕见,1 费攻击):造成 4 点伤害。选择手牌中 1 张[留影纪念],这张牌可以免费打出。消耗。升级:7 伤。
/// 「免费」=该纪念实例本场战斗费用归零(能量费用钩子按实例状态判定)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class GatherLeads : CharlotteCardBase
{
    // 卡面提及[留影纪念],补 Snapshot 词条悬停(悬浮扫描 2026-10-03)。
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get { var set = new HashSet<CardKeyword> { CardKeyword.Exhaust }; CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot); return set; }
    }

    // 卡面提及[留影纪念]名词,直挂纪念标记悬停(设计师 2026-10-03 D3),与 Snapshot 词条并存。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        HoverTipFactory.FromAffliction<SnapshotMemento>();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(4m, ValueProp.Move) };

    public GatherLeads()
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
        CardModel? memento = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 1),
            context: choiceContext,
            player: base.Owner,
            filter: Snapshot.IsMemento,
            source: this)).FirstOrDefault();
        if (memento != null)
        {
            memento.EnergyCost.UpgradeBy(-memento.EnergyCost.GetResolved());
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
