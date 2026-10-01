using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace CharlotteMod.Content.Powers;

/// <summary>
/// [聚焦]:叠层负面效果,敌我皆可持有(吸引视线/临场发挥会给自身叠加,转为资源)。
/// 每当[留影]或打出[新闻]牌时,全部持有者失去等同层数的生命(结算收口见 Mechanics/Focus,
/// 伤害走 Unblockable|Unpowered——不可格挡、不吃力量,PoisonPower 同款)。
/// 回合结束时层数 -1(默认理解:持有者一方回合结束衰减,待设计确认,见 local_dev 待确认清单)。
/// </summary>
[RegisterPower]
public sealed class FocusPower : CharlottePowerBase
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }
}
