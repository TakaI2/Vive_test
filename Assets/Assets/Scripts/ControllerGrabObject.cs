using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject;
    private GameObject objectInHand;


    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();   
    }

    private void SetCollidingObject(Collider col)
    {
        if(collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other) //トリガーコライダーが別のコライダーに入ると他のコライダを潜在的なターゲットとして設定
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other) //
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other) //コライダがオブジェクトを終了し、未グラブのターゲットを放棄すると、このコードはそのターゲットをnullに設定して削除する。
    {
        if(!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()  //プレイヤーの手の中にゲームオブジェクトを移動し、cpllidingObject変数からGameObjectを削除する。
    {
        objectInHand = collidingObject;
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint() //オブジェクトをつかむとき、コントローラとオブジェクトを接続するジョイントを生成
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject() //オブジェクトを放したとき、コントローラの速度と回転を与え、投げられるようにする。
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        objectInHand = null;
    }

    // Update is called once per frame
    void Update () {
		if(Controller.GetHairTriggerDown())
        {
            if(collidingObject)
            {
                GrabObject();
            }
        }

        if(Controller.GetHairTriggerUp())
        {
            if(objectInHand)
            {
                ReleaseObject();
            }
        }

	}
}
