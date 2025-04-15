using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

interface IInteractable
{
    void NoHover();
    void Hover();
}

public class Interaction : MonoBehaviour
{
    public GameObject player;
    private GameObject objectPreviouslyHit;
    public static bool InputLock = false;
    public Transform InteractorSource;
    public float InteractRange;

    public float rotationSensitivity = 2.0f;
    public GameObject offset;

    public static bool isExamining = false;

    private Vector3 lastMousePosition;
    private Transform examinedObject;

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    private GameObject pointer;

    RaycastHit hitInfo;

    void Start()
    {
        pointer = GameObject.Find("Pointer");
    }

    void Update()
    {
        if (!isExamining)
        {
            HandleRaycast();
        }
        else
        {
            HandleExaminationInput();
        }

        if (isExamining)
        {
            StartExamination();
            Examine();
        }
        else
        {
            StopExamination();
            NonExamine();
        }
    }

    void HandleRaycast()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out hitInfo, InteractRange))
        {
            GameObject hitObject = hitInfo.collider.gameObject;

            if (hitObject.TryGetComponent(out IInteractable interactObj))
            {
                if (objectPreviouslyHit != hitObject)
                {
                    HandlePreviousHitObject();
                    objectPreviouslyHit = hitObject;
                }

                HandleInteraction(interactObj, hitObject);
            }
        }
        else
        {
            HandlePreviousHitObject();
            objectPreviouslyHit = null;
        }
    }

    void HandlePreviousHitObject()
    {
        if (objectPreviouslyHit != null && objectPreviouslyHit.TryGetComponent(out IInteractable interactObj))
        {
            interactObj.NoHover();
        }
    }

    void HandleExaminationInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isExamining = false;
        }
    }

    void HandleInteraction(IInteractable interactObj, GameObject hitObject)
    {
        string tag = hitObject.tag;

        switch (tag)
        {
            default:
                interactObj.Hover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isExamining = true;
                    examinedObject = hitInfo.transform;
                    originalPositions[examinedObject] = examinedObject.position;
                    originalRotations[examinedObject] = examinedObject.rotation;
                }
                break;
        }
    }

    void StartExamination()
    {
        lastMousePosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pointer.SetActive(false);
        InputLock = true;

        player.GetComponent<FirstPersonLook>()?.ResetLook();
    }

    void StopExamination()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pointer.SetActive(true);
        InputLock = false;
    }

    void Examine()
    {
        if (examinedObject != null)
        {
            examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.2f);

            Physics.IgnoreCollision(examinedObject.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

            RotateObject();
            lastMousePosition = Input.mousePosition;
        }
    }

    void NonExamine()
    {
        if (examinedObject != null)
        {
            Physics.IgnoreCollision(examinedObject.GetComponent<Collider>(), player.GetComponent<Collider>(), false);

            if (originalPositions.ContainsKey(examinedObject))
            {
                examinedObject.position = Vector3.Lerp(examinedObject.position, originalPositions[examinedObject], 0.2f);
            }
            if (originalRotations.ContainsKey(examinedObject))
            {
                examinedObject.rotation = Quaternion.Slerp(examinedObject.rotation, originalRotations[examinedObject], 0.2f);
            }
        }
    }

    void RotateObject()
    {
        float XaxisRotation = -Input.GetAxis("Mouse X");
        float YaxisRotation = Input.GetAxis("Mouse Y");
        if (Input.GetMouseButton(1))
        {
            examinedObject.rotation =
                Quaternion.AngleAxis(XaxisRotation * rotationSensitivity, transform.up) *
                Quaternion.AngleAxis(YaxisRotation * rotationSensitivity, transform.right) *
                examinedObject.rotation;
        }
    }
}
