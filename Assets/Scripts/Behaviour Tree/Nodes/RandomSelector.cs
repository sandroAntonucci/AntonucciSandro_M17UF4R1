using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomSelector : PrioritySelector
{

    protected override List<Node> SortChildren() => children.Shuffle().ToList();

    public RandomSelector(string name) : base(name) { }



}
