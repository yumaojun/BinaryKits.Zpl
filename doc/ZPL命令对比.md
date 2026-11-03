# 请对比一下目录下两个项目：BinaryKits.Zpl和ZPL-extension，分别支持的ZPL命令数目和情况。ZPL 命令支持对比：BinaryKits.Zpl vs ZPL-extension

> 生成日期：2026-10-20
> 分析对象：`D:\Projects\BinaryKits.Zpl`、`D:\Projects\ZPL-extension`

## 1. 项目性质差异

| | BinaryKits.Zpl | ZPL-extension |
|---|---|---|
| 语言/形态 | C# / .NET 类库 | TypeScript / VS Code 扩展 |
| 用途 | **程序化生成** ZPL（对象→ZPL 文本）+ 解析器 | **编辑/检查/预览** ZPL（语法高亮、校验、悬停、补全、Canvas 预览） |
| 解析器 | 有（`Protocol` 项目，22 个命令） | 有（`parser.ts`，覆盖更广） |
| 预览 | Labelary 在线预览 / Viewer 网页 | 内置 Canvas 渲染 + bwip-js 条码 |
| 打印 | 无（走 Labelary） | 无（编辑工具） |

一句话：BinaryKits 是"**往外写** ZPL 的生成库"，ZPL-extension 是"**往里读** ZPL 的编辑器"。

## 2. 命令覆盖数量对比

### ZPL-extension（三层覆盖，逐层递减）

| 层级 | 数量 | 说明 |
|---|---|---|
| 语法高亮 | 全量（`\^[A-Z][A-Z0-9]?`、`~[A-Z][A-Z0-9]?` 通配） | 任何 ^XX/~XX 都高亮 |
| 命令规格表 `commandSpecs.ts` | **211 个**（`^` 164 + `~` 47） | 用于识别/校验/悬停/补全 |
| 类型化解析（parser.ts 专属 AST） | ~45 个 | 结构化解析的命令 |
| 实际渲染预览 | ~30 条码 + 若干图形/文本 | 真正画出图 |

### BinaryKits.Zpl

| 层级 | 数量 |
|---|---|
| 生成器元素（`Label/Elements`） | 47 个 .cs 文件 |
| 可生成的条码类型 | 14 种（+ ^BY 默认参数） |
| 生成器发出的命令 | ~38 个 |
| 解析器（`Protocol/Commands`） | 22 个命令 |

## 3. 总览

| category | 数量 | BinaryKits 覆盖 |
|---|---|---|
| setup 打印机设置 | 55 | 1（^LH） |
| comm 通信/控制 | 43 | 0 |
| barcode 条码 | 31 | 14 |
| rfid RFID | 23 | 0 |
| field 字段 | 19 | 10 |
| memory 存储/文件 | 14 | 3（~DG ~DY ^XG） |
| graphic 图形 | 10 | 7 |
| misc 其他 | 7 | 0 |
| font 字体 | 6 | 3 |
| format 格式 | 2 | 2 |
| barcode-defaults | 1 | 1 |
| **合计** | **211** | **41** |

> 标记说明：✅ = BinaryKits 生成器会输出该命令；❌ = 不会；⚠️ = 仅解析器有/是空壳。

## 4. 按 category 完整清单

### 4.1 format 格式（2）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^XA | 开始标签格式 | ✅ |
| ^XZ | 结束标签格式 | ✅ |

### 4.2 field 字段（19）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^FB | 字段块（自动换行） | ✅ |
| ^FD | 字段数据 | ✅ |
| ^FH | 十六进制转义指示 | ✅ |
| ^FM | 多字段原点 | ❌ |
| ^FN | 字段编号（变量槽） | ✅ |
| ^FO | 字段原点（左上） | ✅ |
| ^FP | 字段参数 | ❌ |
| ^FQ | 字段数量 | ❌ |
| ^FR | 反向打印 | ✅ |
| ^FS | 字段分隔 | ✅ |
| ^FT | 字段排版（基线） | ✅ |
| ^FV | 字段变量值 | ❌ |
| ^FW | 默认字段方向 | ✅ |
| ^FX | 注释 | ✅ |
| ^FY | 字段类型标识 | ❌ |
| ^SF | 序列化字段 | ❌ |
| ^SN | 序列化数据 | ❌ |
| ^TB | 文本块 | ✅ |
| ^TO | 文本排版原点 | ❌ |

