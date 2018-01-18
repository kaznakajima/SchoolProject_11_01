using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataSave : MonoBehaviour {

    public List<string[]> csvDatas = new List<string[]>();
    public string fileName;
    enum DataName
    {
        BGM,
        SE
    }

	void Start () {
        if (!(File.Exists(string.Format("{0}.csv", fileName)) == false))
        {
            Debug.Log("データ見つかった");
            CsvRead(fileName);
        }
        int i = 0;
        while (i < 31)
        {
            logSave("BGMDate", i, "b", true);
            i++;
        }
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < csvDatas.Count; i++)
            {
                for (int j = 0; j < csvDatas[i].Length; j++)
                {
                    Debug.Log("csvDatas[" + i + "][" + j + "] = " + csvDatas[i][j]);
                }
            }
        }
		
	}
    public void logSave(string fileName,int num, string txt,bool change)
    {
        StreamWriter sw = new StreamWriter(string.Format(
            "../NakajimaProject/Assets/Resources/{0}.csv", fileName),change); //true=追記 false=上書き
        sw.WriteLine(num);
        sw.Flush();
        sw.Close();
    }

    public void CsvRead(string FileName)
    {

        TextAsset csv = Resources.Load(FileName) as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }


    }
}
