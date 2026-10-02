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
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 早间特报(普通,0 费技能):固有。从 3 张[新闻]中挑选 1 张加入你的手牌。消耗。
/// 升级:从全部(7 张)[新闻]中挑选 1 张。
/// 选卡器分档:FromChooseACardScreen(三选一大卡屏)硬上限 3 张(>3 直接 ArgumentException,
/// 2026-10-02 升级版 7 张炸出牌事故);升级版走 FromSimpleGrid 网格。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class MorningEdition : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword> { CardKeyword.Innate, CardKeyword.Exhaust };
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    public MorningEdition()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState is not { } combatState)
        {
            return;
        }
        List<CardModel> options = IsUpgraded
            ? News.CreateRandomDistinct(7, base.Owner, combatState)
            : News.CreateRandomDistinct(3, base.Owner, combatState);
        if (options.Count == 0)
        {
            return;
        }
        if (options.Count == 1)
        {
            await CardPileCmd.AddGeneratedCardsToCombat(options, PileType.Hand, base.Owner);
            return;
        }
        CardModel? chosen;
        if (options.Count <= 3)
        {
            chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);
        }
        else
        {
            chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, base.Owner,
                new CardSelectorPrefs(new LocString("card_selection", "STS2_CHARLOTTE_TO_PICK_NEWS"), 1))).FirstOrDefault();
        }
        if (chosen != null)
        {
            await CardPileCmd.AddGeneratedCardsToCombat(new[] { chosen }, PileType.Hand, base.Owner);
        }
    }
}
