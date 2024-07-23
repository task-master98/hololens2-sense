/*
#TODO: After turning the position should be retained until the user comes within some distance
after which the radial view should be enabled again.
*/
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class NavigationControiler : MonoBehaviour
{
    public GameObject mainWindow;
    public float moveDistance = 2.0f;
    public float moveSpeed = 1.0f;
    public float turnSpeed = 90.0f;
    public Interactable leftArrow;
    public Interactable rightArrow;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private RadialView radialView;
    private bool isMoving = false;
    private bool isTurning = false;

    void Start()
    {
        radialView = mainWindow.GetComponent<RadialView>();
        if (radialView == null)
        {
            Debug.LogError("Radial View not intiialized");
            return;
        }

        targetPosition = mainWindow.transform.position;
        targetRotation = mainWindow.transform.rotation;

        leftArrow.OnClick.AddListener(() => StartTurn(Vector3.left));
        rightArrow.OnClick.AddListener(() => StartTurn(Vector3.right));

    }

    void Update()
    {
        if (isTurning)
        {
            mainWindow.transform.rotation = Quaternion.RotateTowards(mainWindow.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            if (Quaternion.Angle(mainWindow.transform.rotation, targetRotation) < 0.1f)
            {
                isTurning = false;
                StartMove();
            }
        }

        if (isMoving)
        {
            mainWindow.transform.position = Vector3.MoveTowards(mainWindow.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if mainWindow has reached the target position
            if (Vector3.Distance(mainWindow.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                radialView.enabled = true;
            }
        }
    }

    void StartTurn(Vector3 direction)
    {
        radialView.enabled = false;

        if (direction == Vector3.left)
        {
            targetRotation = Quaternion.Euler(0, mainWindow.transform.eulerAngles.y - 90, 0);
        }
        else if (direction == Vector3.right)
        {
            targetRotation = Quaternion.Euler(0, mainWindow.transform.eulerAngles.y + 90, 0);
        }

        isTurning = true;
    }

    void StartMove()
    {
        // Set the target position to move forward in the current direction
        targetPosition = mainWindow.transform.position + mainWindow.transform.forward * moveDistance;
        isMoving = true;
    }
}
