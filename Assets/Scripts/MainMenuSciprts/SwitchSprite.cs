using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSprite : MonoBehaviour
{
    [SerializeField] public Image ProfileImg;
    [SerializeField] public Sprite Sprite1;
    [SerializeField] public Sprite Sprite2;

    private List<Sprite> spriteList;

    void Start()
    {
        spriteList = new List<Sprite>();
        spriteList.Add(Sprite1);
        spriteList.Add(Sprite2);
    }

    public void PreviousBtnClick()
    {
        int index = spriteList.FindIndex(current => current == ProfileImg.sprite);


        if (index == -1)
            return;

        index--;
        if (index == -1)
            index = spriteList.Count - 1;

        ProfileImg.sprite = spriteList[index];
    }

    public void NextBtnClick()
    {
        int index = spriteList.FindIndex(current => current == ProfileImg.sprite);

        if (index == -1)
            return;

        index++;
        if (index == spriteList.Count)
            index = 0;

        ProfileImg.sprite = spriteList[index];
    }
}
