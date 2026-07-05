using UnityEngine;
using System.Collections.Generic;

public class ProceduralSlot : MonoBehaviour
{
    public VisualType acceptedVisualType;

    public SlotSize acceptedSlotSize;

    public PlacementZone acceptedPlacementZone;

    public ObjectDatabase database;

    private SpriteRenderer targetRenderer;

    public ProceduralSpriteData currentData;
    
    void Awake()
    {
        targetRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //Debug.Log("ProceduralSlot START: " + gameObject.name);
        GenerateSprite();
    }

    public void GenerateSprite()
    {
        //Debug.Log(" Generando sprites para " + gameObject.name);
        //Debug.Log("Sprites en la DB: " + database.sprites.Count);
        List<ProceduralSpriteData> compatibleSprites =
            new List<ProceduralSpriteData>();

        foreach (ProceduralSpriteData data
            in database.sprites)
        {
            bool compatible = true;

            if (data.visualType != acceptedVisualType)
            {
                compatible = false;
            }
            if(data.visualType != VisualType.Floor && data.visualType != VisualType.Ceiling){
                if (data.slotSize != acceptedSlotSize)
                {
                    compatible = false;
                }
                if(acceptedSlotSize == SlotSize.Half)
                {
                    if (data.placementZone != acceptedPlacementZone)
                    {
                        compatible = false;
                    }
                }
            }
            
            if (compatible)
            {
                compatibleSprites.Add(data);
            }
        }

        //Debug.Log(
        //    gameObject.name +
        //    " compatibles: " +
        //    compatibleSprites.Count
        //);

        if (compatibleSprites.Count > 0)
        {
            
            ProceduralSpriteData chosen =
                compatibleSprites[
                    Random.Range(
                        0,
                        compatibleSprites.Count
                    )
                ];

            //Debug.Log(
            //    gameObject.name +
            //    " eligió: " +
            //    chosen.sprite.name
            //);

            currentData = chosen;
            targetRenderer.sprite = chosen.sprite;

            SpriteAnimator animator = GetComponent<SpriteAnimator>();

            if (animator != null)
            {
                animator.SetOriginalSprite(chosen.sprite);
            }

            // COLLIDER RESET LIMPIO
            BoxCollider box = GetComponent<BoxCollider>();
            PolygonCollider2D poly = GetComponent<PolygonCollider2D>();

            // eliminar ambos primero (evita conflictos)
            if (box != null)
                Destroy(box);

            if (poly != null)
                Destroy(poly);

            // decidir según el sprite elegido
            if (chosen.visualType == VisualType.FlatWall)
            {
                gameObject.AddComponent<BoxCollider>();
            }
            else
            {
                gameObject.AddComponent<PolygonCollider2D>();
            }
        }
    }

    public void Regenerate()
    {
        GenerateSprite();
    }
}