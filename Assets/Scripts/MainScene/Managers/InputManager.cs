using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public bool LeanLeftInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            return true;
        }
        return false;
    }


    public bool LeanRightInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return true;
        }
        return false;
    }


    //Used for both exit tutorial and to drink
    public bool SpaceBarDown()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }
        return false;
    }
}
