using UnityEngine;

public class ViewCamera : MonoBehaviour
{
    public bool isLookat;
    public bool isForward;
    void LateUpdate()
    {
        if (isForward)
        {
            transform.forward = Camera.main.transform.forward;
        }


        if (isLookat)
        {
            transform.LookAt(Camera.main.transform.position, Vector3.up);
        }

        Vector3 averageDirection = (Camera.main.transform.forward + (isLookat ? (Camera.main.transform.position - transform.position).normalized : Vector3.zero)).normalized;
        transform.forward = averageDirection;

    }

    /* void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Quaternion lookAtRotation = Quaternion.LookRotation(
            cam.transform.position - transform.position,
            Vector3.up
        );

        transform.rotation = lookAtRotation;
    } */

    /* public bool isLookat;
    public bool isForward;
    void LateUpdate()
    {
        if (isForward)
        {
            transform.forward = Camera.main.transform.forward;

        }


        if (isLookat)
        {
            //transform.forward = (Camera.main.transform.position - transform.position).normalized;

            //Quaternion lookAtRotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position,
            //Vector3.up);
            //transform.rotation = lookAtRotation; 

            //transform.LookAt(Camera.main.transform.position, Vector3.up);


            Quaternion lookAtRotation = Quaternion.LookRotation(
                    Camera.main.transform.position - transform.position,
                    Vector3.up
                );

            transform.rotation = lookAtRotation;
            //Quaternion.LookRotation = (Camera.main.transform.position - transform.position).normalized;
        }

        Vector3 averageDirection = (Camera.main.transform.forward + (isLookat ? (Camera.main.transform.position - transform.position).normalized : Vector3.zero)).normalized;
        transform.forward = averageDirection;

    } */

    /* void LateUpdate()
{
    Camera cam = Camera.main;
    if (cam == null) return;

    Quaternion forwardRotation =
        Quaternion.LookRotation(cam.transform.forward, Vector3.up);

    Quaternion lookAtRotation =
        Quaternion.LookRotation(cam.transform.position - transform.position, Vector3.up);

    transform.rotation = Quaternion.Slerp(
        forwardRotation,
        lookAtRotation,
        0.5f
    );
} */

    /* [Range(0f, 10f)]
    public float halfwayTilt;
    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        //float halfwayTilt = 10f; // Half the tilt shown in the first example
        transform.rotation =
            Quaternion.AngleAxis(halfwayTilt, cam.transform.forward)
            * cam.transform.rotation;
    } */

    /* void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Quaternion screenAligned = cam.transform.rotation;
        Quaternion facingCamera = Quaternion.LookRotation(
            cam.transform.position - transform.position,
            cam.transform.up
        );

        float blend = 0.3f; // 0 = screen-aligned; 1 = facing camera
        transform.rotation = Quaternion.Slerp(screenAligned, facingCamera, blend);
    } */

    /*    void LateUpdate()
   {
       Camera cam = Camera.main;
       if (cam == null) return;

       Quaternion cameraForwardRotation =
           Quaternion.LookRotation(cam.transform.forward, Vector3.up);

       Quaternion lookAtCameraRotation =
           Quaternion.LookRotation(
               cam.transform.position - transform.position,
               Vector3.up
           );

       transform.rotation = Quaternion.Slerp(
           cameraForwardRotation,
           lookAtCameraRotation,
           0.5f
       );
   } */

}
