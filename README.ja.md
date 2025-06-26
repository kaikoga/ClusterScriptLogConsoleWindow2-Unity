# ClusterScript Log Console Window 2

## これは何ですか？

- ClusterScript のログウィンドウです

### 基本機能

- cluster アプリから収集したログを表示
- cluster アプリから収集したログを検索・絞り込んで表示

### なんかすごい機能

cluster アプリで入ってるワールドのシーンを Unity で開きながら使うと大変便利です

- ログを出したアイテムを開いているシーン内から検索
  - シングルクリックでシーン上のアイテムをハイライト、ダブルクリックでソースコードにジャンプ
  - 名寄せはアイテムの Id や名前で行います
- アイテムリストウィンドウ
  - ログに表示されたことのあるアイテムは `ClusterScript Item Inspector` に表示されます
  - シングルクリックでシーン上のアイテムをハイライト、ダブルクリックで絞り込めます

### よくわからない機能

- SourceMap 対応
  - 例えば、 PSMerger が出力した SourceMap を認識して、合成前のソースコードにジャンプ
  - コード中の位置はログメッセージから推測されます
  - 名寄せは以下の方法で行われます
    - `*.js` ファイルの横に `*.js.map` があれば、それ
    - なければ、 `{ファイルのGUID}.map`
- 拡張スクリプトログ対応
  - 処理系が対応している場合、コード中の位置とスタック情報をログに含めることができます
  - cluster は拡張スクリプトログに非対応ですが、 cluster のスクリプト API 互換の処理系から出力したログを読み込むことができます
  - 拡張スクリプトログのフォーマットは `Silksprite.ClusterScriptLogConsoleWindow2.Format.OutputScriptableIteLogExt` を見てください
- エディタプレビューログ対応
  - エディタプレビューの際、 Unity プロジェクトの `Logs/EditorPreviewLog.log` に出力されるログを表示します
  - Cluster Creator Kit はエディタプレビューログに非対応ですが、 cluster のスクリプト API 互換の処理系から出力したログを読み込むことができます

## インストール方法

```json
{
  "dependencies": {
    "net.kaikoga.cslcw2": "https://github.com/kaikoga/PSMerger-Unity.git"
  }
}
```

## 使用方法

ClusterScript Log Console Window の代わりに使ってください
