using System.Collections.Generic;

public class CommonParams
{
    // 基本URL
    public static string URLBasics = "https://lootbox2.gjmj.net";

    // UUID取得URL
    public static string URLGetUUID = $"{URLBasics}/uuid";

    // register（名前）登録URL
    public static string URLRegister = $"{URLBasics}/register";

    // session id取得URL
    public static string URLGetSession = $"{URLBasics}/session/get";

    // ガチャリスト取得URL
    public static string URLGetLootBoxList = $"{URLBasics}/loot_box/list";

    // 手持ちリスト取得URL
    public static string URLGetTakeList = $"{URLBasics}/card/list";

    // Bearerの設定
    public static string GetAuthorization(string sessionId)
        => $"Bearer {sessionId}";

    // 実際にガチャを引く
    public static string URLDrawLootBox(string deckId)
        => $"{URLBasics}/loot_box/draw/{deckId}";

    // 個別のカード情報を取得
    public static string URLGetCardDetail(string cardId)
        => $"{URLBasics}/card/detail/{cardId}";

    // 個別のカード画像を取得
    public static string URLGetImageData(string cardId)
        => $"{URLBasics}/card/image/{cardId}";
}


// =====================================================
// JSON展開用クラス
// =====================================================

// UUID取得
[System.Serializable]
public class UUIDMethod
{
    [System.Serializable]
    public class UuidStatus
    {
        public string uuid;
    }

    public UuidStatus response;
    public int status_code;
}


// Session取得
[System.Serializable]
public class SessionIDMethod
{
    [System.Serializable]
    public class SessionParam
    {
        public string session_id;
    }

    public SessionParam response;
    public int status_code;
}


// ガチャ一覧
[System.Serializable]
public class DeckDetail
{
    public string id;
    public string name;
    public string detail;
    public bool can_loot;
}


[System.Serializable]
public class DecksMethod
{
    [System.Serializable]
    public class DeckMethod
    {
        public List<DeckDetail> decks;
    }

    public DeckMethod response;
    public int status_code;
}


// ガチャ結果
// 10連ならcard_idsに10個入る
[System.Serializable]
public class GachaGetIDMethod
{
    [System.Serializable]
    public class GachaGetID
    {
        public List<int> card_ids;
    }

    public GachaGetID response;
    public int status_code;
}


// 手持ちカード
[System.Serializable]
public class TakeCardsMethod
{
    [System.Serializable]
    public class CardParam
    {
        public string card_id;
        public int quantity;
    }

    [System.Serializable]
    public class CardMethod
    {
        public List<CardParam> cards;
    }

    public CardMethod response;
    public int status_code;
}


// カード情報
[System.Serializable]
public class CardInfoMethod
{
    [System.Serializable]
    public class CardInfo
    {
        public string card_id;
        public string card_name;
        public int offense;
        public int defense;
        public string description;
    }

    public CardInfo response;
    public int status_code;
}

[System.Serializable]
public class RegisterResponse
{
    public int status_code;
}