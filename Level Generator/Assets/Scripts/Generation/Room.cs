using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //entry is aligned with previous piece's exit
    bool isAligned;
    //piece is rotated correctly as to not collide with any other
    bool isConnected;
    //type of the room
    string roomType;
    //object containing the level's pieces
    GameObject levelObject;
    //object containing the Generator script
    GameObject generatorObject;
    //room current ones was connected to
    public GameObject previousRoom;

    private void Start()
    {
        this.levelObject = GetLevelObject();
        this.generatorObject = GetGeneratorObject();
        generatorObject.GetComponent<Generator>().AddRoomToCurrentList(this.gameObject);
        //Only the first room is intantiated with the tag "Previous Room"
        if (this.gameObject.tag=="Previous Room")
        {
            this.SetIsAlignedToTrue();
            this.SetIsConnectedToTrue();
            generatorObject.GetComponent<Generator>().renderedLevel += this.roomType + " ";
            this.GetRoomMainExit(this.gameObject).tag = "Connected_Exit";
            string nextRoom = generatorObject.GetComponent<Generator>().GetNextAssetInQueue();
            InstantiateNextRoom(nextRoom, this.levelObject);
        }
        //All the remaining rooms use this
        else
        {
            this.isAligned = false;
            this.isConnected = false;
            AlignRoomsMainExit();
            if (CheckRoomCollisions() == false)
            {
                generatorObject.GetComponent<Generator>().renderedLevel += this.roomType + " ";
                string nextRoom = generatorObject.GetComponent<Generator>().GetNextAssetInQueue();
                SwitchExits();
                if (nextRoom != null)
                    {
                //    AssignNewMainExit();
                    
                    InstantiateNextRoom(nextRoom, this.levelObject);
                }
                else
                {
                    PlayerPrefs.DeleteAll();
                    generatorObject.GetComponent<Generator>().AddRoomsToLevelList();
                    generatorObject.GetComponent<Generator>().ExpandNextPath();
                    //AssignNewMainExit();
                    //bool hasPathToExpand = generatorObject.GetComponent<Generator>().HasMainExits();
                    //if (hasPathToExpand == true)
                    //{
                    //    generatorObject.GetComponent<Generator>().ExpandNextPath();
                    //}
                    //generatorObject.GetComponent<Generator>().PopulateRooms();

                }
            }
            else
            {
                    BacktrackRoom();
            }

        }
    }


    public void SwitchMainExitToConnected()
    {
        Transform mainExit = GetRoomMainExit(this.previousRoom);
        if (mainExit != null)
        {
            mainExit.tag = "Connected_Exit";
            mainExit.gameObject.name = "Connected Exit Connector";
        }
    }

    public void SwitchExitToMain()
    {
        if (GetRoomEntry(this.previousRoom) != null)
        {
            Transform roomEntry = GetRoomEntry(this.previousRoom);
            foreach (Transform child in roomEntry.transform)
            {
                if (child.CompareTag("Exit"))
                {
                    child.tag = "Main_Exit";
                    child.gameObject.name = "Main Exit Connector";
                    return;
                }
            }
        }
    }

    public void SwitchExits()
    {
        SwitchMainExitToConnected();
        SwitchExitToMain();
    }

    //Set function for isAligned
    public void SetIsAlignedToTrue()
    {
        isAligned = true;
    }

    //Get function for isAligned
    public bool GetIsAligned()
    {
        return isAligned;
    }

    //Set function for isConnected
    public void SetIsConnectedToTrue()
    {
        this.isConnected = true;
    }

    //Get function for isConnected
    public bool GetIsConnected()
    {
        return this.isConnected;
    }

    //Set function for levelObject
    public void SetLevelObject(GameObject levelObject)
    {
        this.levelObject = levelObject;
    }

    //Get function for levelObject
    public GameObject GetLevelObject()
    {
        return GameObject.Find("Level");
    }

    //Set function for generator
    public void SetGeneratorObject(GameObject generator)
    {
        this.generatorObject = generator;
    }

    //Get function for generator
    public GameObject GetGeneratorObject()
    {
        return GameObject.Find("Generator");
    }

    //Set function for previousRoom
    public void SetPreviousRoom(GameObject previousRoom)
    {
        this.previousRoom = previousRoom;
    }

    //Get function for previousRoom
    public GameObject GetPreviousRoom()
    {
        return this.previousRoom;
    }

    //Obtains a room's game object from its collider
    public GameObject GetRoomGameObject(Collider collider)
    {
        GameObject room = collider.gameObject;
        while (room.tag!="New Room" && room.tag!="Previous Room" && room.tag != "Room")
        {
            room = room.transform.parent.gameObject;
        }
        return room;
    }

    public void setRoomType(string roomType)
    {
        this.roomType = roomType;
    }

    public string GetRoomType()
    {
        return this.roomType;
    }

    //Obtains a room's entry collider from its game object
    public Transform GetRoomEntry(GameObject room)
    {
        if (room != null)
        {

     
        Transform entryTransform = room.transform.Find("Entry Connector");
        return entryTransform;
        }
        return null;
    }

    //Obtains a room's main exit from its game object
    public Transform GetRoomMainExit(GameObject room)
    {
        if (room != null)
        {
            if (GetRoomEntry(room) == null)
            {
                Transform exitTransform = room.transform.Find("Main Exit Connector");
                return exitTransform;
            }
            else
            {
                Transform exitTransform = GetRoomEntry(room).Find("Main Exit Connector");
                return exitTransform;
            }
        }
        return null;
    }


    public Transform GetUnconnectedExit()
    {
        Transform unconnectedExit=null;
        Transform entry = GetRoomEntry(this.previousRoom);
        if (entry != null)
        {
            foreach (Transform child in entry)
            {
                if (child.tag == "Exit")
                {
                    unconnectedExit = child;
                    return unconnectedExit;
                }
            }
        }

        return unconnectedExit;
    }

    public void AssignNewMainExit()
    {
        Transform mainExit = GetRoomMainExit(this.previousRoom);
        mainExit.tag = "Connected_Exit";
        Transform newMainExit = GetUnconnectedExit();
        if (newMainExit != null)
        {
            newMainExit.tag = "Main_Exit";
        }

    }

    //Obtains the rooms collider from its game object
    public Transform GetColliderContainer(GameObject room)
    {
        Transform colliderContainerTransform;
        if (GetRoomEntry(room) == null)
        {
            colliderContainerTransform = this.gameObject.transform.Find("Collider Container");
            return colliderContainerTransform;
        }
        colliderContainerTransform = GetRoomEntry(room).Find("Collider Container");
        return colliderContainerTransform;
    }

    //Gets all of the rooms colliders
    public BoxCollider[] GetAllRoomColliders()
    {
        return this.transform.GetComponentsInChildren<BoxCollider>();
    }

    //Disables all of the colliders from the input array
    public void DisableColliderArray(BoxCollider[] colliderArray)
    {
        foreach (BoxCollider collider in colliderArray)
        {
            collider.enabled = false;
        }
    }

    //Enables all of the colliders from the input array
    public void EnableColliderArray(BoxCollider[] colliderArray)
    {
        foreach (BoxCollider collider in colliderArray)
        {
            collider.enabled = true;
        }
    }

    //Aligns a room's main exit with the next rooms entry
    public void AlignRoomsMainExit()
    {
        Transform entry = GetRoomEntry(this.gameObject);
        Transform mainExit = GetRoomMainExit(this.previousRoom);
        BoxCollider mainExitCollider = mainExit.GetComponent<BoxCollider>();
        BoxCollider entryCollider = entry.GetComponent<BoxCollider>();
        entryCollider.transform.position = mainExitCollider.transform.position;
        entryCollider.transform.forward = mainExitCollider.transform.forward;
        this.SetIsAlignedToTrue();
    }
    
    void OnDrawGizmos()
    {
        if (this.tag == "New Room")
        {
            // Draw a yellow cube at the transform position
            Gizmos.color = Color.red;
            Transform colliderContainer = GetColliderContainer(this.gameObject);
            BoxCollider roomCollider = colliderContainer.GetComponent<BoxCollider>();
            Vector3 roomColliderCenter = roomCollider.bounds.center;
            Vector3 castBoxSize = (roomCollider.bounds.size);
            Vector3 worldCenter = roomCollider.transform.TransformPoint(roomCollider.center);
            Vector3 worldHalfExtents = roomCollider.transform.TransformVector(roomCollider.size * 0.5f);
            //Gizmos.DrawWireCube(worldCenter, worldHalfExtents*2f);
        }
        //Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(1,1,1));
    }
    

    Vector3 AbsoluteValue(Vector3 absoluteVector)
    {
        return new Vector3(Mathf.Abs(absoluteVector.x), Mathf.Abs(absoluteVector.y), Mathf.Abs(absoluteVector.z));
    }

    //Checks if the room is colliding with any other room
    public bool CheckRoomCollisions()
    {
        bool isColliding = false;
        Transform colliderContainer = GetColliderContainer(this.gameObject);
        BoxCollider roomCollider = colliderContainer.GetComponent<BoxCollider>();
        Vector3 roomColliderCenter = colliderContainer.position;
        Vector3 castBoxSize = (roomCollider.bounds.size) / 2;
        BoxCollider previousRoomExitCollider = GetRoomMainExit(this.previousRoom).GetComponent<BoxCollider>();
        BoxCollider[] allRoomColliders = GetAllRoomColliders();
        Vector3 worldCenter = roomCollider.transform.TransformPoint(roomCollider.center);
        Vector3 worldHalfExtents = AbsoluteValue(roomCollider.transform.TransformVector(roomCollider.size * 0.5f));

        DisableColliderArray(allRoomColliders);
        previousRoomExitCollider.enabled = false;
        isColliding = Physics.CheckBox(worldCenter, worldHalfExtents);


        previousRoomExitCollider.enabled = true;
        EnableColliderArray(allRoomColliders);
        return isColliding;
    }

    //Instantiates the next room in the queue
    public void InstantiateNextRoom(string nextRoom, GameObject level)
    {
        GameObject roomAsset = this.generatorObject.GetComponent<Generator>().GetRoomAssetFromName(nextRoom);
        GameObject room = Instantiate(roomAsset, this.transform.position, Quaternion.identity, level.transform);
        room.tag = "New Room";
        room.GetComponent<Room>().SetPreviousRoom(this.gameObject);
        room.GetComponent<Room>().roomType = nextRoom;
        //generatorObject.GetComponent<Generator>().AddRoomToCurrentList(room);
    }

    //Instantiates the end piece
    public void InstantiateEndPiece(GameObject level)
    {
        GameObject roomAsset = this.generatorObject.GetComponent<Generator>().GetEndPiece();
        GameObject room = Instantiate(roomAsset, this.transform.position, Quaternion.identity, level.transform);
        room.tag = "New Room";
        room.GetComponent<Room>().SetPreviousRoom(this.gameObject);
    }


    public void BacktrackRoom()
    {
        generatorObject.GetComponent<Generator>().BacktrackRooms();
    }


}