﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chara : MonoBehaviour
{

    private Animator animator;
    private CharacterController cCon;
    private float x;
    private float y;
    private Vector3 velocity;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        cCon = GetComponent<CharacterController>();
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        //　地面に接地してる時は初期化
        if (cCon.isGrounded)
        {
            velocity = Vector3.zero;

            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            Vector3 input = new Vector3(x, 0, y);

            //　方向キーが多少押されている
            if (input.magnitude > 0.1f && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                animator.SetFloat("Speed", input.magnitude);

                transform.LookAt(transform.position + input);

                velocity += input.normalized * 2;
                //　キーの押しが小さすぎる場合は移動しない
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }

            if (Input.GetButtonDown("Fire1")
                && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
                && !animator.IsInTransition(0)
            )
            {
                animator.SetBool("Attack", true);
            }
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        cCon.Move(velocity * Time.deltaTime);

    }

}
