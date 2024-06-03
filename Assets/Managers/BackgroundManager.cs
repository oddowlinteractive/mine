using UnityEngine;

namespace Managers
{
    public class BackgroundManager : MonoBehaviour
    {
        [Header("Managers")]
        public GridManager gm;
        
        [Header("Sky Backgrounds")]
        public SpriteRenderer skyboxSky;
        public SpriteRenderer skyboxMountains;
        public SpriteRenderer skyboxHills;
        public SpriteRenderer skyboxTrees;
    
        [Header("Cave Backgrounds")]
        public SpriteRenderer caveWallsDeep;
        public SpriteRenderer caveWalls;
        public SpriteRenderer caveForeground;

    
        // Start is called before the first frame update
        void Start()
        {
            Configure();
            
        }

        // Update is called once per frame
        void Configure()
        {
            skyboxSky.transform.position = new Vector3(0,(float)(gm.mapHeight + skyboxSky.size.y * 0.5f),0);
        }
    }
}
