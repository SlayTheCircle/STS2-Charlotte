#!/usr/bin/env bash
# 从模板派生新 Mod:一次性完成全部身份改名并自验。
# 用法: scripts/init-mod.sh <mod-id> [PascalName] [--name "<英文显示名>"] [--cn-name "<中文名>"]
#                            [--summary "<中文一句话>"] [--author "<作者>"] [--commit] [--fresh-git]
#   mod-id      形如 STS2-Foo(至少一段连字符;拒绝 STS2-Template)
#   PascalName  类名前缀/命名空间短名,默认取 id 末段;多段 id 建议显式给(如 STS2-Silver-Tongue → SilverTongue)
# 令牌序是负载承载的(见 docs/history/decisions.md):长令牌先消解,通用规则只动词边界。
set -euo pipefail
cd "$(dirname "$0")/.."

MOD_ID="${1:?用法: init-mod.sh <mod-id> [PascalName] [--name ...] [--cn-name ...] [--commit] [--fresh-git]}"
shift || true
SHORT=""; NAME=""; CN=""; SUMMARY=""; AUTHOR=""; DO_COMMIT=0; FRESH_GIT=0
while [[ $# -gt 0 ]]; do
    case "$1" in
        --name) NAME="${2:?--name 需要值}"; shift 2 ;;
        --cn-name) CN="${2:?--cn-name 需要值}"; shift 2 ;;
        --summary) SUMMARY="${2:?--summary 需要值}"; shift 2 ;;
        --author) AUTHOR="${2:?--author 需要值}"; shift 2 ;;
        --commit) DO_COMMIT=1; shift ;;
        --fresh-git) FRESH_GIT=1; shift ;;
        *) SHORT="$1"; shift ;;
    esac
done

# ---- 预检 ----
[[ "$MOD_ID" =~ ^[A-Za-z][A-Za-z0-9]*(-[A-Za-z0-9]+)+$ ]] || { echo "错误: mod-id 形如 STS2-Foo(至少一段连字符): $MOD_ID" >&2; exit 1; }
[[ "$MOD_ID" != "STS2-Template" ]] || { echo '错误: 不能把模板派生成它自己。' >&2; exit 1; }
[[ -f STS2-Template.json ]] || { echo '错误: 不在模板检出根目录(缺 STS2-Template.json)。' >&2; exit 1; }
SHORT="${SHORT:-${MOD_ID##*-}}"
[[ "$SHORT" =~ ^[A-Z][A-Za-z0-9]*$ ]] || { echo "错误: PascalName 形如 Foo(大驼峰): $SHORT" >&2; exit 1; }
# 身份契约:mod id 恰为 STS2-<PascalName>(组织约定)。多段 id(如 STS2-Silver-Tongue)会让
# 清单末段派生的短名与类前缀分叉,审计与导出器随即失配——这类形态在入口拒绝。
[[ "$MOD_ID" == "STS2-$SHORT" ]] || { echo "错误: mod-id 必须恰为 STS2-<PascalName>(得 STS2-$SHORT): $MOD_ID" >&2; exit 1; }
[[ -z "$(git status --porcelain 2>/dev/null)" ]] || { echo '错误: 工作树不干净;先提交或暂存。' >&2; exit 1; }

