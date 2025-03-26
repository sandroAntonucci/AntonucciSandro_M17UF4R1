using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStrategy
{

    // Every strategy need to execute some behaviour
    Node.Status Process();
    void Reset();

}
