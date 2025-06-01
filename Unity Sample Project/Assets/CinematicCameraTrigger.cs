using UnityEngine;
using System.Collections;

public class CinematicCameraTrigger : MonoBehaviour
{
    [Header("Ruta de la cámara")]
    public Transform[] cameraPathPoints;        // Puntos por los que se mueve la cámara

    [Header("Cámaras")]
    public Camera cinematicCamera;              // Cámara que se moverá
    public Camera playerCamera;                 // Cámara del jugador (si se quiere reactivar)

    [Header("Fade")]
    public ScreenFader screenFader;             // Script del fade
    public float waitAtEnd = 1f;                // Tiempo de espera al final antes del fade
    public float cameraSpeed = 2f;              // Velocidad de movimiento de la cámara

    private bool hasPlayed = false;

    public FirstPersonMovement firstP;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;
            StartCoroutine(PlayCinematic());
        }
    }

    private IEnumerator PlayCinematic()
    {
        // Desactivar cámara del jugador si está asignada
        if (playerCamera != null)
            playerCamera.enabled = false;

        firstP.canmove = false;
        cinematicCamera.gameObject.SetActive(true);

        for (int i = 0; i < cameraPathPoints.Length - 1; i++)
        {
            Vector3 startPos = cameraPathPoints[i].position;
            Quaternion startRot = cameraPathPoints[i].rotation;
            Vector3 endPos = cameraPathPoints[i + 1].position;
            Quaternion endRot = cameraPathPoints[i + 1].rotation;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * cameraSpeed;
                cinematicCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
                cinematicCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
        }

        // Espera final antes del fade
        yield return new WaitForSeconds(waitAtEnd);

        // Fade to black
        if (screenFader != null)
            yield return screenFader.FadeOut();

        // Desactivar cámara cinemática
        cinematicCamera.gameObject.SetActive(false);

        // Reactivar cámara del jugador
        if (playerCamera != null)
            playerCamera.enabled = true;

        firstP.canmove = true;
        // Fade in desde negro
        if (screenFader != null)
            yield return screenFader.FadeIn();
    }
}
