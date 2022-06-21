using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTileOccupation
{
    EMPTY,
    PLAYER,
    OCCUPIED,
    //DEAD_END,
}

public class TileNode : MonoBehaviour
{
    public GameObject objectOnTile;         //??Όμ΄ NULL???λ ???μ ?λ ?€λΈ?νΈ
    public eTileOccupation eOnTileObject;   //??Όμ ?μ ?ν (λΉμ??, ?λ ?΄μ΄κ° ?μ ?μ , ?μ΄?μ΄ ?μ ?μ)
    public Point tilePos = new Point();


    public eTileOccupation onTileObject;
    public int posX;
    public int posY;

    RaycastHit hit;

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.up, 2f, LayerMask.GetMask("Wall")))
        {
            this.eOnTileObject = eTileOccupation.OCCUPIED;
        }

        // Debug.DrawRay(transform.position, Vector3.up * 2f, Color.red);
    }
}
