#!/usr/bin/env bash
# 冒烟构建的素材铺装:从 local_dev 美术母版铺出 audit-assets 要求的全部图片。
# 正式美术管线(B10)落地前的一次性路径,布局与 Navia scripts/art 同约定:
#   卡图 750×570(先古 606×852) / 图标 256² + 金色描边变体 / news 从 新闻卡/ 取。
set -euo pipefail
cd "$(dirname "$0")/../.."
source scripts/dev-env.sh

SRC="${ART_SOURCE_DIR:?需要 .local-dev.env 的 ART_SOURCE_DIR}"
DST="assets/$MOD_ID/images"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

# ---- 卡图(95 = 88 常规 + 7 新闻;先古两张开竖窗) ----
declare -A CARDS=(
    ['打击']='CharlotteStrike' ['防御']='CharlotteDefend'
    ['咔嚓！']='CharlotteKacha' ['茄子！']='CharlotteSayCheese'
    ['连续抓拍']='RapidSnaps' ['闪光打击']='FlashStrike' ['变焦摄像']='ZoomCamera'
    ['聚焦打击']='FocusStrike' ['早间特报']='MorningEdition' ['撰写稿件']='WriteUp'
    ['正当防卫']='JustifiedDefense' ['羽笔打击']='QuillStrike' ['不攻自破']='SelfDefeating'
    ['按下快门']='PressShutter' ['可靠信源']='ReliableSource' ['精彩纷呈']='PhotoOp'
    ['针砭时弊']='ScathingCritique' ['校对样稿']='ProofreadDraft' ['一针见血']='PiercingPoint'
    ['鞭辟入里']='IncisiveAnalysis' ['抽丝剥茧']='UnravelThread' ['掷地有声']='ResoundingWords'
    ['高危调查']='HighRiskInvestigation' ['笔走龙蛇']='BrushstrokeFlow'
    ['走街串巷']='DoorToDoor' ['核实流程']='FactCheck' ['时效核验']='DeadlineCheck'
    ['吸引视线']='DrawAttention' ['线索收集']='GatherLeads' ['临场发挥']='Improvisation'
    ['占领头版']='ClaimTheHeadline' ['精力充沛']='SecondWind' ['左右逢源']='WellConnected'
    ['有力物证']='SolidEvidence' ['忠实记录']='FaithfulRecord' ['检索信息']='InfoRetrieval'
    ['脑洞大开']='Brainstorm' ['延时摄影']='TimeLapse' ['深入调查']='InDepthInvestigation'
    ['加班加点']='Overtime' ['奋笔疾书']='DeadlineRush' ['背光拍摄']='BacklitShot'
    ['真相大白']='TruthRevealed' ['剪贴相册']='Scrapbook' ['谨遵事实']='Adherence'
    ['时效原则']='Timeliness' ['复制胶卷']='ReplayRoll' ['妙笔生花']='FloweryPen'
    ['广角镜头']='WideAngleLens' ['曝光录像']='ExposureFootage' ['争分夺秒']='RaceTheClock'
    ['最佳记者']='BestJournalist' ['切中要害']='SharpReport' ['多重取证']='MultipleExhibits'
    ['唇枪舌剑']='WarOfWords' ['单刀直入']='SoloThrust' ['摄影技巧']='PhotographySkill'
    ['摄影形态']='PhotographyForm' ['真实至上']='TruthAbove' ['职业素养']='Professionalism'
    ['爆炸新闻']='BreakingNews' ['神来之笔']='Masterstroke' ['辟谣祛魅']='DebunkAndDispel'
    ['三审三校']='TripleCheck' ['洛阳纸贵']='LuoyangPaper' ['追踪调查']='TrackingInvestigation'
    ['按图索骥']='FollowTheClues' ['现场报道']='LiveReport' ['冷色摄影']='CoolToneShot'
    ['实地考察']='SiteSurvey' ['再拍一张']='OneMoreShot' ['核验稿件']='VerifyCopy'
    ['勘正谬误']='Errata' ['暂时规避']='TemporaryEvade' ['文思泉涌']='FlowOfIdeas'
    ['温馨笔触']='WarmBrushstroke' ['紧跟时事']='OnTheBeat' ['野外调查']='FieldInvestigation'
    ['全勤奖金']='PerfectAttendance' ['动作捕捉']='MotionCapture' ['特制镜头']='CustomLens'
    ['跟进调查']='FollowUpInvestigation' ['风雨满楼']='StormBrewing' ['大字通告']='FrontPageNotice'
    ['力透纸背']='PenetratingProse' ['开门见山']='CutToTheChase'
)
declare -A ANCIENT_ART=( ['笑一个！']='Smile' ['看镜头！']='LookAtCamera' )
declare -A NEWS_ART=(
    ['沐芒公告']='CharlotteGleamBulletin' ['寻人启事']='CharlotteMissingPersonNotice'
    ['灾害预警']='CharlotteHazardWarning' ['审判新闻']='CharlotteJudgmentNews'
    ['街头采访']='CharlotteStreetInterview' ['广告宣传']='CharlottePaidPromotion'
    ['独家报道']='CharlotteExclusiveReport'
)

mkdir -p "$DST/cards" "$DST/relics" "$DST/potions" "$DST/powers"
ok=0; miss=0
for zh in "${!CARDS[@]}"; do
    cls="${CARDS[$zh]}"; src="$SRC/卡图/$zh.png"
    if [[ ! -f "$src" ]]; then echo "缺卡图: $zh"; miss=$((miss+1)); continue; fi
    convert "$src" -resize 750x570^ -gravity center -extent 750x570 "$DST/cards/$cls.png"
    ok=$((ok+1))
