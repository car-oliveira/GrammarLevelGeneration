using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connectors : MonoBehaviour
{
    bool isConnected = false;

    public void setIsConnecteToTrue()
    {
        isConnected = true;
    }

    public bool getIsConnected()
    {
        return isConnected;
    }
}
