using UnityEditor;
using UnityEngine;

namespace OMSB
{
    /// <summary>
    /// 配置ツール
    /// </summary>
    public class TransformHelper : EditorWindow
    {
        //=======================================================================================================
        //. 定数
        //=======================================================================================================
        #region -- 定数

        const string TOOL_NAME = "TransformHelper";

        #endregion

        //=======================================================================================================
        //. enum
        //=======================================================================================================
        #region -- enum

        /// <summary>
        /// 方向タイプ定義
        /// </summary>
        private enum ETypeDirection
        {
            pX,
            mX,
            pY,
            mY,
            pZ,
            mZ,
        }

        #endregion

        //=======================================================================================================
        //. メンバ
        //=======================================================================================================
        #region -- フィールド

        /// <summary>
        /// スクロールの位置
        /// </summary>
        private Vector2 m_ScrollPos = Vector2.zero;

        /// <summary>
        /// 相対移動値
        /// </summary>
        private float m_ChangePos = 0f;

        /// <summary>
        /// 相対回転値
        /// </summary>
        private float m_ChangeRot = 0f;

        /// <summary>
        /// サイズチェックのログ
        /// </summary>
        private string m_SizeCheckLog = "Select \"SizeCheck\"";
        /// <summary>
        /// サイズチェックのログタイプ
        /// </summary>
        private MessageType m_SizeCheckMesType = MessageType.Info;

        #endregion

        //=======================================================================================================
        //. イベント
        //=======================================================================================================
        #region -- イベント

        /// <summary>
        /// メニューのウィンドウに追加
        /// </summary>
        [MenuItem("OMSB/" + TOOL_NAME)]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<TransformHelper>(TOOL_NAME);
        }

        //=======================================================================================================
        //. UI
        //=======================================================================================================
        #region -- UI

        #endregion

