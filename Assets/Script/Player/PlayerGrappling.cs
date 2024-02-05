using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class PlayerGrappling : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("SWINGING")]
    [SerializeField] private ConeRaycast swingConeRaycast;
    [SerializeField] private float maxSwingingDistance;
    private Vector3 swingPoint;
    private SpringJoint springJoint;
    public bool isSwinging = false;
    public bool trySwing = false;
    public float radiusIncreaseSpeed;
    public float endCurveSpeedBoost;
    [SerializeField] private LineRenderer swingLineRenderer;

    [Header("GRAPPLING")]
    [SerializeField] private ConeRaycast accelerationConeRaycast;
    private Vector3 grapplePoint;
    public bool isGrappling = false;
    public bool grappleFreeze = false;
    [SerializeField] private float grappleFreezeDelay;
    [SerializeField] private float overshootYAxis;
    [SerializeField] private LineRenderer leftGrapplingLineRenderer;
    [SerializeField] private LineRenderer rightGrapplingLineRenderer;


    private void LateUpdate()
    {
        if (springJoint)
        {
            swingLineRenderer.SetPosition(0, player.playerMesh.transform.position);
            if (swingLineRenderer.GetPosition(1) != swingPoint)
            {
                swingLineRenderer.SetPosition(1, Vector3.Lerp(swingLineRenderer.GetPosition(1), swingPoint, 0.1f));
            }
        }
        if (isGrappling)
        {
            leftGrapplingLineRenderer.SetPosition(0, player.leftHand.transform.position);
            if (leftGrapplingLineRenderer.GetPosition(1) != grapplePoint)
            {
                leftGrapplingLineRenderer.SetPosition(1, Vector3.Lerp(leftGrapplingLineRenderer.GetPosition(1), grapplePoint, 0.1f));
            }
            rightGrapplingLineRenderer.SetPosition(0, player.rightHand.transform.position);
            if (rightGrapplingLineRenderer.GetPosition(1) != grapplePoint)
            {
                rightGrapplingLineRenderer.SetPosition(1, Vector3.Lerp(rightGrapplingLineRenderer.GetPosition(1), grapplePoint, 0.1f));
            }
        }
    }

    private void Update()
    {
        if (trySwing) StartSwing();
        else StopSwing();

        if (grappleFreeze)
        {
            player.rigibody.velocity = Vector3.zero;
        }
        if (isGrappling && Vector3.Distance(transform.position, grapplePoint) < 3)
        {
            StopGrapple();
        }
        //if (isGrappling && !player.rightShoulder && !player.leftShoulder)
        //{
        //    StopGrapple();
        //}
        
        if (accelerationConeRaycast.contactPointList.Count > 0 && !isGrappling)
        {
            float _distance = 1000;
            Vector3 _point = Vector3.zero;
            for (int i = 0; i < accelerationConeRaycast.contactPointList.Count; i++)
            {
                float _currentDistance = Vector3.Distance(accelerationConeRaycast.perfectPoint.position, accelerationConeRaycast.contactPointList[i]);
                if (_currentDistance < _distance)
                {
                    _point = accelerationConeRaycast.contactPointList[i];
                    _distance = _currentDistance;
                }
            }
            Vector3 _direction = _point - GPCtrl.Instance.player.playerMesh.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(GPCtrl.Instance.player.playerMesh.transform.position, _direction, out hit, maxSwingingDistance))
            {
                GPCtrl.Instance.grapplePointIndicator.ShowAtWorldPosition(hit.point);
                grapplePoint = hit.point;
            }
        } else if (!isGrappling)
        {
            grapplePoint = Vector3.zero;
            GPCtrl.Instance.grapplePointIndicator.Hide();
        }

        if (player.rightShoulder && player.leftShoulder && grapplePoint != Vector3.zero && !isGrappling)
        {
            StartGrapple();
        }
    }

    #region Swing

    public void StartSwing()
    {
        if (springJoint) return;
        if (swingConeRaycast.radius < swingConeRaycast.maxRadius)
            swingConeRaycast.radius += Time.deltaTime * radiusIncreaseSpeed;
        if (swingConeRaycast.contactPointList.Count > 0)
        {
            float _distance = 1000;
            Vector3 _point = Vector3.zero;
            for (int i = 0; i < swingConeRaycast.contactPointList.Count; i++)
            {
                float _currentDistance = Vector3.Distance(swingConeRaycast.perfectPoint.position, swingConeRaycast.contactPointList[i]);
                if (_currentDistance < _distance)
                {
                    _point = swingConeRaycast.contactPointList[i];
                    _distance = _currentDistance;
                }
            }
            Vector3 _direction = _point - GPCtrl.Instance.player.playerMesh.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(GPCtrl.Instance.player.playerMesh.transform.position, _direction, out hit, maxSwingingDistance)) {
                isSwinging = true;
                swingPoint = hit.point;
                springJoint = gameObject.AddComponent<SpringJoint>();
                springJoint.autoConfigureConnectedAnchor = false;
                springJoint.connectedAnchor = swingPoint;

                float _distanceFromPoint = Vector3.Distance(transform.position, swingPoint) + 10;

                springJoint.maxDistance = _distanceFromPoint * 0.8f;
                springJoint.minDistance = _distanceFromPoint * 0.25f;

                springJoint.spring = 10;
                springJoint.damper = 5f;
                springJoint.massScale = 4.5f;
                swingLineRenderer.positionCount = 2;
                swingLineRenderer.SetPosition(1, GPCtrl.Instance.player.playerMesh.transform.position);
            }
        }
    }

    public void StopSwing()
    {
        if (!springJoint) return;
        swingConeRaycast.radius = swingConeRaycast.minRadius;
        GPCtrl.Instance.player.playerMovement.currentMoveSpeed++;
        Destroy(springJoint);
        swingLineRenderer.positionCount = 0;
        isSwinging = false;
        Debug.Log("SCALAR : " + Vector3.Dot(player.rigibody.velocity, player.playerMovement.orientation.transform.forward));
        if (Vector3.Dot(player.rigibody.velocity, player.playerMovement.orientation.transform.forward) > .5f)
        {
            Debug.Log("add acceleration");
            player.rigibody.AddForce(player.rigibody.velocity.normalized * endCurveSpeedBoost, ForceMode.Impulse);
        }
    }
    #endregion

    #region Grapple
    public void StartGrapple()
    {
        Debug.Log(grapplePoint);
        Debug.Log("START GRAPPLE");
        isGrappling = true;
        grappleFreeze = true;
        rightGrapplingLineRenderer.positionCount = 2;
        rightGrapplingLineRenderer.SetPosition(0, player.rightHand.transform.position);
        rightGrapplingLineRenderer.SetPosition(1, player.rightHand.transform.position);

        leftGrapplingLineRenderer.positionCount = 2;
        leftGrapplingLineRenderer.SetPosition(0, player.leftHand.transform.position);
        leftGrapplingLineRenderer.SetPosition(1, player.leftHand.transform.position);
        Invoke(nameof(ExecuteGrapple), grappleFreezeDelay);
    }

    public void ExecuteGrapple()
    {
        grappleFreeze = false;
        Vector3 _lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float _grapplePointRelativeYPos = grapplePoint.y - _lowestPoint.y;
        float _highestPointOnArc = _grapplePointRelativeYPos + overshootYAxis;

        if (_grapplePointRelativeYPos < 0) _highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, _highestPointOnArc);
        Debug.Log("EXECUTE GRAPPLE");
    }

    public void StopGrapple()
    {
        isGrappling = false;
        rightGrapplingLineRenderer.positionCount = 0;
        leftGrapplingLineRenderer.positionCount = 0;
        grapplePoint = Vector3.zero;
        Debug.Log("STOP GRAPPLE");
    }

    private void JumpToPosition(Vector3 _targetPosition, float _trajectoryHeight)
    {
        player.rigibody.velocity = CalculateJumpVelocity(player.transform.position, _targetPosition, _trajectoryHeight);
    }

    private Vector3 CalculateJumpVelocity(Vector3 _startPoint, Vector3 _endPoint, float _trajectoryHeight)
    {
        float _gravity = Physics.gravity.y;
        float _displacementY = _endPoint.y - _startPoint.y;
        Vector3 _displacementXZ = new Vector3(_endPoint.x - _startPoint.x, 0f, _endPoint.z - _startPoint.z);

        Vector3 _velocityY = Vector3.up * Mathf.Sqrt(-2 * _gravity * _trajectoryHeight);
        Vector3 _velocityXZ = _displacementXZ / (Mathf.Sqrt(-2 * _trajectoryHeight / _gravity)
            + Mathf.Sqrt(2 * (_displacementY - _trajectoryHeight) / _gravity));
        return _velocityXZ + _velocityY;
    }
    #endregion
}
