using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using CharlotteMod.Content.Timeline;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.RelicPools;

/// <summary>遗物池(TypeList 模式):成员由 [RegisterRelic(typeof(CharlotteRelicPool))] 特性聚合。</summary>
public sealed class CharlotteRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "charlotte";

    public override Color LabOutlineColor => new Color("8CCEEA");

    // 世界线门控(NaviaRelicPool/IroncladRelicPool 同款):第三章·独家爆料揭示前,章内 3 遗物不进掉落池。
    public override System.Collections.Generic.IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
    {
        var list = base.AllRelics.ToList();
        if (!unlockState.IsEpochRevealed<Charlotte3Epoch>())
        {
            list.RemoveAll(r => Charlotte3Epoch.RelicUnlockTypes.Any(t => ModelDb.GetId(t) == r.Id));
        }
        return list;
    }
}
