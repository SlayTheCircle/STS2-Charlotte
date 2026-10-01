using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Keywords;

namespace CharlotteMod.Content.Keywords;

/// <summary>
/// 专有名词悬停解释的接线助手。机制名词先在 ModEntry 注册
/// (RegisterCardKeywordOwnedByLocNamespace),文本唯一权威来源为
/// localization/{lang}/card_keywords.json(键 = 常量 + ".title/.description")。
/// 修改机制实现时必须同步核对解释文本,文案不准就是事故。
/// </summary>
public static class CharlotteKeywords
{
    /// <summary>[留影]:消耗手牌换抽牌堆纪念克隆。效果文本的唯一权威来源是 card_keywords.json。</summary>
    public const string Snapshot = "STS2_CHARLOTTE_KEYWORD_SNAPSHOT";

    /// <summary>[聚焦]:叠层 debuff,留影/新闻结算时全体持有者掉血,回合末 -1。</summary>
    public const string Focus = "STS2_CHARLOTTE_KEYWORD_FOCUS";

    /// <summary>[新闻]:虚无无色 token 族,打出时结算聚焦。</summary>
    public const string News = "STS2_CHARLOTTE_KEYWORD_NEWS";

    /// <summary>把已注册的关键词(若注册表可用)并入卡牌关键词集合。未注册时静默跳过,不抛错。</summary>
    public static void AddTo(HashSet<CardKeyword> set, params string[] ids)
    {
        foreach (string id in ids)
        {
            if (ModKeywordRegistry.TryGetCardKeyword(id, out CardKeyword keyword))
            {
                set.Add(keyword);
            }
        }
    }

    /// <summary>
    /// 把已注册关键词转为悬停提示。遗物/能力/药水/附魔等非卡牌模型的 <c>AdditionalHoverTips</c> 用
    /// (卡牌本体走 CanonicalKeywords,由卡牌悬停管线自动展开;这些模型没有该管线,须显式挂)。
    /// </summary>
    public static IEnumerable<IHoverTip> HoverTips(params string[] ids) => ids.ToHoverTips();
}
