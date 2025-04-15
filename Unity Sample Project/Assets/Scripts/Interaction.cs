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

                    // Teletransportar davant la càmera
                    examinedObject.position = Camera.main.transform.position + Camera.main.transform.forward * 0.8f;

                    // Evitar col·lisions des del principi
                    Physics.IgnoreCollision(examinedObject.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

                    // Capturar posició del ratolí
                    lastMousePosition = Input.mousePosition;
                }
                break;
        }
    }

    void StartExamination()
    {
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
            Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.8f;
            examinedObject.position = Vector3.Lerp(examinedObject.position, targetPosition, 0.4f);

            RotateObject();
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
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

        if (mouseDelta.sqrMagnitude > 0.01f)
        {
            float rotationX = mouseDelta.x * rotationSensitivity;
            float rotationY = -mouseDelta.y * rotationSensitivity;

            examinedObject.Rotate(Camera.main.transform.up, rotationX, Space.World);
            examinedObject.Rotate(Camera.main.transform.right, rotationY, Space.World);
        }

        lastMousePosition = Input.mousePosition;
    }
}
