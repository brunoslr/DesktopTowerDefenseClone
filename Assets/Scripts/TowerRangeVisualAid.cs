using UnityEngine;


/// <summary>
/// This class implements an visual aid to show the towers range in game 
/// using a transparent sphere prefab that is enableb on mouse enter and 
/// disabled on mouse exit
/// </summary>
public class TowerRangeVisualAid : MonoBehaviour {

    public GameObject rangeSpherePrefab;
    public Color hoverColor;

    private Renderer rend;
    private Color startColor;

    void Start()
    {

        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }


    void OnMouseEnter()
    {
        rend.material.color = hoverColor;
        rangeSpherePrefab.SetActive(true);
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
        rangeSpherePrefab.SetActive(false);
    }

}
