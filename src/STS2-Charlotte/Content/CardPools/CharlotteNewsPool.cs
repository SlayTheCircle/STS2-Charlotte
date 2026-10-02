using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CharlotteMod.Content.CardPools;

/// <summary>
/// [新闻]池:无色视觉族,仅作七张新闻 token 卡的归属(卡框/图集解析用)。
/// Token 稀有度使新闻牌不进任何奖励/商店池,只由夏洛蒂的卡牌生成;
/// 能量配色沿用主池,卡框走无色默认(PoolFrameMaterial 不覆写)。
/// 必须注册为共享池:CardModel.Pool 的懒查找只在 ModelDb.AllCardPools(角色池+共享池)里找归属,
/// 不注册的新闻卡实例一旦被访问 Pool(生成入堆/镜像预览)直接 InvalidProgramException
/// 「is not in any card pool」炸断出牌流程(2026-10-02 占领头版事故)。
/// </summary>
[RegisterSharedCardPool]
public sealed class CharlotteNewsPool : TypeListCardPoolModel
{
    public override string Title => "charlottenews";

    public override string EnergyColorName => "charlotte";

    public override bool IsColorless => true;

    public override Color DeckEntryCardColor => new Color("8CCEEA");
}
