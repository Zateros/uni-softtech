using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialScripts : MonoBehaviour
{
    [SerializeField] public GameObject SetProfilePanel;
    [SerializeField] public GameObject TutorialPanel;
    [SerializeField] public TMP_InputField InputName;
    [SerializeField] public Toggle TutorialCheck;
    [SerializeField] public TextMeshProUGUI TutorialText;
    [SerializeField] private string _playerName;


    public void OnFirstConfirmBtnClick()
    {
        if(TutorialCheck.isOn)
        {
            SetTutorialMessage();
            SetProfilePanel.SetActive(false);
            TutorialPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync(1);
        }
    }

    public void SetTutorialMessage()
    {
        if(InputName.text == null)
        {
            _playerName = "";
        }
        else
        {
            _playerName = InputName.text;
        }

        string difficulty;
        int timeToSurvive;

        switch (DifficultyBtns.difficulty)
        {
            case 0:
                difficulty = "Easy";
                timeToSurvive = 3;
                break;
            case 1:
                difficulty = "Medium";
                timeToSurvive = 6;
                break;
            case 2:
                difficulty = "Hard";
                timeToSurvive = 12;
                break;
            default:
                difficulty = "";
                timeToSurvive = -1;
                break;
        }

            TutorialText.text =
$@"Welcome to the game {_playerName}!

You are playing on {difficulty} mode.

You can buy and sell animals, plants and vehicles in this game. Your animals get thirsty and hungry over time. They'll die if they can't find water or food. Animals of the same species form groups and over time they multiply. A red dot will mark every animal you own on the minimap.
You'll have turists come to your safari, who'll rent a jeep, and go around on the trails you built. At the exit they'll leave satisfaction points about how expensive the entrance fee and the jeep was, how much they had to wait, how long the ride was, and how many animals they saw.
At night only a small portion of the map will be visible around the roads. You can buy chips for animals, to make them visible at night.

At the start of the game you will be given a random number of herbivorous and carnivorous animals, turists and a jeep.
You can find the price and sell price of every purchasable item, if you click the button on the toolbar.
To sell items you have to turn on sell mode, and then click on the things you want to sell.
You can stop, and fast forward time by using the buttons below the minimap.

Your goal is to keep all your stats (herbivore count, carnivore count, turist count, turist satisfaction, money) above the minimum for {timeToSurvive} in-game months. Depending on the difficulty it will be harder to keep your numbers above minimum, and you'll get a harder terrain to play on. You'll receive in-game notifications, if one of these values get low.

Hope you enjoy the game!";
    }
}
