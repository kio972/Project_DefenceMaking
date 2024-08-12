using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;

public class Test2 : Singleton<Test2>
{

    public class Apple
    {
        public int a;
        public Apple(int a) { this.a = a; }
    }

    public ReactiveCollection<Apple> tests { get; private set; } = new ReactiveCollection<Apple>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            tests.Add(new Apple(0));
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            tests.RemoveAt(0);
        }
    }
}