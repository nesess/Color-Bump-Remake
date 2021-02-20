using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private Text cLvlText, nLvlText;
    private Image fill;

    private int level;
    private float startDistance, distance;

    private GameObject player, finish, hand;

    private TextMesh levelNo;



    void Awake()
    {
        cLvlText = GameObject.Find("CurrentLevelText").GetComponent<Text>();
        nLvlText = GameObject.Find("NextLevelText").GetComponent<Text>();
        fill = GameObject.Find("Fill").GetComponent<Image>();

        player = GameObject.Find("Player");
        finish = GameObject.Find("Finish");
        hand = GameObject.Find("Hand");

        levelNo = GameObject.Find("LevelNo").GetComponent<TextMesh>();
    }

    void Start()
    {

        level = PlayerPrefs.GetInt("Level");

        levelNo.text = "LEVEL " + level;

        cLvlText.text = level.ToString();
        nLvlText.text = level + 1 + "";

        startDistance = Vector3.Distance(player.transform.position, finish.transform.position);
    }

    void Update()
    {
        distance = Vector3.Distance(player.transform.position, finish.transform.position);
        if(player.transform.position.z < finish.transform.position.z)
        {
            fill.fillAmount = 1 - (distance / startDistance);
        }
        
    }

    public void RemoveUI()
    {

        hand.SetActive(false);

    }

}
