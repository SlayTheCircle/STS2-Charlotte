using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// [新闻]审判新闻(0 费攻击):选择 1 张手牌[留影]。虚无。
/// 注意:本卡打出自身会结算一次[聚焦](新闻),[留影]效果再结算一次——双触发是设计语义的自然结果。
/// </summary>
[RegisterCard(typeof(CharlotteNewsPool))]
public sealed class CharlotteJudgmentNews : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            HashSet<CardKeyword> set = News.KeywordSet();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    public CharlotteJudgmentNews()
        : base(0, CardType.Attack, CardRarity.Token, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await News.Play(choiceContext, this, () => Snapshot.FromHand(choiceContext, this, base.Owner));
    }
}
