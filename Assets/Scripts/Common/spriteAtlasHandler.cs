using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class spriteAtlasHandler : MonoBehaviour
{
    [SerializeField]
    SpriteAtlas atlas;

    [SerializeField]
    string woodlandSet;

    protected Sprite[] allSpriteSets;
    protected List<Sprite> woodlandSpriteSets;

    // Start is called before the first frame update
    void Start()
    {
        collectAllSprites();
        // ListSpriteNames();
        //GetComponent<Image>().sprite = atlas.GetSprite(spriteName);   
        woodlandSpriteSets = collectSpriteSets(woodlandSet);
    }

    // list down sprite names if possible
    private void ListSpriteNames()
    {
        Debug.Log("Listing down sprites");
        foreach (Sprite sprite in allSpriteSets) Debug.Log(sprite.name);
    }

    // keep a master list of sprites present in the atlas
    private void collectAllSprites()
    {
        List<Sprite> sprites = new List<Sprite>();
        allSpriteSets = new Sprite[atlas.spriteCount];
        atlas.GetSprites(allSpriteSets);
    }

    // get a specific set of sprites for a specific tilemap
    private List<Sprite> collectSpriteSets(string assetPrefix)
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (Sprite sprite in allSpriteSets) {
            if (sprite.name.StartsWith(assetPrefix))
            {
                sprites.Add(sprite);
            }
        }

        return sprites;
    }
}
