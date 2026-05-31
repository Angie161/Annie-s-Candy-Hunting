using UnityEngine;
using System.Collections.Generic;

public class ProceduralSlot : MonoBehaviour
{
    public VisualType acceptedVisualType;

    public SlotSize acceptedSlotSize;

    public PlacementZone acceptedPlacementZone;

    public ObjectDatabase database;

    private SpriteRenderer targetRenderer;
    
    void Awake()
    {
        targetRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        GenerateSprite();
    }

    public void GenerateSprite()
    {
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

        if (compatibleSprites.Count > 0)
        {
            ProceduralSpriteData chosen =
                compatibleSprites[
                    Random.Range(
                        0,
                        compatibleSprites.Count
                    )
                ];

            targetRenderer.sprite = chosen.sprite;

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
}