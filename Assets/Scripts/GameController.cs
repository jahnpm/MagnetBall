using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public static List<GameObject> balls;
    public static int scoreLeft = 0;
    public static int scoreRight = 0;
    public static float timeLeft = 120.0f;
    public int ballCount = 0;
    public TMP_Text scoreText;
    public TMP_Text timeText;

    GameObject ballPrefab;
    int lastSpawnTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        balls = new List<GameObject>();
        ballPrefab = transform.Find("Ball").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        timeText.text = (int)timeLeft + "s";

        if (timeLeft <= 0)
        {
            if (scoreLeft < scoreRight)
                timeText.text = "WIN";

            if (scoreLeft > scoreRight)
                timeText.text = "LOOSE";

            if (scoreLeft == scoreRight)
                timeText.text = "TIE";

            for (int i = 0; i < balls.Count; i++)
            {
                Destroy(balls[i]);
            }

            balls.Clear();

            if (Input.GetKey(KeyCode.Space))
            {
                timeLeft = 120.0f;
                scoreLeft = 0;
                scoreRight = 0;
            }
        }

        if (timeLeft > 0)
        {
            scoreText.text = scoreLeft + " : " + scoreRight;

            int currentTime = (int)Time.realtimeSinceStartup;

            if (currentTime % 5 == 0 && currentTime != lastSpawnTime)
            {
                GameObject go = Instantiate(ballPrefab, transform.position + 0.375f * Vector3.up, Quaternion.identity);
                go.SetActive(true);
                balls.Add(go);
            }

            lastSpawnTime = currentTime;

            List<int> removeIndices = new List<int>();

            for (int i = 0; i < balls.Count; i++)
            {
                Rigidbody ballBody = balls[i].GetComponent<Rigidbody>();

                if (ballBody.position.y < -1.0f)
                    removeIndices.Add(i);
            }

            foreach (int idx in removeIndices)
            {
                Destroy(balls[idx]);
                balls.RemoveAt(idx);
            }

            ballCount = balls.Count;
        }
    }
}
