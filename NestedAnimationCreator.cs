using UnityEngine;
using UnityEditor;

#if UNITY_4_6
// Unity 5以前はAnimatorControllerクラスがUnityEditorInternal名前空間で定義されている
using UnityEditorInternal;
#else
using UnityEditor.Animations;
#endif

// 作成するアニメーションクリップの名前を入力するためのダイアログのクラス
public class RenameWindow : EditorWindow
{
    public string CaptionText { get; set; }     // ダイアログのキャプション
    public string ButtonText { get; set; }      // ボタンのラベル
    public string NewName { get; set; }         // 入力された名前
    public System.Action<string>
        OnClickButtonDelegate
    { get; set; }       // ボタンが押されたときに実行されるデリゲート

    void OnGUI()
    {
        NewName = EditorGUILayout.TextField(CaptionText, NewName);
        if (GUILayout.Button(ButtonText))
        {
            if (OnClickButtonDelegate != null)
            {
                // ボタンが押されたら、あらかじめ設定されたデリゲートに入力された名前を渡す
                OnClickButtonDelegate.Invoke(NewName.Trim());
            }

            Close();
            GUIUtility.ExitGUI();
        }
    }
}

public class NestedAnimationCreator : MonoBehaviour
{
    // Assetsメニュー→「Create」に「Nested Animation」の項目を追加する
    [MenuItem("Assets/Create/Nested Animation")]
    public static void Create()
    {
        // Projectビューで選択されているアニメーターコントローラーを取得する
        AnimatorController selectedAnimatorController =
            Selection.activeObject as AnimatorController;

        // アニメーターコントローラーが選択されていなければエラー
        if (selectedAnimatorController == null)
        {
            Debug.LogWarning("No animator controller selected.");
            return;
        }

        // 作成するアニメーションクリップの名前を入力するダイアログを開く
        RenameWindow renameWindow =
            EditorWindow.GetWindow<RenameWindow>("Create Nested Animation");
        renameWindow.CaptionText = "New Animation Name";
        renameWindow.NewName = "New Clip";
        renameWindow.ButtonText = "Create";
        // ダイアログのボタンが押されたら呼ばれるメソッドのデリゲート
        renameWindow.OnClickButtonDelegate = (string newName) =>
        {
            if (string.IsNullOrEmpty(newName))
            {
                Debug.LogWarning("Invalid name.");
                return;
            }

            // ダイアログで入力された名前でアニメーションクリップを作成する
            AnimationClip animationClip =
                AnimatorController.AllocateAnimatorClip(newName);

            // 選択されたアニメーターコントローラーのサブアセットとして
            // 作成したアニメーションクリップを追加する
            AssetDatabase.AddObjectToAsset(animationClip, selectedAnimatorController);

            // アニメーターコントローラーをインポートし直して変更を反映する
            AssetDatabase.ImportAsset(
                AssetDatabase.GetAssetPath(selectedAnimatorController));
        };
    }

    // サブアセットとして作成したアニメーションクリップはAssetsメニュー→Deleteでは
    // 削除することができないので、Assetsメニューに「Delete Sub Asset」の項目を追加する
    [MenuItem("Assets/Delete Sub Asset")]
    public static void Delete()
    {
        Object[] selectedAssets = Selection.objects;
        if (selectedAssets.Length < 1)
        {
            Debug.LogWarning("No sub asset selected.");
            return;
        }

        foreach (Object asset in selectedAssets)
        {
            // 選択されたオブジェクトがサブアセットだったら削除する
            if (AssetDatabase.IsSubAsset(asset))
            {
                string path = AssetDatabase.GetAssetPath(asset);
                DestroyImmediate(asset, true);
                AssetDatabase.ImportAsset(path);
            }
        }
    }
}