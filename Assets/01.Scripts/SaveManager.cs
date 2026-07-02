using System;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static SaveData saveData;


    public static void Save()
    {
        GameManager.Instance.SavePlayTime();

        string json = JsonUtility.ToJson(saveData, true);

        string path = Application.persistentDataPath + "/playerData.json";

        File.WriteAllText(path, json);

        Debug.Log("저장 완료: " + path);
    }

    public static bool Load()
    {
        string path = Application.persistentDataPath + "/playerData.json";

        if (!File.Exists(path))
        {
            Debug.Log("세이브 파일 없음");
            saveData = new();

            return false;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        saveData = data;
        return true;
    }

    public static void DeleteSave()
    {
        string path = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(path))
        {
            File.Delete(path);

            // 메모리에 있는 데이터도 초기화
            saveData = new SaveData();

            Debug.Log("세이브 파일 삭제 완료: " + path);
        }
        else
        {
            Debug.Log("삭제할 세이브 파일이 없습니다.");
        }
    }

    [Serializable]
    public class SaveData
    {
        public OrePieceData2 orePiece = new();
        public NPCData npc = new();
        public VillageData village = new();
        public bool isInit;
        public OrePieceSaveData pinData;
        public EndingLog endingLog = new();
    }

    [Serializable]
    public class OrePieceData2
    {
        public int[] pieceCnt = new int[10];
        public int[] upkeepCost = new int[10];
    }

    [Serializable]
    public class NPCData
    {
        public int unlockIdx;
        public bool isUnlock = true;
        public bool isDialogue;
    }

    [Serializable]
    public class VillageData
    {
        public bool[] isUnlock = new bool[7];
        public Elder elder = new();
        public Mog mog = new();
        public Khan khan = new();
        public Boom boom = new();
        public Luna luna = new();
        public Pora pora = new();
        public Crow crow = new();
    }

    [Serializable]
    public class Elder
    {
        public int level;
        public int x = 8, y = 12;
        public int baseTileHP = 1;
        public OreEnum oreIdx = OreEnum.Coal;
    }

    [Serializable]
    public class Mog
    {
        public int level;
        public float inner = 1.5f;
        public float outer = 3f;
        public int twinkleProb;
    }

    [Serializable]
    public class Khan
    {
        public int level;
        public int power = 1;
        public int durability = 40;
    }

    [Serializable]
    public class Boom 
    {
        public int[] bombCnt = new int[6];
    }

    [Serializable]
    public class Luna
    {
        public int[] level = new int[3];
        public int oreProb = 10;
        public int orePieceMin = 5, orePieceMax = 10;
        public int obstacleProb = 15;
    }

    [Serializable]
    public class Pora
    {
        public int[] potionCnt = new int[6];
    }

    [Serializable]
    public class Crow
    {
        public bool isSetting = false;
        public int[] giveOreIdx = new int[4], getOreIdx = new int[4];
        public int[] giveOreQuantity = new int[4], getOreQuantity = new int[4];
        public bool[] isSoldOut = new bool[4];
        public bool[] isActive = new bool[4]
        {
            true, true, true, true
        };
    }

    [Serializable]
    public class OrePieceSaveData
    {
        public OrePieceInfo[] orePieces;

        [Serializable]
        public class OrePieceInfo
        {
            public OreEnum type;
            public int amount;
        }
    }

    [Serializable]
    public class EndingLog
    {
        public int pickaxCnt;
        public int bombCnt;
        public int potionCnt;
        public int mineCnt;
        public int[] orePieces = new int[10];
        public int playTime;
    }
}
