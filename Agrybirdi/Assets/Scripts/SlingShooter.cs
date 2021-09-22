using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShooter : MonoBehaviour
{
    public CircleCollider2D collider;
    public LineRenderer trajectory;

    private Vector2 _startPos;
    private Bird _bird;

    [SerializeField] private float _radius = 0.75f;
    [SerializeField] private float _throwSpeed = 30f;

    private void Start() {
        _startPos = transform.position;
    }

    private void OnMouseUp() {
        collider.enabled = false;
        Vector2 velocity = _startPos - (Vector2)transform.position;
        float distance = Vector2.Distance(_startPos, transform.position);

        _bird.Shoot(velocity, distance, _throwSpeed);

        //Return slingshot to its initial position
        gameObject.transform.position = _startPos;
        trajectory.enabled = false;
    }

    private void OnMouseDrag() {
        //Changing mouse position to world position
        Vector2 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Calculate so that the 'rubber' of the slingshot is within the specified radius
        Vector2 dir = p - _startPos;
        if (dir.sqrMagnitude > _radius){
            dir = dir.normalized * _radius;
        }

        transform.position = _startPos + dir;

        float distance = Vector2.Distance(_startPos, transform.position);

        if(!trajectory.enabled){
            trajectory.enabled = true;
        }

        DisplayTrajectory(distance);
    }

    public void InitiateBird(Bird bird){
        _bird = bird;
        _bird.MoveTo(gameObject.transform.position, gameObject);
        collider.enabled = true;
    }

    public void DisplayTrajectory(float distance){
        if(_bird == null){
            return;
        }

        Vector2 velocity = _startPos - (Vector2)transform.position;
        int segmentCount = 5;
        Vector2[] segments = new Vector2[segmentCount];

        //Starting trajectory
        segments[0] = transform.position;

        //Starting Velocity
        Vector2 segVelocity = velocity * _throwSpeed * distance;

        for (int i = 1; i < segmentCount; i++){
            float elapsedTime = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + segVelocity * elapsedTime + 0.5f * Physics2D.gravity * Mathf.Pow(elapsedTime, 2);
        }

        trajectory.positionCount = segmentCount;
        for (int i = 0; i < segmentCount; i++){
            trajectory.SetPosition(i, segments[i]);
        }
    }
}
