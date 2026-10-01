# 新仓上手（从模板派生）

从本模板派生一个新 Mod 的完整清单。逐步执行，每步的验证命令都应通过再进下一步。本页覆盖**基建**（仓库、凭据、环境）；从设计稿开始铺内容的推荐路径见[内容开发 SOP](content-sop.md)。

## 1. 建仓与派生

```bash
gh repo create SlayTheCircle/<新ModId> --template SlayTheCircle/STS2-Charlotte --public
git clone git@github.com:SlayTheCircle/<新ModId>.git && cd <新ModId>
git config core.hookspath .githooks
scripts/init-mod.sh <新ModId> <PascalName> --cn-name "<中文名>" [--name "<英文名>"]
```

`init-mod.sh` 完成全部身份改名并自验；详见脚本头部注释。完成后本仓库的 `README` 换为衍生仓视角、`.template-origin` 记录模板版本。

## 2. CI 凭据（需要维护者的 GitHub 权限）

1. **circle-refs PAT**：编辑 fine-grained PAT（`sts2-mods-ci-refs-read`）的 Repository access，把新仓加入 Only select repositories（连同 circle-refs 与美术仓）。PAT 值存于源工程 `local_dev/REFS-TOKEN.token`。
2. **REFS_TOKEN secret**：在新仓 Settings → Secrets and variables → Actions 添加 `REFS_TOKEN` = PAT 值。此后 Compile check（双目标编译）与 Release 工作流可用；此前只有无 secret 的源码检查会绿。

## 3. 私有侧

1. 美术仓：创建私有仓 `SlayTheCircle/<新ModId>-art`（结构参考源工程：`卡图/`、`遗物/`、`立绘/`、`发布素材/` 等），克隆到本机后配置 `.local-dev.env` 的 `ART_SOURCE_DIR`。
2. `local_dev/`：init 已拷入 `workshop/publish.sh`（工坊发布）与 `description.bbcode` 骨架；补 `local_dev/README.md` 私有导航。整个目录不入库。
3. 工坊封面 `cover.jpg` 放 `local_dev/workshop/`（≥512²，建议 1024² JPEG）。

## 4. 本机环境

```bash
cp .local-dev.env.example .local-dev.env   # 填 GAME_DIR/RITSULIB_DIR 等
./scripts/restore-refs.sh                  # 从游戏安装复制编译引用
./scripts/check.sh --source-only           # 应绿
GAME_REFS_DIR=libs/game ./scripts/build.sh --dll-only
```

RitsuLib 完整依赖包（含 `compat/`、`shared/`、`RitsuLib.References.props`）放 `libs/RitsuLib/`，或从组织共享位置符号链接。

## 5. 首次发版

1. CHANGELOG 整理 Unreleased → `## [x.y.z]` 段（**段缺失或为空会中止 Release 构建**）；清单 version 同步。
2. 提交、打 tag `vx.y.z`、推送 → Release 工作流产出候选 ZIP 并建**草稿** Release。
3. 下载草稿工件覆盖安装到游戏 `mods/`（**先关游戏；备份移出 mods/，游戏递归扫描会加载备份目录**）试玩验收。
4. 验收通过：GitHub 草稿转正；工坊 `local_dev/workshop/publish.sh <Steam账号> <tag>` 首发（自动捕获物品 ID 回填 `publishedfileid.txt`，之后的更新走同 ID）。
5. Steam 登录：首次需密码 + 手机令牌五位码（`--code` 模式静默读入）；成功一次后凭据缓存免密。VDF 描述不解析 `\n` 转义——描述单源 `description.bbcode` 用真实换行。

## 6. 模板回流

模板修了通用 bug（Loader、审计、管线）时：对照 `.template-origin` 记录的模板版本，把修复按 cherry-pick / 手工同步进衍生仓，并在衍生仓提交信息注明来源模板版本。反向的新教训同样提给模板仓。
