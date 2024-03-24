using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBreakableByGun
{
    public void TakeDamage(RaycastHit hit, float damage);   
}
