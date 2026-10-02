#!/usr/bin/env bash
# 从美术母版(ART_SOURCE_DIR)全量再生成 assets/ 派生素材;CI 的 release 构建入口。
# 派生职责:stage-smoke(卡图/图标/能量) → characters(角色套图) → stories(事件/纪元)。
# 映射表按本 Mod 惯例内嵌在各派生脚本(stage-smoke 的 CARDS/POWERS 等);mappings.sh 保持空表,
# validate-sources.sh 的映射巡检因此空转,缺件硬失败由各派生脚本自行保证(2026-10-03 起为 CI 硬门)。
set -euo pipefail
source "$(dirname "$0")/dev-env.sh"
: "${ART_SOURCE_DIR:?请配置 ART_SOURCE_DIR 指向美术母版(本地=美术素材/,CI=美术仓 checkout)}"
[[ -d "$ART_SOURCE_DIR" ]] || { echo "错误: ART_SOURCE_DIR 不存在: $ART_SOURCE_DIR" >&2; exit 1; }

bash "$MOD_ROOT/scripts/art/stage-smoke.sh"
bash "$MOD_ROOT/scripts/art/characters.sh"
bash "$MOD_ROOT/scripts/art/stories.sh"
echo "prep-art: 全量派生完成(卡图/图标/能量/角色/事件/纪元)。"
