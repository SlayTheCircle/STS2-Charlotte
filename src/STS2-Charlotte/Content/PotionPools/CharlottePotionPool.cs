using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using CharlotteMod.Content.Timeline;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.PotionPools;

/// <summary>药水池(TypeList 模式):成员由 [RegisterPotion(typeof(CharlottePotionPool))] 特性聚合。</summary>
public sealed class CharlottePotionPool : TypeListPotionPoolModel
{
    public override string EnergyColorName => "charlotte";

    public override Color LabOutlineColor => new Color("8CCEEA");

    // 世界线门控(NaviaPotionPool/RegentPotionPool 同款):第四章·独家专访揭示前,章内 3 药水不进奖励池。
    public override System.Collections.Generic.IEnumerable<PotionModel> GetUnlockedPotions(UnlockState unlockState)
    {
        var list = base.AllPotions.ToList();
        if (!unlockState.IsEpochRevealed<Charlotte4Epoch>())
        {
            list.RemoveAll(p => Charlotte4Epoch.PotionUnlockTypes.Any(t => ModelDb.GetId(t) == p.Id));
        }
        return list;
    }
}
