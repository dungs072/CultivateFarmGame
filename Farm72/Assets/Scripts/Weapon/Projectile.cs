using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Projectile : NetworkBehaviour
{
    [SerializeField] private float speed = 20f;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }
    private void Update()
    {
        rb.velocity += transform.forward * Time.deltaTime * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!hasAuthority) { return; }
        if(other.TryGetComponent<Projectile>(out Projectile projectile)) { return; }
        CmdDestroyProjectile();
    }
    [Command]
    private void CmdDestroyProjectile()
    {
        NetworkServer.Destroy(gameObject);
    }
}
