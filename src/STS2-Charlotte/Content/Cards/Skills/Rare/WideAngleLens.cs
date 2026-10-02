using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 广角镜头(稀有,2 费技能):虚无。[留影]你所有的手牌。这些牌的费用降低 1。消耗。升级:费用 1。
/// 「这些牌」= 生成的[留影纪念](待确认项#4 默认理解):纪念克隆费用比原牌少 1。
/// 世界线第二章解锁件。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class WideAngleLens : CharlotteCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword> { CardKeyword.Ethereal, CardKeyword.Exhaust };
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    public WideAngleLens()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 「这些牌」= 本次留影生成的纪念:以抽牌堆既有纪念为基线,只对新增纪念减费,
        // 不波及此前回合/其他来源的旧纪念(2026-10-03 审阅 #20)。
        var before = new HashSet<CardModel>(CardPile.GetCards(base.Owner, PileType.Draw).Where(Snapshot.IsMemento));
        List<CardModel> hand = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        foreach (CardModel victim in hand)
        {
            await Snapshot.Card(choiceContext, victim, base.Owner);
        }
        // 广角折扣:本次生成的纪念克隆(抽牌堆新入的纪念)费用再 -1。
        foreach (CardModel memento in CardPile.GetCards(base.Owner, PileType.Draw)
                     .Where(Snapshot.IsMemento)
                     .Where(m => !before.Contains(m)))
        {
            if (memento.EnergyCost.GetResolved() > 0)
            {
                memento.EnergyCost.UpgradeBy(-1);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
