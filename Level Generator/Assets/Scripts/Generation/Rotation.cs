using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    int rotationCount;
    bool isConnected;
    GameObject generatorObject;
    GameObject levelObject;
    int collisionCount;
    GameObject roomObject;
    Room roomScript;


    public void Start()
    {
        this.rotationCount = 0;
        this.isConnected = false;
        this.collisionCount = 0;
        this.roomObject = GetRoomGameObject(this.gameObject.GetComponent<BoxCollider>());
        this.roomScript = roomObject.GetComponent<Room>();
        BoxCollider col = this.gameObject.GetComponent<BoxCollider>();
        Vector3 size = col.bounds.size / 2;
        bool check = Physics.CheckBox(col.center, size);
        Debug.Log("DOES THIS WORK???: " + check);

    }

    private void LateUpdate()
    {
        
        //if (this.collisionCount == 0 && this.isConnected == false)
        //{
        //    this.SetIsConnectedToTrue();
        //    string nextRoom = GameObject.Find("Generator").GetComponent<Generator>().GetNextAssetInQueue();
        //    InstantiateNextRoom(nextRoom, GameObject.Find("Level"));
        //}
        
    }

    //private void OnTriggerEnter(Collider collider)
    //{
    //    if (this.gameObject.name == "Collider Container" && collider.gameObject.name == "Collider Container")
    //    {
    //        if (this.roomScript.GetIsAligned() == false && this.roomObject.tag=="New Room")
    //        {
    //            //this.roomScript.AlignRoomsMainExit();
    //        }
    //        //Debug.Log("COLLISION ENTER: " + this.collisionCount + " " + this.roomObject.name);
    //        IncrementCollisionCount();
    //    }
    //}

    //private void OnTriggerStay(Collider collider)
    //{
    //    if (this.gameObject.name == "Collider Container" && collider.gameObject.name == "Collider Container")
    //    {
    //        GameObject room = GetRoomGameObject(this.gameObject.GetComponent<BoxCollider>());
    //        if (this.roomScript.GetIsAligned() == true && this.GetIsConnected() == false)
    //        {
    //            Transform entry = this.roomObject.transform.Find("Entry Connector");
    //            entry.transform.Rotate(0.0f, 90.0f, 0.0f);
    //            this.rotationCount++;
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider collider)
    //{
    //    if (this.gameObject.name == "Collider Container" && collider.gameObject.name == "Collider Container")
    //    {
    //        //GameObject thisRoom = GetRoomGameObject(this.gameObject.GetComponent<BoxCollider>());
    //        DecreaseCollisionCount();
            
    //        if (this.collisionCount == 0 && this.isConnected == false)
    //        {
    //            this.SetIsConnectedToTrue();
    //            string nextRoom = GameObject.Find("Generator").GetComponent<Generator>().GetNextAssetInQueue();
    //            InstantiateNextRoom(nextRoom, GameObject.Find("Level"));
    //        }
            
    //    }
    //}


    public void IncrementCollisionCount()
    {
        this.collisionCount++;
    }

    public void DecreaseCollisionCount()
    {
        if (this.collisionCount > 0)
        {
            this.collisionCount--;
        }

    }

    public void SetIsConnectedToTrue()
    {
        this.isConnected = true;
    }

    public bool GetIsConnected()
    {
        return this.isConnected;
    }

    public GameObject GetRoomGameObject(Collider collider)
    {
        GameObject room = collider.gameObject;
        while (room.tag != "New Room" && room.tag != "Previous Room" && room.tag != "Room")
        {
            room = room.transform.parent.gameObject;
        }
        return room;
    }

    public GameObject GetLevelObject()
    {
        return GameObject.Find("Level");
    }

    public GameObject GetGeneratorObject()
    {
        return GameObject.Find("Generator");
    }

    public GameObject GetPreviousRoom(GameObject level)
    {
        foreach (Transform room in level.transform)
        {
            if (room.tag == "Previous Room")
            {
                return room.gameObject;
            }
        }
        return null;
    }

    public void InstantiateNextRoom(string nextRoom, GameObject level)
    {
        Debug.Log("COLLISION ENTER: " + this.collisionCount + " " + this.roomObject.name);
        GameObject roomAsset = GameObject.Find("Generator").GetComponent<Generator>().GetRoomAssetFromName(nextRoom);
        GameObject thisRoomObject = GetRoomGameObject(this.GetComponent<BoxCollider>());
        Room roomScript = thisRoomObject.GetComponent<Room>();
        GameObject previousRoom = roomScript.GetPreviousRoom();
        previousRoom.tag = "Room";
        thisRoomObject.tag = "Previous Room";
        GameObject room = Instantiate(roomAsset, thisRoomObject.transform.position, Quaternion.identity, level.transform);
        room.tag = "New Room";
        room.GetComponent<Room>().SetPreviousRoom(thisRoomObject);
    }
}