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
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 再拍一张(罕见,1 费技能):获得 6 点格挡。选择手牌中的 1 张[留影纪念],将一张复制品放入你的抽牌堆。升级:9 格挡。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class OneMoreShot : CharlotteCardBase
{
    // 卡面提及[留影纪念],挂 Snapshot 供悬停(剪贴相册同款,#63)。
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(6m, ValueProp.Move) };

    public OneMoreShot()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        CardModel? memento = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 0, 1),
            context: choiceContext,
            player: base.Owner,
            filter: Snapshot.IsMemento,
            source: this)).FirstOrDefault();
        if (memento != null)
        {
            // 0.6s:与 Snapshot 收口口径一致(2026-10-02 两轮反馈;此处是当时漏改的调用点)。
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(memento.CreateClone(), PileType.Draw, base.Owner, CardPilePosition.Random),
                0.6f);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}
