using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;

public class SaveManager : ScriptableObject
{
	private const string saveName = "UDSave";
	private const string version = "2";
	private const string cryptPw = "qWg+!k85%2sgr305f4d~#S";

	public void Init()
	{
        if((string) Get("version") != version)
			UpdateSaveVersion();
	}

	//Структура сохранения поумолчанию
	private Hashtable LoadDefault()
	{
		Hashtable defaultData = new Hashtable();
		defaultData.Add("version",version); // Версия сохранения
		defaultData.Add("balans","0"); // Баланс игрока
        defaultData.Add("highscore","0"); // Лучший результат

		return defaultData;
	}
	
	private Hashtable GetDataSave()
	{
		string saveDataEncoded;
		saveDataEncoded = PlayerPrefs.GetString(saveName,"");
		if(saveDataEncoded == "" || saveDataEncoded == null)
		{
			Hashtable defaultSaveData = LoadDefault();
			string defaultSaveDataJson = MiniJSON.jsonEncode(defaultSaveData);
            //saveDataEncoded = AES.Encrypt(defaultSaveDataJson,cryptPw);
            saveDataEncoded = defaultSaveDataJson;
        }

        string saveJson = saveDataEncoded;
        //string saveJson = AES.Decrypt(saveDataEncoded,cryptPw);
		return MiniJSON.jsonDecode(saveJson) as Hashtable;
	}

	//Обновить имеющуюся структуру на новую
	private void UpdateSaveVersion()
	{
		Hashtable oldSave = GetDataSave();
		Hashtable newSaveTemplate = LoadDefault();
		Hashtable newSave = new Hashtable();

		List<string> noUpdate = new List<string>{"version"}; //значения данных ключей не обновляем

		ICollection keys = newSaveTemplate.Keys;
		foreach(string key in keys)
		{
			if(oldSave.ContainsKey(key) && !noUpdate.Contains(key))
				newSave.Add(key,(string) oldSave[key]);
			else
				newSave.Add(key,(string) newSaveTemplate[key]);
		}

		string saveDataJson = MiniJSON.jsonEncode(newSave);
        //string saveDataEncoded = AES.Encrypt(saveDataJson,cryptPw);
        string saveDataEncoded = saveDataJson;

        PlayerPrefs.SetString(saveName,saveDataEncoded);
		PlayerPrefs.Save();

		newSaveTemplate = null;
		oldSave = null;
		newSave = null;
	}

	//Получить значение
	public string Get(string key)
	{
		Hashtable save = GetDataSave();

		string result = "";
        if(save != null)
        {
            if((bool)save.ContainsKey(key))
                result = (string)save[key];

            save = null;
        }


		return result;
	}

	//Установить значение
	public void Set(string key, string val)
	{
		Hashtable save = GetDataSave();

		if(save.ContainsKey(key))
		{
			save[key] = val;

			string saveDataJson = MiniJSON.jsonEncode(save);
            //string saveDataEncoded = AES.Encrypt(saveDataJson,cryptPw);
            string saveDataEncoded = saveDataJson;
            PlayerPrefs.SetString(saveName,saveDataEncoded);
			PlayerPrefs.Save();
		}
	}
}