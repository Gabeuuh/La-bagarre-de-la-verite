using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText; 
    private int score = 0;
    public List<GameObject> targets = new List<GameObject>();
    
    public void UpdateScore(int ScoreToAdd){
        score += ScoreToAdd;
        ScoreText.text = "score : " + score;
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