PREFIX="$(printf '%s' "${MOD_ID//-/_}" | tr '[:lower:]' '[:upper:]')_"
SNAKE="$(printf '%s' "$SHORT" | sed -E 's/([a-z0-9])([A-Z])/\1_\2/g' | tr '[:lower:]' '[:upper:]')"
NS="${SHORT}Mod"
LOWER="$(printf '%s' "$SHORT" | tr '[:upper:]' '[:lower:]')"
CN="${CN:-$SHORT}"; NAME="${NAME:-$SHORT}"
SUMMARY="${SUMMARY:-${CN}角色模组}"
AUTHOR="${AUTHOR:-$(git config user.name 2>/dev/null || echo 3aKHP)}"
echo "派生: STS2-Template → $MOD_ID (Short=$SHORT NS=$NS Prefix=$PREFIX)"

# ---- 1. 谱系记录(先行,避免被通用 sed 改写) ----
python3 - "$MOD_ID" <<'PY'
import json, sys, datetime
from pathlib import Path
p = Path(".template-origin")
d = json.loads(p.read_text(encoding="utf-8"))
d["derived"] = sys.argv[1]
d["derivedAt"] = datetime.date.today().isoformat()
p.write_text(json.dumps(d, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
PY

# ---- 2. 结构改名(git mv 保留历史) ----
git mv STS2-Template.json "$MOD_ID.json"
git mv src/STS2-Template.Loader "src/$MOD_ID.Loader"
git mv "src/$MOD_ID.Loader/STS2-Template.Loader.csproj" "src/$MOD_ID.Loader/$MOD_ID.Loader.csproj"
git mv src/STS2-Template "src/$MOD_ID"
git mv "src/$MOD_ID/STS2-Template.csproj" "src/$MOD_ID/$MOD_ID.csproj"
# 类文件统一前缀改名:审计与生成器按文件名推导类名,文件名必须与类名同步。
while IFS= read -r f; do
    base="$(basename "$f")"; dir="$(dirname "$f")"
    git mv "$f" "$dir/${SHORT}${base#Template}"
done < <(git ls-files | grep -E '(^|/)Template[A-Za-z0-9]*\.cs$')
if [[ -d assets/STS2-Template ]]; then git mv assets/STS2-Template "assets/$MOD_ID"; fi

# ---- 3. 内容 sed(令牌序:长→短;代码文件跑通用词边界规则,Markdown 只动复合令牌防散文误伤) ----
CODE_EXTS='\.(cs|csproj|props|json|yml|yaml|sh|py|gd|tscn|tres|txt|gitignore|example|bbcode|md5s?|orig)$'
mapfile -t files < <(git ls-files)
md_compound=(TemplateCardBase TemplateRelicBase TemplatePotionBase TemplatePowerBase
              TemplateSupportEnchantment TemplateEventBase TemplateKeywords TemplateTraining
              TemplateCardPool TemplateRelicPool TemplatePotionPool TemplateVigorPower
              TemplateVigor TemplateLocket TemplateTonic TemplateStrike TemplateDefend TemplateMod)
for f in "${files[@]}"; do
    [[ -f "$f" ]] || continue
    grep -Il '' "$f" >/dev/null 2>&1 || continue   # 只碰文本文件
    [[ "$f" == ".template-origin" || "$f" == "scripts/init-mod.sh" || "$f" == templates/* ]] && continue   # 不改写自身/谱系/脚手架原样
    sed -i \
        -e "s/STS2-Template-art/${MOD_ID}-art/g" \
        -e "s/STS2-Template\.Loader/${MOD_ID}.Loader/g" \
        -e "s/STS2-Template/${MOD_ID}/g" \
        -e "s/STS2_TEMPLATE_/${PREFIX}/g" \
        -e "s/TEMPLATE_/${SNAKE}_/g" \
        -e "s/_TEMPLATE\./_${SNAKE}./g" \
        -e "s/\.TEMPLATE\./.${SNAKE}./g" \
        -e "s/_template\b/_${LOWER}/g" \
        -e "s/TemplateMod/${NS}/g" \
        "$f"
    if [[ "$f" =~ $CODE_EXTS && "$f" != *.md ]]; then
        sed -i -E "s/\bTemplate/${SHORT}/g" "$f"
    else
        for t in "${md_compound[@]}"; do
            sed -i "s/${t}/${SHORT}${t#Template}/g" "$f"
        done
        sed -i -e "s/\"template\"/\"${LOWER}\"/g" -e "s/template_support\.png/${LOWER}_support.png/g" "$f"
    fi
done

# ---- 4. 结构化清单编辑(sed 之外的语义字段) ----
python3 - "$MOD_ID" "$CN" "$NAME" "$SUMMARY" "$AUTHOR" <<'PY'
import json, sys
from pathlib import Path
mod_id, cn, name, summary, author = sys.argv[1:6]
p = Path(f"{mod_id}.json")
d = json.loads(p.read_text(encoding="utf-8"))
d["name"] = f"{cn} | {name}"
d["author"] = author
d["description"] = f"{summary}。A Slay the Spire 2 mod by SlayTheCircle."
p.write_text(json.dumps(d, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
PY

# ---- 5. README 换衍生视角 + 私有侧脚手架 ----
python3 - "$MOD_ID" "$CN" "$NAME" "$SUMMARY" <<'PY'
import sys
from pathlib import Path
mod_id, cn, name, summary = sys.argv[1:5]
for src, dst in (("templates/readme.zh.md", "README.md"), ("templates/readme.en.md", "README.en.md")):
    text = Path(src).read_text(encoding="utf-8")
    for k, v in {"__MOD_ID__": mod_id, "__CN_NAME__": cn, "__MOD_SUMMARY__": summary,
                 "__MOD_SUMMARY_EN__": name, "__VDF_TITLE__": f"{cn} | {name}"}.items():
        text = text.replace(k, v)
    Path(dst).write_text(text, encoding="utf-8")
PY
mkdir -p local_dev/workshop
sed -e "s/__MOD_ID__/$MOD_ID/g" -e "s/__VDF_TITLE__/${CN} | ${NAME}/g" \
    templates/workshop/publish.sh.example > local_dev/workshop/publish.sh
chmod +x local_dev/workshop/publish.sh
sed -e "s/__MOD_ID__/$MOD_ID/g" templates/workshop/description.bbcode.example > local_dev/workshop/description.bbcode
cp templates/local-dev.env.notes.md local_dev/SCAFFOLD-NOTES.md

# ---- 6. 重新生成卡表 ----
python3 scripts/export-card-table.py

# ---- 7. 自验:残留令牌必须为零 + 源码检查 ----
resid=$(grep -rn -e 'STS2-Template' -e 'STS2_TEMPLATE_' -e 'TEMPLATE_' -e 'TemplateMod' -e 'TemplateCardBase' \
        --include='*.cs' --include='*.csproj' --include='*.json' --include='*.sh' --include='*.py' \
        --include='*.gd' --include='*.yml' --include='*.md' --exclude=init-mod.sh \
        --exclude-dir=templates --exclude-dir=.git . 2>/dev/null \
        | grep -v 'STS2-Template](https://github.com/SlayTheCircle/STS2-Template)' || true)
if [[ -n "$resid" ]]; then
    echo '错误: 残留模板令牌(须为零):' >&2; echo "$resid" >&2; exit 1
fi
./scripts/check.sh --source-only

# ---- 8. 可选编译验证 ----
if [[ -n "${GAME_REFS_DIR:-}" ]] || { [[ -f .local-dev.env ]] && grep -q '^GAME_REFS_DIR=' .local-dev.env; }; then
    refs111="${GAME_REFS_DIR:-$(sed -n 's/^GAME_REFS_DIR=//p' .local-dev.env)}"
    refs107="${refs111%game}game-0.107.1"
    for target in 0.111.0 0.107.1; do
        echo "== 目标 $target =="
        case "$target" in
            0.111.0) refs="$refs111" ;;
            *) refs="$refs107"
               if [[ ! -d "$refs" ]]; then echo "警告: 未找到 $refs,跳过 0.107.1 编译验证。" >&2; continue; fi ;;
        esac
        GAME_REFS_DIR="$refs" RITSULIB_TARGET=$target bash scripts/build.sh --dll-only
    done
else
    echo '警告: 未配置 GAME_REFS_DIR,跳过编译验证——首次构建前务必自跑 build.sh --dll-only。' >&2
fi

# ---- 9. 收尾:自删与提示 ----
git rm -q -f scripts/init-mod.sh templates/readme.zh.md templates/readme.en.md \
    templates/workshop/publish.sh.example templates/workshop/description.bbcode.example \
    templates/local-dev.env.notes.md 2>/dev/null || true
rmdir templates/workshop templates 2>/dev/null || true
if [[ "$FRESH_GIT" == 1 ]]; then
    rm -rf .git && git init -b main -q && git add -A
    echo '已重建 git 历史(--fresh-git)。'
fi
if [[ "$DO_COMMIT" == 1 ]]; then
    git add -A
    git commit -q -m "chore: derive ${MOD_ID} from STS2-Template" -m "One-pass identity rename via init-mod.sh: manifest, source tree, namespaces, base family, localization keys, scripts and workflow references. README switched to the derived perspective; private workshop scaffolding staged under local_dev/."
    echo "已提交: derive ${MOD_ID}"
fi
echo
echo '完成。后续步骤:'
echo '  1. git config core.hookspath .githooks   # 激活提交前审计'
echo '  2. 阅读 docs/dev/onboarding.md(CI 凭据/美术仓/首次发版)'
echo '  3. local_dev/SCAFFOLD-NOTES.md 说明私有侧脚手架'