### 4.3 font 字体（6）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^A | 可缩放/点阵字体 | ✅ |
| ^CF | 默认字体（改字体） | ⚠️ 仅解析器有 |
| ^CI | 国际字符集 | ✅ |
| ^CO | 缓存开关 | ❌ |
| ^CW | 字体标识符重映射 | ✅ |
| ^DU | 下载 Unicode 字体 | ❌ |

### 4.4 graphic 图形（10）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^GB | 方框/线 | ✅ |
| ^GC | 圆 | ✅ |
| ^GD | 斜线 | ✅ |
| ^GE | 椭圆 | ✅ |
| ^GF | 图形字段（位图） | ✅ |
| ^GS | 图形符号 | ✅ |
| ^ID | 图像删除 | ❌ |
| ^IL | 图像装载 | ❌ |
| ^IM | 图像移动 | ✅ |
| ^IS | 图像保存 | ❌ |

### 4.5 barcode-defaults（1）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^BY | 条码默认参数 | ✅ |

### 4.6 barcode 条码（31）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^B0 | Aztec | ❌（BinaryKits 的 Aztec 用 ^BO 非标准拼写） |
| ^B1 | Code 11 | ❌ |
| ^B2 | Interleaved 2 of 5 | ✅ |
| ^B3 | Code 39 | ✅ |
| ^B4 | Code 49 | ❌ |
| ^B5 | Planet Code (USPS) | ❌ |
| ^B7 | PDF417 | ✅ |
| ^B8 | EAN-8 | ❌ |
| ^B9 | UPC-E | ✅ |
| ^BA | Code 93 | ✅ |
| ^BB | CODABLOCK | ❌ |
| ^BC | Code 128 | ✅ |
| ^BD | MaxiCode | ✅ |
| ^BE | EAN-13 | ✅ |
| ^BF | MicroPDF417 | ❌ |
| ^BI | ISBT-128 | ❌ |
| ^BJ | Standard 2 of 5 | ❌ |
| ^BK | ANSI Codabar | ✅ |
| ^BL | LOGMARS | ❌ |
| ^BM | MSI Plessey | ❌ |
| ^BN | Aztec（别名） | ❌ |
| ^BO | Aztec（别名） | ✅（BinaryKits 输出此拼写） |
| ^BP | Plessey | ❌ |
| ^BQ | QR Code | ✅ |
| ^BR | GS1 DataBar | ❌ |
| ^BS | UPC/EAN 扩展 | ✅ |
| ^BT | TLC39 | ❌ |
| ^BU | UPC-A | ✅ |
| ^BV | Korean Postal | ❌ |
| ^BX | Data Matrix | ✅ |
| ^BZ | POSTNET | ❌ |

