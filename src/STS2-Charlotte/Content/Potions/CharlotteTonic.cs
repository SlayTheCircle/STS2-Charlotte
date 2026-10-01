using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using CharlotteMod.Content.PotionPools;

namespace CharlotteMod.Content.Potions;

/// <summary>
/// 示例药剂(普通药水):获得 8 点格挡。药水接线样例(Usage/TargetType/OnUse)。
/// </summary>
[RegisterPotion(typeof(CharlottePotionPool))]
public sealed class CharlotteTonic : CharlottePotionBase
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, new BlockVar(8m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move), null);
    }
}
