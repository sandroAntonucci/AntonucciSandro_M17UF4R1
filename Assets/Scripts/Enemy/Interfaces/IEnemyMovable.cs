using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IEnemyMovable
{
    void MoveTo(Vector3 position);
    void StopMovement();

}
