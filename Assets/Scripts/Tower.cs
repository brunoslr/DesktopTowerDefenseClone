using UnityEngine;

/// <summary>
/// This class contains the towers behavior and methods to find enemies and shoot them
/// </summary>
public class Tower : MonoBehaviour {

    public Transform enemyOnTarget;

    public int damage = 5;
    public float range = 3f;
    public float fireRate = 0.2f;
    private float fireCountdown = 0f;


    
    public string enemyTag = "Enemy";

    public GameObject bulletPrefab;
    public Transform firePoint;
    

	// Use this for initialization
	void Start ()
    {
        //The towers only check for new targets twice each second, to reduce overload
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
	}

   	// Update is called once per frame
	void Update ()
    {
        if (enemyOnTarget == null)
            return;

        if(fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    /// <summary>
    /// Method to find enemies on range and lock to the enemy transform
    /// on the closest target on range to be used when instantiating the bullet 
    /// </summary>
    void UpdateTarget()
    {

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;

            }

            if (nearestEnemy != null && shortestDistance <= range)
            {
                enemyOnTarget = nearestEnemy.transform;
            }
            else
                enemyOnTarget = null;

        }
    }

    /// <summary>
    /// Instatiate bullets and set them to chase the enemy on target
    /// </summary>
    void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if(bullet !=null)
        {
            bullet.SetDamage(damage);
            bullet.Chase(enemyOnTarget); 
        }


    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
