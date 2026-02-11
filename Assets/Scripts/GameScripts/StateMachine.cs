using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class StateMachine<EState> : MonoBehaviour where EState : Enum
{


    public IState CurrentState { get; private set; }
    protected Dictionary<EState, BaseState<EState>> statesDict = new Dictionary<EState, BaseState<EState>> ();
    protected BaseState<EState> currentState = null;

    protected bool isTransitioning = false;


    private void Awake()
    {

    }

    private void Start()
    {
        // Estrategia inicial: Si no hay nada seleccionado, tomamos el primero
        currentState.OnEnter();
    }

    private void Update()
    {   if (!isTransitioning)
        {
            currentState?.OnUpdate();
        }
        
    }


    public void ChangeState(EState stateKey)
    {
        isTransitioning = true;
        currentState?.OnExit();
        currentState = statesDict[stateKey];
        currentState.OnEnter();
        isTransitioning = false;
        Debug.Log($"<color=green>Estado cambiado a:</color> {stateKey}");
    }
}