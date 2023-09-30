using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IF THE PLAYER AND STARTOBJECT ARE NOT WITHIN LINE OF SIGHT OF EACH OTHER, THIS PROBABLY WONT WORK
// MUST ADJUST IF THIS BECOMES AN ISSUE LATER
public class AttachToRope : MonoBehaviour
{
    // get the start and end positions of object we are trying to attach to
    public GameObject startObject, endObject;
    private Vector3[] lineIndexes;
    private float distanceFromIndex, shortestDistance = 10000;
    private Vector3 newPlayerPos;
    private float startX, startY, startZ;
    private float endX, endY, endZ;
    
    // Start is called before the first frame update
    void Start()
    {
        
        startX = startObject.transform.position.x;
        startY = startObject.transform.position.y;
        startZ = startObject.transform.position.z;
        endX = endObject.transform.position.x;
        endY = endObject.transform.position.y;
        endZ = endObject.transform.position.z;

        /*
        Vector3 differenceStartToEnd;
        Vector3 differenceStartToPlayer;
        */ 

        RaycastHit hit;

        // if the player and the startObject are within light of sight of each other
        if (Physics.Raycast(transform.position, startObject.transform.position-transform.position, out hit, Vector3.Magnitude(startObject.transform.position-transform.position)))
        {            
            Debug.Log(hit.collider.gameObject);
             /*
            // get the distance between the start and end objects
            differenceStartToEnd = new Vector3(
                startX - endX,
                startY - endY,
                startZ - endZ
            );
            
            Debug.Log(differenceStartToEnd);

            // get the difference between the startObject and the player
            differenceStartToPlayer = new Vector3(
                startX - transform.position.x,
                startY - transform.position.y,
                startZ - transform.position.z
            );

            Debug.Log(differenceStartToPlayer);

            if(startX > endX)
            {
                transform.position.x = (startX - differenceStartToPlayer.x);
            }
            else
            {
                transform.position.x = (startX + differenceStartToPlayer.x);
            }
            */

            // get the number of line indexes created from line renderer
            lineIndexes = new Vector3[startObject.GetComponent<LineRenderer>().positionCount];

            // put all line indexes in a vector3 array
            startObject.GetComponent<LineRenderer>().GetPositions(lineIndexes);

            // find closet lineIndex to the player, and attach player to that lineIndex
            for(int i = 0; i < lineIndexes.Length-1; i++)
            {   
                // find the closet lineIndex to the player
                distanceFromIndex = Vector3.Distance (lineIndexes[i], transform.position);
                
                //Debug.Log(distanceFromIndex);
                // determine if the latest player/lineIndex distance is shorter than the previous one
                if (distanceFromIndex < shortestDistance)
                {
                    newPlayerPos = lineIndexes[i];
                    shortestDistance = distanceFromIndex;
                }
                //Debug.Log(lineIndexes[i]);
            }

            Debug.Log(newPlayerPos);
            // update the player's transform to the nearest lineIndex
            
            transform.position = newPlayerPos;
        }
        else
        {            
            Debug.Log("Did not Hit");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
