using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Minimap : MonoBehaviour {

	// The color used for drawing. Will be updated when selecting a terrain area.
	public Color drawingColor = Color.white;

	// The resoulution of a chunk in pixels of the minimap texture
	public int chunkResolution = 5;

	// The texture of the map
	private Texture2D minimapTexture;

	// The RectTransform component of the minimap
	private RectTransform myRectTransform;

	// The RawImage component of the minimap
	private RawImage minimapUIElement;


	// Use this for initialization
	void Start () {
		//Initialize pointers to components
		myRectTransform = gameObject.GetComponent<RectTransform> ();
		minimapUIElement = gameObject.GetComponent<RawImage> ();

		//Create the default minimap texture
		int size = TerrainCharacteristicsManager.Instance.mapSize * chunkResolution;
		minimapTexture = new Texture2D(size, size);
		minimapTexture.wrapMode = TextureWrapMode.Clamp; //Make the edges of the image not take the opposite pixels, like with tiling.
		for (int i = 0; i < minimapTexture.width; i++) {
			for (int j = 0; j < minimapTexture.height; j++) {
				minimapTexture.SetPixel(i, j, Color.white);
			}
		}
		minimapTexture.Apply ();
		minimapUIElement.texture = minimapTexture;
	}

	public void DrawOnMinimap(BaseEventData bed) {
		Vector2 normCoords = PointerPositionToNormalizedImageCoordinates ((PointerEventData)bed);

		// Get the texture coordinates
		if (normCoords.x < 0 || normCoords.x > 1 || normCoords.y < 0 || normCoords.y > 1)
			return; // Coloring outside the image --> ignore
		Vector2 textureCoords = new Vector2(Mathf.Lerp(0, minimapTexture.width, normCoords.x),
		                                    Mathf.Lerp(0, minimapTexture.height, normCoords.y));

		// Optimization for coloring the same color
		if (drawingColor.Equals(minimapTexture.GetPixel ((int)textureCoords.x, (int)textureCoords.y)))
			return;

		// Draw on the texture pixel
		minimapTexture.SetPixel ((int)textureCoords.x, (int)textureCoords.y, drawingColor);
		minimapTexture.Apply ();

		// Reload ChunkLoader. TODO: Possible optimization: reload only affected chunks
		ChunkLoader.Instance.ReloadAllChunks ();
	}

	// Gives the color of the requested pixel of the minimap.
	// If the pixel coords are out of boundaries, it will return the color of the closest pixel.
	public Color getMinimapTexturePixel(int x, int y) {
		return minimapTexture.GetPixel ((int)Mathf.Clamp(x, 0, minimapTexture.width - 1), 
		                                (int)Mathf.Clamp(y, 0, minimapTexture.height - 1));
	}

	private Vector2 PointerPositionToNormalizedImageCoordinates(PointerEventData ped) {
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (myRectTransform, ped.position, ped.pressEventCamera, out localPoint); //Get the coordinates of the pointer relative to this object
		localPoint += myRectTransform.rect.size / 2; //Make (0,0) the bottom left corner
		return new Vector2(localPoint.x / myRectTransform.rect.size.x, localPoint.y / myRectTransform.rect.size.y); //Normalize to [0,1] intervals (inside the image)
	}
}