done
for zh in "${!ANCIENT_ART[@]}"; do
    cls="${ANCIENT_ART[$zh]}"; src="$SRC/卡图/$zh（先古）.png"
    if [[ ! -f "$src" ]]; then echo "缺先古卡图: $zh"; miss=$((miss+1)); continue; fi
    convert "$src" -resize 606x852^ -gravity center -extent 606x852 "$DST/cards/$cls.png"
    ok=$((ok+1))
done
for zh in "${!NEWS_ART[@]}"; do
    cls="${NEWS_ART[$zh]}"; src="$SRC/卡图/新闻卡/$zh.png"
    if [[ ! -f "$src" ]]; then echo "缺新闻卡图: $zh"; miss=$((miss+1)); continue; fi
    convert "$src" -resize 750x570^ -gravity center -extent 750x570 "$DST/cards/$cls.png"
    ok=$((ok+1))
done
echo "卡图: $ok 张就绪(缺 $miss)"

# ---- 遗物/药水图标 256² + 描边 ----
stage_icons() {  # $1=源目录 $2=目标目录 $3..=中文名:类名
    local srcdir="$1" dstdir="$2"; shift 2
    local n=0
    for pair in "$@"; do
        local zh="${pair%%:*}" cls="${pair##*:}"
        local src="$SRC/$srcdir/$zh.png"
        if [[ ! -f "$src" ]]; then echo "缺图标: $srcdir/$zh"; continue; fi
        convert "$src" -resize 256x256 "$DST/$dstdir/$cls.png"
        convert "$DST/$dstdir/$cls.png" -bordercolor none -border 6 -alpha extract \
            -morphology Dilate Disk:5 -gravity center -crop 256x256+0+0 +repage "$TMP/mask.png"
        convert -size 256x256 xc:'#f4cf70' "$TMP/mask.png" -alpha off -compose CopyOpacity \
            -composite "$DST/$dstdir/${cls}_outline.png"
        n=$((n+1))
    done
    echo "$dstdir 图标: $n 套就绪(含描边)"
}
stage_icons 遗物 relics \
    '温亨廷先生:MonsieurVerite' '温亨廷先生——千织屋纪念版:MonsieurVeriteChioriyaEdition' \
    '[特殊分析变焦镜头]:SpecialAnalysisZoomLens' '镜头盖:LensCap' '采访稿记录本:InterviewNotebook' \
    '大份的炸鱼薯条:LargeFishAndChips' '维修工具——温亨廷先生专用版:RepairTools'
stage_icons 药水 potions \
    '香浓热咖啡:HotCoffee' '镜头清洁剂:LensCleaner' '枫丹洋葱汤:FontaineOnionSoup' '提神醒脑茶:RefreshingTea'

# ---- Power 图标 256²(无描边;临时 power 图标与能力卡图标共用母版) ----
declare -A POWERS=(
    ['聚焦']='LensFocusPower' ['下回合聚焦']='DelayedFocusPower' ['跟进调查']='FollowUpPower'
    ['临场发挥']='FocusShieldPower' ['精力充沛']='SecondWindPower' ['左右逢源']='PressMomentumPower'
    ['忠实记录']='FaithfulRecordPower' ['脑洞大开']='BrainstormPower' ['看镜头！']='LookAtCameraPower'
    ['紧跟时事']='TopicalLinkPower' ['文思泉涌']='FlowOfIdeasPower' ['特制镜头']='CustomLensPower'
    ['动作捕捉']='MotionCapturePower' ['职业素养']='ProfessionalismPower' ['温馨笔触']='WarmBrushstrokePower'
    ['风雨满楼']='StormBrewingPower' ['按下快门']='ShutterPursuitPower' ['剪贴相册']='ScrapbookPower'
    ['时效原则']='TimelinessPower' ['复制胶卷']='ReplayRollPower' ['妙笔生花']='FloweryPenPower'
    ['切中要害']='SharpReportPower' ['摄影技巧']='PhotographySkillPower' ['摄影形态']='PhotographyFormPower'
    ['真实至上']='TruthAbovePower' ['三审三校']='TripleCheckPower' ['洛阳纸贵']='LuoyangPaperPower'
    ['最佳记者']='BestJournalistPower' ['镜头清洁剂']='LensCleanerPower' ['职业素养']='ProfessionalismPower'
)
ok=0
for zh in "${!POWERS[@]}"; do
    src="$SRC/图标/$zh-图标.png"
    [[ -f "$src" ]] || src="$SRC/图标/$zh.png"
    if [[ ! -f "$src" ]]; then echo "缺 power 图标: $zh → ${POWERS[$zh]}"; continue; fi
    convert "$src" -resize 256x256 "$DST/powers/${POWERS[$zh]}.png"
    ok=$((ok+1))
done
# 无母版补位:战斗计数器(永不可见)与洋葱汤热气(药水母版复用)
convert "$SRC/图标/聚焦-图标.png" -resize 256x256 "$DST/powers/CombatTrackerPower.png" 2>/dev/null || true
convert "$SRC/药水/枫丹洋葱汤.png" -resize 256x256 "$DST/powers/OnionSoupPower.png"
echo "Power 图标: $ok+2 张就绪"
echo "铺装完成 → $DST"
