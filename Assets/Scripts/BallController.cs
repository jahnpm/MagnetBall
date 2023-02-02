using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Color color;
    public bool scored;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 2) == 0)
            color = Color.red;
        else
            color = Color.blue;

        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 0.375f)
            transform.position = new Vector3(transform.position.x, 0.375f, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "LeftGoalTrigger")
        {
            GameController.scoreRight++;
            scored = true;
        }

        if (other.name == "RightGoalTrigger")
        {
            GameController.scoreLeft++;
            scored = true;
        }
    }
}
