using Godot;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace CharlotteMod.Content.CardPools;

/// <summary>
/// 夏洛蒂卡池(TypeList 模式):池成员由各卡类上的 [RegisterCard(typeof(CharlotteCardPool))] 特性自动聚合,
/// 本类只负责主题属性。增删卡 = 增删卡类文件,无需改动本文件。
/// </summary>
public sealed class CharlotteCardPool : TypeListCardPoolModel
{
    // 主题色 #8CCEEA(H≈197.9°→h≈.55):vanilla 全部卡框共用一张底图,颜色即 HSV 着色参数
    // (校准:红 h.025 / 橙 h.12 / 绿 h.32 / 蓝 h.55 / 粉 h.965)。
    // 与原版蓝同相位,靠 s/v 拉开辨识度;进游戏目视后微调这三个数。
    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateHsvShaderMaterial(0.55f, 1.0f, 1.15f);

    public override string Title => "charlotte";

    public override string EnergyColorName => "charlotte";

    public override Material? PoolFrameMaterial => _poolFrameMaterial;

    // 能量图标:美术已交付 能量.png,待素材管线(prep-art/PCK)接入后覆写 Big/TextEnergyIconPath。

    public override Color DeckEntryCardColor => new Color("8CCEEA");

    public override Color EnergyOutlineColor => new Color("356D85");

    public override bool IsColorless => false;
}
