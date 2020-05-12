using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.Assertions;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager instance;

    public bool hasCheckpoint { get; private set; } = false;

    [SerializeField]
    private CheckPointObject[] checkPointPrefabs;

    private HashSet<CheckPoint> checkpoints = new HashSet<CheckPoint>();

    private void Awake()
    {
        if (!CheckPointManager.instance)
        {
            instance = this;
        }
    }

    private void InvokeOnCheckpointObjects(MethodInfo method, object[] parameters)
    {
        print("Invoking method on " + checkPointPrefabs.Length + " prefabs");
        foreach (CheckPointObject checkpointObject in checkPointPrefabs)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(checkpointObject.tag);

            print("Found " + gameObjects.Length + " objects with tag " + checkpointObject.tag);

            foreach (GameObject gameObject in gameObjects)
            {
                print("Invoking " + method + " on " + gameObject);
                gameObject.SetActive(true);
                method.Invoke(gameObject.GetComponent<CheckPointObject>(), parameters);
            }
        }
    }

    private void InvokedMethodNamed(string methodName)
    {
        Type type = Type.GetType("CheckPointObject");
        MethodInfo methodInfo = type.GetMethod(methodName);
        this.InvokeOnCheckpointObjects(methodInfo, null);
    }

    public void SaveState(CheckPoint checkpoint)
    {        
        checkpoints.Add(checkpoint);
        foreach (CheckPoint cp in checkpoints)
        {
            cp.gameObject.SetActive(cp != checkpoint);
        }

        print("CheckPointManager SaveState");
        hasCheckpoint = true;
        this.InvokedMethodNamed("SaveState");
    }

    public void LoadState()
    {
        print("CheckPointManager LoadState");
        Assert.IsTrue(hasCheckpoint);
        this.InvokedMethodNamed("LoadState");        
    }
}
