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

    #region Player One

    public bool LeanLeftPlayerOneInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            return true;
        }
        return false;
    }


    public bool LeanRightPlayerOneInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return true;
        }
        return false;
    }

    public bool QuickTimePlayerOneInput()
    {
        if (Input.GetKey(KeyCode.S))
        {
            return true;
        }
        return false;
    }

    public bool CatchGlassPlayerOneInput()
    {
        if (Input.GetKey(KeyCode.E))
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

    #endregion

    #region Player Two

    public bool LeanLeftPlayerTwoInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            return true;
        }
        return false;
    }


    public bool LeanRightPlayerTwoInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            return true;
        }
        return false;
    }

    public bool QuickTimePlayerTwoInput()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            return true;
        }
        return false;
    }

    public bool CatchGlassPlayerTwoInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            return true;
        }
        return false;
    }

    public bool DrinkPlayerTwoInput()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            return true;
        }
        return false;
    }

    #endregion
}
