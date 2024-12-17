using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IKeyControl
{
    void HandleInput(KeyCode key);
}

public class ResearchKeyController : MonoBehaviour, IKeyControl
{
    [SerializeField]
    private List<GameObject> researchPages = new List<GameObject>();
    private int _row = 0;
    private int _col = 0;

    private Dictionary<GameObject, ScrollRect> researchScrollDic;
    private Dictionary<GameObject, BiMap<ResearchSlot, (int row, int col)>> researchBiMapDic;

    public void HandleInput(KeyCode key)
    {


    }

    private void Start()
    {
        researchScrollDic = new Dictionary<GameObject, ScrollRect>();
        researchBiMapDic = new Dictionary<GameObject, BiMap<ResearchSlot, (int row, int col)>>();

        foreach (var page in researchPages)
        {
            ScrollRect researchPage = page.GetComponentInChildren<ScrollRect>(true);
            researchScrollDic[page] = researchPage;

            GridLayoutGroup group = page.GetComponentInChildren<GridLayoutGroup>(true);
            int rowCount = group.constraintCount;
            BiMap<ResearchSlot, (int row, int col)> biMap = new BiMap<ResearchSlot, (int row, int col)>();
            ResearchSlot slots = group.GetComponentInChildren<ResearchSlot>(true);
        }
    }
}
