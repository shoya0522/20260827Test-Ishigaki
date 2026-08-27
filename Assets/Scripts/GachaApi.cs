using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class GachaApi : MonoBehaviour
{
    private string uuid;
    private string sessionId;

    private Button drawButton;
    private VisualElement cardArea;

    private Label cardNameLabel;
    private Label offenseLabel;
    private Label defenseLabel;
    private Label descriptionLabel;

    private bool isInitializing = false;

    private void Start()
    {
        if (isInitializing)
        {
            return;
        }

        isInitializing = true;

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Debug.Log("=== API初期化開始 ===");

        yield return StartCoroutine(GetUUID());

        // UUID取得に失敗した場合はここで終了
        if (string.IsNullOrEmpty(uuid))
        {
            Debug.LogError("UUIDが取得できなかったため、初期化を終了します。");
            yield break;
        }

        yield return StartCoroutine(Register());

        // 登録に失敗した場合はSession取得へ進まない
        if (!isRegistered)
        {
            Debug.LogError("ユーザー登録に失敗したため、初期化を終了します。");
            yield break;
        }

        yield return StartCoroutine(GetSession());

        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError("Session IDが取得できなかったため、初期化を終了します。");
            yield break;
        }

        SetupUI();

        Debug.Log("=== API初期化完了 ===");
    }

    private IEnumerator GetUUID()
    {
        using (UnityWebRequest request =
               UnityWebRequest.Get(CommonParams.URLGetUUID))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("UUID取得失敗 : " + request.error);
                yield break;
            }

            string json = request.downloadHandler.text;

            Debug.Log("UUID Response:");
            Debug.Log(json);

            UUIDMethod data =
                JsonUtility.FromJson<UUIDMethod>(json);

            if (data == null || data.status_code != 200)
            {
                Debug.LogError("UUID取得エラー");
                yield break;
            }

            uuid = data.response.uuid;

            Debug.Log("取得したUUID : " + uuid);
        }
    }

    private bool isRegistered = false;

    private IEnumerator Register()
    {
        WWWForm form = new WWWForm();

        form.AddField("uuid", uuid);
        form.AddField("name", "UnityGacha");

        Debug.Log("=== ユーザー登録開始 ===");
        Debug.Log("UUID : " + uuid);
        Debug.Log("Name : UnityGacha");

        using (UnityWebRequest request =
               UnityWebRequest.Post(CommonParams.URLRegister, form))
        {
            yield return request.SendWebRequest();

            Debug.Log("Register HTTP Status : " + request.responseCode);

            string json = request.downloadHandler.text;

            Debug.Log("Register Response:");
            Debug.Log(json);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "ユーザー登録失敗 : HTTP/" +
                    request.responseCode +
                    " " +
                    request.error
                );

                yield break;
            }

            RegisterResponse data =
                JsonUtility.FromJson<RegisterResponse>(json);

            if (data == null || data.status_code != 200)
            {
                Debug.LogError("ユーザー登録エラー");
                yield break;
            }

            isRegistered = true;

            Debug.Log("ユーザー登録成功！");
        }
    }

    private IEnumerator GetSession()
    {
        WWWForm form = new WWWForm();

        form.AddField("uuid", uuid);

        using (UnityWebRequest request =
               UnityWebRequest.Post(CommonParams.URLGetSession, form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Session取得失敗 : " + request.error);
                Debug.LogError(request.downloadHandler.text);
                yield break;
            }

            string json = request.downloadHandler.text;

            Debug.Log("Session Response:");
            Debug.Log(json);

            SessionIDMethod data =
                JsonUtility.FromJson<SessionIDMethod>(json);

            if (data == null || data.status_code != 200)
            {
                Debug.LogError("Session取得エラー");
                yield break;
            }

            sessionId = data.response.session_id;

            Debug.Log("取得したSession ID : " + sessionId);
        }
    }

    private void SetupUI()
    {
        UIDocument uiDocument =
            FindFirstObjectByType<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("UIDocumentが見つかりません");
            return;
        }

        VisualElement root =
            uiDocument.rootVisualElement;

        drawButton =
            root.Q<Button>("DrawButton");

        cardArea =
            root.Q<VisualElement>("CardArea");

        cardNameLabel =
            root.Q<Label>("CardNameLabel");

        offenseLabel =
            root.Q<Label>("OffenseLabel");

        defenseLabel =
            root.Q<Label>("DefenseLabel");

        descriptionLabel =
            root.Q<Label>("DescriptionLabel");

        if (drawButton == null)
        {
            Debug.LogError("DrawButtonが見つかりません");
            return;
        }

        if (cardNameLabel == null)
        {
            Debug.LogError("CardNameLabelが見つかりません");
            return;
        }

        if (offenseLabel == null)
        {
            Debug.LogError("OffenseLabelが見つかりません");
            return;
        }

        if (defenseLabel == null)
        {
            Debug.LogError("DefenseLabelが見つかりません");
            return;
        }

        if (descriptionLabel == null)
        {
            Debug.LogError("DescriptionLabelが見つかりません");
            return;
        }

        drawButton.clicked -= OnDrawButtonClicked;
        drawButton.clicked += OnDrawButtonClicked;

        Debug.Log("ガチャボタンの設定完了！");
    }

