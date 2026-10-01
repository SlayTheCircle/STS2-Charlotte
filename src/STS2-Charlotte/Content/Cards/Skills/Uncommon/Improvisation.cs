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
using CharlotteMod.Content.Keywords;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 临场发挥(罕见,1 费技能):给予自身 2 层[聚焦],获得 10 点格挡。本回合内,你的生命值不会因为[聚焦]而减少。升级:14 格挡。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class Improvisation : CharlotteCardBase
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            var set = new HashSet<CardKeyword>();
            CharlotteKeywords.AddTo(set, CharlotteKeywords.Focus);
            return set;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<LensFocusPower>(2m),
        new BlockVar(10m, ValueProp.Move),
    };

    public Improvisation()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LensFocusPower>(choiceContext, base.Owner.Creature, base.DynamicVars["LensFocusPower"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<FocusShieldPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}
