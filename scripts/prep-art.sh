#!/usr/bin/env bash
# 从配置的美术母版生成资源；各类派生职责位于 scripts/art/。
# 模板只保留确定性的 convert 包装与母版校验骨架；各类派生脚本（卡图横幅、遗物图标描边、
# 立绘、能量计、事件/纪元图）由衍生仓按 Mod 重写——完整参考实现见源工程 STS2-Navia。
set -euo pipefail
source "$(dirname "$0")/dev-env.sh"
SRC="${ART_SOURCE_DIR:-}"
[[ -n "$SRC" && -d "$SRC" ]] || { echo '错误: 请配置存在的 ART_SOURCE_DIR。' >&2; exit 1; }
source "$MOD_ROOT/scripts/art/common.sh"
source "$MOD_ROOT/scripts/art/mappings.sh"
source "$MOD_ROOT/scripts/art/validate-sources.sh"
echo "母版校验通过（当前映射：卡 ${#CARDS[@]}、遗物 ${#RELICS[@]}、药水 ${#POTIONS[@]}、能力 ${#POWERS[@]}）。"
echo "提示: 模板不含派生脚本——在 scripts/art/ 下按 Mod 实现后接入本文件。"