### 4.7 setup 打印机设置（55）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^LH | 标签原点 | ✅ |
| ^PW | 打印宽度 | ❌ |
| ^LL | 标签长度 | ❌ |
| ^LR | 标签反向打印 | ❌ |
| ^LS | 标签移位 | ❌ |
| ^LT | 标签顶部 | ❌ |
| ^PR | 打印速率 | ❌ |
| ^PM | 镜像打印 | ❌ |
| ^PO | 打印方向 | ❌ |
| ^PQ | 打印份数 | ❌ |
| ^PN | 立即送出 | ❌ |
| ^PP | 可编程暂停 | ❌ |
| ^PS | 打印启动 | ❌ |
| ^MD | 打印浓度 | ❌ |
| ^MT | 介质类型 | ❌ |
| ^MM | 打印模式 | ❌ |
| ^MN | 介质跟踪 | ❌ |
| ^MU | 计量单位 | ❌ |
| ^MW | 打印头预警 | ❌ |
| ^MC | 清除位图 | ❌ |
| ^MF | 介质走纸 | ❌ |
| ^MI | 介质初始化 | ❌ |
| ^MP | 模式保护 | ❌ |
| ^MQ | 模式查询 | ❌ |
| ^MS | 介质感应 | ❌ |
| ^JM | 每毫米点数 | ❌ |
| ^JU | 配置更新 | ❌ |
| ^JS | 传感器选择 | ❌ |
| ^JZ | 出错重打 | ❌ |
| ^JK | 电池清零 | ❌ |
| ^JA | 取消所有待处理命令 | ❌ |
| ^JB | 初始化 Flash | ❌ |
| ^JC | 介质传感器校准 | ❌ |
| ^JD | 开启通讯诊断 | ❌ |
| ^JE | 关闭通讯诊断 | ❌ |
| ^JF | 电池状态设置 | ❌ |
| ^JG | 传感器曲线校准 | ❌ |
| ^JH | 预警设置 | ❌ |
| ^JI | ZBI 启动 | ❌ |
| ^JL | 标签长度(边缘) | ❌ |
| ^JN | 打印头致命测试 | ❌ |
| ^JO | 打印头非致命测试 | ❌ |
| ^JP | 暂停并取消 | ❌ |
| ^JR | 上电复位 | ❌ |
| ^JT | 打印头测试间隔 | ❌ |
| ^JW | 色带张力 | ❌ |
| ^JX | 取消部分输入 | ❌ |
| ^JY | 网络打印端口 | ❌ |
| ^LF | 列出字体链接 | ❌ |
| ^LN | 列出网络状态 | ❌ |
| ^SL | 模式与语言 | ❌ |
| ^SO | 偏移设置 | ❌ |
| ^SR | 打印头阻值 | ❌ |
| ^SS | 打印机设置 | ❌ |
| ^ST | 日期时间(RTC) | ❌ |

### 4.8 comm 通信/控制（43）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^CC | 改 ^ 前缀 | ❌ |
| ^CD | 改分隔符 | ❌ |
| ^CT | 改 ~ 前缀 | ❌ |
| ^SC | 串口设置 | ❌ |
| ~CC | 改 ^ 前缀 | ❌ |
| ~CD | 改分隔符 | ❌ |
| ~CT | 改 ~ 前缀 | ❌ |
| ~SD | 浓度设置 | ❌ |
| ~TA | 撕纸位置 | ❌ |
| ~JS | 介质传感器 | ❌ |
| ~JR | 上电复位 | ❌ |
| ~JC | 传感器校准 | ❌ |
| ~JL | 标签长度 | ❌ |
| ~HS | 主机状态请求 | ❌ |
| ~HI | 主机标识 | ❌ |
| ~HM | 主机 RAM 状态 | ❌ |
| ~HQ | 主机查询 | ❌ |
| ~PH | 回位 | ❌ |
| ~PL | 送出标签 | ❌ |
| ~PR | 打印速率 | ❌ |
| ~PS | 打印启动 | ❌ |
| ~PP | 可编程暂停 | ❌ |
| ~RO | 高级计数器复位 | ❌ |
| ~HB | 电池状态 | ❌ |
| ~HD | 主机目录 | ❌ |
| ~HU | 主机上传 | ❌ |
| ~KB | 电池清零 | ❌ |
| ~NC | 网络连接 | ❌ |
| ~NR | 队列服务器 | ❌ |
| ~NT | SMTP 服务器 | ❌ |
| ~WC | 无线配置标签 | ❌ |
| ~AB | 中止批量打印 | ❌ |
| ~AC | 取消队列格式 | ❌ |
| ~JA ~JD ~JE ~JF ~JG ~JI ~JN ~JO ~JP ~JX | 任务状态类 | ❌ |