        /// <summary>
        /// メイン描画処理
        /// </summary>
        void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUI.skin.scrollView);
            {
                if (EditorGUIEx.DrawGroup("Tools Action"))
                {
                    UIPositionEx();
                    EditorGUILayout.Space();
                    UIAddPosition();
                    EditorGUILayout.Space();
                    UIAddPositionParts();
                    EditorGUILayout.Space();
                    UIAddRotation();
                    EditorGUILayout.Space();
                    UISetScale();
                    EditorGUILayout.Space();
                    UIReversal();

                    EditorGUILayout.Space();
                }

                if (EditorGUIEx.DrawGroup("Sub Tools"))
                {
                    if (GUILayout.Button("SizeCheck"))
                    {
                        var size = GetGameObjSize();
                        if (float.IsNaN(size.x))
                        {
                            m_SizeCheckLog = "Select the mesh or the mesh's parent object.";
                            m_SizeCheckMesType = MessageType.Warning;
                        }
                        else
                        {
                            m_SizeCheckLog = Selection.activeGameObject.name + " : " + size.ToString();
                            m_SizeCheckMesType = MessageType.Info;
                        }
                    }

                    EditorGUILayout.HelpBox(m_SizeCheckLog, m_SizeCheckMesType);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 【UI】特殊移動関連
        /// </summary>
        void UIPositionEx()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox("Move - Special", MessageType.None);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Origin"))
                    {
                        SetPosition(Vector3.zero);
                    }

                    GUI.enabled = CheckSelectionOne();
                    {
                        if (GUILayout.Button("Approximate"))
                        {
                            SetApproximation();
                        }
                    }
                    GUI.enabled = true;
                }
            }
        }

        /// <summary>
        /// 【UI】相対移動
        /// </summary>
        void UIAddPosition()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox("Move - Relative", MessageType.None);
                m_ChangePos = EditorGUILayout.FloatField("Move Value", m_ChangePos);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("+X"))
                    {
                        Vector3 trans = new Vector3(m_ChangePos, 0, 0);
                        AddPosition(trans);
                    }
                    if (GUILayout.Button("-X"))
                    {
                        Vector3 trans = new Vector3(m_ChangePos * -1, 0, 0);
                        AddPosition(trans);
                    }
                    if (GUILayout.Button("+Y"))
                    {
                        Vector3 trans = new Vector3(0, m_ChangePos, 0);
                        AddPosition(trans);
                    }
                    if (GUILayout.Button("-Y"))
                    {
                        Vector3 trans = new Vector3(0, m_ChangePos * -1, 0);
                        AddPosition(trans);
                    }
                    if (GUILayout.Button("+Z"))
                    {
                        Vector3 trans = new Vector3(0, 0, m_ChangePos);
                        AddPosition(trans);
                    }
                    if (GUILayout.Button("-Z"))
                    {
                        Vector3 trans = new Vector3(0, 0, m_ChangePos * -1);
                        AddPosition(trans);
                    }
                }
            }
        }

        /// <summary>
        /// 【UI】パーツ幅移動
        /// </summary>
        void UIAddPositionParts()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox("Move - ItemSize", MessageType.None);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = CheckSelectionOne();
                    {
                        if (GUILayout.Button("+X"))
                        {
                            AddPositionParts(ETypeDirection.pX);
                        }
                        if (GUILayout.Button("-X"))
                        {
                            AddPositionParts(ETypeDirection.mX);
                        }
                        if (GUILayout.Button("+Y"))
                        {
                            AddPositionParts(ETypeDirection.pY);
                        }
                        if (GUILayout.Button("-Y"))
                        {
                            AddPositionParts(ETypeDirection.mY);
                        }
                        if (GUILayout.Button("+Z"))
                        {
                            AddPositionParts(ETypeDirection.pZ);
                        }
                        if (GUILayout.Button("-Z"))
                        {
                            AddPositionParts(ETypeDirection.mZ);
                        }
                    }
                    GUI.enabled = true;
                }
            }
        }

        /// <summary>
        /// 【UI】相対回転
        /// </summary>
        void UIAddRotation()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox("Rotate - Relative", MessageType.None);

                m_ChangeRot = EditorGUILayout.FloatField("Rotate Value", m_ChangeRot);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("+X"))
                    {
                        Vector3 trans = new Vector3(m_ChangeRot, 0, 0);
                        AddRotate(trans);
                    }
                    if (GUILayout.Button("-X"))
                    {
                        Vector3 trans = new Vector3(m_ChangeRot * -1, 0, 0);
                        AddRotate(trans);
                    }
                    if (GUILayout.Button("+Y"))
                    {
                        Vector3 trans = new Vector3(0, m_ChangeRot, 0);
                        AddRotate(trans);
                    }
                    if (GUILayout.Button("-Y"))
                    {
                        Vector3 trans = new Vector3(0, m_ChangeRot * -1, 0);
                        AddRotate(trans);
                    }
                    if (GUILayout.Button("+Z"))
                    {
                        Vector3 trans = new Vector3(0, 0, m_ChangeRot);
                        AddRotate(trans);
                    }
                    if (GUILayout.Button("-Z"))
                    {
                        Vector3 trans = new Vector3(0, 0, m_ChangeRot * -1);
                        AddRotate(trans);
                    }
                    EditorGUILayout.EndHorizontal();

                    // 汎用回転
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+90°"))
                    {
                        Vector3 trans = new Vector3(0, 90.0f, 0);
                        AddRotate(trans);
                    }
                    if (GUILayout.Button("-90°"))
                    {
                        Vector3 trans = new Vector3(0, -90.0f, 0);
                        AddRotate(trans);
                    }
                    if (GUILayout.Button("+180°"))
                    {
                        Vector3 trans = new Vector3(0, 180.0f, 0);
                        AddRotate(trans);
                    }
                }
            }
        }

        /// <summary>
        /// 【UI】スケール
        /// </summary>
        void UISetScale()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox("Scale", MessageType.None);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("x0.5"))
                    {
                        Vector3 trans = new Vector3(0.5f, 0.5f, 0.5f);
                        SetScale(trans);
                    }
                    if (GUILayout.Button("x1"))
                    {
                        Vector3 trans = Vector3.one;
                        SetScale(trans);
                    }
                    if (GUILayout.Button("x2"))
                    {
                        Vector3 trans = new Vector3(2.0f, 2.0f, 2.0f);
                        SetScale(trans);
                    }
                    if (GUILayout.Button("x4"))
                    {
                        Vector3 trans = new Vector3(4.0f, 4.0f, 4.0f);
                        SetScale(trans);
                    }
                }
            }
        }

        /// <summary>
        /// 【UI】反転
        /// </summary>
        void UIReversal()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox("Inversion", MessageType.None);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("X"))
                    {
                        Vector3 trans = new Vector3(-1.0f, 1.0f, 1.0f);
                        MultiScale(trans);
                    }
                    if (GUILayout.Button("Y"))
                    {
                        Vector3 trans = new Vector3(1.0f, -1.0f, 1.0f);
                        MultiScale(trans);
                    }
                    if (GUILayout.Button("Z"))
                    {
                        Vector3 trans = new Vector3(1.0f, 1.0f, -1.0f);
                        MultiScale(trans);
                    }
                }
            }
        }

        #endregion

        //=======================================================================================================
        //. 設定
        //=======================================================================================================
        #region -- 設定

        /// <summary>
        /// 選択オブジェクトの位置を設定
        /// </summary>
        /// <param name="trans"></param>
        void SetPosition(Vector3 trans)
        {
            foreach (GameObject gameObj in Selection.objects)
            {
                Transform transform = gameObj.transform;

                Undo.RecordObject(transform, TOOL_NAME + " - Position");
                transform.localPosition = trans;
            }
        }

        /// <summary>
        /// 選択オブジェクトの位置の加算処理
        /// </summary>
        /// <param name="trans"></param>
        void AddPosition(Vector3 trans)
        {
            foreach (GameObject gameObj in Selection.objects)
            {
                Transform transform = gameObj.transform;

                Undo.RecordObject(transform, TOOL_NAME + " - Position");
                transform.localPosition += trans;
            }
        }

        /// <summary>
        /// 選択オブジェクトに角度の加算処理
        /// </summary>
        /// <param name="trans"></param>
        void AddRotate(Vector3 trans)
        {
            foreach (GameObject gameObj in Selection.objects)
            {
                Transform transform = gameObj.transform;

                Undo.RecordObject(transform, TOOL_NAME + " - Rotation");
                transform.localEulerAngles += trans;
            }
        }

        /// <summary>
        /// 選択オブジェクトにスケールの設定処理
        /// </summary>
        /// <param name="trans"></param>
        void SetScale(Vector3 trans)
        {
            foreach (GameObject gameObj in Selection.objects)
            {
                Transform transform = gameObj.transform;

                Undo.RecordObject(transform, TOOL_NAME + " - Scale");
                transform.localScale = trans;
            }
        }

        /// <summary>
        /// 選択オブジェクトにスケールの乗算処理
        /// </summary>
        /// <param name="trans"></param>
        void MultiScale(Vector3 trans)
        {
            foreach (GameObject gameObj in Selection.objects)
            {
                Transform transform = gameObj.transform;

                Undo.RecordObject(transform, TOOL_NAME + " - Scale");

                transform.localScale = Vector3.Scale(transform.localScale, trans);
            }
        }

        /// <summary>
        /// 選択オブジェクトのパーツ幅移動
        /// </summary>
        /// <param name="type"></param>
        void AddPositionParts(ETypeDirection type)
        {
            foreach (GameObject gameObj in Selection.objects)
            {
                Transform transform = gameObj.transform;
                Vector3 trans = new Vector3();
                float x, y, z;

                Undo.RecordObject(transform, TOOL_NAME + " - Add Position Parts");

                var size = GetGameObjSize();

                switch (type)
                {
                    case ETypeDirection.pX:
                        x = size.x;
                        trans = new Vector3(x, 0, 0);
                        break;
                    case ETypeDirection.mX:
                        x = size.x;
                        trans = new Vector3(-x, 0, 0);
                        break;
                    case ETypeDirection.pY:
                        y = size.y;
                        trans = new Vector3(0, y, 0);
                        break;
                    case ETypeDirection.mY:
                        y = size.y;
                        trans = new Vector3(0, -y, 0);
                        break;
                    case ETypeDirection.pZ:
                        z = size.z;
                        trans = new Vector3(0, 0, z);
                        break;
                    case ETypeDirection.mZ:
                        z = size.z;
                        trans = new Vector3(0, 0, -z);
                        break;
                }

                // サイズが取得出来たか判定
                if (!float.IsNaN(trans.x) && !float.IsNaN(trans.y) && !float.IsNaN(trans.z))
                {
                    transform.localPosition += trans;
                }
                else
                {
                    Debug.LogWarning("[" + Selection.activeGameObject.name + "」にはMeshが含まれていません");
                }
            }
        }

        /// <summary>
        /// 選択オブジェクトを近似値に設定
        /// ※複数オブジェ非対応
        /// </summary>
        void SetApproximation()
        {
            GameObject target = Selection.activeObject as GameObject;

            // 近似値の計算
            var size = GetGameObjSize();
            float setX = Mathf.Round(target.transform.position.x / size.x) * size.x;
            float setY = Mathf.Round(target.transform.position.y / size.y) * size.y;
            float setZ = Mathf.Round(target.transform.position.z / size.z) * size.z;

            Vector3 result = new Vector3(setX, setY, setZ);

            // サイズが取得出来たか判定
            if (!float.IsNaN(result.x) && !float.IsNaN(result.y) && !float.IsNaN(result.z))
            {
                target.transform.position = result;
            }
            else
            {
                Debug.LogWarning("[" + Selection.activeGameObject.name + "」にはMeshが含まれていません");
            }
        }

        #endregion

        //=======================================================================================================
        //. 判定
        //=======================================================================================================
        #region -- 判定

        /// <summary>
        /// 選択オブジェが一つか判定
        /// </summary>
        /// /// <returns></returns>
        bool CheckSelectionOne()
        {
            if (Selection.objects.Length > 1)
            {
                return false;
            }

            return true;
        }

        #endregion

        //=======================================================================================================
        //. 取得
        //=======================================================================================================
        #region -- 取得

        /// <summary>
        /// ゲームオブジェクトのサイズを取得
        /// </summary>
        /// <returns>ゲームオブジェクトのサイズ</returns>
        private Vector3 GetGameObjSize()
        {
            Vector3 size = new Vector3();
            float minX = float.NaN;
            float maxX = float.NaN;
            float minY = float.NaN;
            float maxY = float.NaN;
            float minZ = float.NaN;
            float maxZ = float.NaN;

            foreach (var gameObj in Selection.gameObjects)
            {
                // Rendererを含むオブジェクトに対して
                Renderer[] targets = gameObj.GetComponentsInChildren<Renderer>();
                foreach (Renderer render in targets)
                {
                    float lowX = render.transform.position.x - (render.bounds.size.x / 2.0f);
                    float lowY = render.transform.position.y - (render.bounds.size.y / 2.0f);
                    float lowZ = render.transform.position.z - (render.bounds.size.z / 2.0f);

                    float highX = render.transform.position.x + (render.bounds.size.x / 2.0f);
                    float highY = render.transform.position.y + (render.bounds.size.y / 2.0f);
                    float highZ = render.transform.position.z + (render.bounds.size.z / 2.0f);

                    // 代入されたことがなかったら
                    if (float.IsNaN(minX) && float.IsNaN(maxX) &&
                        float.IsNaN(minY) && float.IsNaN(maxY) &&
                        float.IsNaN(minZ) && float.IsNaN(maxZ))
                    {
                        minX = lowX;
                        maxX = highX;
                        minY = lowY;
                        maxY = highY;
                        minZ = lowZ;
                        maxZ = highZ;
                    }

                    // 入れ替え判定
                    if (minX > lowX)
                    {
                        minX = lowX;
                    }
                    if (maxX < highX)
                    {
                        maxX = highX;
                    }
                    if (minY > lowY)
                    {
                        minY = lowY;
                    }
                    if (maxY < highY)
                    {
                        maxY = highY;
                    }
                    if (minZ > lowZ)
                    {
                        minZ = lowZ;
                    }
                    if (maxZ < highZ)
                    {
                        maxZ = highZ;
                    }
                }
            }

            // サイズの計算
            float sizeX = maxX - minX;
            float sizeY = maxY - minY;
            float sizeZ = maxZ - minZ;

            size = new Vector3(sizeX, sizeY, sizeZ);

            return size;
        }

        #endregion
    }
}