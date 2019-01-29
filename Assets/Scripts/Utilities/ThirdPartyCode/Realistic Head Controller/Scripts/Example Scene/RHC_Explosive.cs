/**
 * Advanced Camera & Head Controller
 * Copyright (c) 2017-Present, Inan Evin, Inc. All rights reserved.
 * Author: Inan Evin
 * https://www.inanevin.com/realistic-head-controller
 * Video Tutorials: 
 * contact: inanevin@gmail.com
 * live chat: https://discord.gg/AQ3VGBX
 *
 * Feel free to ask about the package and talk about recommendations!
 *
 *
 * Class Description:
 *
 * This class is used for controlling explosives in the demo scene. When player is close enough, explosives can be triggered via key press.
 */


using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class RHC_Explosive : MonoBehaviour
{

    public Transform t_CameraController;    // Target player transform to receive distance from.
    public GameObject go_Interact;          // Object containing interact UIs.
    private bool b_IsLooking;				// Is player looking at this explosive.
    public GameObject go_Explosion;         // Explosion child

    void Update()
    {
        // If the camera is looking towards this, set b_IsLooking to true, else false.
        RaycastHit hit;
        if (Physics.Raycast(t_CameraController.position, t_CameraController.forward, out hit))
        {
            if (hit.transform == transform)
            {
                if (!b_IsLooking)
                {
                    b_IsLooking = true;
                    go_Interact.SetActive(true);
                }
            }
            else
            {
                if (b_IsLooking)
                {
                    b_IsLooking = false;
                    go_Interact.SetActive(false);
                }
            }
        }
        else
        {
            if (b_IsLooking)
            {
                b_IsLooking = false;
                go_Interact.SetActive(false);
            }
        }


        if (b_IsLooking)
        {
            // If player is in distance, make the UI object look at player.
            go_Interact.transform.LookAt(t_CameraController);

            // Interacted.
            if (Input.GetKeyDown(KeyCode.F))
                PVO_Interacted();
        }
    }

    public void PVO_Interacted()
    {
        // Call generic shake event.
        RHC_EventManager.PSVO_CallGenericShake(transform.position);
        go_Explosion.transform.parent = null;
        go_Explosion.SetActive(true);
        // Deactive gameobject.
        gameObject.SetActive(false);
    }


}
