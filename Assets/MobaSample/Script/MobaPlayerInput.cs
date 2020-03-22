using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobaPlayerInput : MonoBehaviour
{

    public MobaPlayer player;

    void Awake()
    {
        player = GetComponent<MobaPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");

        float horizontalInput = Input.GetAxis("Horizontal");


        if (player)
        {
            player.VertivalInput = verticalInput;
            player.HorizontalInput = horizontalInput;
        }
    }
}
