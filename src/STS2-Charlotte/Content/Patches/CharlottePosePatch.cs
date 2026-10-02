using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using CharlotteMod.Content.Characters;

namespace CharlotteMod.Content.Patches;

/// <summary>
/// 非 Spine 小人的姿势切换:NCreature.SetAnimationTrigger 对 _spineAnimator 为 null 的
/// Sprite2D 小人完全是 no-op——攻击/施法/受击没有任何形态反馈。
/// 本补丁在夏洛蒂(非 Spine)收到 Attack/Cast/Hit 触发时切换战斗小人 Sprite 的纹理,
/// 短暂展示对应姿势立绘后切回站立图;Dead 不接(倒下为横图且死亡另有表现,直接换图会破版)。
/// 补丁由 ModEntry.Init 的显式 PatchAll 挂载;纹理预加载缓存,热切零 IO。
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
internal static class CharlottePosePatch
{
    private static readonly Dictionary<string, Texture2D?> Poses = new()
    {
        ["Attack"] = Load("charlotte_attack"),
        ["Cast"] = Load("charlotte_skill"),
        ["Hit"] = Load("charlotte_hit"),
    };

    private static readonly Texture2D? Normal = Load("charlotte_normal");

    // 每个小人一个版本号:新触发使旧恢复协程失效,避免连击时旧恢复覆盖新姿势。
    private static readonly Dictionary<NCreature, uint> Versions = new();

    private static Texture2D? Load(string name)
    {
        return GD.Load<Texture2D>($"res://STS2-Charlotte/images/characters/{name}.png");
    }

    // 与 AttackAnimDelay(0.15)+出牌节奏相配的展示时长;Hit 稍长便于看清受击反馈。
    private const int AttackHoldMs = 400;
    private const int HitHoldMs = 550;

    private static async void Postfix(NCreature __instance, string trigger)
    {
        try
        {
            if (trigger != "Attack" && trigger != "Cast" && trigger != "Hit")
            {
                return;
            }
            if (__instance.Entity.Player?.Character is not Charlotte || __instance.HasSpineAnimation)
            {
                return;
            }
            if (!Poses.TryGetValue(trigger, out Texture2D? pose) || pose == null || Normal == null)
            {
                return;
            }
            if (__instance.Visuals.GetNodeOrNull<Sprite2D>("%Visuals") is not { } sprite)
            {
                return;
            }
            Versions.TryGetValue(__instance, out uint version);
            uint mine = ++version;
            Versions[__instance] = mine;
            sprite.Texture = pose;
            await Task.Delay(trigger == "Hit" ? HitHoldMs : AttackHoldMs);
            if (Versions.TryGetValue(__instance, out uint current) && current == mine && GodotObject.IsInstanceValid(sprite))
            {
                sprite.Texture = Normal;
            }
        }
        catch (Exception e)
        {
            Log.Error($"[STS2-Charlotte] 小人姿势切换失败({trigger}): {e}");
        }
    }
}
