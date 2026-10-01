using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// [新闻]灾害预警(0 费攻击):获得 6 点格挡。虚无,消耗。
/// </summary>
[RegisterCard(typeof(CharlotteNewsPool))]
public sealed class CharlotteHazardWarning : CharlotteCardBase
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            HashSet<CardKeyword> set = News.KeywordSet();
            set.Add(CardKeyword.Exhaust);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(6m, ValueProp.Move) };

    public CharlotteHazardWarning()
        : base(0, CardType.Attack, CardRarity.Token, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await News.Play(choiceContext, this, async () =>
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        });
    }
}
