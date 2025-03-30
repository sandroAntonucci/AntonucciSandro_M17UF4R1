using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Policies
{
    public static readonly IPolicy RunForever = new RunForeverPolicy();
    public static readonly IPolicy RunUntilSuccess = new RunUntilSuccessPolicy();
    public static readonly IPolicy RunUntilFailure = new RunUntilFailurePolicy();

    class RunForeverPolicy : IPolicy
    {
        public bool ShouldReturn(Node.Status status) => false;
    }

    class RunUntilSuccessPolicy : IPolicy
    {
        public bool ShouldReturn(Node.Status status) => status == Node.Status.Success;
    }

    class RunUntilFailurePolicy : IPolicy
    {
        public bool ShouldReturn(Node.Status status) => status == Node.Status.Failure;
    }
}

public interface IPolicy
{
    bool ShouldReturn(Node.Status status);
}