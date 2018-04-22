using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

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
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown("Fire2")) {
            if (Physics.Raycast(ray, out hit, 100)) {
                if (hit.collider.CompareTag("Enemy")) {
                    targetedEnemy = hit.transform;
                    enemyClicked = true;
                } else {
                    walking = true;
                    enemyClicked = false;
                    navAgent.destination = hit.point;
                    navAgent.isStopped = false;
                }
            }
        }

        if (enemyClicked) {
            MoveAndShoot();
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
        print("fire!!");
    }
}
