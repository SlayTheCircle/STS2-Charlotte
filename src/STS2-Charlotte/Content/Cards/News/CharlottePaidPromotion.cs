using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// [新闻]广告宣传(0 费技能):获得 1 点能量,然后丢弃 1 张牌(Acrobatics 同款选弃)。虚无。
/// </summary>
[RegisterCard(typeof(CharlotteNewsPool))]
public sealed class CharlottePaidPromotion : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => News.KeywordSet();

    public CharlottePaidPromotion()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await News.Play(choiceContext, this, async () =>
        {
            await PlayerCmd.GainEnergy(1m, base.Owner);
            CardModel? discarded = (await CardSelectCmd.FromHandForDiscard(
                choiceContext, base.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
            if (discarded != null)
            {
                await CardCmd.Discard(choiceContext, discarded);
            }
        });
    }
}
