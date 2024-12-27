using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Enemy hoverbot;
    public Enemy hoverbotEnemy;
    public Item heart;
    public Item ammo;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int score;

    public GameObject menuPanel;
    public GameObject settingPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public TextMeshProUGUI maxScoreTxt; // ?
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI playerTimeTxt;
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI AmmoTxt;
    public TextMeshProUGUI coinTxt;

    private bool isGamePaused = false;

    void Start()
    {
        hoverbot.gameObject.SetActive(false);
        //hoverbotEnemy.gameObject.SetActive(false);

        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
    }

    void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore)
        {
            maxScoreTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
            maxScoreTxt.text = string.Format("{0:n0}", player.score);
        }
    }

    public void GameStart()
    {
        isBattle = true;

        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        hoverbot.gameObject.SetActive(true);
        //hoverbotEnemy.gameObject.SetActive(true);

        ammo.gameObject.SetActive(true);
        heart.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        player.gameObject.SetActive(false);
        overPanel.SetActive(true);
    }

    public void Restart()
    {
        overPanel.SetActive(false);
        gamePanel.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void GameSetting()
    {
        settingPanel.SetActive(true);
    }

    public void GameSettingBack()
    {
        settingPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        playerHealthTxt.text = player.health + " / " + player.maxHealth;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);

        playerTimeTxt.text = string.Format("{0:00}", hour) + ":" + 
            string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        coinTxt.text = string.Format("{0:n0}", player.coin);
        if(player.equipWeapon == null)
        {
            AmmoTxt.text = "- / " + player.ammo;
        }
        else if(player.equipWeapon.type == Weapon.Type.Melee)
        {
            AmmoTxt.text = "- / "+ player.ammo;
        }
        else
        {
            AmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        }
    }
}
