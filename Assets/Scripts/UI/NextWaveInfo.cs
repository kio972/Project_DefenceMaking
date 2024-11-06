using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx.Triggers;

public class NextWaveInfo : MonoBehaviour
{
    [SerializeField]
    GameObject waveIcons;
    [SerializeField]
    GameObject waveBtn;

    List<NextWaveInfoSlot> infoSlots;

    [SerializeField]
    private Image btnUI;

    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        waveIcons.SetActive(false);
        waveBtn.SetActive(true);

        infoSlots = transform.GetComponentsInChildren<NextWaveInfoSlot>(true).ToList();

        GameManager.Instance.timer.Where(x => x > 720f && !gameObject.activeSelf)
            .Subscribe(x =>
            {
                bool isUpated = UpdateWaveInfo();
                gameObject.SetActive(isUpated);
            });
        GameManager.Instance.timer.Where(x => x <= 720f && gameObject.activeSelf).Subscribe(x => gameObject.SetActive(false));

        await UniTask.WaitUntil(() => GameManager.Instance.IsInit, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        transform.position = NodeManager.Instance.startPoint.transform.position;

        btnUI.OnPointerEnterAsObservable().Subscribe(_ => waveIcons.SetActive(true)).AddTo(gameObject);
        btnUI.OnPointerExitAsObservable().Subscribe(_ => waveIcons.SetActive(false)).AddTo(gameObject);
    }

    private List<WaveData> GetWaveSummary(List<WaveData> waveData)
    {
        Dictionary<string, int> waveCounts = new Dictionary<string, int>();
        Dictionary<string, string> waveTarget = new Dictionary<string, string>();
        Dictionary<string, CardGrade> waveGrade = new Dictionary<string, CardGrade>();
        foreach (WaveData wave in waveData)
        {
            int index = UtilHelper.Find_Data_Index(wave.adventurerName, DataManager.Instance.battler_Table, "name");
            string type = DataManager.Instance.battler_Table[index]["type"].ToString();

            if (!waveCounts.ContainsKey(type))
            {
                waveCounts.Add(type, 0);
                waveTarget.Add(type, null);
                waveGrade.Add(type, CardGrade.none);
            }

            waveCounts[type] += wave.number; //같은 종류의 모험가수 합산
            if (System.Enum.TryParse(DataManager.Instance.battler_Table[index]["rate"].ToString(), out CardGrade grade))
            {
                if ((int)grade > (int)waveGrade[type]) //등급이 높은 모험가를 대상 모험가로 지정
                {
                    waveGrade[type] = grade;
                    waveTarget[type] = wave.adventurerName;
                }
            }
        }

        List<WaveData> waves = new List<WaveData>();
        foreach(string type in waveCounts.Keys)
            waves.Add(new WaveData(waveTarget[type], waveCounts[type]));

        return waves;
    }

    private bool UpdateWaveInfo()
    {
        int nextWave = GameManager.Instance.CurWave + 1;
        if (nextWave < 0 || nextWave >= DataManager.Instance.waveLevelTable.Count)
            return false;

        List<WaveData> waveData = waveData = new List<WaveData>(DataManager.Instance.waveLevelTable[nextWave]);
        if (waveData.Count <= 0)
            return false;

        waveData = GetWaveSummary(waveData);

        foreach (var item in infoSlots)
            item.gameObject.SetActive(false);

        for(int i = 0; i < waveData.Count; i++)
        {
            if(infoSlots.Count <= i)
                infoSlots.Add(Instantiate(infoSlots[0], infoSlots[0].transform.parent));

            infoSlots[i].SetWaveInfo(waveData[i]);
            infoSlots[i].gameObject.SetActive(true);
        }

        return true;
    }
}
