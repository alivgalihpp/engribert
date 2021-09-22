using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bird : MonoBehaviour
{
    public enum BirdState {
        Idle, Thrown, HitSomething
    }
    public GameObject parent;
    public Rigidbody2D rb2d;
    public CircleCollider2D collider;

    public UnityAction onBirdDestroyed = delegate { };
    public UnityAction<Bird> onBirdShot = delegate { };
    
    private BirdState _state;
    public BirdState state {
        get {
            return _state;
        }
    }

    private float _minVelocity = 0.05f;
    private bool _flagDestroy = false;

    private void Start() {
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        collider.enabled = false;
        _state = BirdState.Idle;
    }

    private void FixedUpdate() {
        if (_state == BirdState.Idle && rb2d.velocity.sqrMagnitude >= _minVelocity){
            _state = BirdState.Thrown;
        }

        if ((_state == BirdState.Thrown || _state == BirdState.HitSomething) && rb2d.velocity.sqrMagnitude < _minVelocity && !_flagDestroy){
            //Destroy game object after 2 sec
            _flagDestroy = true;
            StartCoroutine(DestroyAfter(2));
        }
    }

    private void OnDestroy() {
        if (_state == BirdState.Thrown || _state == BirdState.HitSomething){
            onBirdDestroyed();
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        _state = BirdState.HitSomething;
    }

    private IEnumerator DestroyAfter(float second){
        yield return new WaitForSeconds(second);
        Destroy(gameObject);
    }

    public void MoveTo(Vector2 target, GameObject parent){
        gameObject.transform.SetParent(parent.transform);
        gameObject.transform.position = target;
    }

    public void Shoot(Vector2 velocity, float distance, float speed){
        collider.enabled = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.velocity = velocity * speed * distance;
        onBirdShot(this);
    }

    public virtual void OnTap(){
        //Do Nothing
    }
}
