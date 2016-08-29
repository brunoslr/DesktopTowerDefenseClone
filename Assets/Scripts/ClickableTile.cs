using UnityEngine;

/// <summary>
///Class that countain places towers on attached tiles 
/// and contains visual aids to place it
/// </summary>
public class ClickableTile : MonoBehaviour {

    public int tileX;
    public int tileY;

    public Color hoverColor;

    private Renderer rend;
    private Color startColor;

    void Start()
    {

        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }


    void OnMouseDown()
    {
        BuildManager.instance.BuildTowerOnPosition(tileX, tileY, transform.position);

    }

    void OnMouseEnter()
    {
        rend.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
    }


}
