using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedText : MonoBehaviour
{
    public Rigidbody playerRB;
    private Text speedText;

    // Start is called before the first frame update
    void Start()
    {
        speedText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = playerRB.velocity.magnitude;
        speedText.text = "Speed: " + speed.ToString();
    }
}
