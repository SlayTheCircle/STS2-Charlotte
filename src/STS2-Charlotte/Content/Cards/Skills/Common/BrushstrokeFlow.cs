using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Cards;

/// <summary>
/// 笔走龙蛇(普通,1 费技能):获得 9 点格挡。下个回合开始时,你的格挡不会消失
/// (Prolong 同型:把当前格挡记入 BlockNextTurnPower,等效不消失)。升级:13 格挡。
/// </summary>
[RegisterCard(typeof(CharlotteCardPool))]
public sealed class BrushstrokeFlow : CharlotteCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(9m, ValueProp.Move) };

    public BrushstrokeFlow()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (base.Owner.Creature.Block > 0)
        {
            await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, base.Owner.Creature, base.Owner.Creature.Block, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}
