using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaText : MonoBehaviour
{
    public Rigidbody playerRB;
    private Text Stamina;

    // Start is called before the first frame update
    void Start()
    {
        Stamina = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = playerRB.velocity.magnitude;
        Stamina.text = "Stamina: " + speed.ToString();
    }
}
