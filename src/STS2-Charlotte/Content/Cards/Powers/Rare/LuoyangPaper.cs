using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 洛阳纸贵(稀有,2 费能力):每回合你第一次打出[gold]新闻[/gold]时，将这张[gold]新闻[/gold]的1张复制品加入你的手牌。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class LuoyangPaper : CharlotteCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<LuoyangPaperPower>(1m) };

    public LuoyangPaper()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }


    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LuoyangPaperPower>(choiceContext, base.Owner.Creature, base.DynamicVars["LuoyangPaperPower"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }

}
