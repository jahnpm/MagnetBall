using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public float pushPower = 1;
    public float pullPower = 1;
    public bool AI = false;

    private Color color;
    private Rigidbody rb;
    private NavMeshAgent agent;
    public bool ballChosen = false;
    public bool ballShooting = false;
    public Vector3 randomDestination = Vector3.zero;
    private Rigidbody nearestBall;

    Vector3 inputDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (AI)
            rb.isKinematic = true;
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (color == Color.red || color == Color.blue)
        {
            foreach (GameObject ball in GameController.balls)
            {
                Rigidbody ballBody = ball.GetComponent<Rigidbody>();
                Vector3 direction = rb.position - ballBody.position;

                if (ball.GetComponent<BallController>().color == color)
                {
                    ballBody.AddForce((pushPower * -direction.normalized) / (direction.magnitude * direction.magnitude));
                }
                else
                {
                    ballBody.AddForce((pullPower * direction.normalized) / (direction.magnitude * direction.magnitude));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AI)
        {
            if (nearestBall == null || nearestBall.position.y < -1.0f || nearestBall.gameObject.GetComponent<BallController>().scored)
            {
                ballChosen = false;
                ballShooting = false;
            }

            if (!ballChosen && !ballShooting)
            {
                float minDistance = 1000;
                nearestBall = null;

                foreach (GameObject ball in GameController.balls)
                {
                    if (ball.GetComponent<BallController>().scored)
                        continue;

                    Rigidbody ballBody = ball.GetComponent<Rigidbody>();
                    Vector3 direction = rb.position - ballBody.position;

                    if (direction.magnitude < minDistance)
                    {
                        minDistance = direction.magnitude;
                        nearestBall = ballBody;
                    }
                }

                if (nearestBall != null)
                {
                    ballChosen = true;

                    if (nearestBall.GetComponent<BallController>().color == Color.red)
                        color = Color.blue;
                    else
                        color = Color.red;
                }
            }

            if (ballChosen)
            {
                agent.SetDestination(nearestBall.position);
            }

            if (nearestBall != null)
            {
                if (Vector3.Distance(rb.position, nearestBall.position) < 0.65f && ballChosen)
                {
                    ballChosen = false;
                    ballShooting = true;

                    randomDestination = new Vector3(Random.Range(-2.0f, -1.5f), 0.375f, Random.Range(5.0f, 2.0f));
                    agent.SetDestination(randomDestination);
                }
            }

            if (ballShooting)
            {
                RaycastHit hitInfo;
                Physics.Raycast(rb.position, (nearestBall.position - rb.position).normalized, out hitInfo);

                if (!(hitInfo.collider.gameObject.name == "LeftGoal") && rb.position.x >= -2.0f && rb.position.x < nearestBall.position.x)
                {
                    color = nearestBall.GetComponent<BallController>().color;
                    agent.SetDestination(nearestBall.position);
                }
                else
                {
                    if (Vector3.Distance(rb.position, randomDestination) < 0.5f)
                    {
                        randomDestination = new Vector3(Random.Range(-2.0f, -1.5f), 0.375f, Random.Range(2.0f, 5.0f));
                        agent.SetDestination(randomDestination);
                    }
                }

                if (Vector3.Distance(rb.position, nearestBall.position) > 3.0f)
                    ballShooting = false;
            }
        }

        if (!AI)
        {
            inputDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
                inputDirection += Vector3.left;

            if (Input.GetKey(KeyCode.D))
                inputDirection += Vector3.right;

            if (Input.GetKey(KeyCode.W))
                inputDirection += Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                inputDirection += Vector3.back;

            rb.MovePosition(rb.position + inputDirection.normalized * speed * Time.deltaTime);

            if (Input.GetMouseButton(0))
                color = Color.red;

            if (Input.GetMouseButton(1))
                color = Color.blue;

            if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                color = Color.white;

            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
                color = Color.white;
        }

        transform.Find("Light").GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
