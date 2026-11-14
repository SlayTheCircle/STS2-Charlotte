using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Mechanics;

namespace CharlotteMod.Content.Cards;

/// <summary>此刻长存(稀有,1 费技能,联机限定):留影全部手牌，每张使所有存活玩家获得 4/6 格挡。消耗。</summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class MomentEternal : CharlotteCardBase
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword> { CardKeyword.Exhaust };
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Snapshot);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new BlockVar(4m, ValueProp.Move) };

    public MomentEternal()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> hand = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        foreach (CardModel card in hand)
        {
            await Snapshot.Card(choiceContext, card, base.Owner);
            foreach (Creature teammate in base.CombatState!.GetTeammatesOf(base.Owner.Creature)
                         .Where(c => c.IsAlive && c.Player != null))
            {
                await CreatureCmd.GainBlock(teammate, base.DynamicVars.Block, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);
    }
}
