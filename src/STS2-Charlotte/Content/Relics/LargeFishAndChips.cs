using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using CharlotteMod.Content.RelicPools;

namespace CharlotteMod.Content.Relics;

/// <summary>
/// 大份的炸鱼薯条:拾起时减少 6 点最大生命值并回复全部生命值;每次战斗结束时获得 1 点最大生命值。
/// 枫丹不得不品的美食之一。
/// </summary>
[RegisterRelic(typeof(CharlotteRelicPool))]
public sealed class LargeFishAndChips : CharlotteRelicBase
{
    private const decimal MaxHpLostOnPickup = 6m;

    private const decimal MaxHpPerCombat = 1m;

    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterObtained()
    {
        Flash();
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, MaxHpLostOnPickup, false);
        await CreatureCmd.SetCurrentHp(base.Owner.Creature, base.Owner.Creature.MaxHp);
    }

    public override async Task AfterCombatEnd(CombatRoom _)
    {
        Flash();
        await CreatureCmd.GainMaxHp(base.Owner.Creature, MaxHpPerCombat);
    }
}
