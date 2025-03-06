using UnityEngine;
using DTerrain;

public class TerrainManager : MonoBehaviour
{
    private const float CRANE_CLEAR_RADIUS = 150f;

    public static TerrainManager singleton;

    //[SerializeField] private int outlineSize = 4;

    [SerializeField] private BasicPaintableLayer primaryLayer = default;
    [SerializeField] private BasicPaintableLayer secondaryLayer = default;

    [SerializeField] private Sprite terrainSprite = default;

    [SerializeField] private GameObject alien = null;

    private Shape[] destroyCircle = new Shape[500];
    //private Shape[] outlineCircle = new Shape[100];

    private Shape layerShape;

    public int PPU { get; private set; }

    private void Awake()
    {
        singleton = this;

        PPU = primaryLayer.PPU;

        layerShape = Shape.GenerateShapeRect(terrainSprite.texture.width, Mathf.RoundToInt(CRANE_CLEAR_RADIUS * PPU)/*terrainSprite.texture.height*/);

        Invoke("SpawnAliens", 0.1f);
    }

    private void SpawnAliens() {
        //Generate small pockets with aliens in them
        for(int i = 0; i < 10; i++) {
            Vector3 alienPos = new Vector3(Random.Range(1.0f, 23.0f), Random.Range(4.0f, 29.0f), 0);
            Explode(alienPos, 1*PPU);
            Instantiate(alien, alienPos, Quaternion.identity);
        }
    }

    private Shape GetDestroyCircle(int size)
    {
        if(!destroyCircle[size])
            destroyCircle[size] = Shape.GenerateShapeCircle(size);// * PPU);

        return destroyCircle[size];
    }

    public void Explode(Vector3 position, int size)
    {
        //int unscaledSize = Mathf.RoundToInt((float)size / PPU);

        if(size >= destroyCircle.Length)
            Debug.LogError($"Explode was called with too big size. Unscaled size: {size}, Max: {destroyCircle.Length - 1}");

        primaryLayer?.Paint(new PaintingParameters()
        {
            Color = Color.clear,
            Position = new Vector2Int((int)(position.x * primaryLayer.PPU) - size, (int)(position.y * primaryLayer.PPU) - size),
            Shape = GetDestroyCircle(size),
            PaintingMode = PaintingMode.REPLACE_COLOR,
            DestructionMode = DestructionMode.DESTROY
        });

        secondaryLayer?.Paint(new PaintingParameters()
        {
            Color = Color.clear,
            Position = new Vector2Int((int)(position.x * secondaryLayer.PPU) - size, (int)(position.y * secondaryLayer.PPU) - size),
            Shape = GetDestroyCircle(size),
            PaintingMode = PaintingMode.REPLACE_COLOR,
            DestructionMode = DestructionMode.NONE
        });
    }

    public void ClearCrane(float altitude)
    {
        const int size = (int)CRANE_CLEAR_RADIUS;

        primaryLayer?.Paint(new PaintingParameters()
        {
            Color = Color.clear,
            Position = new Vector2Int(0, (int)(altitude * primaryLayer.PPU) - size),
            Shape = layerShape,
            PaintingMode = PaintingMode.REPLACE_COLOR,
            DestructionMode = DestructionMode.DESTROY
        });

        secondaryLayer?.Paint(new PaintingParameters()
        {
            Color = Color.clear,
            Position = new Vector2Int(0, (int)(altitude * secondaryLayer.PPU) - size),
            Shape = layerShape,
            PaintingMode = PaintingMode.REPLACE_COLOR,
            DestructionMode = DestructionMode.NONE
        });
    }

    /*
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            primaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.clear,
                Position = Vector2Int.zero,
                Shape = layerShape,
                PaintingMode = PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.DESTROY
            });

            secondaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.clear,
                Position = Vector2Int.zero,
                Shape = layerShape,
                PaintingMode = PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.NONE
            });
        }
    }*/
}
