using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootingDistance = 0;
    private Transform targetedEnemy;
    private bool enemyClicked;
    private bool walking;
    private Animator animator;
    private NavMeshAgent navAgent;
    private float nextFire;
    private float timeBetweenShots = 2f;

	// Use this for initialization
	void Start () {
        Assert.IsNotNull(bulletSpawnPoint);
        Assert.IsNotNull(bulletPrefab);
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {

		if (!isLocalPlayer) {
			return;
		}

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown("Fire2")) {

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag("Enemy")) {
                    targetedEnemy = hit.transform;
                    enemyClicked = true;
                } else {
                    walking = true;
                    enemyClicked = false;
                    navAgent.SetDestination(hit.point);
                    navAgent.isStopped = false;

                    print("hit: " + hit.point);
                    print("destination: " + navAgent.destination);
                }
            }
        }

        if (enemyClicked) {
            MoveAndShoot();
            return;
        }

        if (navAgent.remainingDistance <= navAgent.stoppingDistance) {
            walking = false;
        }
        else {
            walking = true;
        }

        animator.SetBool("isWalking", walking);
	}

    void MoveAndShoot() {
        if (targetedEnemy == null) {
            return;
        }

        navAgent.destination = targetedEnemy.position;
        if (navAgent.remainingDistance >= shootingDistance) {
            navAgent.isStopped = false;
            walking = true;
        }

        if (navAgent.remainingDistance < shootingDistance) {
            transform.LookAt(targetedEnemy);

            if (Time.time > nextFire) {
                nextFire = Time.time + timeBetweenShots;
                Fire();
            }
            navAgent.isStopped = true;
            walking = false;
        }
    }

    void Fire() {
        animator.SetTrigger("Attack");
        GameObject fireball = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation) as GameObject;
        fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward * 300;
        Destroy(fireball, 3.5f);
    }
}
