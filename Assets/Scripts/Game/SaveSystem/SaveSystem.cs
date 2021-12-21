
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static void SavePlayerSlot(PlayerInventory inventory, PlayerHealth energy,
        MapSaveSystem map,float[] pos, string sectorName, int slotIndex,Gun gun)
    {
        string slotPath = GetSlotPath(slotIndex);
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + slotPath;
        FileStream stream = new FileStream(path, FileMode.Create);
        GameData data = new GameData(inventory, energy,map, pos, sectorName,gun);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    private static string GetSlotPath(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                return "/player.xd1";
            case 1:
                return "/player.xd2";
            case 2:
                return "/player.xd3";
        }
        return null;
    }
    public static bool FileExists(int slotIndex){
        string path = Application.persistentDataPath + GetSlotPath(slotIndex);

        return File.Exists(path) ? true : false;
    }
    public static GameData LoadPlayerSlot(int slotIndex)
    {

        string path = Application.persistentDataPath + GetSlotPath(slotIndex);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
    public static void ErasePlayerSlot(int slotIndex)
    {
        System.IO.File.Delete(Application.persistentDataPath + GetSlotPath(slotIndex));
    }
}
