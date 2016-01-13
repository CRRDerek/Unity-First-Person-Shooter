﻿using UnityEngine;
using System.Collections;

public class ShootScriptAssaultRifle : MonoBehaviour
{
    // Source: http://noobtuts.com/unity/first-person-shooter-game

    // Rocket Prefab
    public GameObject rocketPrefab;
    
    // Name of weapon associated
    public string weaponModelName = "CQAssaultRifle";
    
    // Does this weapon have burst fire?
    public bool canBurstFire = true;

    // Is this weapon in burst fire mode?
    private bool burstFire = false;

    // Burst-fire animation speed multiplier
    public float burstFireSpeed = 1.0f;

    // Single-fire animation speed multiplier
    public float singleFireSpeed = 3.0f;

    // Prefab to use as the gun's muzzle flash
    public GameObject muzzleFlashPrefab;

    // Boolean to represent whether weapon is held
    public bool currentWeapon = false;

    // Boolean to represent whether weapon is owned
    public bool hasWeapon = false;

    // Key to switch to weapon
    public KeyCode weaponCode = KeyCode.Alpha1;

    private Animator rifleAnimator;



    void Start()
    {
        // Get the rifle animator
        rifleAnimator = GameObject.Find(weaponModelName).GetComponent<Animator>();
        rifleAnimator.SetFloat("FiringSpeedMultiplier", singleFireSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // If this weapon's key is pressed and the player has this weapon, toggle it.
        if (Input.GetKeyDown(weaponCode) && hasWeapon)
        {
            if (!currentWeapon)
            {
                currentWeapon = true;
                // Put away all other weapons

            }
            else
            {
                currentWeapon = false;
            }
        }

        if (currentWeapon)
        {
            // Rescale the weapon so that it is visible
            GameObject.Find(weaponModelName).transform.parent.localScale = new Vector3(1, 1, 1);

            // left mouse held down? rifle firing animation finished?
            if (Input.GetMouseButton(0) && !rifleAnimator.GetBool("Firing"))
            {
                // Toggle the firing animation
                rifleAnimator.SetBool("Firing", true);

                if (burstFire)
                {
                    // Burst fire
                    StartCoroutine(Burst());
                }
                else
                {
                    Fire();
                }

            }

            // right mouse clicked? Has burst fire?
            if (Input.GetMouseButtonDown(1) && canBurstFire)
            {
                if (burstFire)
                {
                    rifleAnimator.SetFloat("FiringSpeedMultiplier", singleFireSpeed);
                    burstFire = false;
                }
                else
                {
                    rifleAnimator.SetFloat("FiringSpeedMultiplier", burstFireSpeed);
                    burstFire = true;
                }
            }
        }
        else
        {
            // If the weapon is not currently held, rescale it is that it is invisible
            GameObject.Find(weaponModelName).transform.parent.gameObject.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    // Asynchronous method to fire three shots
    private IEnumerator Burst()
    {
        // Fire three times            
        Fire();
        yield return new WaitForSeconds(0.05f);
        Fire();
        yield return new WaitForSeconds(0.05f);
        Fire();
    }

    private void Fire()
    {
        // spawn rocket
        // - Instantiate means 'throw the prefab into the game world'
        // - (GameObject) cast is required because unity is stupid
        // - transform.position because we want to instantiate it exactly
        //   where the weapon is
        // - transform.parent.rotation because the rocket should face the
        //   same direction that the player faces (which is the weapon's
        //   parent.
        //   we can't just use the weapon's rotation because the weapon is
        //   always rotated like 45° which would make the bullet fly really
        //   weird
        GameObject g = (GameObject)Instantiate(rocketPrefab,
                                               transform.position,
                                               transform.parent.rotation);

        // make the rocket fly forward by simply calling the rigidbody's
        // AddForce method
        // (requires the rocket to have a rigidbody attached to it)
        float force = g.GetComponent<BulletController>().speed;
        g.GetComponent<Rigidbody>().AddForce(g.transform.forward * force);

        // Instantiate a prefab for the muzzle flash
        GameObject muzzleFlash = (GameObject)Instantiate(muzzleFlashPrefab,
            transform.position,
            transform.parent.rotation);
        // Set the parent of the flash to be this
        muzzleFlash.transform.parent = transform;

    }
}
