# amatib_webgl_unity

## ビルドについて

### PCの場合

#### Player Settings -> Version
- バージョンの末尾に「pc」を付与してビルドしてください。

ex. 1.0.0pc

#### Build Settings -> Texture Compression

- 「DXT」を設定してください。

### SPの場合

#### Player Settings -> Version
- バージョンの末尾に「sp」を付与してビルドしてください。

ex. 1.0.0pc

#### Build Settings -> Texture Compression

- 「ASTC」を設定してください。

## 参考

### Unity WebGL
[Unity WebGL](https://docs.unity3d.com/ja/2022.1/Manual/webgl.html)
### Point Cloud Importer/Renderer for Unity
[keijiro / Pcx](https://github.com/keijiro/Pcx)
### WebGL上で点群を表示するShader/Script
[【Unity】Pcxで読み込んだ点群をWebGL上で表示する](https://qiita.com/Y0241-N/items/8a2cb1cc6600d7936dc8)
### Unity WebGLビルドメモ
[Unity WebGL ビルドメモ](https://framesynthesis.jp/tech/unity/webgl/)
### TextMeshProで日本語フォントを利用する場合、変更する場合はAssetsを変更
[TextMeshProのフォントアセット生成](https://futabazemi.net/unity/textmeshpro_font_asset)
### Addressablesでリソースを更新したらビルドが必要
Addressaable Asset Settings->Manage Groups->Build->New Build->Default Buid Script
[[Editor] Unable to load runtime data at location ...](https://forum.unity.com/threads/editor-unable-to-load-runtime-data-at-location.726560/)
