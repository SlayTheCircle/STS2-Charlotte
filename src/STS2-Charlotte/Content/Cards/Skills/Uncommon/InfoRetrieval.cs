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
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 检索信息(罕见,1 费技能):抽 4 张牌。丢弃其中所有的无色牌。升级:抽 6 张。
/// 无色判定:无色池卡(含[新闻])或[留影纪念]。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class InfoRetrieval : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new CardsVar(4) };

    public InfoRetrieval()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    private static bool IsColorlessJunk(CardModel c) => (c.Pool?.IsColorless ?? false) || Snapshot.IsMemento(c);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> drawn = await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        List<CardModel> junk = drawn.Where(IsColorlessJunk).ToList();
        if (junk.Count > 0)
        {
            await CardCmd.Discard(choiceContext, junk);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(2m);
    }
}