### 4.9 memory 存储/文件（14）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^DF | 下载格式 | ❌ |
| ^XF | 召回格式 | ⚠️ 空壳（抛异常） |
| ^DG | 下载图形（^ 前缀） | ❌（BinaryKits 用 ~DG） |
| ^XG | 召回图形 | ✅ |
| ^DC | 下载 CRC | ❌ |
| ^EF | 擦除文件 | ❌ |
| ^EG | 擦除图形 | ❌ |
| ~DG | 下载图形 | ✅ |
| ~DY | 下载对象 | ✅ |
| ~DB | 下载点阵字体 | ❌ |
| ~DS | 下载 Intellifont | ❌ |
| ~DU | 下载 TrueType 字体 | ❌ |
| ~DN | 中止下载 | ❌ |
| ~EG | 擦除所有可下载对象 | ❌ |

### 4.10 rfid RFID（23）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^RA | RFID 读访问密码 | ❌ |
| ^RB | 定义 RFID 位 | ❌ |
| ^RC | RFID 改锁定标记位 | ❌ |
| ^RE | RFID 错误报告 | ❌ |
| ^RF | RFID 读/写字段 | ❌ |
| ^RI | RFID 标签 ID 命令 | ❌ |
| ^RL | RFID 锁定命令 | ❌ |
| ^RM | RFID 使能 | ❌ |
| ^RN | RFID 检测多个 | ❌ |
| ^RP | RFID 写密码 | ❌ |
| ^RQ | RFID 快速写 | ❌ |
| ^RR | RFID 重试 | ❌ |
| ^RS | RFID 配置 | ❌ |
| ^RT | RFID 标签读 | ❌ |
| ^RU | RFID 上传 | ❌ |
| ^RV | RFID 校验 | ❌ |
| ^RW | RFID 读/写模式 | ❌ |
| ^RZ | RFID 置零写 | ❌ |
| ^HL | 主机链接标签列表 | ❌ |
| ^HV | 主机校验（RFID/打印机响应） | ❌ |
| ^WD | 在标签上写数据 | ❌ |
| ^WT | 写 RFID 标签 | ❌ |
| ~RV | 返回 RFID 数据 | ❌ |

### 4.11 misc 其他（7）

| 命令 | 含义 | BinaryKits |
|---|---|---|
| ^XB | 抑制回退 | ❌ |
| ^SP | 局部走纸启动 | ❌ |
| ^KD | KDU: 定义 | ❌ |
| ^KL | KDU: 语言 | ❌ |
| ^KN | KDU: 定义打印机名 | ❌ |
| ^KP | KDU: 定义密码 | ❌ |
| ^KV | KDU: 校验 | ❌ |

## 5. 结论

1. **数量上 ZPL-extension 明显更大**：211 个已知命令规格（164 `^` + 47 `~`）＋ 30 种条码渲染，覆盖 ZPL II 规范的大部分；BinaryKits 生成器只覆盖 ~38 个命令 / 14 种条码，解析器 22 个命令。

2. **方向相反、不可直接替代**：
   - 想**用代码生成** ZPL → 用 BinaryKits（ZPL-extension 没有生成 API，只是编辑器）。
   - 想**编辑/校验/预览**手写的 ZPL → 用 ZPL-extension（BinaryKits 的 Protocol 解析器覆盖面远不如它）。

3. **ZPL-extension 的强项**：全谱条码（经 bwip-js）、前后缀重定义（^CC/^CT/^CD）、^DF/^XF/^GF 等二进制大字段的特殊处理、~ 前缀控制命令、RFID/设置/通信长尾命令。

4. **BinaryKits 的强项**：真正的面向对象生成 API（`ZplText`、`ZplBarcode`、`ZplQrCode` 等）、图像压缩（ACS/Z64/B64）、Labelary 集成——这是"从对象到 ZPL"的能力，ZPL-extension 不具备。

5. **几个已知问题**：
   - BinaryKits 的 Aztec 用 `^BO`（非标准拼写，标准应为 `^B0`）。
   - BinaryKits 的 `^XF`（召回格式）是空壳，`Render()` 直接抛异常。
   - BinaryKits 生成器无 `^CF`（仅 Protocol 解析器有）。
