using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Changes sprites on button click.
/// </summary>
public class SwitchSprite : MonoBehaviour
{
    public Image ProfileImg;
    public Sprite Sprite1;
    public Sprite Sprite2;
    public Sprite Sprite3;

    private List<Sprite> _spriteList;

    void Start()
    {
        _spriteList = new List<Sprite>();
        _spriteList.Add(Sprite1);
        _spriteList.Add(Sprite2);
        _spriteList.Add(Sprite3);
    }

    /// <summary>
    /// Sets current sprite to previous one.
    /// </summary>
    public void PreviousBtnClick()
    {
        int index = _spriteList.FindIndex(current => current == ProfileImg.sprite);


        if (index == -1)
            return;

        index--;
        if (index == -1)
            index = _spriteList.Count - 1;

        ProfileImg.sprite = _spriteList[index];
    }


    /// <summary>
    /// Sets current sprite to next one.
    /// </summary>
    public void NextBtnClick()
    {
        int index = _spriteList.FindIndex(current => current == ProfileImg.sprite);

        if (index == -1)
            return;

        index++;
        if (index == _spriteList.Count)
            index = 0;

        ProfileImg.sprite = _spriteList[index];
    }
}
