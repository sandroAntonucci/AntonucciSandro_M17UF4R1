using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : IStrategy
{

    readonly Func<bool> predicate;

    public Condition(Func<bool> predicate)
    {
        this.predicate = predicate;
    }

    public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;

}
