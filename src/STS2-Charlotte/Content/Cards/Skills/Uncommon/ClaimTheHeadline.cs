using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 占领头版(罕见,1 费技能):在你的抽牌堆里生成随机 3 张[新闻],并随机抽取其中一张。升级:5 张。
/// 「随机抽取其中一张」:从生成的几张中抽 1(抽牌堆随机位放置后抽——这里直接把选中张置顶抽取,其余随机位)。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class ClaimTheHeadline : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new CardsVar(3) };

    public ClaimTheHeadline()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState is not { } combatState)
        {
            return;
        }
        List<CardModel> generated = News.CreateRandomDistinct(base.DynamicVars.Cards.IntValue, base.Owner, combatState);
        if (generated.Count == 0)
        {
            return;
        }
        CardModel drawn = base.Owner.RunState.Rng.CombatCardGeneration.NextItem(generated);
        List<CardModel> rest = generated.Where(c => c != drawn).ToList();
        if (rest.Count > 0)
        {
            await CardPileCmd.AddGeneratedCardsToCombat(rest, PileType.Draw, base.Owner, CardPilePosition.Random);
        }
        await CardPileCmd.AddGeneratedCardsToCombat(new[] { drawn }, PileType.Draw, base.Owner, CardPilePosition.Top);
        await CardPileCmd.Draw(choiceContext, 1m, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(2m);
    }
}
