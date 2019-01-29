/**
 * Realistic Head Controller
 * Copyright (c) 2017-Present, Inan Evin, Inc. All rights reserved.
 * Author: Inan Evin
 * https://www.inanevin.com/realistic-head-controller
 * contact: inanevin@gmail.com
 * live chat: https://discord.gg/eZAz3KW
 *
 * Feel free to ask about the package and talk about recommendations!
 *
 *
 * Class Description:
 *
 * This class is used for controlling head & camera rotation based on mouse input. Generally, in FPS games, the camera would be the child of the player. If that's the case, 
 * checking b_RotatesPlayer variable as true and assigning the player transform would work. In that case, the script will rotate the camera (this transform) based on the vertical
 * input of the mouse, and will rotate the player based on the horizontal input of the mouse. Otherwise, both vertical & horizontal input of the mouse will rotate the camera. (this transform)
 *
 */

// Library
using UnityEngine;

public class RHC_CameraController : MonoBehaviour
{

    #region Variable(s)

    public bool b_RotatesPlayer;                            // If this is enabled, the script will rotate the player transform on it's Y axis.
    public Transform t_Player;                              // Player transform if b_RotatesPlayer is enabled.
    public Vector2 v2_Sensitivity;                          // Mouse sensitivity.
    public Vector2 v2_Smooth;                               // Mouse smooth.
    public float f_MinY;                                    // Minimum angle on X rotation axis. (mouse Y)
    public float f_MaxY;                                    // Maximum angle on X rotation axis. (mouse Y)

    private Vector2 v2_MouseInputs;                         // Storage for mouse inputs.
    private Vector2 v2_SmoothedInputs;                      // Storage for smoothing mouse inputs.
    private Vector2 v2_RefInputs;                           // Reference used in Mathf.SmoothDamp method.
    private Quaternion q_This;                              // Storage for the rotation of this transform.
    private Quaternion q_Player;                            // Storage for the rotation of player transform.(if b_RotatesPlayer is enabled)
    private Transform t_This;                               // This transform.

    #endregion

    #region Initializer(s)

    void Awake()
    {
        // Store variables.
        t_This = transform;
        q_This = t_This.localRotation;
        if (b_RotatesPlayer)
            q_Player = t_Player.localRotation;
    }

    #endregion

    #region Control(s)

    void UpdateWithInput()
    {
        // Get mouse inputs & multiply them with sensitivity, and then store them.
        v2_MouseInputs.x += Input.GetAxis("Mouse X") * v2_Sensitivity.x;
        v2_MouseInputs.y -= Input.GetAxis("Mouse Y") * v2_Sensitivity.y;

        // Clamp vertical mouse input.
        v2_MouseInputs.y = Mathf.Clamp(v2_MouseInputs.y, f_MinY, f_MaxY);

        // Smooth out mouse inputs.
        v2_SmoothedInputs.x = Mathf.SmoothDamp(v2_SmoothedInputs.x, v2_MouseInputs.x, ref v2_RefInputs.x, v2_Smooth.x);
        v2_SmoothedInputs.y = Mathf.SmoothDamp(v2_SmoothedInputs.y, v2_MouseInputs.y, ref v2_RefInputs.y, v2_Smooth.y);

        // Create rotations based on the angles from smoothed mouse inputs.
        Quaternion q_X = Quaternion.AngleAxis(v2_SmoothedInputs.x, Vector3.up);
        Quaternion q_Y = Quaternion.AngleAxis(v2_SmoothedInputs.y, Vector3.right);

        // Rotate transform.
        if (b_RotatesPlayer)
        {
            t_Player.localRotation = q_Player * q_X;
            t_This.localRotation = q_This * q_Y;
        }
        else
            t_This.localRotation = q_This * q_X * q_Y;
    }

    #endregion
}
