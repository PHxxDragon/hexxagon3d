using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Material distance1Material;
    [SerializeField] private Material distance2Material;
    [SerializeField] private GameObject selectorPrefab;

    private List<GameObject> instantiatedSelectors = new List<GameObject>();

    public void ShowSelection(Dictionary<Vector3, bool> tileData)
    {
        ClearSelection();
        foreach (var data in tileData)
        {
            GameObject selector = Instantiate(selectorPrefab, data.Key, Quaternion.identity);
            instantiatedSelectors.Add(selector);
            foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>())
            {
                setter.SetSingleMaterial(data.Value ? distance1Material : distance2Material);
            }
        }
    }

    public void ClearSelection()
    {
        foreach (var selector in instantiatedSelectors)
        {
            Destroy(selector.gameObject);
        }
        instantiatedSelectors.Clear();
    }
}
