using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.RelicPools;

/// <summary>遗物池(TypeList 模式):成员由 [RegisterRelic(typeof(CharlotteRelicPool))] 特性聚合。</summary>
public sealed class CharlotteRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "template";

    public override Color LabOutlineColor => new Color("E8B23A");
}
