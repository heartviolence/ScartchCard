using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCard : MonoBehaviour
{
    public enum CardImage
    {
        Gold =0,
        Meat =1,
        Howl=2,
        DogFood=3,
        Bone=4
    }
    public enum RewardType
    {
        PolyChrome,
        Denny
    }
    public class Reward
    {
        public RewardType type;
        public int count = 0;
    }

    public class GachaRecord
    {
        public Reward reward;
        public float probability;
        public CardImage image;
    }

    [SerializeField]
    Image imageObject;

    [SerializeField]
    PaintWithMouseUI rayer;

    [SerializeField]
    List<Sprite> sprites;

    [SerializeField]
    Button nextButton;

    int day = 0;

    List<GachaRecord> specialTable;
    List<GachaRecord> normalTable;

    [SerializeField]
    RewardManager rewardManager;

    [SerializeField]
    TMP_Text polyChromeText;

    [SerializeField]
    TMP_Text dennyText;

    [SerializeField]
    TMP_Text daysText;

    GachaRecord currentGacha;
    bool isUpdateGachaReward=true;

    private void Start()
    {
        specialTable = new List<GachaRecord>()
        {
            new GachaRecord() {reward = new Reward(){type = RewardType.PolyChrome, count =20 }, probability = 63.99f,image=CardImage.Howl },
            new GachaRecord() {reward = new Reward(){type = RewardType.PolyChrome, count =40 }, probability = 30f,image=CardImage.Howl },
            new GachaRecord() {reward = new Reward(){type = RewardType.PolyChrome, count =60 }, probability = 6f ,image=CardImage.Gold },
            new GachaRecord() {reward = new Reward(){type = RewardType.PolyChrome, count =100 }, probability = 0.01f ,image=CardImage.Gold}
        };

        normalTable = new List<GachaRecord>()
        {
            new GachaRecord() {reward = new Reward(){type = RewardType.Denny, count =5888 }, probability = 35f,image=CardImage.DogFood },
            new GachaRecord() {reward = new Reward(){type = RewardType.Denny, count =2888 }, probability = 29f ,image=CardImage.Bone},
            new GachaRecord() {reward = new Reward(){type = RewardType.Denny, count =8888 }, probability = 20f,image=CardImage.Meat },
            new GachaRecord() {reward = new Reward(){type = RewardType.Denny, count =12888 }, probability = 10f ,image=CardImage.Meat},
            new GachaRecord() {reward = new Reward(){type = RewardType.PolyChrome, count =10 }, probability = 6f ,image=CardImage.Meat}
        };
        nextButton.onClick.AddListener(Next);
        rayer.OnClear += () => UpdateReward();
    } 

    public void Next()
    {
        if(isUpdateGachaReward == false)
        {
            return;
        }
        day++;
        if (day > 30)
        {
            day = 1;
        }
        daysText.text = $"{day.ToString()} Days";

        GachaRecord record =null;
        if (day == 3 ||
            day == 8 ||
            day == 15 ||
            day == 20 ||
            day == 27)
        {
            record = SpecialRandom();
        }
        else
        {
            record = NormalRandom();
        }
        currentGacha = record;
        isUpdateGachaReward = false;
        Change(record.image);
    }

    void Change(CardImage cardimage)
    {
        rayer.ResetAndDraw();
        imageObject.sprite = sprites[(int)(cardimage)];
    }

    GachaRecord SpecialRandom()
    {
        return Gacha(specialTable);
    }

    GachaRecord NormalRandom()
    {
        return Gacha(normalTable);
    }

    GachaRecord Gacha(List<GachaRecord> table)
    {
        var rd = UnityEngine.Random.Range(0f, 100f);
        float acc = 0f;

        foreach (var record in table)
        {
            acc += record.probability;
            if (rd <= acc)
            {
                return record;
            }
        }

        return null;
    }

    void UpdateReward()
    {
        if(isUpdateGachaReward || currentGacha == null)
        {
            return;
        }
        isUpdateGachaReward = true;

        switch(currentGacha.reward.type)
        {
            case RewardType.PolyChrome:
                UpdatePolyChrome(currentGacha.reward.count);
                break;
            case RewardType.Denny:
                UpdateDenny(currentGacha.reward.count);
                break;
        }
    }

    void UpdateDenny(int Count)
    {
        rewardManager.AddDenny(Count);
        dennyText.text = Count.ToString();
        polyChromeText.text = "0";
    }

    void UpdatePolyChrome(int Count)
    {
        rewardManager.AddPolyChrome(Count);
        polyChromeText.text = Count.ToString();
        dennyText.text = "0";
    }
}
