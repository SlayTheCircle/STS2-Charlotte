using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.HoverTips;
using CharlotteMod.Content.Afflictions;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 背光拍摄(罕见,2 费技能):选择你手牌中的 1 张牌。如果是[留影纪念],则将 2 张 0 费复制品加入你的手牌;
/// 如果不是,则[留影]这张牌,然后抽 3 张牌。升级:费用 1。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class BacklitShot : CharlotteCardBase
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

    // 卡面提及[留影纪念]名词与[留影]动作各一条:动作走 Snapshot 词条(上),名词挂纪念标记(设计师 2026-10-03 D3)。
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        HoverTipFactory.FromAffliction<SnapshotMemento>();

    public BacklitShot()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        CardModel? chosen = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 1),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this)).FirstOrDefault();
        if (chosen == null)
        {
            return;
        }
        if (Snapshot.IsMemento(chosen))
        {
            for (int i = 0; i < 2; i++)
            {
                CardModel copy = chosen.CreateClone();
                copy.EnergyCost.UpgradeBy(-copy.EnergyCost.Canonical);
                await CardPileCmd.AddGeneratedCardsToCombat(new[] { copy }, PileType.Hand, base.Owner);
            }
        }
        else
        {
            await Snapshot.Card(choiceContext, chosen, base.Owner);
            await CardPileCmd.Draw(choiceContext, 3m, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
