using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace CharlotteMod.Content.Powers;

/// <summary>
/// 快门追击(按下快门附加的敌方 debuff):本回合内,持有者每受到 1 次攻击牌的伤害,
/// 额外失去 Amount 点生命(不可格挡)。回合结束移除。
/// </summary>
[RegisterPower]
public sealed class ShutterPursuitPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && target.IsAlive && cardSource != null && cardSource.Type == CardType.Attack && cardSource.Owner.Creature != base.Owner)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
