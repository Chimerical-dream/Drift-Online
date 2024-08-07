using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int Record;
    public CarViewData CarViewData;
    public string Nickname
    {
        set
        {
            if(value.Length < 3)
            {
                nickname = defaultNickname;
            }
            else
            {
                nickname = value;
            }
        }

        get
        {
            return nickname;
        }
    }

    private string nickname;
    private string defaultNickname => System.Environment.MachineName;

    public SaveData()
    {
        CarViewData = new CarViewData();
        Nickname = defaultNickname;
        Record = 0;
    }
}
