# TransformHelper
Unityでマップなどの配置をサポートするツール

![image](https://user-images.githubusercontent.com/1855970/126735725-96c0bfab-cda3-43ea-92b8-84f6d9ae928d.png)

# 機能概要

|項目             |説明|
|----             |----|
|Move - Special   |Origin：原点(0,0,0)に移動<br>Approximate：サイズの近似値に移動<br>例：10*5のサイズのオブジェクトがx:9, y:5.5にいるとx:10, y:5に移動する|
|Move - Relative  |数値の値だけ相対的に移動|
|Move - ItemSize  |選択オブジェクトのサイズ幅で移動|
|Rotate - Relative|数値の値だけ相対的に回転|
|Scale            |スケール値を設定(絶対値)|
|Inversion        |反転する(スケールに-1を掛ける)|
|SizeCheck        |選択オブジェクトのサイズを確認|

※「Approximate」「Move-ItemSize」は仕様上、複数オブジェクトに対応していませんので、<br>
複数選択時はボタンが押せないようになっています。<br>
(選択したオブジェクトの配下にあるメッシュオブジェクトを全て取得してサイズを計算しているため)

# インストール方法
![image](https://user-images.githubusercontent.com/1855970/125154251-1180df80-e194-11eb-90d7-a11e31e40c5f.png)<br>
PackageManagerより「Add package from git URL」に以下のURLを入力してください。<br>
`https://github.com/omsb/TransformHelper.git?path=Assets/TransformHelper`

# 使い方
1.上部タブより「OMSB」>「TransformHelper」を選択<br>
![image](https://user-images.githubusercontent.com/1855970/126736166-4cdb8892-3cf4-4c81-a4d7-9bc3e22a15a8.png)<br>
2.Transformを変更したいオブジェクトを選択<br>
3.各ボタンを選択
