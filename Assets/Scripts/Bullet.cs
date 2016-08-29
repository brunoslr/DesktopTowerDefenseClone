using UnityEngine;

/// <summary>
/// This class represents the bullets spawned by the towers and it's behavior
/// chasing the target
/// </summary>
public class Bullet : MonoBehaviour {

    private Transform target;
    private float speed = 5f;
    public int bulletDamage = 5;


    public void Chase(Transform _target)
    {
        target = _target;
    }


	// Update is called once per frame
	void Update ()
    {
         if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if(dir.magnitude<=distanceThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);


	}

    void HitTarget()
    {
        target.GetComponent<Enemy>().ReceiveDamage(bulletDamage);
        Destroy(gameObject);
    }

    internal void SetDamage(int damage)
    {
        bulletDamage = damage;
    }
}
