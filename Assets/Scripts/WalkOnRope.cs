using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IF THE PLAYER AND STARTOBJECT ARE NOT WITHIN LINE OF SIGHT OF EACH OTHER, THIS PROBABLY WONT WORK
// MUST ADJUST IF THIS BECOMES AN ISSUE LATER
public class WalkOnRope : MonoBehaviour
{
    // get the start and end positions of object we are trying to attach to
    public GameObject startObject, endObject;
    private Vector3[] lineIndexes;
    private float distanceFromIndex, shortestDistance = 10000;
    private Vector3 newPlayerPos;
    private float startX, startY, startZ;
    private float endX, endY, endZ;
    private int currentIndex, nextIndex, prevIndex;
    private float forwardInput;
    public float speed = 20.0f;
    
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
                    // position to update the player to
                    newPlayerPos = lineIndexes[i];

                    currentIndex = i;
                    nextIndex = currentIndex + 1;
                    prevIndex = currentIndex - 1;

                    shortestDistance = distanceFromIndex;
                }
                //Debug.Log(lineIndexes[i]);

            }
            

            Debug.Log(Vector3.Distance( lineIndexes[0], lineIndexes[1]));
            Debug.Log(newPlayerPos);
            // update the player's transform to the nearest lineIndex
            
            
        }
        else
        {            
            Debug.Log("Did not Hit");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        forwardInput = Input.GetAxis("Vertical");
        
        
        
        /*
        // Move the vehicle forward based on vertical input
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);

        transform.position = newPlayerPos;
        transform.LookAt(endObject.transform);
        */
    
        if(Input.GetButton("Vertical"))
        {
            // while player is holding forward, move towards next vertice
            if(Input.GetKey(KeyCode.W))
            {
                // move player to position of next index in lineRenderer
                transform.position = Vector3.MoveTowards(transform.position, lineIndexes[nextIndex], (speed * Time.deltaTime * forwardInput));
            }
            
            // while player is holding back, move towards previous vertice
            if(Input.GetKey(KeyCode.S))
            {
                // move player to position of next index in lineRenderer
                transform.position = Vector3.MoveTowards(transform.position, lineIndexes[prevIndex], (speed * Time.deltaTime * -forwardInput));
            }

            // check if player is close enough to the nextIndex position
            if (Vector3.Distance(transform.position, lineIndexes[nextIndex]) < 0.001f)
            {
                // change currentIndex, nextIndex, and prevIndex
                currentIndex = nextIndex;

                // if theres more rope to travel on going forwards
                if(nextIndex < lineIndexes.Length - 1)
                {
                    nextIndex = currentIndex + 1;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, lineIndexes[prevIndex], (speed * Time.deltaTime * forwardInput));
                }
                // if theres more rope to travel on going backwards
                if(prevIndex > 0)
                {
                    prevIndex = currentIndex - 1;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, lineIndexes[nextIndex], (speed * Time.deltaTime * forwardInput));
                }
            }
            if(Vector3.Distance(transform.position, lineIndexes[prevIndex]) < 0.001f)
            {
                // change currentIndex, nextIndex, and prevIndex
                currentIndex = prevIndex;

                // if theres more rope to travel on going forwards
                if(nextIndex < lineIndexes.Length - 1)
                {
                    nextIndex = currentIndex + 1;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, lineIndexes[prevIndex], (speed * Time.deltaTime * forwardInput));
                }
                // if theres more rope to travel on going backwards
                if(prevIndex > 0)
                {
                    prevIndex = currentIndex - 1;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, lineIndexes[nextIndex], (speed * Time.deltaTime * forwardInput));
                }
            }
        }
        
        transform.LookAt(endObject.transform);
    }
}
