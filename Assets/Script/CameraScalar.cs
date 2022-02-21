using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float padding = 2;
    public float aspectRatio = 0.625f;
    public float yOffset = 1;
     

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

        // width >= heigh => offSize = (w/2 + padd) / 0.625 else offSize = w/2 + padd
        if(board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPosition;
        if(board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
