using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class WaitWhile : CustomYieldInstruction
//{
//    Func<bool> m_Predicate;

//    public override bool keepWaiting { get { return m_Predicate(); } }

//    public WaitWhile(Func<bool> predicate) { m_Predicate = predicate; }
//}

// Same WaitWhile implemented by inheriting from IEnumerator.
//class WaitWhile : IEnumerator
//{
//    Func<bool> m_Predicate;

//    public object Current { get { return null; } }

//    public bool MoveNext() { return m_Predicate(); }

//    public void Reset() { }

//    public WaitWhile(Func<bool> predicate) { m_Predicate = predicate; }
//}

public class QueuedCorroutine
{

    private QueuedCorroutine next;
    private QueuedCorroutine prev;

    private bool executed = false;

    private IEnumerator enumerator;

    private MonoBehaviour monoBehaviour;


    public QueuedCorroutine(MonoBehaviour monoBehaviour, IEnumerator enumerator)
    {
        this.monoBehaviour = monoBehaviour;
        this.enumerator = enumerator;
    }

    public static QueuedCorroutine Create(MonoBehaviour monoBehaviour, IEnumerator enumerator)
    {
        return new QueuedCorroutine(monoBehaviour, enumerator);
    }
    public QueuedCorroutine Next(QueuedCorroutine next)
    {
        this.next = next;
        this.next.prev = this;
        return next;
    }
    public QueuedCorroutine Next(IEnumerator enumerator)
    {
        next = new QueuedCorroutine(monoBehaviour, enumerator)
        {
            prev = this
        };
        return next;
    }
    public void Execute()
    {
        Execute(monoBehaviour);
    }

    private void Execute(MonoBehaviour monoBehaviour)
    {
        if (prev != null&&!prev.executed)
        {
            prev.Execute();
        }
        else
        {
            //we are the first
            executed = true;
            monoBehaviour.StartCoroutine(baseCoroutine(monoBehaviour));
        }
    }

    private void Reset()
    {
        next.Reset();
        executed = false;
    }

    IEnumerator baseCoroutine(MonoBehaviour monoBehaviour)
    {

        yield return monoBehaviour.StartCoroutine(enumerator);

        next?.Execute();
    }

    //private readonly List<Func<IEnumerator>> enumerators = new List<Func<IEnumerator>>();
    //public Action OnAllDone = delegate { };

    //// Start is called before the first frame update
    //void Start()
    //{
    //    Enqueue(a).Enqueue(b).Run();
    //}
    //IEnumerator a()
    //{
    //    Debug.Log("A");
    //    yield return new WaitForSeconds(1.0f);
    //}

    //IEnumerator b()
    //{
    //    Debug.Log("B");
    //    yield return null;
    //}
    //public QueuedCorroutine Enqueue(Func<IEnumerator> enumerator)
    //{
    //    enumerators.Add(enumerator);

    //    return this;
    //}

    //public void Run()
    //{
    //    TriggerNextCoroutine(0);
    //}

    //private void TriggerNextCoroutine(int index)
    //{
    //    if (index < enumerators.Count)
    //    {
    //        StartCoroutine(baseCoroutine(enumerators[index], index));
    //    }
    //    else
    //    {
    //        OnAllDone?.Invoke();
    //    }
    //}

    //private IEnumerator baseCoroutine(Func<IEnumerator> enumerator, int index)
    //{
    //    yield return StartCoroutine(enumerator());

    //    TriggerNextCoroutine(index+1);

    //    yield return null;
    //}
}

public class DelayAction
{
    private float time; 
    private readonly Action action;

    public DelayAction(MonoBehaviour mono, Action action, float time)
    {
        this.time = time;
        this.action = action;
        QueuedCorroutine.Create(mono, First()).Next(Enumerator()).Execute();
    }
    IEnumerator Enumerator()
    {
        action?.Invoke();
        yield return null;
    }
    IEnumerator First()
    {
        yield return new WaitForSeconds(time);
    }
}

public class WaitForPredicateAction
{
    private WaitWhile waitWhile;

    private readonly Action action;

    public WaitForPredicateAction(MonoBehaviour mono,Func<bool> predicate, Action action)
    {
        this.action = action;

        waitWhile = new WaitWhile(predicate);

        mono.StartCoroutine(Enumerator());
    }
    IEnumerator Enumerator()
    {
        yield return waitWhile;

        action?.Invoke();

        yield return null;
    }
}