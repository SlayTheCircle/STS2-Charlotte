using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using CharlotteMod.Content.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CharlotteMod.Content.Mechanics;

/// <summary>聚焦伤害观察者:能力实现本接口,由 Focus.TriggerAll 在每次聚焦造成伤害后分发(谨遵事实)。</summary>
public interface IFocusDamageObserver
{
    Task OnFocusDamage(PlayerChoiceContext ctx, decimal amount);
}

/// <summary>施加[聚焦]的观察者:能力/遗物实现本接口,由 LensFocusPower.AfterApplied 分发。</summary>
public interface IFocusApplyObserver
{
    Task OnFocusApplied(MegaCrit.Sts2.Core.Entities.Players.Player player);
}

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
            int amount = creature.GetPowerAmount<LensFocusPower>();
            if (amount <= 0 || creature.HasPower<FocusShieldPower>())
            {
                continue;
            }
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), creature, amount,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            // 合作口径(设计师 2026-10-03 A5 裁决):场上任意玩家的聚焦伤害观察者都结算——
            // 此前只通知触发者本人,搭档留影/打出新闻时己方谨遵事实不触发。单人语义不变。
            foreach (Creature observer in combatState.Creatures.Where(c => c.IsAlive && c.Player != null))
            {
                foreach (PowerModel power in observer.Powers)
                {
                    if (power is IFocusDamageObserver o)
                    {
                        await o.OnFocusDamage(new ThrowingPlayerChoiceContext(), amount);
                    }
                }
            }
        }
    }
}
