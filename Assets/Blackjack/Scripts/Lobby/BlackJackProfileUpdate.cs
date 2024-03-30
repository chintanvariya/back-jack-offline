using BlackJackOffline;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackJackProfileUpdate : MonoBehaviour
{
    [SerializeField]
    private Text userNameTxt;
    [SerializeField]
    private Image profileImage;
    // Start is called before the first frame update
    void Start()
    {
        userNameTxt.text = BlackJackDataManager.userName;
        profileImage.sprite = BlackJackGameManager.instance.profileSprites[BlackJackDataManager.ProfileImage];
    }

    [Header("------------ Select Avatar ----------------")]
    [SerializeField]
    private List<GameObject> selectObject;
    public SimpleScrollSnap avatarScroll;
    
    public void SelectAvatarClicked(int index)
    {
        int count = 1;
        foreach (var item in selectObject)
        {
            item.SetActive(false);
            if (count == index)
            {
                BlackJackDataManager.ProfileImage = index;
                item.SetActive(true);
            }
            count++;
        }
    }

    public void selectProfileImage()
    {
        if (PlayerPrefs.GetInt("ProfileImage", 1) != 1)
        {
            selectObject[BlackJackDataManager.ProfileImage - 1].SetActive(true);
            avatarScroll.StartingPanel = BlackJackDataManager.ProfileImage - 1;
        }
    }

    public void UpdateProfileAndLobbyData()
    {
        userNameTxt.text = BlackJackDataManager.userName;
        profileImage.sprite = BlackJackGameManager.instance.profileSprites[BlackJackDataManager.ProfileImage];
        BlackJackGameManager.instance.UpdateUserInfo();
    }

    [Header("------------ Statistics ----------------")]
    [SerializeField]
    private Text gamePlay;
    [SerializeField]
    private Text gameWon, gameloss;

    public void SetstatisticsValues()
    {
        gamePlay.text = BlackJackDataManager.RoundPlay.ToString();
        gameWon.text = BlackJackDataManager.RoundWin.ToString();
        gameloss.text = BlackJackDataManager.RoundLoss.ToString();
    }
}
