using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class CheckPointObject : MonoBehaviour
{
    public abstract void SaveState();
    public abstract void LoadState();

    protected void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(this.GetInstanceID() + "/" + key, value);
    }

    protected float RestoreFloat(string key)
    {
        string keyName = this.GetInstanceID() + "/" + key;
        Assert.IsTrue(PlayerPrefs.HasKey(keyName));
        return PlayerPrefs.GetFloat(keyName);
    }

    protected void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(this.GetInstanceID() + "/" + key, value);
    }

    protected int RestoreInt(string key)
    {
        string keyName = this.GetInstanceID() + "/" + key;
        Assert.IsTrue(PlayerPrefs.HasKey(keyName));
        return PlayerPrefs.GetInt(keyName);
    }

    protected void SaveBool(string key, bool value)
    {
        SaveInt(key, value ? 1 : 0);
    }

    protected bool RestoreBool(string key)
    {
        return RestoreInt(key) == 1 ? true : false;
    }

    protected void SaveVector3(string key, Vector3 vector)
    {
        SaveFloat(key + ".x", vector.x);
        SaveFloat(key + ".y", vector.y);
        SaveFloat(key + ".z", vector.z);
    }

    protected Vector3 RestoreVector3(string key)
    {
        float x = RestoreFloat(key + ".x");
        float y = RestoreFloat(key + ".y");
        float z = RestoreFloat(key + ".z");

        return new Vector3(x, y, z);
    }

    protected void SaveVector3Int(string key, Vector3Int vector)
    {
        SaveInt(key + ".x", vector.x);
        SaveInt(key + ".y", vector.y);
        SaveInt(key + ".z", vector.z);
    }

    protected Vector3Int RestoreVector3Int(string key)
    {
        int x = RestoreInt(key + ".x");
        int y = RestoreInt(key + ".y");
        int z = RestoreInt(key + ".z");

        return new Vector3Int(x, y, z);
    }


    protected void SavePosition(Vector3 vector)
    {
        print("Saving position for " + this.tag + " " + vector);
        SaveVector3("position", vector);
    }

    protected Vector3 RestorePosition()
    {
        Vector3 vector = RestoreVector3("position");
        print("Restoring position for " + this.tag + " " + vector);
        return vector;
    }
}
