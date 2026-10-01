using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CharlotteMod.Content.CardPools;
using CharlotteMod.Content.Keywords;

namespace CharlotteMod.Content.Mechanics;

/// <summary>
/// [新闻]家族接线:七张虚无无色 token 卡(注册于 CharlotteNewsPool,Token 稀有度,不进奖励/商店池)。
/// 不设共享基类——花名册审计按 Content/Cards 下非 Base 前缀类文件计数,多一个基类会被误计为卡;
/// 统一从 <see cref="KeywordSet"/> 取关键词、从 <see cref="Play"/> 走「先效果、后[聚焦]结算」的固定顺序。
/// 留影纪念克隆复用原牌的同一 OnPlay,新闻克隆打出同样触发聚焦。
/// </summary>
public static class News
{
    /// <summary>[新闻]族判据:注册于 CharlotteNewsPool 的卡(留影纪念克隆与原牌同池同型,同样命中)。</summary>
    public static bool IsNews(CardModel card) => card.Pool is CharlotteNewsPool;

    /// <summary>新闻牌关键词集(虚无 + [新闻]横幅);调用方在其上追加消耗/[留影]/[聚焦]等后返回。</summary>
    public static HashSet<CardKeyword> KeywordSet()
    {
        var set = new HashSet<CardKeyword> { CardKeyword.Ethereal };
        CharlotteKeywords.AddTo(set, CharlotteKeywords.News);
        return set;
    }

    /// <summary>新闻牌打出统一入口:先执行卡牌效果,再结算[聚焦]。</summary>
    public static async Task Play(PlayerChoiceContext ctx, CardModel card, Func<Task> effect)
    {
        await effect();
        if (card.CombatState is { } combatState)
        {
            await Focus.TriggerAll(combatState);
        }
    }
}
