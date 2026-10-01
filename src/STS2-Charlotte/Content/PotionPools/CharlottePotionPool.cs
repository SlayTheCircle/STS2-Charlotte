using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.PotionPools;

/// <summary>药水池(TypeList 模式):成员由 [RegisterPotion(typeof(CharlottePotionPool))] 特性聚合。</summary>
public sealed class CharlottePotionPool : TypeListPotionPoolModel
{
    public override string EnergyColorName => "template";

    public override Color LabOutlineColor => new Color("E8B23A");
}
