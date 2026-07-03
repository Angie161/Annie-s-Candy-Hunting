using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProceduralSpriteData
{
    public Sprite sprite;

    public VisualType visualType;

    public SlotSize slotSize;

    public PlacementZone placementZone;

    public List<SpriteAnimationData> anomalies =
        new List<SpriteAnimationData>();

    public List<SpriteAnimationData> distractions =
        new List<SpriteAnimationData>();
}