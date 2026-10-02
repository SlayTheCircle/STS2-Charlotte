using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Timeline.Scaffolding;

namespace CharlotteMod.Content.Timeline;

/// <summary>
/// 夏洛蒂的世界线故事(设计:世界线剧情四章)。
/// 必须继承 ModStoryTemplate:屏幕槽位合并/布局注册表整条链只认 RitsuLib 脚手架家族
/// (Navia 2026-09-30 事故:直接继承 StoryModel 导致时间线界面整列不渲染,解锁提示却照常触发)。
/// Id=Slugify(StoryKey)="CHARLOTTE",loc 键 epochs.STORY_CHARLOTTE;
/// 章节顺序=ModStoryEpochBindings 注册顺序(AutoRegister 按类型名扫描,1→4 稳定)。
/// </summary>
[RegisterStory]
public sealed class CharlotteStory : ModStoryTemplate
{
    protected override string StoryKey => "Charlotte";
}
