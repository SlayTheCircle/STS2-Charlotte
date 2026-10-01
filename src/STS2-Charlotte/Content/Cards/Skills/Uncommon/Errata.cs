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
/// 勘正谬误(罕见,1 费技能):丢弃至多 2 张[新闻],每丢弃 1 张就获得 6 点格挡。升级:9 点。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class Errata : CharlotteCardBase
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2),
        new BlockVar(6m, ValueProp.Move),
    };

    public Errata()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prompt = new LocString("cards", base.Id.Entry + ".selectionScreenPrompt");
        IEnumerable<CardModel> chosen = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(prompt, 0, base.DynamicVars.Cards.IntValue),
            context: choiceContext,
            player: base.Owner,
            filter: News.IsNews,
            source: this);
        List<CardModel> discarded = chosen.ToList();
        if (discarded.Count > 0)
        {
            await CardCmd.Discard(choiceContext, discarded);
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue * discarded.Count, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}