// ボタンが押された時の処理
    private void OnDrawButtonClicked()
    {
        Debug.Log("ガチャボタンが押されました！");
        StartCoroutine(DrawLootBox());
    }

// ガチャを引くときの処理
    private IEnumerator DrawLootBox()
    {
         string deckId = "1";

    using (UnityWebRequest request =
           UnityWebRequest.PostWwwForm(
               CommonParams.URLDrawLootBox(deckId),
               ""))
        {
        request.SetRequestHeader(
            "Authorization",
            CommonParams.GetAuthorization(sessionId)
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ガチャ失敗 : " + request.error);
            Debug.LogError("Response : " + request.downloadHandler.text);
            yield break;
        }

        string json = request.downloadHandler.text;

        Debug.Log("=== Gacha Response ===");
        Debug.Log(json);

        GachaGetIDMethod data =
            JsonUtility.FromJson<GachaGetIDMethod>(json);

        if (data == null || data.status_code != 200)
        {
            Debug.LogError("ガチャAPIエラー");
            yield break;
        }

        if (data.response == null ||
            data.response.card_ids == null ||
            data.response.card_ids.Count == 0)
        {
            Debug.LogError("card_idsが取得できませんでした");
            yield break;
        }

        int cardId = data.response.card_ids[0];

        Debug.Log("取得したCard ID : " + cardId);

        string cardIdString = cardId.ToString();

        yield return StartCoroutine(
                GetCardDetail(cardIdString));

            yield return StartCoroutine(
                GetCardImage(cardIdString));
        }
    }

// カードのイメージ表示用
    private IEnumerator GetCardImage(string cardId)
{
    string url = CommonParams.URLGetImageData(cardId);

    Debug.Log("カード画像取得 : " + url);

    using (UnityWebRequest request =
           UnityWebRequestTexture.GetTexture(url))
    {
        // 認証を追加
        request.SetRequestHeader(
            "Authorization",
            CommonParams.GetAuthorization(sessionId)
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "カード画像取得失敗 : " +
                request.error
            );

            Debug.LogError(
                "Response Code : " +
                request.responseCode
            );

            yield break;
        }

        Texture2D texture =
            DownloadHandlerTexture.GetContent(request);

        if (texture == null)
        {
            Debug.LogError("Texture2Dの取得に失敗しました");
            yield break;
        }

        Debug.Log("カード画像取得成功！");

        DisplayCardImage(texture);
    }
}

// カードの詳細表示用
private IEnumerator GetCardDetail(string cardId)
{
    string url = CommonParams.URLGetCardDetail(cardId);

    Debug.Log("カード詳細取得 : " + url);

    using (UnityWebRequest request =
           UnityWebRequest.Get(url))
    {
        request.SetRequestHeader(
            "Authorization",
            CommonParams.GetAuthorization(sessionId)
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "カード詳細取得失敗 : " +
                request.error
            );

            yield break;
        }

        string json = request.downloadHandler.text;

        Debug.Log("=== Card Detail Response ===");
        Debug.Log(json);

        CardInfoMethod data =
            JsonUtility.FromJson<CardInfoMethod>(json);

        if (data == null || data.status_code != 200)
        {
            Debug.LogError("カード詳細取得エラー");
            yield break;
        }

        Debug.Log("カード名 : " + data.response.card_name);
        Debug.Log("攻撃力 : " + data.response.offense);
        Debug.Log("防御力 : " + data.response.defense);
        Debug.Log("説明 : " + data.response.description);

        cardNameLabel.text =
            data.response.card_name;

        offenseLabel.text =
            "攻撃力 : " + data.response.offense;

        defenseLabel.text =
            "防御力 : " + data.response.defense;

        descriptionLabel.text =
            data.response.description;
    }
}

    private void DisplayCardImage(Texture2D texture)
{
    if (cardArea == null)
    {
        Debug.LogError("CardAreaがありません");
        return;
    }

    cardArea.style.backgroundImage =
        new StyleBackground(texture);
}
}