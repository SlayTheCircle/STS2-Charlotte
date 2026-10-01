using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using CharlotteMod.Content.Powers;

namespace CharlotteMod.Content.Mechanics;

/// <summary>
/// [聚焦]结算引擎:[留影]与打出[新闻]两个触发口的共同出口。
/// 全部持有[聚焦]的单位(敌我双方,含夏洛蒂自身)各失去等同层数的生命。
/// 单一收口:后续「聚焦造成伤害时」的联动(谨遵事实/切中要害等)在此挂接。
/// </summary>
public static class Focus
{
    public static async Task TriggerAll(ICombatState combatState)
    {
        foreach (Creature creature in combatState.Creatures)
        {
            if (!creature.IsAlive)
            {
                continue;
            }
            int amount = creature.GetPowerAmount<FocusPower>();
            if (amount <= 0)
            {
                continue;
            }
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), creature, amount,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        }
    }
}
