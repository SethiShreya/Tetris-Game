using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockBehaviour : MonoBehaviour
{
    public float fallTime=1f;
    private float previousTime;
    private SoundManager soundManager;
    private static int width = 10;
    private static int height = 20;
    public Vector3 rotationPoint;
    private static Transform[,] grid = new Transform[width, height];
    private UIManager uIManager;

    private void Start()
    {

        uIManager = GameObject.FindObjectOfType<UIManager>();
        if (uIManager == null)
        {
            Debug.Log("Ui manager is not found");
        }
        soundManager = GameObject.FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.Log("sound manager not found");
        }   
    }

    void Update()
    {
        // for Movement of block
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left;
            if (!isValid())
            {
                transform.position -= Vector3.left;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right;
            if (!isValid())
            {
                transform.position -= Vector3.right;
            }
        }

        // rotation
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);

            if (!isValid())
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }

        }


        // speed 
        

        if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
            previousTime = Time.time;
            transform.position += Vector3.down;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                uIManager.IncreaseScore();
            }

            soundManager.SelectAudio(1, 0.5f);

            if (!isValid())
            {
                transform.position -= Vector3.down;
                SpawnManager spawner = FindObjectOfType<SpawnManager>();
                if (spawner == null)
                {
                    Debug.Log("Spawn Manager Component not found");
                }
                else
                {
                    AddToGrid();
                    // it is disabling the script ahead functions will still works the same only update function will not be called as the script is being disabled
                    this.enabled = false;
                    // it is checking for the lines and reducing it
                    CheckForLines();
                    //// spawning a new block
                    if (!uIManager.isGameOver)
                    {
                        spawner.SpawnBlock();
                    }

                }

            }
        }
    }

    

    void AddToGrid()
    {
        foreach(Transform children in transform)
        {
            var roundedX = Mathf.RoundToInt(children.position.x);
            var roundedY = Mathf.RoundToInt(children.position.y);
            grid[roundedX, roundedY] = children;
        }
    }

    bool isValid()
    {
        foreach(Transform children in transform)
        {
            var roundedX = Mathf.RoundToInt(children.position.x);
            var roundedY = Mathf.RoundToInt(children.position.y);

            
            //if (roundedY > height - 3)
            //{
            //    Debug.Log("Exceed the height");
            //    uIManager.isGameOver = true;   
            //}

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                
               return false;
            }

            
            // this function is acting like a collider as if the grid already have a value that is a block then other block cannot go at its place
            if (grid[roundedX, roundedY] != null)
            {
                return false;
            }

        }
        return true;
    }

    void CheckForLines()
    {
        for (int i= height-1; i>=0; i--)
        {
            if (HasLine(i))
            {
                uIManager.GetComponent<UIManager>().IncreaseScore(50);
                soundManager.SelectAudio(2);
                DeleteRow(i);
                ReduceRow(i);
                soundManager.SelectAudio(3);
            }
        }
    }

    bool HasLine(int line)
    {
        for (int i=0; i<width; i++)
        {
            if (grid[i, line] == null)
            {
                return false;
            }
        }
        return true;
    }

    void DeleteRow(int line)
    {
        for (int i=0; i<width; i++)
        {
            if (grid[i, line] == null)
            {
                
                continue;
            }
            
            
            Destroy(grid[i, line].gameObject);
            grid[i, line] = null;
        }
    }

    void ReduceRow(int line)
    {
        for (int r=line; r<height-2; r++)
        {
            for (int c = 0; c < width; c++)
            {
                
                
                if (grid[c, r+1] != null)
                {
                    grid[c, r] = grid[c, r + 1];
                    grid[c, r].gameObject.transform.position -= new Vector3(0, 1, 0);
                }
                else
                {
                    grid[c, r] = grid[c, r + 1];
                }
                                
            }
        }
        
    }
}

