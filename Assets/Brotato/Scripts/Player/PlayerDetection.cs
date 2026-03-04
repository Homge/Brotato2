using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerDetection : MonoBehaviour
{
    [Header(" Colliders ")]
    [SerializeField] private CircleCollider2D collectableCollider;

    /*
    private void FixedUpdate()
    {
        Collider2D[] candyColliders = Physics2D.OverlapCircleAll(
            (Vector2)transform.position + daveCollider.offset,
            daveCollider.radius);

        foreach(Collider2D collider in candyColliders)
        {
            if (collider.TryGetComponent(out Candy candy))
            {
                Debug.Log("Collected : " + candy.name);
                Destroy(candy);
            }
        }
    }
    */

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out ICollectable collectable))
        {
            if (!collider.IsTouching(collectableCollider))
                return;

            collectable.Collect(GetComponent<Player>());
        }
    }    
}
