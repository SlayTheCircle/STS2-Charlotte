using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.CardPools;

/// <summary>
/// [新闻]池:无色视觉族,仅作七张新闻 token 卡的归属(卡框/图集解析用)。
/// Token 稀有度使新闻牌不进任何奖励/商店池,只由夏洛蒂的卡牌生成;
/// 能量配色沿用主池,卡框走无色默认(PoolFrameMaterial 不覆写)。
/// </summary>
public sealed class CharlotteNewsPool : TypeListCardPoolModel
{
    public override string Title => "charlottenews";

    public override string EnergyColorName => "charlotte";

    public override bool IsColorless => true;

    public override Color DeckEntryCardColor => new Color("8CCEEA");
}
